 
DECLARE @InstanceCount INT = 6;  
DECLARE @CurrentInstance INT = 3;  
SELECT  @CurrentInstance % @InstanceCount
SELECT (RowNumber % @InstanceCount) as C,*
FROM ( SELECT *, ROW_NUMBER() OVER (ORDER BY UserId) AS RowNumber
       FROM HD..Users --where UserId in (1108,1107,1106,1105,1200)
) AS T
WHERE T.RowNumber % @InstanceCount = @CurrentInstance % @InstanceCount
