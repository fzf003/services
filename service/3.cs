{
    "requestId": [
    "{\"CusCode\":\"CUS2310160013\",\"ProCode\":\"PRO20210224_478089_8\",\"ProName\":\"森林狂想曲\",\"PartId\":168998,\"PartName\":\"二层次卧地面-地面装饰材料-瓷砖\",\"Num\":1,\"NO\":1,\"CaiGouNum\":1,\"SjNum\":0,\"Saleway\":\"单独销售\",\"activeflag\":\"赠送\",\"activeid\":170}",
    "{\"CusCode\":\"CUS2310160013\",\"ProCode\":\"PRO20210225_100986_919\",\"ProName\":\"踢脚线\",\"PartId\":168998,\"PartName\":\"二层次卧地面-地面装饰材料-瓷砖\",\"Num\":2,\"NO\":2,\"CaiGouNum\":2,\"SjNum\":0,\"Saleway\":\"单独销售\",\"activeflag\":\"赠送\",\"activeid\":170}"
]
}



        [HttpPost]
        public IEnumerable<QueryChangeProduct> Post([FromBody]MyRequest myRequest)
        {

            return myRequest.RequestId.Select(p => JsonConvert.DeserializeObject<QueryChangeProduct>(p)).ToList();
            
        }


public class MyRequest
{
    public List<string> RequestId { get; set; }
}

public class QueryChangeProduct
{
    /// <summary>
    /// 活动金额
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// 单独销售价
    /// </summary>
    public decimal AlonePrice { get; set; }
    /// <summary>
    /// 活动Id
    /// </summary>
    public int ActiveId { get; set; }
    /// <summary>
    /// 活动物料
    /// </summary>
    public string ProCode { get; set; }
};
