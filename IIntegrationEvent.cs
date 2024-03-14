/// <summary>
/// 集成事件接口
/// </summary>
public interface IIntegrationEvent
{

};


/// <summary>
/// 客户创建订单集成事件
/// </summary>
/// <param name="Name"></param>
/// <param name="OrderId"></param>
internal record UserCreateOrderIntegrationEvent (string Name,string OrderId): IIntegrationEvent
{
    
}

 