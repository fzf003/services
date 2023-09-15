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


----处理延迟时间信息，并备份到备份表

WITH MESSAGE AS (
  SELECT TOP(1) *
  FROM HD..Users WITH (UPDLOCK, readpast, rowlock)
  ORDER BY UserId)
DELETE FROM MESSAGE
OUTPUT
  deleted.UserName,
  deleted.Sex,
  deleted.DueAfter
  INTO HD..MUser;

---查询下一条到期时间
SELECT DueAfter FROM HD..Users WITH (READPAST) ORDER BY DueAfter 


 ---查找包含视图的存储过程和仕途
select distinct OBJECT_NAME(Id) from syscomments  where id in (select Id from sysobjects where type in ('V','P','F'))
 and text like '%V_PM_ContranctAmount%'
	



