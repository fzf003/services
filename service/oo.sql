


WITH t AS(
SELECT   ROW_NUMBER() OVER(order by OrderStateCode ) as RowNum, a.*,ISNULL(c.OrderState,'未下单') AS OrderState,ISNULL(c.OrderStateCode,0) AS OrderStateCode,ISNULL(b.TaskID,0) AS TaskID,ISNULL(d.ContractAmt,0) AS ContractAmt,ISNULL(e.DownpaymentDate,CONVERT(DATETIME,'1970-01-01 00:00:00')) AS DownpaymentDate,b.ProjectID  FROM
(


SELECT s.SN,s.ID,s.Supplier,s.SupplierCode,s.PtName,s.Cuscode,s.type,s.PtCode,s.category,s.SalaWay,s.backcount FROM (

SELECT ROW_NUMBER() OVER(PARTITION BY SupplierCode,PtName,SN,[Type],category ORDER BY PtName) AS nums,* FROM (

SELECT  a.Taskid,(SELECT  b.SerialNum FROM   BPMDB..BPMInstTasks  b  WHERE b.TaskId =a.Taskid) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,

(SELECT   c.Type FROM  SCYXDATA..PM_Price_Change_M c WHERE c.Taskid=a.Taskid) AS  category, 
(select count(1) from SCYXDATA..PM_Price_T t2  
					where Taskid =a.Taskid
					and t2.Type='减' 
					and t2.OrderState!='已下单') as backcount FROM  SCYXDATA..PM_Price_T a  
					WHERE Flag IN (5,6) AND a.UseState=1 AND a.Cuscode='CUS2401160038' 
					-- and Taskid in(7701503)

					--and ProCode in('PRO20240315_136040_5')
UNION ALL
SELECT a.Taskid,(SELECT   b.ContractNo FROM  SCYXDATA..CRM_ConstructionContract_M b  WHERE b.CusCode=a.Cuscode AND b.State=1 ) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,
'优选整装' AS category, 0 as backcount FROM  SCYXDATA..PM_Price_T a  WHERE Flag IN (1,2,3,4) AND a.UseState=1 AND a.Cuscode='CUS2401160038') f
 
) s WHERE s.nums=1   

) a  

LEFT JOIN dbo.SCM_Order_T b ON a.ID = b.ProjectID 
LEFT join dbo.SCM_Order_M c on b.OrderCode = c.OrderCode 
LEFT JOIN dbo.CRM_CollectionRelationContract d ON a.SN=d.ContractCode 
LEFT JOIN JZDATA..CRM_Customer e ON a.Cuscode=e.CusCode 
WHERE 1 = 1  --AND a.Cuscode='CUS2110310106' and  a.SN in('CTUYXSGB2100032') and   a.SupplierCode in('CTUS0361')

) 

 SELECT
    *
   -- ROW_NUMBER ( ) OVER ( ORDER BY RowNum   ) AS RowId 
FROM
    t 
	where 1=1    and t.SN in('ZCBG202410170015') 
	and t.SupplierCode in('PEKS1747')
	--and t.ProCode in('PRO20240315_136040_5')

	
	and (t.SN in(SELECT ContractCode FROM SCYXDATA..CRM_CollectionRelationContract WHERE States=1 AND CusCode='CUS2401160038'
and ContractType in('主材变更','定制变更','施工变更','服务包变更')) or t.type in('正常') )
ORDER BY
    RowNum offset 0 ROWS FETCH NEXT 15 ROWS ONLY






select * from SCYXDATA..PM_price_T where TaskId in(7701503)

select * from SCYXDATA..PM_price_Change_M where SN in('ZCBG202410170015')


select Id,* from SCYXDATA..PM_price_T where  Taskid in(7701503)
 and ProCode in('PRO20240315_136040_5')

select * from SCYXDATA..PM_price_T where  Id in(2004336)

select * from SCYXDATA..SCM_Order_T where ProjectId in(select Id from SCYXDATA..PM_price_T where  Taskid in(7701503))

 and ProjectCode in('PRO20240315_136040_5')

 

 



update SCYXDATA..PM_price_T set M_ID=17380 where id in (2004336)








WITH t AS(
SELECT   ROW_NUMBER() OVER(order by OrderStateCode ) as RowNum, a.*,ISNULL(c.OrderState,'未下单') AS OrderState,ISNULL(c.OrderStateCode,0) AS OrderStateCode,ISNULL(b.TaskID,0) AS TaskID,ISNULL(d.ContractAmt,0) AS ContractAmt,ISNULL(e.DownpaymentDate,CONVERT(DATETIME,'1970-01-01 00:00:00')) AS DownpaymentDate FROM (
SELECT s.SN,s.ID,s.Supplier,s.SupplierCode,s.PtName,s.Cuscode,s.type,s.PtCode,s.category,s.SalaWay,s.backcount FROM (
SELECT ROW_NUMBER() OVER(PARTITION BY SupplierCode,PtName,SN,[Type],category ORDER BY PtName) AS nums,* FROM (
SELECT  a.Taskid,(SELECT  b.SerialNum FROM   BPMDB..BPMInstTasks  b WITH(NOLOCK)  WHERE b.TaskId =a.Taskid) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,
(SELECT   c.Type FROM  SCYXDATA..PM_Price_Change_M c WITH(NOLOCK) WHERE c.Taskid=a.Taskid) AS  category, (select count(1) from SCYXDATA..PM_Price_T t2  
					where Taskid =a.Taskid
					and t2.Type='减' 
					and t2.OrderState!='已下单') as backcount FROM  SCYXDATA..PM_Price_T a WITH(NOLOCK)  WHERE Flag IN (5,6) AND a.UseState=1 AND a.Cuscode='CUS2411080002' 
UNION ALL
SELECT  a.Taskid,(SELECT  b.SerialNum FROM   BPMDB..BPMInstTasks  b WITH(NOLOCK)  WHERE b.TaskId =a.Taskid) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,
'活动产品包变更' AS  category, (SELECT COUNT(1) FROM SCYXDATA..PM_Price_T t2 WITH(NOLOCK)  
					WHERE Taskid =a.Taskid
					AND t2.Type='减' 
					AND t2.OrderState!='已下单') AS backcount FROM  SCYXDATA..PM_Price_T a WITH(NOLOCK)  WHERE Flag IN (8) AND a.UseState=1  AND ABS(ISNULL(a.Num,0))>0 AND a.Cuscode='CUS2411080002' 
UNION ALL
SELECT a.Taskid,(SELECT   b.ContractNo FROM  SCYXDATA..CRM_ConstructionContract_M b WITH(NOLOCK)  WHERE b.CusCode=a.Cuscode AND b.State=1 ) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,
'优选整装' AS category, 0 as backcount FROM  SCYXDATA..PM_Price_T a WITH(NOLOCK)  WHERE Flag IN (1,2,3,4) AND a.UseState=1 AND a.Cuscode='CUS2411080002'
UNION ALL
SELECT a.Taskid,(SELECT   b.ContractNo FROM  SCYXDATA..CRM_ConstructionContract_M b WITH(NOLOCK)  WHERE b.CusCode=a.Cuscode AND b.State=1 ) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,
'活动产品包' AS category, 0 as backcount FROM  SCYXDATA..PM_Price_T a WITH(NOLOCK)  WHERE Flag IN (7) AND a.UseState=1  AND ABS(ISNULL(a.Num,0))>0 AND a.Cuscode='CUS2411080002'
) f ) s WHERE s.nums=1 ) a 
LEFT JOIN dbo.SCM_Order_T b WITH(NOLOCK) ON a.ID = b.ProjectID 
LEFT join dbo.SCM_Order_M c WITH(NOLOCK) on b.OrderCode = c.OrderCode 
LEFT JOIN dbo.CRM_CollectionRelationContract d WITH(NOLOCK) ON a.SN=d.ContractCode 
LEFT JOIN JZDATA..CRM_Customer e WITH(NOLOCK) ON a.Cuscode=e.CusCode 
WHERE 1 = 1  AND a.Cuscode='CUS2411080002') 

SELECT
    *
   -- ROW_NUMBER ( ) OVER ( ORDER BY RowNum   ) AS RowId 
FROM
    t 
	where 1=1   and (t.SN in(SELECT ContractCode FROM SCYXDATA..CRM_CollectionRelationContract WHERE States=1 AND CusCode='CUS2411080002'
and ContractType in('主材变更','定制变更','施工变更','服务包变更')) or t.type in('正常') or t.category in('活动产品包变更') )
ORDER BY
    RowNum offset 0 ROWS FETCH NEXT 15 ROWS ONLY

