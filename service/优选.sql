
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

