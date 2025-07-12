
 
SET NOCOUNT ON：执行时不返回每条语句影响的行数，减少网络开销。
SET XACT_ABORT ON：遇到运行时错误自动回滚整个事务，保证一致性。

/*
This script defines the schema and core procedures for Orleans Streaming Message Queuing in SQL Server.
It is optimized for high-frequency polling, efficient deadlock avoidance, and self-healing fragmentation.
*/

/* Sequence for generating unique MessageId values */
CREATE SEQUENCE OrleansStreamMessageSequence
AS BIGINT
START WITH 1
INCREMENT BY 1
NO MAXVALUE
NO CYCLE
CACHE 1000;
GO

/* Main Streaming Message Queue Table */
CREATE TABLE OrleansStreamMessage
(
	ServiceId NVARCHAR(150) NOT NULL,
    ProviderId NVARCHAR(150) NOT NULL,
	QueueId NVARCHAR(150) NOT NULL,
	MessageId BIGINT NOT NULL,
	Dequeued INT NOT NULL,
	VisibleOn DATETIME2(7) NOT NULL,
	ExpiresOn DATETIME2(7) NOT NULL,
	CreatedOn DATETIME2(7) NOT NULL,
	ModifiedOn DATETIME2(7) NOT NULL,
	Payload VARBINARY(MAX) NOT NULL,
	CONSTRAINT PK_OrleansStreamMessage PRIMARY KEY CLUSTERED
	(
		ServiceId ASC,
        ProviderId ASC,
		QueueId ASC,
		MessageId ASC
	)
);
GO

/* Dead Letter Table for Failed or Expired Messages */
CREATE TABLE OrleansStreamDeadLetter
(
	ServiceId NVARCHAR(150) NOT NULL,
    ProviderId NVARCHAR(150) NOT NULL,
	QueueId NVARCHAR(150) NOT NULL,
	MessageId BIGINT NOT NULL,
	Dequeued INT NOT NULL,
	VisibleOn DATETIME2(7) NOT NULL,
	ExpiresOn DATETIME2(7) NOT NULL,
	CreatedOn DATETIME2(7) NOT NULL,
	ModifiedOn DATETIME2(7) NOT NULL,
    DeadOn DATETIME2(7) NOT NULL,
	RemoveOn DATETIME2(7) NOT NULL,
	Payload VARBINARY(MAX) NULL,
	CONSTRAINT PK_OrleansStreamDeadLetter PRIMARY KEY CLUSTERED
	(
		ServiceId ASC,
        ProviderId ASC,
		QueueId ASC,
		MessageId ASC
	)
);
GO

/* Control Table for Stream Scheduling */
CREATE TABLE OrleansStreamControl
(
	ServiceId NVARCHAR(150) NOT NULL,
    ProviderId NVARCHAR(150) NOT NULL,
	QueueId NVARCHAR(150) NOT NULL,
    EvictOn DATETIME2(7) NOT NULL,
	CONSTRAINT PK_OrleansStreamControl PRIMARY KEY CLUSTERED
	(
		ServiceId ASC,
        ProviderId ASC,
		QueueId ASC
	)
);
GO

/* Procedure: QueueStreamMessage */
CREATE PROCEDURE QueueStreamMessage
	@ServiceId NVARCHAR(150),
    @ProviderId NVARCHAR(150),
	@QueueId NVARCHAR(150),
	@Payload VARBINARY(MAX),
	@ExpiryTimeout INT
AS
BEGIN
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @MessageId BIGINT = NEXT VALUE FOR OrleansStreamMessageSequence;
DECLARE @Now DATETIME2(7) = SYSUTCDATETIME();
DECLARE @ExpiresOn DATETIME2(7) = DATEADD(SECOND, @ExpiryTimeout, @Now);

INSERT INTO OrleansStreamMessage
(
	ServiceId,
    ProviderId,
	QueueId,
	MessageId,
	Dequeued,
	VisibleOn,
	ExpiresOn,
	CreatedOn,
	ModifiedOn,
	Payload
)
OUTPUT
    Inserted.ServiceId,
    Inserted.ProviderId,
    Inserted.QueueId,
    Inserted.MessageId
VALUES
(
	@ServiceId,
    @ProviderId,
	@QueueId,
	@MessageId,
	0,
	@Now,
	@ExpiresOn,
	@Now,
	@Now,
	@Payload
);

END
GO

/* Procedure: GetStreamMessages
   - Retrieves a batch of messages for processing.
   - Performs scheduled eviction of expired/faulted messages and dead letters.
*/
CREATE PROCEDURE GetStreamMessages
	@ServiceId NVARCHAR(150),
    @ProviderId NVARCHAR(150),
	@QueueId NVARCHAR(150),
    @MaxCount INT,
	@MaxAttempts INT,
	@VisibilityTimeout INT,
    @RemovalTimeout INT,
    @EvictionInterval INT,
    @EvictionBatchSize INT
AS
BEGIN
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Now DATETIME2(7) = SYSUTCDATETIME();
DECLARE @VisibleOn DATETIME2(7) = DATEADD(SECOND, @VisibilityTimeout, @Now);

/* Check if eviction is due */
DECLARE @EvictOn DATETIME2(7) =
(
    SELECT EvictOn
    FROM OrleansStreamControl
    WHERE
        ServiceId = @ServiceId
        AND ProviderId = @ProviderId
        AND QueueId = @QueueId
);

IF @EvictOn IS NULL OR @EvictOn < @Now
BEGIN
    WITH Candidate AS
    (
        SELECT
            ServiceId = @ServiceId,
            ProviderId = @ProviderId,
            QueueId = @QueueId,
            Now = @Now,
            EvictOn = DATEADD(SECOND, @EvictionInterval, @Now)
    )
    MERGE OrleansStreamControl WITH (UPDLOCK, HOLDLOCK) AS T
    USING Candidate AS S
    ON T.ServiceId = S.ServiceId
    AND T.ProviderId = S.ProviderId
    AND T.QueueId = S.QueueId
    WHEN MATCHED AND T.EvictOn < S.Now THEN
    UPDATE SET T.EvictOn = S.EvictOn
    WHEN NOT MATCHED BY TARGET THEN
    INSERT
    (
        ServiceId,
        ProviderId,
        QueueId,
        EvictOn
    )
    VALUES
    (
        ServiceId,
        ProviderId,
        QueueId,
        EvictOn
    );

    IF (@@ROWCOUNT > 0)
    BEGIN
        EXECUTE EvictStreamMessages
            @ServiceId = @ServiceId,
            @ProviderId = @ProviderId,
            @QueueId = @QueueId,
            @MaxAttempts = @MaxAttempts,
            @RemovalTimeout = @RemovalTimeout,
            @BatchSize = @EvictionBatchSize;

        EXECUTE EvictStreamDeadLetters
            @ServiceId = @ServiceId,
            @ProviderId = @ProviderId,
            @QueueId = @QueueId,
            @BatchSize = @EvictionBatchSize;
    END;
END;

/* Update and return eligible messages */
WITH Batch AS
(
	SELECT TOP (@MaxCount)
		ServiceId,
        ProviderId,
		QueueId,
		MessageId,
		Dequeued,
		VisibleOn,
		ExpiresOn,
		CreatedOn,
		ModifiedOn,
		Payload
	FROM
		OrleansStreamMessage WITH (UPDLOCK)
	WHERE
		ServiceId = @ServiceId
        AND ProviderId = @ProviderId
		AND QueueId = @QueueId
		AND Dequeued < @MaxAttempts
		AND VisibleOn <= @Now
		AND ExpiresOn > @Now
	ORDER BY
        ServiceId,
        ProviderId,
        QueueId,
		MessageId
)
UPDATE Batch
SET
	Dequeued += 1,
	VisibleOn = @VisibleOn,
	ModifiedOn = @Now
OUTPUT
	Inserted.ServiceId,
    Inserted.ProviderId,
	Inserted.QueueId,
	Inserted.MessageId,
	Inserted.Dequeued,
	Inserted.VisibleOn,
	Inserted.ExpiresOn,
	Inserted.CreatedOn,
	Inserted.ModifiedOn,
	Inserted.Payload
FROM
	Batch;
END
GO

/* Procedure: ConfirmStreamMessages
   - Confirms and removes the specified delivered messages.
*/
CREATE PROCEDURE ConfirmStreamMessages
	@ServiceId NVARCHAR(150),
    @ProviderId NVARCHAR(150),
	@QueueId NVARCHAR(150),
    @Items NVARCHAR(MAX)
AS
BEGIN
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ItemsTable TABLE
(
    MessageId BIGINT PRIMARY KEY NOT NULL,
    Dequeued INT NOT NULL
);
WITH Items AS
(
	SELECT Value FROM STRING_SPLIT(@Items, '|')
)
INSERT INTO @ItemsTable
(
    MessageId,
    Dequeued
)
SELECT
	CAST(SUBSTRING(Value, 1, CHARINDEX(':', Value, 1) - 1) AS BIGINT) AS MessageId,
	CAST(SUBSTRING(Value, CHARINDEX(':', Value, 1) + 1, LEN(Value)) AS INT) AS Dequeued
FROM
	Items;

DECLARE @Count INT = (SELECT COUNT(*) FROM @ItemsTable);

WITH Batch AS
(
	SELECT TOP (@Count)
		*
	FROM
		OrleansStreamMessage AS M WITH (UPDLOCK, HOLDLOCK)
	WHERE
		ServiceId = @ServiceId
        AND ProviderId = @ProviderId
		AND QueueId = @QueueId
        AND EXISTS
        (
            SELECT *
            FROM @ItemsTable AS I
            WHERE I.MessageId = M.MessageId
            AND I.Dequeued = M.Dequeued
        )
	ORDER BY
        ServiceId,
        ProviderId,
        QueueId,
		MessageId
)
DELETE FROM Batch
OUTPUT
    Deleted.ServiceId,
    Deleted.ProviderId,
    Deleted.QueueId,
    Deleted.MessageId;
END
GO

/* Procedure: FailStreamMessage
   - Moves exhausted/expired messages to dead letter or makes them visible again.
*/
CREATE PROCEDURE FailStreamMessage
	@ServiceId NVARCHAR(150),
    @ProviderId NVARCHAR(150),
	@QueueId NVARCHAR(150),
    @MessageId BIGINT,
	@MaxAttempts INT,
	@RemovalTimeout INT
AS
BEGIN
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Now DATETIME2(7) = SYSUTCDATETIME();
DECLARE @RemoveOn DATETIME2(7) = DATEADD(SECOND, @RemovalTimeout, @Now);

UPDATE OrleansStreamMessage
SET
    VisibleOn = @Now,
    ModifiedOn = @Now
WHERE
    ServiceId = @ServiceId
    AND ProviderId = @ProviderId
    AND QueueId = @QueueId
    AND MessageId = @MessageId
    AND Dequeued < @MaxAttempts;

IF @@ROWCOUNT > 0 RETURN;

DELETE FROM OrleansStreamMessage
OUTPUT
    Deleted.ServiceId,
    Deleted.ProviderId,
    Deleted.QueueId,
    Deleted.MessageId,
    Deleted.Dequeued,
    Deleted.VisibleOn,
    Deleted.ExpiresOn,
    Deleted.CreatedOn,
    Deleted.ModifiedOn,
    @Now AS DeadOn,
    @RemoveOn AS RemoveOn,
    Deleted.Payload
INTO OrleansStreamDeadLetter
(
    ServiceId,
    ProviderId,
    QueueId,
    MessageId,
    Dequeued,
    VisibleOn,
    ExpiresOn,
    CreatedOn,
    ModifiedOn,
    DeadOn,
    RemoveOn,
    Payload
)
WHERE
    ServiceId = @ServiceId
    AND ProviderId = @ProviderId
    AND QueueId = @QueueId
    AND MessageId = @MessageId;

END
GO

/* Procedure: EvictStreamMessages
   - Batch moves expired/exhausted messages to dead letters.
*/
CREATE PROCEDURE EvictStreamMessages
	@ServiceId NVARCHAR(150),
    @ProviderId NVARCHAR(150),
	@QueueId NVARCHAR(150),
	@BatchSize INT,
	@MaxAttempts INT,
	@RemovalTimeout INT
AS
BEGIN
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Now DATETIME2(7) = SYSUTCDATETIME();
DECLARE @RemoveOn DATETIME2(7) = DATEADD(SECOND, @RemovalTimeout, @Now);

WITH Batch AS
(
	SELECT TOP (@BatchSize)
		ServiceId,
        ProviderId,
		QueueId,
		MessageId,
		Dequeued,
		VisibleOn,
		ExpiresOn,
		CreatedOn,
		ModifiedOn,
		DeadOn = @Now,
		RemoveOn = @RemoveOn,
		Payload
	FROM
		OrleansStreamMessage WITH (UPDLOCK)
	WHERE
		ServiceId = @ServiceId
        AND ProviderId = @ProviderId
		AND QueueId = @QueueId
        AND VisibleOn <= @Now
		AND
		(
			Dequeued >= @MaxAttempts
			OR
			ExpiresOn <= @Now
		)
	ORDER BY
        ServiceId,
        ProviderId,
        QueueId,
		MessageId
)
DELETE FROM Batch
OUTPUT
	Deleted.ServiceId,
    Deleted.ProviderId,
	Deleted.QueueId,
	Deleted.MessageId,
	Deleted.Dequeued,
	Deleted.VisibleOn,
	Deleted.ExpiresOn,
	Deleted.CreatedOn,
	Deleted.ModifiedOn,
	Deleted.DeadOn,
	Deleted.RemoveOn,
	Deleted.Payload
INTO OrleansStreamDeadLetter
(
	ServiceId,
    ProviderId,
	QueueId,
	MessageId,
	Dequeued,
	VisibleOn,
	ExpiresOn,
	CreatedOn,
	ModifiedOn,
	DeadOn,
	RemoveOn,
	Payload
);
END
GO

/* Procedure: EvictStreamDeadLetters
   - Removes dead letter messages scheduled for deletion.
*/
CREATE PROCEDURE EvictStreamDeadLetters
	@ServiceId NVARCHAR(150),
    @ProviderId NVARCHAR(150),
	@QueueId NVARCHAR(150),
	@BatchSize INT
AS
BEGIN
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Now DATETIME2(7) = SYSUTCDATETIME();

WITH Batch AS
(
    SELECT TOP (@BatchSize)
        ServiceId,
        ProviderId,
        QueueId,
        MessageId
    FROM
        OrleansStreamDeadLetter WITH (UPDLOCK)
    WHERE
        ServiceId = @ServiceId
        AND ProviderId = @ProviderId
        AND QueueId = @QueueId
        AND RemoveOn <= @Now
    ORDER BY
        ServiceId,
        ProviderId,
        QueueId,
        MessageId
)
DELETE FROM Batch;

END
GO
