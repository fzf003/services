
USE [SCYXDATA]
GO

/****** Object:  UserDefinedFunction [dbo].[F_ProCodeNum]    Script Date: 2024/6/3 23:43:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO














/*
  获取当前物料的数量
*/

ALTER FUNCTION [dbo].[F_ProCodeNum]
(
 	@MainID as decimal(18, 2),
	@IsAuto as Int,
	@ServiceName as NVARCHAR(50),
	@MID AS INT,
	@Active_SalePriceTotal as decimal(18, 2)
)
RETURNS decimal(18, 2) 
AS
BEGIN
 DECLARE @Num decimal(18, 2) 
 DECLARE @JIAN  decimal(18, 2)=0;
 DECLARE @ADD  decimal(18, 2)=0;
  DECLARE @MMAIN INT=0;
   SELECT @MMAIN=ISNULL(MainID,0) FROM SCYXDATA..PM_Price_Service_T WHERE ID IN(@MainID)

 IF @IsAuto =1
 BEGIN
	select @Num= ISNULL(SUM(Num),0)  
	 FROM dbo.PM_Price_Service_T WITH(NOLOCK)
		WHERE   (UseState = 1) and   (MainId in(@MainID) OR ID IN(@MainID))  AND IsAuto in(@IsAuto)  AND M_ID =@MID
		 AND Active_SalePriceTotal=@Active_SalePriceTotal
 END
 /*ELSE IF EXISTS(SELECT * FROM SCYXDATA..PM_Price_Service_T WHERE ID IN(@MainID) AND SalePrice=0  )
 BEGIN


 IF @MMAIN=0
 BEGIN
select @JIAN= ISNULL(SUM(Num),0)  
	 FROM dbo.PM_Price_Service_T WITH(NOLOCK)
		WHERE   (UseState = 1) and   (
		  ID IN(@MainID) OR MainID IN(@MainID) 
		  )  AND IsAuto in(@IsAuto)  AND M_ID =@MID AND Num<0  and UseState in(1) 
		 --AND Active_SalePriceTotal=0 AND SalePriceTotal=0
		 and SalePrice=0

	 select @ADD= ISNULL(SUM(Num),0)  
	 FROM dbo.PM_Price_Service_T WITH(NOLOCK)
		WHERE   (UseState = 1) and   (
		  ID IN(@MainID)
		  )  AND IsAuto in(@IsAuto)  AND M_ID =@MID AND Num>0  and UseState in(1) 
		   and SalePrice=0
		 --AND Active_SalePriceTotal=@Active_SalePriceTotal AND SalePriceTotal=0

		SET @Num=@JIAN+@ADD
END
ELSE
BEGIN
    select @JIAN= ISNULL(SUM(Num),0)  
	 FROM dbo.PM_Price_Service_T WITH(NOLOCK)
		WHERE   (UseState = 1) and   (
		   MainID IN(@MainID) 
		  )  AND IsAuto in(@IsAuto)  AND M_ID =@MID AND Num<0  and UseState in(1) 
		 --AND Active_SalePriceTotal=@Active_SalePriceTotal AND SalePriceTotal=0
		  and SalePrice=0

	 select @ADD= ISNULL(SUM(Num),0)  
	 FROM dbo.PM_Price_Service_T WITH(NOLOCK)
		WHERE   (UseState = 1) and   (
		  ID IN(@MainID)  
		  )  AND IsAuto in(@IsAuto)  AND M_ID =@MID AND Num>0  and UseState in(1) 
		 --AND Active_SalePriceTotal=@Active_SalePriceTotal AND SalePriceTotal=0
		   and SalePrice=0

		SET @Num=@JIAN+@ADD
END
 
		 
   
 END*/
 ELSE

 BEGIN

		 
 IF @MMAIN=0
 BEGIN
select @JIAN= ISNULL(SUM(Num),0)  
	 FROM dbo.PM_Price_Service_T WITH(NOLOCK)
		WHERE   (UseState = 1) and   (
		  ID IN(@MainID) OR MainID IN(@MainID) 
		  )  AND IsAuto in(@IsAuto)  AND M_ID =@MID AND Num<0  and UseState in(1) 
		 

	 select @ADD= ISNULL(SUM(Num),0)  
	 FROM dbo.PM_Price_Service_T WITH(NOLOCK)
		WHERE   (UseState = 1) and   (
		  ID IN(@MainID)
		  )  AND IsAuto in(@IsAuto)  AND M_ID =@MID AND Num>0  and UseState in(1) 
		   

		SET @Num=@JIAN+@ADD
END
ELSE
BEGIN
    select @JIAN= ISNULL(SUM(Num),0)  
	 FROM dbo.PM_Price_Service_T WITH(NOLOCK)
		WHERE   (UseState = 1) and   (
		   MainID IN(@MainID) 
		  )  AND IsAuto in(@IsAuto)  AND M_ID =@MID AND Num<0  and UseState in(1) 
		 

	 select @ADD= ISNULL(SUM(Num),0)  
	 FROM dbo.PM_Price_Service_T WITH(NOLOCK)
		WHERE   (UseState = 1) and   (
		  ID IN(@MainID)  
		  )  AND IsAuto in(@IsAuto)  AND M_ID =@MID AND Num>0  and UseState in(1) 
		 

		SET @Num=@JIAN+@ADD
END

 END

	RETURN(@Num)
END
GO


