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
