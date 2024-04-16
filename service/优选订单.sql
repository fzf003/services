
UPDATE FSD SET FSD.SettlementAmount=(SELECT SUM(CaiGouJiaTotal) FROM SCYXDATA..SCM_Order_T WHERE OrderCode in(FPTS.OrderCode))
FROM SCYXDATA..SCM_Order_M As FSD
INNER JOIN  (SELECT OrderCode FROM SCYXDATA..SCM_Order_M  WHERE SettlementAmount in(-28313.10)) AS FPTS
ON FSD.OrderCode=FPTS.OrderCode
WHERE  FSD.OrderCode  IN (select OrderCode from SCYXDATA..SCM_Order_M  where SettlementAmount in(-28313.10))



  SELECT orderNum,CaiGouJiaTotal,* FROM SCYXDATA..SCM_Order_T WHERE orderNum<0 and CaiGouJiaTotal>0  

 
USE SCYXDATA


Declare @OrderCode nvarchar(200)='STROD202404050055' 

update SCYXDATA..SCM_Order_T set CaiGouJiaTotal=CaiGouJiaTotal*-1 where OrderCode in(@OrderCode)
 AND CaiGouJiaTotal>0 and OrderNum<0

 update SCYXDATA..SCM_Order_M set SettlementAmount=(select SUM(CaiGouJiaTotal) from SCYXDATA..SCM_Order_T where OrderCode in(@OrderCode))
 where OrderCode in(@OrderCode)
 

update    JZDATA..SCM_Order_T2 set DealSettlePriceTotal=DealSettlePriceTotal*-1 where OrderCode in(@OrderCode)  AND DealSettlePriceTotal>0 and Number<0

update   JZDATA..SCM_Order_M    set SettlementAmount=(select SUM(DealSettlePriceTotal) from JZDATA..SCM_Order_T2  where OrderCode in(@OrderCode))   where OrderCode in(@OrderCode)


 

---结算金额为正
Declare @OrderCode nvarchar(200)='STROD202211040054' 

update SCYXDATA..SCM_Order_T set CaiGouJiaTotal=CaiGouJiaTotal*-1 where OrderCode in(@OrderCode)

 update SCYXDATA..SCM_Order_M set SettlementAmount=(select SUM(CaiGouJiaTotal) from SCYXDATA..SCM_Order_T where OrderCode in(@OrderCode))
 where OrderCode in(@OrderCode)

update    JZDATA..SCM_Order_T2 set DealSettlePriceTotal=DealSettlePriceTotal*-1 where OrderCode in(@OrderCode)

update   JZDATA..SCM_Order_M    set SettlementAmount=(select SUM(DealSettlePriceTotal) from JZDATA..SCM_Order_T2  where OrderCode in(@OrderCode))   where OrderCode in(@OrderCode)


---订单数量为正
Declare @OrderCode nvarchar(200)='STROD202209140033' 

update SCYXDATA..SCM_Order_T set CaiGouJiaTotal=CaiGouJiaTotal*-1,OrderNum=OrderNum*-1,QuantityNum=QuantityNum*-1 where OrderCode in(@OrderCode)

 update SCYXDATA..SCM_Order_M set SettlementAmount=(select SUM(CaiGouJiaTotal) from SCYXDATA..SCM_Order_T where OrderCode in(@OrderCode)
)
where OrderCode in(@OrderCode)

update    JZDATA..SCM_Order_T2 set DealSettlePriceTotal=DealSettlePriceTotal*-1,JSPrcieTotal=JSPrcieTotal*-1,Number=Number*-1 where OrderCode in(@OrderCode)

update   JZDATA..SCM_Order_M    set SettlementAmount=(select SUM(DealSettlePriceTotal) from JZDATA..SCM_Order_T2  where OrderCode in(@OrderCode))   where OrderCode in(@OrderCode)


