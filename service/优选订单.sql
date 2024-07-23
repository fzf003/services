



UPDATE FSD SET FSD.SettlementAmount=(SELECT SUM(CaiGouJiaTotal) FROM SCYXDATA..SCM_Order_T WHERE OrderCode in(FPTS.OrderCode))
FROM SCYXDATA..SCM_Order_M As FSD
INNER JOIN  (SELECT OrderCode FROM SCYXDATA..SCM_Order_M  WHERE SettlementAmount in(-28313.10)) AS FPTS
ON FSD.OrderCode=FPTS.OrderCode
WHERE  FSD.OrderCode  IN (select OrderCode from SCYXDATA..SCM_Order_M  where SettlementAmount in(-28313.10))

  select  state,OrderCode,* from SCYXDATA..SCM_Order_M
  WHERE TaskID IN( SELECT TaskID FROM SCYXDATA..SCM_Order_T WHERE orderNum<0 and CaiGouJiaTotal>0  )
  ORDER BY CreateTime DESC 

  SELECT orderNum,CaiGouJiaTotal,* FROM SCYXDATA..SCM_Order_T WHERE orderNum<0 and CaiGouJiaTotal>0  

 
USE SCYXDATA


  -----没有结算金额----------------------------------------------------------------------------------------------------------------------------------------

  

SELECT ProjectCode,SalaWay,OrderNum,CaiGouJia,CaiGouJiaTotal,TotalPrice,JSPrcieTotal,* FROM SCYXDATA..SCM_Order_T WHERE orderCode in('STROD202406130077')

AND SalaWay IN('标配') AND ISNULL(CaiGouJiaTotal,0)=0 




SELECT TaskId,Num,ProCode,CaiGouJia,CaiGouJiaTotal,Active_CaiGoujiaTotal,JSPrcieTotal,SalaWay,ISConversion,* FROM SCYXDATA..PM_Price_T WHERE ID IN(
SELECT ProjectID FROM SCYXDATA..SCM_Order_T WHERE orderCode in('STROD202406130079'))
and SalaWay in('标配') AND   ISNULL(CaiGouJiaTotal,0)=0

Update  SCYXDATA..PM_Price_T SET CaiGouJiaTotal=JSPrcieTotal,Active_CaiGoujiaTotal=JSPrcieTotal WHERE ID IN(
SELECT ProjectID FROM SCYXDATA..SCM_Order_T WHERE orderCode in('STROD202406130080'))
and SalaWay in('标配') AND   ISNULL(CaiGouJiaTotal,0)=0


SELECT TaskId,ProCode,CaiGouJia,CaiGouJiaTotal,Active_CaiGoujiaTotal,JSPrcieTotal,SalaWay,ISConversion,* FROM SCYXDATA..PM_Price_T WHERE M_ID IN(12988)
 AND Taskid>0 and SalaWay in('标配') AND  (ISNULL(CaiGouJiaTotal,0)=0 or ISNULL(Active_CaiGoujiaTotal,0)=0)

   

Update  SCYXDATA..PM_Price_T SET CaiGouJiaTotal=JSPrcieTotal,Active_CaiGoujiaTotal=JSPrcieTotal WHERE ID IN(
SELECT ProjectID FROM SCYXDATA..SCM_Order_T WHERE orderCode in('STROD202406130079'))
and SalaWay in('标配') AND   ISNULL(CaiGouJiaTotal,0)=0



 select ProjectCode,SalaWay,OrderNum,CaiGouJia,CaiGouJiaTotal,TotalPrice,JSPrcieTotal,* from SCYXDATA..SCM_Order_T where ProjectID in(
 SELECT Id  FROM SCYXDATA..PM_Price_T WHERE M_ID IN(12988)
 AND Taskid>0 and SalaWay in('标配') AND  (ISNULL(CaiGouJiaTotal,0)=0 or ISNULL(Active_CaiGoujiaTotal,0)=0))


  
 

 
Declare @OrderCode nvarchar(200)='STROD202306230073' 

update SCYXDATA..SCM_Order_T set CaiGouJiaTotal=JSPrcieTotal where OrderCode in(@OrderCode) AND  SalaWay IN('标配') AND ISNULL(CaiGouJiaTotal,0)=0 

update SCYXDATA..SCM_Order_M set SettlementAmount=(select SUM(CaiGouJiaTotal) from SCYXDATA..SCM_Order_T where OrderCode in(@OrderCode)) where OrderCode in(@OrderCode)
 
Update  SCYXDATA..PM_Price_T SET CaiGouJiaTotal=JSPrcieTotal,Active_CaiGoujiaTotal=JSPrcieTotal WHERE ID IN(
SELECT ProjectID FROM SCYXDATA..SCM_Order_T WHERE orderCode in(@OrderCode))
and SalaWay in('标配') AND ISNULL(CaiGouJiaTotal,0)=0

update    JZDATA..SCM_Order_T2 set DealSettlePriceTotal=JSPrcieTotal where OrderCode in(@OrderCode)  

update   JZDATA..SCM_Order_M    set SettlementAmount=(select SUM(DealSettlePriceTotal) from JZDATA..SCM_Order_T2  where OrderCode in(@OrderCode))   where OrderCode in(@OrderCode)








  

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


----撤销订单
Declare @OrderCode nvarchar(200)='STROD202403290061' 

 UPDATE SCYXDATA..SCM_Order_M set SupplierCode='删除-'+SupplierCode,state=-1,CusCode=CusCode+'-1' where OrderCode in(@OrderCode)

 UPDATE SCYXDATA..SCM_Order_T set ProjectID=ProjectID*-1 where  OrderCode in(@OrderCode)

 UPDATE BPMDB.dbo.BPMInstTasks SET State='Aborted' WHERE Taskid in (
 SELECT TaskID FROM SCYXDATA..SCM_Order_M WHERE OrderCode in(@OrderCode)
)

---审批完成未结算
IF EXISTS(SELECT * FROM JZDATA..SCM_Order_M WHERE  OrderCode in(@OrderCode))
BEGIN
   UPDATE JZDATA..SCM_Order_M SET STATE=-1 WHERE  OrderCode in(@OrderCode)
END
  
 

 

------结算金额为正
Declare @OrderCode nvarchar(200)='STROD202404050055' 

update SCYXDATA..SCM_Order_T set CaiGouJiaTotal=CaiGouJiaTotal*-1 where OrderCode in(@OrderCode) AND CaiGouJiaTotal>0 and OrderNum<0

update SCYXDATA..SCM_Order_M set SettlementAmount=(select SUM(CaiGouJiaTotal) from SCYXDATA..SCM_Order_T where OrderCode in(@OrderCode)) where OrderCode in(@OrderCode)
 

update    JZDATA..SCM_Order_T2 set DealSettlePriceTotal=DealSettlePriceTotal*-1 where OrderCode in(@OrderCode)  AND DealSettlePriceTotal>0 and Number<0

update   JZDATA..SCM_Order_M    set SettlementAmount=(select SUM(DealSettlePriceTotal) from JZDATA..SCM_Order_T2  where OrderCode in(@OrderCode))   where OrderCode in(@OrderCode)

  

---订单数量为正
Declare @OrderCode nvarchar(200)='STROD202209140033' 

update SCYXDATA..SCM_Order_T set CaiGouJiaTotal=CaiGouJiaTotal*-1,OrderNum=OrderNum*-1,QuantityNum=QuantityNum*-1 where OrderCode in(@OrderCode)

 update SCYXDATA..SCM_Order_M set SettlementAmount=(select SUM(CaiGouJiaTotal) from SCYXDATA..SCM_Order_T where OrderCode in(@OrderCode)
)
where OrderCode in(@OrderCode)

update    JZDATA..SCM_Order_T2 set DealSettlePriceTotal=DealSettlePriceTotal*-1,JSPrcieTotal=JSPrcieTotal*-1,Number=Number*-1 where OrderCode in(@OrderCode)

update   JZDATA..SCM_Order_M    set SettlementAmount=(select SUM(DealSettlePriceTotal) from JZDATA..SCM_Order_T2  where OrderCode in(@OrderCode))   where OrderCode in(@OrderCode)


