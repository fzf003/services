


 
Create PROCEDURE [Proc_CreateProducts]
@newProducts dbo.NewProductType READONLY

AS
BEGIN
 INSERT INTO [HD].[dbo].[Users](UserName,Sex,DueAfter,State)
 OUTPUT inserted.*
 SELECT UserName,Sex,DueAfter,State
 FROM @newProducts 
 
END

 ---用户自定义类型的表

CREATE TYPE dbo.NewProductType AS TABLE  
( [UserName] nvarchar(50) not null, [Sex] nvarchar(25) not null,  DueAfter datetime, State int not null ) 



DECLARE @newProductsTable AS dbo.NewProductType;  

-- 插入一些示例数据到表值变量中
INSERT INTO @newProductsTable
VALUES ('ProductName1', 'ProductDescription1', 100.00, 5),
       ('ProductName2', 'ProductDescription2', 200.00, 10);

	   select * from @newProductsTable

	   EXEC Proc_CreateProducts @newProducts=@newProductsTable




DECLARE @newProductsTable AS dbo.NewProductType;  
	   
-- 插入一些示例数据到表值变量中
INSERT INTO @newProductsTable
VALUES ('ProductName1', 'ProductDescription11', GETDATE(), 5),
       ('ProductName22', 'ProductDescription22', GETDATE(), 10);


MERGE [HD].[dbo].[Users] AS c
USING @newProductsTable AS n
ON (c.UserName = n.UserName)
WHEN MATCHED THEN
    UPDATE SET c.Sex = n.Sex, c.DueAfter = n.DueAfter,c.State=n.State
WHEN NOT MATCHED BY TARGET THEN
    INSERT (UserName,Sex,DueAfter,State)
    VALUES (n.UserName, n.Sex, n.DueAfter,n.State)
WHEN NOT MATCHED BY SOURCE THEN
    DELETE;
