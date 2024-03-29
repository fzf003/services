


SCZS.YX.BPMCOREServices/Services/QuotePriceService.cs
SCZS.YX.BPMCOREServices/IServices/IQuotePriceService.cs
SCZS.YX.BPMCOREModel/Filter/QueryProductLineFilter.cs
SCZS.YX.BPMCOREModel/DTO/QueryProductResponse.cs
SCZS.YX.BPMCORE/Controllers/CustomerProductLineController.cs
SCZS.YX.BPMCORE/Controllers/CustomerController.cs
SCZS.YX.BPMCOREModel/DTO/QuotePriceItemGxList.cs
SCZS.YX.BPMCOREServices/Services/QuotePriceService.cs
SCZS.YX.BPMCOREModel/DTO/QuotePriceItemGxList.cs
SCZS.YX.BPMCOREModel/DTO/QuotePriceItemGxList.cs
SCZS.YX.BPMCOREModel/Filter/DragSortFilter.cs
SCZS.YX.BPMCOREModel/Filter/UpdateSortItemFilter.cs
SCZS.YX.BPMCOREModel/Filter/QueryProductLineFilter.cs
SCZS.YX.BPMCORE/Controllers/CustomerProductLineController.cs
SCZS.YX.BPMCOREModel/DTO/PMPriceGZSort.cs
SCZS.YX.BPMCOREModel/Filter/UpdateSortItemFilter.cs
SCZS.YX.BPMCOREModel/Filter/QuotePriceItemGxInsert.cs
SCZS.YX.BPMCOREModel/Filter/QueryProductLineFilter.cs


  
  SELECT * FROM (
  SELECT  PM.ID,PM.CusCode FROM (SELECT CusCode FROM JZDATA..CRM_Customer  WHERE CustomerType IN('尚层优选') AND CusStateCode <32) T
   LEFT JOIN SCYXDATA..PM_Price_M PM 
   ON PM.CUSCODE=T.CUSCODE
   WHERE PM.Status IN(0,1)  
   ) N

      

DECLARE @MID INT=1312;
DECLARE @CusCode NVARCHAR(50)='CUS2007260007';
DECLARE @FoolSpacePartId INT;
DECLARE F1 CURSOR FOR
SELECT Id  FROM SCYXDATA..V_GXPart_Infor WHERE CusCode IN(@CusCode);
OPEN F1;
FETCH NEXT FROM F1 INTO @FoolSpacePartId;
WHILE @@FETCH_STATUS = 0
BEGIN

PRINT @FoolSpacePartId

IF EXISTS (SELECT ID FROM SCYXDATA..PM_Price_M WHERE ID=@MID AND Status IN(0,1))
BEGIN
IF EXISTS( SELECT ID FROM SCYXDATA..PM_Price_GZ_T WHERE M_ID = @MID AND FoolSpacePartId=@FoolSpacePartId)
BEGIN

WITH CTE AS (
    SELECT ROW_NUMBER() OVER (ORDER BY ID DESC) AS RowNum,ID
    FROM SCYXDATA..PM_Price_GZ_T
    WHERE M_ID = @MID AND FoolSpacePartId=@FoolSpacePartId
)

UPDATE FSD SET FSD.SortNO=T.RowNum
FROM SCYXDATA..PM_Price_GZ_T FSD 
INNER JOIN CTE as T ON T.ID=FSD.ID
WHERE FSD.M_ID =@MID AND FSD.FoolSpacePartId =@FoolSpacePartId;
END

END


 FETCH NEXT FROM F1 INTO @FoolSpacePartId;
END
CLOSE F1;
DEALLOCATE F1;
