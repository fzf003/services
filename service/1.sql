	  use BPMHRM
	  SELECT NewDeptCode,DeptCode,* FROM HR_OpenAccount_M 
	  WHERE     Account in('GZS0007')  

	  UPDATE  HR_OpenAccount_M SET IsDeptManage=1  WHERE  Account in('GZS0007') 







 

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
