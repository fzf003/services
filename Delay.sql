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




