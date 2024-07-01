
/****
查询活动信息
  
select IsActiveValid,ActiveId,Status,* from SCYXDATA..CRM_PackageContract  where TaskID in(7150185) 

select * from SCYXDATA..CRM_CancelContract where cusCode in('CUS2403050021')

select * from SCYXDATA..CRM_ConstructionContract_M  where CusCode='CUS2403050021'

select * from SCYXDATA..Base_ActiveConfig_M where taskid in(7121303)

select * from SCYXDATA..PM_Price_M where id in (16629)


****/

















declare @Cuscode nvarchar(50)='CUS2307220006';
 
WITH t AS(
SELECT   ROW_NUMBER() OVER(order by OrderStateCode ) as RowNum, a.*,ISNULL(c.OrderState,'未下单') AS OrderState,ISNULL(c.OrderStateCode,0) AS OrderStateCode,ISNULL(b.TaskID,0) AS TaskID,ISNULL(d.ContractAmt,0) AS ContractAmt,ISNULL(e.DownpaymentDate,CONVERT(DATETIME,'1970-01-01 00:00:00')) AS DownpaymentDate FROM (
SELECT s.SN,s.ID,s.Supplier,s.SupplierCode,s.PtName,s.Cuscode,s.type,s.PtCode,s.category,s.SalaWay,s.backcount FROM (
SELECT ROW_NUMBER() OVER(PARTITION BY SupplierCode,PtName,SN,[Type],category ORDER BY PtName) AS nums,* FROM (
SELECT  a.Taskid,(SELECT  b.SerialNum FROM   BPMDB..BPMInstTasks  b WITH(NOLOCK)  WHERE b.TaskId =a.Taskid) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,
(SELECT   c.Type FROM  SCYXDATA..PM_Price_Change_M c WITH(NOLOCK) WHERE c.Taskid=a.Taskid) AS  category, (select count(1) from SCYXDATA..PM_Price_T t2  
					where Taskid =a.Taskid
					and t2.Type='减' 
					and t2.OrderState!='已下单') as backcount FROM  SCYXDATA..PM_Price_T a WITH(NOLOCK)  WHERE Flag IN (5,6) AND a.UseState=1 AND a.Cuscode=@Cuscode
UNION ALL
SELECT  a.Taskid,(SELECT  b.SerialNum FROM   BPMDB..BPMInstTasks  b WITH(NOLOCK)  WHERE b.TaskId =a.Taskid) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,
'活动产品包变更' AS  category, (SELECT COUNT(1) FROM SCYXDATA..PM_Price_T t2 WITH(NOLOCK)  
					WHERE Taskid =a.Taskid
					AND t2.Type='减' 
					AND t2.OrderState!='已下单') AS backcount FROM  SCYXDATA..PM_Price_T a WITH(NOLOCK)  WHERE Flag IN (8) AND a.UseState=1  AND ABS(ISNULL(a.Num,0))>0 AND a.Cuscode=@Cuscode 
UNION ALL
SELECT a.Taskid,(SELECT   b.ContractNo FROM  SCYXDATA..CRM_ConstructionContract_M b WITH(NOLOCK)  WHERE b.CusCode=a.Cuscode AND b.State=1 ) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,
'优选整装' AS category, 0 as backcount FROM  SCYXDATA..PM_Price_T a WITH(NOLOCK)  WHERE Flag IN (1,2,3,4) AND a.UseState=1 AND a.Cuscode=@Cuscode
UNION ALL
SELECT a.Taskid,(SELECT   b.ContractNo FROM  SCYXDATA..CRM_ConstructionContract_M b WITH(NOLOCK)  WHERE b.CusCode=a.Cuscode AND b.State=1 ) SN ,a.ID, a.Supplier,a.SupplierCode,a.PtName,a.Cuscode,ISNULL(a.[Type],'正常') AS [type],a.PtCode,a.SalaWay,
'活动产品包' AS category, 0 as backcount FROM  SCYXDATA..PM_Price_T a WITH(NOLOCK)  WHERE Flag IN (7) AND a.UseState=1  AND ABS(ISNULL(a.Num,0))>0 AND a.Cuscode=@Cuscode


) f ) s WHERE s.nums=1 ) a 
LEFT JOIN dbo.SCM_Order_T b WITH(NOLOCK) ON a.ID = b.ProjectID 
LEFT join dbo.SCM_Order_M c WITH(NOLOCK) on b.OrderCode = c.OrderCode 
LEFT JOIN dbo.CRM_CollectionRelationContract d WITH(NOLOCK) ON a.SN=d.ContractCode 
LEFT JOIN JZDATA..CRM_Customer e WITH(NOLOCK) ON a.Cuscode=e.CusCode 
WHERE 1 = 1  ) 

SELECT
    *
   -- ROW_NUMBER ( ) OVER ( ORDER BY RowNum   ) AS RowId 
FROM
    t 
	where  SN in('DZCN202309280011') and SupplierCode='NKGS0456'



   





select * from SCYXDATA..PM_Price_Change_M where taskid in(6739103)

  select * from SCYXDATA..PM_Price_T WHERE Taskid IN(6739103)

 UPDATE SCYXDATA..PM_Price_T SET usestate=-1 WHERE Taskid in(6739103)
 
 UPDATE BPMDB.dbo.BPMInstTasks SET State='Aborted' WHERE Taskid in (6739103)
 UPDATE SCYXDATA..PM_Price_Change_M SET Status=3 where taskid in(6739103)

UPDATE BPMDB.dbo.BPMInstTasks SET State='Aborted' WHERE Taskid in (
select TaskID from SCYXDATA..SCM_Order_M where OrderCode in('STROD202311080007','STROD202311080010','STROD202311080009','STROD202311080008')
)




UPDATE SCYXDATA..SCM_Order_M set state=-1  where  TaskID in(select TaskID from SCYXDATA..SCM_Order_M where OrderCode in('STROD202311080007','STROD202311080010','STROD202311080009','STROD202311080008')
)

UPDATE  SCYXDATA..SCM_Order_T  set  ProjectID=ProjectID*-1 where TaskID in(select TaskID from SCYXDATA..SCM_Order_M where OrderCode in('STROD202311080007','STROD202311080010','STROD202311080009','STROD202311080008')
)
 

update SCYXDATA..SCM_Order_m set SupplierCode='删除-'+SupplierCode where OrderCode in('STROD202311080007','STROD202311080010','STROD202311080009','STROD202311080008')





 select * from JZDATA..SCM_SYS_SupplierMaintenance_T
where SupplierCode in('ERKS0043') 

update JZDATA..SCM_SYS_SupplierMaintenance_T set ProBrandCode='BL210715'
where SupplierCode in('ERKS0043')  and ProBrand in('德国海福乐（优选）')




DECLARE  @CusCode NVARCHAR(100)='CUS2303240016';
DECLARE  @CompanyCode INT=1;
DECLARE  @MID INT=12544;
DECLARE  @CompanyName NVARCHAR(100)='公司';
DECLARE  @RMID INT=11259;
DECLARE  @RVersion NVARCHAR(100)='SGCPX20230802181554333'
DECLARE  @Version NVARCHAR(100)='SGCPX20230802181554333'
SELECT * FROM (  SELECT ss.DetailClass, ss.Taskid,          
                            ss.Cuscode,                         
                            ss.Supplier,                
                            ss.SupplierCode,            
                            ss.DetailNO,     
                            ss.DetailName,    
                            ss.Unit,          
                            ss.UnitPrice,     
                            ss.ProcessDescription,  
                            ss.ComputeMethod,              
                            ss.TotalPrice,         
                            ss.Num,                 
                            ss.CompanyName,        
                            ss.CompanyCode,         
                            ss.M_ID,                
                            ss.PartID,              
                            ss.Flag,                
                            ss.UseState,            
                            ss.CreateDate,          
                            pp.FoolSpacePartID, 
                            ss.SubQuotaName,
                            ss.PositionName,
                            ss.IsRequired,
                            ss.eSort,
							ss.fSort,
							ss.cSort,
                            ss.SortNO
                            FROM                        
                            (                                   
                            SELECT DISTINCT    
                            d.DetailClass,     
                            -1 AS Taskid,    
                            @CusCode AS Cuscode,   
                            '' AS Supplier, 
                            '' AS SupplierCode,     
                            d.DetailNO,          
                            d.DetailName,        
                            d.Unit,               
                            d.UnitPrice,           
                            d.ProcessDescription,    
                            d.ComputeMethod,        
                            0 AS TotalPrice,         
                            t.Num AS Num,            
                            @CompanyName AS CompanyName,   
                            @CompanyCode AS CompanyCode,   
                            @MID AS M_ID,     
                            -1 AS PartID,        
                            1 AS Flag,        
                            0 AS UseState,       
                            GETDATE() AS CreateDate,     
                            t.FoolName,                 
                            t.SpaceID,         
                            t.UseName,   
                            d.SubQuotaName,      
                            d.PositionName,    
                            d.IsRequired,
                            t.eSort,
							t.fSort,
							t.cSort,
                            t.SortNO
                            FROM   (  SELECT gzt.*,  hp.FoolName, 
                            hp.SpaceID,     
                            hp.UseName    
                            FROM SCYXDATA..PM_Price_GZ_T gzt WITH (NOLOCK) 
                            INNER JOIN SCYXDATA..PM_ProjectSpaceInfor_Part hp WITH (NOLOCK)
                            ON gzt.FoolSpacePartId = hp.ID  ) t INNER JOIN SCYXDATA.[dbo].[PM_Price_Part] p WITH (NOLOCK)  ON t.M_ID = p.M_ID  AND t.FoolName = p.FoolName AND t.SpaceID = p.SpaceID
                            INNER JOIN SCYXDATA..V_GZ_ProductLineView d WITH (NOLOCK)  ON d.DetailNO = t.DetailNO  WHERE t.M_ID = @RMID  and d.VersionCode=@RVersion ) AS ss     
                            INNER JOIN  ( 
                            SELECT DISTINCT   p2.FoolName, p2.SpaceID,p2.FoolSpacePartID,  hp2.UseName FROM SCYXDATA.[dbo].[PM_Price_Part] p2 WITH (NOLOCK)  INNER JOIN SCYXDATA..PM_ProjectSpaceInfor_Part hp2 WITH (NOLOCK)
                            ON p2.FoolSpacePartID = hp2.ID        
                            --INNER JOIN SCYXDATA..PM_Price_GZ_T gzt2 WITH (NOLOCK)
                            -- ON p2.FoolSpacePartID = gzt2.FoolSpacePartId 
                            WHERE p2.M_ID = @MID ) pp  ON ss.FoolName = pp.FoolName  AND ss.SpaceID = pp.SpaceID AND ss.UseName=pp.UseName   ) T 
                            WHERE T.DETAILNO IN (     
							SELECT * FROM SCYXDATA.dbo.QueryMatchProductLine(@CompanyCode,@RVersion,@Version)
                            )

