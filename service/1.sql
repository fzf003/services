	  use BPMHRM
	  SELECT NewDeptCode,DeptCode,* FROM HR_OpenAccount_M 
	  WHERE     Account in('GZS0007')  

	  UPDATE  HR_OpenAccount_M SET IsDeptManage=1  WHERE  Account in('GZS0007') 
