
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

