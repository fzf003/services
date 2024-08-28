
---https://bpm.shangceng.com.cn/bpm/YZSoft/Forms/Post.aspx?pn=定额分类
select * from  SCZSPMDATA..PM_Process_QuotaCategory  where ID IN(297)

select * from PM_Base_QuotaCategory where  ID IN(297,323)

UPDATE PM_Base_QuotaCategory SET CompanyCode=4240 where  ID IN(297)

SELECT * FROM PM_Base_QuotaSubCategory WHERE ParentID=323

UPDATE PM_Base_QuotaCategory SET EnableStatus=1,CategoryName='补充' where  ID IN(323)

SELECT * FROM PM_Base_QuotaCategory   

select * from PM_Base_QuotaCategory where SN in('DEFL20240819001')

select  ID,TaskID,SN,CategoryName,CustomerType,CreateTime,Remark,Sort,   CompanyCode,    CompanyName,QuotaType,1 as EnableStatus,CategoryType
from PM_Base_QuotaCategory where TaskID in(5406362)

use SCZSPMDATA
insert into PM_Base_QuotaCategory (TaskID,SN,CategoryName,CustomerType,CreateTime,Remark,Sort,CompanyCode,CompanyName,QuotaType,EnableStatus,CategoryType)
select  TaskID,SN,CategoryName,CustomerType,CreateTime,Remark,Sort,   CompanyCode,    CompanyName,QuotaType,1 as EnableStatus,CategoryType
from PM_Base_QuotaCategory where TaskID in(5406362)

 
 SELECT * FROM PM_Base_QuotaSubCategory

insert into PM_Base_QuotaSubCategory(SubQuotaName,EnableStatus,Sort,ParentID,CreateTime)
select '补充' as SubQuotaName, 1 as EnableStatus,1 as Sort,322 as ParentID,GETDATE()


select * from BPMHRM..V_GetCompany where code in(1445)

SELECT * FROM PM_Base_QuotaSubCategory WHERE ParentID=323

 SELECT CompanyCode,CompanyName ,* FROM PM_Base_QuotaCategory  where CategoryName in('补充')
	 and id in(SELECT ParentID FROM PM_Base_QuotaSubCategory where SubQuotaName in('补充'))

 DECLARE @TaskID INT=5406362;
 DECLARE @ID INT=0;
UPDATE PM_Base_QuotaCategory SET EnableStatus=1 WHERE TaskID=@TaskID;
SELECT @ID=ID FROM PM_Base_QuotaCategory WHERE TaskID=@TaskID;
IF NOT EXISTS(SELECT * FROM PM_Base_QuotaSubCategory WHERE ParentID=@ID)
BEGIN
INSERT INTO PM_Base_QuotaSubCategory(SubQuotaName,EnableStatus,Sort,ParentID,CreateTime)
SELECT '补充' as SubQuotaName, 1 as EnableStatus,1 as Sort,@ID as ParentID,GETDATE()
END
