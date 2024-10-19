SELECT f.FoolSpacePartName, PM_Price_T.PartId,PM_Price_T.TaskId,PM_Price_T.Id,Num,convertcaigounum,ConvertArea,convertPrice,ProCode,ProName,CaiGouJia,CaiGouJiaTotal,Active_CaiGoujiaTotal,JSPrcieTotal,SalaWay,ISConversion,convertcaigounum,*
FROM SCYXDATA..PM_Price_T 
left join SCYXDATA..V_Part_Infor f
on SCYXDATA..PM_Price_T.PartID=f.ID
WHERE   PM_Price_T.M_ID in(17392) 
       And ProCode in('PRO20240219_217735_49','PRO20240219_518336_16') 
       and AlonePrice>0 and SalaWay in('标配')




   select * from SCYXDATA..PM_Price_M where id in(17392)

 select * from SCYXDATA..PM_Price_T where M_ID in(17392) and ProCode in('PRO20240219_518336_16')
