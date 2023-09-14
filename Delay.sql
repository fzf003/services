--计算时间
DECLARE @DueAfterMilliseconds INT=0;
DECLARE @DueAfterSeconds INT=0;
DECLARE @DueAfterMinutes INT=0;
DECLARE @DueAfterHours INT=1;
DECLARE @DueAfterDays INT=1;
DECLARE @DueAfter DATETIME = GETDATE();
SET @DueAfter = DATEADD(ms, @DueAfterMilliseconds, @DueAfter);
SET @DueAfter = DATEADD(s, @DueAfterSeconds, @DueAfter);
SET @DueAfter = DATEADD(n, @DueAfterMinutes, @DueAfter);
SET @DueAfter = DATEADD(hh, @DueAfterHours, @DueAfter);
SET @DueAfter = DATEADD(d, @DueAfterDays, @DueAfter);

PRINT CONVERT(nvarchar(50), @DueAfter, 120)


---Sql 锁
DECLARE @MLock NVARCHAR(50)='my_lock';

EXEC sp_getapplock @Resource = @MLock, @LockMode = 'Exclusive'

BEGIN TRY
SELECT * FROM HD..Users
END TRY
BEGIN CATCH
   EXEC  sp_releaseapplock @Resource = @MLock;
    THROW;
END CATCH;

EXEC  sp_releaseapplock @Resource = @MLock


----删除

with message as (
  select top(1) *
  from HD..Users with (updlock, readpast, rowlock)
  order by UserId)
delete from message
output
  deleted.UserName,
	deleted.Sex,
	deleted.DueAfter
	INTO HD..MUser;
	



