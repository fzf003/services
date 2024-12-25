public class OrderContext
{
    public OrderContext() { }
    public HttpClient HttpClient { get; set; }
    public bool IsSuccess { get; set; }
}

public interface IOrderContextFactory
{
    OrderContext Create();
}

public class OrderContextFactory : IOrderContextFactory
{
    public OrderContext Create()
    {
        return new OrderContext()
        {
            HttpClient = new HttpClient(),
            IsSuccess = true

        };
    }
}


public interface IHandler
{
    Task HandleAsync(OrderContext context);

    IHandler SetNext(IHandler handler);
}

public abstract class HandlerBase : IHandler
{
    private IHandler _nextHandler;

    public IHandler SetNext(IHandler handler)
    {
        _nextHandler = handler;
        return _nextHandler;
    }

    public virtual async Task HandleAsync(OrderContext context)
    {
        if (_nextHandler != null)
        {
            await _nextHandler.HandleAsync(context);
        }
    }
}

public class CreateOrderHandler : HandlerBase
{
    public override async Task HandleAsync(OrderContext context)
    {
        //var response = await context.HttpClient.PostAsync("https://localhost:7246/OrderAPI/CreateOrder",new StringContent(""));
        var response = new { IsSuccessStatusCode = true };
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("订单已创建!Order created.");
            // Extract order ID or other details if needed
            context.IsSuccess = true;
            await base.HandleAsync(context);
        }
        else
        {
            context.IsSuccess = false;
            // Handle error
        }
    }
}

public class ProcessPaymentHandler : HandlerBase
{
    public override async Task HandleAsync(OrderContext context)
    {
        if (!context.IsSuccess) return;

        // var response = await context.HttpClient.PostAsync("https://api.example.com/payments", new StringContent(JsonConvert.SerializeObject(context.Payment)));
        var response = new { IsSuccessStatusCode = true };
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("支付成功!Payment processed successfully.");
            context.IsSuccess = true;
            await base.HandleAsync(context);
        }
        else
        {
            context.IsSuccess = false;
            // Handle error
        }
    }
}

public class ConfirmOrderHandler : HandlerBase
{
    public override async Task HandleAsync(OrderContext context)
    {
        if (!context.IsSuccess) return;

        var response = new { IsSuccessStatusCode = true }; //await context.HttpClient.PostAsync($"https://localhost:7246/OrderAPI/OrderConfirm", null);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("订单已确认!Order confirmed.");
            context.IsSuccess = true;
            // Order confirmed
        }
        else
        {
            context.IsSuccess = false;
            // Handle error
        }
    }
}

public class OrderService
{
    private readonly IHandler _createOrderHandler;
    private readonly IHandler _processPaymentHandler;
    private readonly IHandler _confirmOrderHandler;

    public OrderService(
        CreateOrderHandler createOrderHandler,
        ProcessPaymentHandler processPaymentHandler,
        ConfirmOrderHandler confirmOrderHandler)
    {
        _createOrderHandler = createOrderHandler;
        _processPaymentHandler = processPaymentHandler;
        _confirmOrderHandler = confirmOrderHandler;

        _createOrderHandler.SetNext(_processPaymentHandler).SetNext(_confirmOrderHandler);
    }

    public async Task HandleOrderCreationAsync(OrderContext context)
    {
        await _createOrderHandler.HandleAsync(context);
    }
}

/*
  
var services=new ServiceCollection();
services.AddTransient<CreateOrderHandler>();
services.AddTransient<ProcessPaymentHandler>();
services.AddTransient<ConfirmOrderHandler>();
services.AddTransient<IOrderContextFactory, OrderContextFactory>();
services.AddTransient<OrderService>();
var provider= services.BuildServiceProvider();


 
var context = provider.GetRequiredService<IOrderContextFactory>().Create();

var orderService =provider.GetRequiredService<OrderService>();

  new OrderService(
    new CreateOrderHandler(),
    new ProcessPaymentHandler(),
    new ConfirmOrderHandler());
    */

//await orderService.HandleOrderCreationAsync(context);
 
