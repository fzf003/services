SELECT 3 as 销售方式,SP.TotalSellingPrice as 销售价 ,SP.ProductCode AS 产品编码,SM.MaterialName AS 产品名称,SM.MaterialCode AS 物料编码,
SM.ProductCategory as 大类,SM.ProductCategoryCode as 大类编码,SM.ProductType as 品类,SM.ProductTypeCode as 品类编码,
SM.ProductSubclass as 子类,SM.ProductSubclassCode as 子类编码,
SM.BrandName as 品牌,SM.BrandCode as 品牌编码,SM.Series as 系列,SM.Marque as 型号,SM.Color as 颜色,SM.MaterialQuality as 材质,
SM.SpecLong as 长,SM.SpecWide as 宽,SM.SpecHigh as 高,sp.ProductState as 产品状态
FROM SCZSSCM..SCM_Product SP
LEFT JOIN SCZSSCM..SCM_Material SM ON SP.MaterialCode=SM.MaterialCode
WHERE SP.CompanyCode in(1) and sp.ProductState in(1,3)  and SM.IsDel=0
UNION 
SELECT (case when SLIP.SaleWay='标配' then 1 else 2 end )as 销售方式,SLIP.AddPrice as 销售价,T.* FROM SCZSSCM..SCM_ProductLine_Item_Product_T SLIP 
LEFT JOIN (SELECT SP.ProductCode AS 产品编码,SM.MaterialName AS 产品名称,SM.MaterialCode AS 物料编码,
SM.ProductCategory as 大类,SM.ProductCategoryCode as 大类编码,SM.ProductType as 品类,SM.ProductTypeCode as 品类编码,
SM.ProductSubclass as 子类,SM.ProductSubclassCode as 子类编码,
SM.BrandName as 品牌,SM.BrandCode as 品牌编码,SM.Series as 系列,SM.Marque as 型号,SM.Color as 颜色,SM.MaterialQuality as 材质,
SM.SpecLong as 长,SM.SpecWide as 宽,SM.SpecHigh as 高,sp.ProductState as 产品状态
FROM SCZSSCM..SCM_Product SP
LEFT JOIN SCZSSCM..SCM_Material SM ON SP.MaterialCode=SM.MaterialCode
WHERE SP.CompanyCode in(1) AND sp.ProductState in(1,3) AND SM.IsDel=0) T
ON SLIP.ProCode=T.产品编码
WHERE SLIP.PId IN(SELECT ID FROM SCZSSCM..V_ProductLineSpacePart_RelatedProduct WHERE  ProductLineCode in('YXP202412240008'))





USE [SCZSCQS]
GO
-- 定义第一个子查询
WITH ProductDetails AS (
    SELECT 
        SP.ProductCode,
        SM.MaterialName AS ProductName,
        SM.MaterialCode,
        SM.ProductCategory,
        SM.ProductCategoryCode,
        SM.ProductType,
        SM.ProductTypeCode,
        SM.ProductSubclass,
        SM.ProductSubclassCode,
        SM.BrandName,
        SM.BrandCode,
        SM.Series,
        SM.Marque,
        SM.Color,
        SM.MaterialQuality,
        SMI.ImgUrl,
        SMI.SmallImgUrl,
        SM.SpecLong,
        SM.SpecWide,
        SM.SpecHigh,
        (CONCAT(ISNULL(SM.SpecLong, 0), '*', ISNULL(SM.SpecWide, 0), '*', ISNULL(SM.SpecHigh, 0))) AS Spec,
        SM.ProductionPlace,
        SP.ProductState,
        SP.CompanyCode,
        SP.TotalSellingPrice AS SalesUnitPrice
    FROM SCZSSCM..SCM_Product SP WITH (NOLOCK)
    LEFT JOIN SCZSSCM..SCM_Material SM WITH (NOLOCK) ON SP.MaterialCode = SM.MaterialCode
    INNER JOIN SCZSSCM..SCM_Material_Images SMI WITH (NOLOCK) ON SM.BaseMaterialCode = SMI.CorrelationId
    WHERE SP.ProductState IN (1, 3) AND SM.IsDel = 0
)

-- 查询主表数据
SELECT 
    '' AS ProductLineCode,
    0 AS PID,
    3 AS SaleWay,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM SCZSCQS..CQS_RequiredItem_Config RC
            INNER JOIN SCZSCQS..CQS_RequiredItem_Config_T RCT ON RC.CODE = RCT.CODE
            WHERE RC.IsUse = 1 AND RC.CompanyCode = PD.CompanyCode AND RCT.ProductCategoryCode = PD.ProductCategoryCode
        ) 
        THEN 1 
        ELSE 0 
    END AS IsRequired,
    0 AS IsMain,
    PD.SalesUnitPrice,
    PD.ProductCode,
    PD.ProductName,
    PD.MaterialCode,
    PD.ProductCategory,
    PD.ProductCategoryCode,
    PD.ProductType,
    PD.ProductTypeCode,
    PD.ProductSubclass,
    PD.ProductSubclassCode,
    PD.BrandName,
    PD.BrandCode,
    PD.Series,
    PD.Marque,
    PD.Color,
    PD.MaterialQuality,
    PD.ImgUrl,
    PD.SmallImgUrl,
    PD.SpecLong,
    PD.SpecWide,
    PD.SpecHigh,
    PD.Spec,
    PD.ProductionPlace,
    PD.ProductState,
    PD.CompanyCode
FROM ProductDetails PD

UNION 

-- 查询子表数据
SELECT 
    ISNULL((SELECT TOP 1 ISNULL(ProductLineCode, '') FROM SCZSSCM..V_ProductLineSpacePart_RelatedProduct WHERE ID = SLIP.PID), '') AS ProductLineCode,
    SLIP.PID,
    CASE 
        WHEN SLIP.SaleWay = '标配' THEN 1 
        ELSE 2 
    END AS SaleWay,
    CASE 
        WHEN SLIP.SaleWay IN ('标配', '升级') THEN 1 
        ELSE 0 
    END AS IsRequired,
    0 AS IsMain,
    SLIP.AddPrice AS SalesUnitPrice,
    T.ProductCode,
    T.ProductName,
    T.MaterialCode,
    T.ProductCategory,
    T.ProductCategoryCode,
    T.ProductType,
    T.ProductTypeCode,
    T.ProductSubclass,
    T.ProductSubclassCode,
    T.BrandName,
    T.BrandCode,
    T.Series,
    T.Marque,
    T.Color,
    T.MaterialQuality,
    T.ImgUrl,
    T.SmallImgUrl,
    T.SpecLong,
    T.SpecWide,
    T.SpecHigh,
    T.Spec,
    T.ProductionPlace,
    T.ProductState,
    T.CompanyCode
FROM SCZSSCM..SCM_ProductLine_Item_Product_T SLIP
INNER JOIN ProductDetails T ON SLIP.ProCode = T.ProductCode
WHERE SLIP.PId > 0;
