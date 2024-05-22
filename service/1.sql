	  use BPMHRM
	  SELECT NewDeptCode,DeptCode,* FROM HR_OpenAccount_M 
	  WHERE     Account in('GZS0007')  

	  UPDATE  HR_OpenAccount_M SET IsDeptManage=1  WHERE  Account in('GZS0007') 





 

 DECLARE @Case_M_SN NVARCHAR(100)='YXP2404290001';
 DECLARE @SN NVARCHAR(100)='';
 IF EXISTS(SELECT A.SN FROM Base_Space_Part_Config_T a WITH(NOLOCK) 
          INNER JOIN  Base_Space_Part_Config_M b WITH(NOLOCK) ON a.SN=b.SN WHERE b.State=1
          AND b.Case_M_SN in(@Case_M_SN) AND SpaceName in('卫生间'))
  BEGIN
  CREATE TABLE #TempIds
(
    ID INT PRIMARY KEY
);
       SELECT DISTINCT @SN= A.SN
		FROM Base_Space_Part_Config_T a WITH(NOLOCK) 
		INNER JOIN  Base_Space_Part_Config_M b WITH(NOLOCK) ON a.SN=b.SN WHERE b.State=1
		AND b.Case_M_SN in(@Case_M_SN) and SpaceName in('卫生间')

		INSERT INTO #TempIds(ID)
		 SELECT ID FROM Base_Space_Part_Config_T WHERE SN=@SN  and SpaceName in('卫生间')

	    UPDATE Base_Space_Part_Config_T SET SpaceID=73,SpaceName='主卫生间' where ID IN(
		  SELECT ID FROM Base_Space_Part_Config_T WHERE SN=@SN  and SpaceName in('卫生间')
		)
		select * from Base_Space_Part_Config_T where id in (SELECT ID FROM #TempIds)
		DROP TABLE #TempIds;
   END





 






 






		  
DECLARE @CaseSN NVARCHAR(100)='';
DECLARE @CompanyName NVARCHAR(100)='';
DECLARE @CompanyCode NVARCHAR(100)='';

DECLARE F1 CURSOR FOR
--SELECT Fields,Sort  FROM JZDATA..CRM_LineTemplate WHERE ProtocolType in('优选整装服务销售合同') AND  CompanyCode IN(1);
 SELECT SN as CaseSN,CompanyCode,CompanyName from SCYXDATA..Base_Case_M where CaseProductType in('X-VILLA别墅整装','X-UNIT平墅整装') and State in(1) and CompanyCode not in(732);
OPEN F1;
FETCH NEXT FROM F1 INTO @CaseSN,@CompanyName,@CompanyCode
WHILE @@FETCH_STATUS = 0
BEGIN

PRINT CONCAT(@CaseSN,'-', @CompanyName)

--DECLARE @Case_M_SN NVARCHAR(100)='YXP2404300016';
 DECLARE @SN NVARCHAR(100)='';
 IF EXISTS(SELECT A.SN FROM Base_Space_Part_Config_T a WITH(NOLOCK) 
          INNER JOIN  Base_Space_Part_Config_M b WITH(NOLOCK) ON a.SN=b.SN WHERE b.State=1
          AND b.Case_M_SN in(@CaseSN) AND SpaceName in('卫生间'))
  BEGIN
       SELECT DISTINCT @SN= A.SN
		FROM Base_Space_Part_Config_T a WITH(NOLOCK) 
		INNER JOIN  Base_Space_Part_Config_M b WITH(NOLOCK) ON a.SN=b.SN WHERE b.State=1
		AND b.Case_M_SN in(@CaseSN) and SpaceName in('卫生间')

	    UPDATE Base_Space_Part_Config_T SET SpaceID=73,SpaceName='主卫生间' where ID IN(
		  SELECT ID FROM Base_Space_Part_Config_T WHERE SN=@SN  and SpaceName in('卫生间')
		)
   END
 


FETCH NEXT FROM F1 INTO @CaseSN,@CompanyName,@CompanyCode
END
CLOSE F1;
DEALLOCATE F1;









 

 DECLARE @Case_M_SN NVARCHAR(100)='YXP2404300016';
 DECLARE @SN NVARCHAR(100)='';
 IF EXISTS(SELECT A.SN FROM Base_Space_Part_Config_T a WITH(NOLOCK) 
          INNER JOIN  Base_Space_Part_Config_M b WITH(NOLOCK) ON a.SN=b.SN WHERE b.State=1
          AND b.Case_M_SN in(@Case_M_SN) AND SpaceName in('卫生间'))
  BEGIN
       SELECT DISTINCT @SN= A.SN
		FROM Base_Space_Part_Config_T a WITH(NOLOCK) 
		INNER JOIN  Base_Space_Part_Config_M b WITH(NOLOCK) ON a.SN=b.SN WHERE b.State=1
		AND b.Case_M_SN in(@Case_M_SN) and SpaceName in('卫生间')

	    UPDATE Base_Space_Part_Config_T SET SpaceID=73,SpaceName='主卫生间' where ID IN(
		  SELECT ID FROM Base_Space_Part_Config_T WHERE SN=@SN  and SpaceName in('卫生间')
		)
   END
