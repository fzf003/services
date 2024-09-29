 SELECT TaskId,PeriodRatioBody,MessageBody,* FROM SCYXDATA..CRM_ConstructionContract_M WHERE CusCode='CUS2404130034'
	 SELECT * FROM SCYXDATA..CRM_ConstructionContract_T_CollectionPeriod_M WHERE TaskID=7628594-- CollectionRatio=0
	 SELECT PayItemName,* FROM SCYXDATA..CRM_ConstructionContract_T_CollectionPeriod_M_PaymentItem_T WHERE TaskID=7628594
	  and PayItemName='基础整装'

   
	 delete SCYXDATA..CRM_ConstructionContract_T_CollectionPeriod_M_PaymentItem_T WHERE TaskID=7646617
	 delete SCYXDATA..CRM_ConstructionContract_T_CollectionPeriod_M WHERE TaskID=7646617

http://10.32.62.11:30317/yx/v1/Contract/SyncContractPeriodSetting?ContractTaskId=7646617&CusCode=CUS2406180032
