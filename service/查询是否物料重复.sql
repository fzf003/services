
USE SCYXDATA
DECLARE @MID INT=17387;    --报价ID
DECLARE @FID INT=4400990;  --空间Id
DECLARE @ProCode NVARCHAR(50)='PRO20240507_532442_10';
DECLARE @RMID INT=16409;

 --SELECT  * FROM SCYXDATA..V_Part_Infor WHERE M_ID in(17387)
DECLARE @CaseCode NVARCHAR(50)='';  --产品线编码
DECLARE @PtCode NVARCHAR(50)='';  --大类编码
DECLARE @PtiCode NVARCHAR(50)='';  --子类编码
DECLARE @Companycode NVARCHAR(50)='';
DECLARE @SpaceID INT=0;
DECLARE @PartID INT=0;

 SELECT SUM(Num)  FROM SCYXDATA..PM_Price_T WHERE M_ID in(@MID) and ProCode in(@ProCode)

 SELECT SUM(Num)  FROM SCYXDATA..PM_Price_T WHERE M_ID in(@RMID) and ProCode in(@ProCode)

SELECT @CaseCode=CaseCode,@Companycode=Companycode FROM SCYXDATA..PM_Price_M WHERE ID=@MID;

SELECT @PtiCode=PtiCode,@PtCode=PtCode,@SpaceID=SpaceID,@PartID=PartID FROM  SCYXDATA..PM_Price_Part WHERE Id=@FID
 
SELECT SpaceID,SpaceName,PartID,PartName,* FROM SCYXDATA..V_ProductInfor 
WHERE Case_M_SN=@CaseCode
 AND CompanyCode in(@Companycode) AND ProCode=@ProCode  AND SpaceID =@SpaceID 
 AND PartID=@PartID
 AND PtCode=@PtCode AND PtiCode=@PtiCode
