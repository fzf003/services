
首期款:
(heji+heji3+heji12+heji13+heji8+heji9+heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.5+heji2+heji5+heji6+heji1+heji4+heji11



(C_heji_NoM2+C_heji3+C_heji12+C_heji13+C_heji8+C_heji9+C_heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.5+C_heji2+C_heji5+C_heji6+C_heji1+C_heji4+C_heji11-SCYXDATA:CRM_ConstructionContract_M.DiscountAmount-M2Amount1



基础整装: heji  C_heji
主材升级: heji1  C_heji1
主材单独销售: heji2  C_heji2
个性化施工: heji3  C_heji3
定制升级: heji4  C_heji4
定制单独销售: heji5  C_heji5
服务包: heji6  C_heji6
标配物料远程运输费: heji12  C_heji12
单独销售物料远程运输费: heji13  C_heji13
远程施工费: heji8  C_heji8
搬运费: heji9  C_heji9
垃圾清理费: heji10  C_heji10 
活动产品包: heji11  C_heji11

首期基础整装: Response.data.JCRatio_1,
			二期基础整装: Response.data.JCRatio_2,
			中期基础整装: Response.data.JCRatio_3,
			尾期基础整装: Response.data.JCRatio_4,
			首期个性化施工: Response.data.GxRatio_1,
			二期个性化施工: Response.data.GxRatio_2,
			中期个性化施工: Response.data.GxRatio_3,
			尾期个性化施工: Response.data.GxRatio_4,
			首期远程施工费: Response.data.YCFRatio_1,
			二期远程施工费: Response.data.YCFRatio_2,
			中期远程施工费: Response.data.YCFRatio_3,
			尾期远程施工费: Response.data.YCFRatio_4,
			首期搬运费: Response.data.TransportRatio_1,
			二期搬运费: Response.data.TransportRatio_2,
			中期搬运费: Response.data.TransportRatio_3,
			尾期搬运费: Response.data.TransportRatio_4,
			首期垃圾清理费: Response.data.RabishRatio_1,
			二期垃圾清理费: Response.data.RabishRatio_2,
			中期垃圾清理费: Response.data.RabishRatio_3,
			尾期垃圾清理费: Response.data.RabishRatio_4,
			首期远程运输费: Response.data.YCTransportRatio_1,
			二期远程运输费: Response.data.YCTransportRatio_2,
			中期远程运输费: Response.data.YCTransportRatio_3,
			尾期远程运输费: Response.data.YCTransportRatio_4,
			首期主材升级: Response.data.ZCRatio_1,
			二期主材升级: Response.data.ZCXSRatio_2,
			中期主材升级: Response.data.ZCXSRatio_3,
			尾期主材升级: Response.data.ZCRatio_4,
			首期主材单独销售: Response.data.ZCXSRatio_1,
			二期主材单独销售: Response.data.DZXSRatio_2,
			中期主材单独销售: Response.data.DZXSRatio_3,
			尾期主材单独销售: Response.data.ZCXSRatio_4,
			首期定制升级: Response.data.DZRatio_1,
			二期定制升级: Response.data.DZRatio_2,
			中期定制升级: Response.data.DZRatio_3,
			尾期定制升级: Response.data.DZRatio_4,
			首期定制单独销售: Response.data.DZXSRatio_1,
			二期定制单独销售: Response.data.DZXSRatio_2,
			中期定制单独销售: Response.data.DZXSRatio_3,
			尾期定制单独销售: Response.data.DZXSRatio_4,
			首期服务包: Response.data.FwbRatio_1,
			二期服务包: Response.data.FwbRatio_2,
			中期服务包: Response.data.FwbRatio_3,
			尾期服务包: Response.data.FwbRatio_4,
			首期产品包: Response.data.HdbRatio_1,
			二期产品包: Response.data.HdbRatio_2,
			中期产品包: Response.data.HdbRatio_3,
			尾期产品包: Response.data.HdbRatio_4
	



活动前
首期款:
XTextBox11
(基础整装+个性化施工+标配物料远程运输费+单独销售物料远程运输费+远程施工费+搬运费+垃圾清理费-整单促销)*0.5+主材单独销售+定制单独销售+服务包+主材升级+定制升级+活动产品包

(heji*JCRatio_1+heji3*GxRatio_1+(heji12+heji13)*YCTransportRatio_1+heji8*YCFRatio_1+heji9*TransportRatio_1+heji10*RabishRatio_1-SCYXData:CRM_ConstructionContract_M.PromotionAmount*JCRatio_1)+heji2*ZCXSRatio_1+heji5*DZXSRatio_1+heji6*FwbRatio_1+heji1*ZCRatio_1+heji4*DZRatio_1+heji11*HdbRatio_1




(heji+heji3+heji12+heji13+heji8+heji9+heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.5+heji2+heji5+heji6+heji1+heji4+heji11

二期：
(基础整装+个性化施工+标配物料远程运输费+单独销售物料远程运输费+远程施工费+搬运费+垃圾清理费-整单促销)*0.3

((heji*JCRatio_2)+(heji3*GxRatio_2)+((heji12+heji13)*YCTransportRatio_2)+(heji8*YCFRatio_2)+(heji9*TransportRatio_2)+(heji10*RabishRatio_2)-(SCYXData:CRM_ConstructionContract_M.PromotionAmount*JCRatio_2))

(heji+heji3+heji12+heji13+heji8+heji9+heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.3


中期:
(基础整装+个性化施工+标配物料远程运输费+单独销售物料远程运输费+远程施工费+搬运费+垃圾清理费-整单促销)*0.18
(heji*JCRatio_3)+(heji3*GxRatio_3)+((heji12+heji13)*YCTransportRatio_3)+(heji8*YCFRatio_3)+(heji9*TransportRatio_3)+(heji10*RabishRatio_3)-(SCYXData:CRM_ConstructionContract_M.PromotionAmount*JCRatio_3)

(heji+heji3+heji12+heji13+heji8+heji9+heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.18

(heji+heji3+heji12+heji13+heji8+heji9+heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.18

尾期：
基础整装+个性化施工+标配物料远程运输费+单独销售物料远程运输费+远程施工费+搬运费+垃圾清理费-整单促销+主材单独销售+主材升级+定制升级+活动产品包-首期-二期-中期
heji+heji3+heji12+heji13+heji8+heji9+heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount+heji2+heji5+heji6+heji1+heji4+heji11-SCYXData:CRM_ConstructionContract_M.DownPayment_NA-secPaymentN-midPaymentN

C_heji_NoM2=C_jichu_NoM2+C_managen_NoM2+C_taxtotal_NoM2-ReductionAmount_OldVersion

活动后:
(基础整装+个性化施工+标配物料远程运输费+单独销售物料远程运输费+远程施工费+搬运费+垃圾清理费-整单促销)*0.5+主材单独销售+定制单独销售+服务包+主材升级+定制升级+活动产品包-活动优惠-平米优惠

 


((C_heji_NoM2*JCRatio_1)+(C_heji3*GxRatio_1)+((C_heji12+C_heji13)*YCTransportRatio_1)+(C_heji8*YCFRatio_1)+(C_heji9*TransportRatio_1)+(C_heji10*RabishRatio_1)-(SCYXData:CRM_ConstructionContract_M.PromotionAmount*JCRatio_1)+(C_heji2*ZCXSRatio_1)+(C_heji5*DZXSRatio_1)+(C_heji6*FwbRatio_1)+(C_heji1*ZCRatio_1)+(C_heji4*DZRatio_1)+(C_heji11*HdbRatio_1)-SCYXDATA:CRM_ConstructionContract_M.DiscountAmount-M2Amount1)

(C_heji_NoM2+C_heji3+C_heji12+C_heji13+C_heji8+C_heji9+C_heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.5+C_heji2+C_heji5+C_heji6+C_heji1+C_heji4+C_heji11-SCYXDATA:CRM_ConstructionContract_M.DiscountAmount-M2Amount1


二期:
(基础整装+个性化施工+标配物料远程运输费+单独销售物料远程运输费+远程施工费+搬运费+垃圾清理费-整单促销)*0.3-平米优惠

((C_heji_NoM2*JCRatio_2)+(C_heji3*GxRatio_2)+(C_heji12+C_heji13)*YCTransportRatio_2+(C_heji8*YCFRatio_2)+(C_heji9*TransportRatio_2)+(C_heji10*RabishRatio_1)-(SCYXData:CRM_ConstructionContract_M.PromotionAmount*JCRatio_2) -M2Amount2)

(C_heji_NoM2+C_heji3+C_heji12+C_heji13+C_heji8+C_heji9+C_heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.3 -M2Amount2


中期:
  

尾期:
(基础整装+个性化施工+标配物料远程运输费+单独销售物料远程运输费+远程施工费+搬运费+垃圾清理费-整单促销)*0.18-平米优惠

(C_heji_NoM2+C_heji3+C_heji12+C_heji13+C_heji8+C_heji9+C_heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.18 -M2Amount3


(C_heji_NoM2+C_heji3+C_heji12+C_heji13+C_heji8+C_heji9+C_heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount)*0.18 -M2Amount3

((C_heji_NoM2*JCRatio_3)+(C_heji3*GxRatio_3)+(C_heji12+C_heji13)*YCTransportRatio_3+(C_heji8*YCFRatio_3)+(C_heji9*TransportRatio_3)+(C_heji10*RabishRatio_3)-(SCYXData:CRM_ConstructionContract_M.PromotionAmount*JCRatio_3) -M2Amount3)



C_heji+C_heji3+C_heji12+C_heji13+C_heji8+C_heji9+C_heji10-SCYXData:CRM_ConstructionContract_M.PromotionAmount-(SCYXDATA:CRM_ConstructionContract_M.DiscountAmount)+C_heji2+C_heji5+C_heji6+C_heji1+C_heji4+C_heji11-SCYXData:CRM_ConstructionContract_M.DownPayment_YA-secPaymentY-midPaymentY






https://bpm.shangceng.com.cn/bpm/YZSoft/Forms/XForm/JZForm/SCYX/CRM/CRM_BuildContract.aspx?tid=7423681

