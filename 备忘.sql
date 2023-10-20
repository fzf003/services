SELECT 
    p.name AS '参数名称',
    Type_Name(p.user_type_id) AS '数据类型',
    p.max_length AS '数据长度',
    p.is_output AS '不否输出'
FROM 
    sys.parameters p
INNER JOIN
    sys.procedures pr ON p.object_id = pr.object_id
WHERE
    pr.name = 'PROC_GZ_ProductLine_M'





 EXEC sp_describe_first_result_set N'Proc_YX_ChangeOrderConvertPianReturnable'
