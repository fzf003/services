

using System.Net;
using Medallion.Threading.SqlServer;
using Polly;
using Polly.Retry;
using Polly.Wrap;

public enum EventState
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancel = 4
}

public record OutMessage<T>(T Message, Guid id, Guid TransactionId, int RetryCount = 0, DateTime CreateTime = default, DateTime UpdateTime = default, EventState State = EventState.Pending) where T : class
{
    public OutMessage<T> withEventState(EventState eventstate)
    {
        return this with { Message = Message, id = id, TransactionId = TransactionId, CreateTime = CreateTime, UpdateTime = DateTime.Now, State = eventstate };
    }

    public static OutMessage<T> Create(T Message, Guid id, Guid TransactionId, int RetryCount = 0, DateTime CreateTime = default, EventState State = EventState.Pending)
    {
        return new OutMessage<T>(Message: Message, id: id, RetryCount: RetryCount, TransactionId: TransactionId, CreateTime: DateTime.Now, State: State);
    }
}



public interface IEventConsumer<T> where T : class
{
    Task Subscribe(Action<T> callback, CancellationToken ct = default);
}


public record CancelEvent(Guid TransactionId, string ConsumerName, Guid Id);

public record ResetEvent(Guid TransactionId, string ConsumerName, Guid Id);




public abstract class AbstractConsumer<T> : IEventConsumer<T> where T : class
{

    RetryPolicy policy = null;

    const string connectionString = "Data Source=10.22.65.11;Initial Catalog=HD;Integrated Security=False;User ID=sa;Password=sczs_dev2020; Max Pool Size=1000;Connect Timeout=3000";

    public AbstractConsumer()
    {
        policy = Policy.Handle<Exception>().WaitAndRetry(2, (retryAttempt) =>
         {
             Console.ForegroundColor = ConsoleColor.Yellow;
             Console.WriteLine("Policy logging: " + retryAttempt);
             Console.ResetColor();
             return TimeSpan.FromSeconds(1);
         });
    }

    public async Task Subscribe(Action<T> callback, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ConsumerName()))
        {
            throw new ArgumentNullException("消费者名称不能为空");
        }

        while (!ct.IsCancellationRequested)
        {
            var @lock = new SqlDistributedLock("MyLockName", connectionString);
            await using (await @lock.AcquireAsync())
            {
                var outMessages = new List<T>();
                ///await this._storage.GetProducts(5, ConsumerName: ConsumerName()).ConfigureAwait(false);

                if (outMessages.Any())
                {
                    var processMessage = outMessages.Select(p => OutMessage<T>.Create(p, Guid.NewGuid(), Guid.NewGuid(), CreateTime: DateTime.Now));

                    List<OutMessage<T>> messagestore = new List<OutMessage<T>>();

                    Console.WriteLine($"生成数据:{outMessages.Count()}");

                    foreach (var outMessage in processMessage)
                    {
                        if (outMessage.Message is null)
                            continue;

                        try
                        {
                            ///反序列化消息主体
                            //  policy.Execute(() => callback(outMessage.Message));

                            callback(outMessage.Message);

                            messagestore.Add(outMessage.withEventState(eventstate: EventState.Completed));

                        }
                        catch (OperationCanceledException) when (ct.IsCancellationRequested)
                        {
                            Console.WriteLine("操作取消");
                        }
                        catch (RequeableException ex) //重新入队
                        {
                            if (outMessage.Message is not null)
                            {
                                var erroroutmessage = await ProcessExceptionHandler(ex, outMessage);
                                messagestore.Add(erroroutmessage);
                            }
                        }
                        catch (DroppableException ex)
                        {
                            if (outMessage.Message is not null)
                            {
                                var erroroutmessage = await ProcessExceptionHandler(ex, outMessage);
                                messagestore.Add(erroroutmessage);
                            }
                        }
                        catch (TimeoutException ex)
                        {
                            if (outMessage.Message is not null)
                            {
                                var erroroutmessage = await ProcessExceptionHandler(ex, outMessage);
                                messagestore.Add(erroroutmessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (outMessage.Message is not null)
                            {
                                var erroroutmessage = await ProcessExceptionHandler(ex, outMessage);
                                messagestore.Add(erroroutmessage);
                            }
                        }

                        finally
                        {

                        }

                    }

                    await CommitAsync(messagestore).ConfigureAwait(false);
                }
            }
            await Task.Delay(1000).ConfigureAwait(false);
        }
    }


    async Task<OutMessage<T>> ProcessExceptionHandler(Exception exception, OutMessage<T> outMessage)
    {

        var newMessage = exception switch
        {
            RequeableException => outMessage.withEventState(eventstate: EventState.Pending), //重新入队
            DroppableException => outMessage.withEventState(eventstate: EventState.Cancel),  //取消不再消费
            TimeoutException => outMessage.withEventState(eventstate: EventState.Pending), //超时重试
            _ => outMessage.withEventState(eventstate: EventState.Cancel)    ///默认取消
        };
        try
        {
            await ExceptionHandleAsync(exception, newMessage).ConfigureAwait(false);
        }
        finally
        {

        }
        return newMessage;
    }

    ///提交更新消费状态
    Task CommitAsync(List<OutMessage<T>> outMessage)
    {
        Console.WriteLine($"消费成功:{outMessage.Count}");

        foreach (var item in outMessage)
        {
            Console.WriteLine(item);
        }

        var updatemessage = outMessage.Select(p => p.State == EventState.Failed).ToList();

        var updatesql = "UPDATE Outmessagge WITH (ROWLOCK,UPDLOCK,READPAST) SET State=@State,UpdateTime=@UpdateTime,OUTPUT INSERTED.* WHERE Id=@Id AND TransactionId=@TransactionId";

        return Task.CompletedTask;
    }

    protected virtual Task ExceptionHandleAsync(Exception exception, OutMessage<T> outMessage) => Task.CompletedTask;

    protected virtual string ConsumerName() => string.Empty;

    protected virtual Task ResetMessage(OutMessage<T> outMessage) => Task.CompletedTask;

    protected virtual Task CancelMessage(OutMessage<T> outMessage) => Task.CompletedTask;

    protected virtual bool IsBatch => true;
}

/*
  1. 竞争消费者模式:获取数据后更改状态Pending 处理中同时更改处理Id(TransactionId)和设置消费者名称之后发布消息。
*/

public class Consumer : AbstractConsumer<Product>
{
    protected override string ConsumerName()
    {
        return "fzf-2";
    }

    protected override Task ExceptionHandleAsync(Exception exception, OutMessage<Product> outMessage)
    {
        Console.WriteLine($"出错:{exception.Message}--{outMessage.Message}");

        return base.ExceptionHandleAsync(exception, outMessage);
    }

    protected override Task CancelMessage(OutMessage<Product> outMessage)
    {
        return base.CancelMessage(outMessage);
    }

}


public class ConsumerRun
{
    public static async Task Init()
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var Consumer1 = new Consumer();
        await Consumer1.Subscribe(Process, cancellationTokenSource.Token);

        var Consumer2 = new Consumer();
        await Consumer2.Subscribe(Process, cancellationTokenSource.Token);
    }

    public static void CreateProject()
    {
        var project = Activator.CreateInstance(typeof(MyProjection), true)! as MyProjection;
        project?.Handle(new UserCreated("fzf003", 1200));
        project?.Handle(new UserCreated("fzf00334", 342));
    }

    static void Process(Product product)
    {
        if (int.TryParse(product.productId, out int productId))
        {
            if (productId % 2 == 0)
            {
                throw new Exception("timeout!!");
            }
        }

        Console.WriteLine(product);
    }

}


////重新入队
public class RequeableException : Exception
{

}
///取消
public class DroppableException : Exception
{

}


public static class PolicyExtensions
{

    public static PolicyBuilder<Product> HandleTransientError()
    {
        return Policy<Product>.Handle<Exception>();//.OrResult(TransientHttpStatusCodePredicate);
    }

    private static readonly Func<Product, bool> TransientHttpStatusCodePredicate = (response) =>
    {
        return true;
    };

    /*
    private static readonly Func<HttpResponseMessage, bool> TransientHttpStatusCodePredicate = (response) =>
    {
        return (int)response.StatusCode >= 500 || response.StatusCode == HttpStatusCode.RequestTimeout;
    };

     public static PolicyBuilder<HttpResponseMessage> HandleTransientHttpError()
    {
        return Policy<HttpResponseMessage>.Handle<HttpRequestException>().OrTransientHttpStatusCode();
    }

     public static PolicyBuilder<HttpResponseMessage> OrTransientHttpStatusCode(this PolicyBuilder policyBuilder)
    {
        if (policyBuilder == null)
        {
            throw new ArgumentNullException(nameof(policyBuilder));
        }

        return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
    }

    /// <summary>
    /// Configures the <see cref="PolicyBuilder{HttpResponseMessage}"/> to handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. 
    /// <para>The conditions configured to be handled are:
    /// <list type="bullet">
    /// <item><description>Network failures (as <see cref="HttpRequestException"/>)</description></item>
    /// <item><description>HTTP 5XX status codes (server errors)</description></item>
    /// <item><description>HTTP 408 status code (request timeout)</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <returns>The <see cref="PolicyBuilder{HttpResponseMessage}"/> pre-configured to handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. </returns>
    public static PolicyBuilder<HttpResponseMessage> OrTransientHttpError(this PolicyBuilder policyBuilder)
    {
        if (policyBuilder == null)
        {
            throw new ArgumentNullException(nameof(policyBuilder));
        }

        return policyBuilder.Or<HttpRequestException>().OrTransientHttpStatusCode();
    }

    /// <summary>
    /// Configures the <see cref="PolicyBuilder{HttpResponseMessage}"/> to handle <see cref="HttpClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure. 
    /// <para>The <see cref="HttpStatusCode"/>s configured to be handled are:
    /// <list type="bullet">
    /// <item><description>HTTP 5XX status codes (server errors)</description></item>
    /// <item><description>HTTP 408 status code (request timeout)</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <returns>The <see cref="PolicyBuilder{HttpResponseMessage}"/> pre-configured to handle <see cref="HttpClient"/> requests that fail with <see cref="HttpStatusCode"/>s indicating a transient failure. </returns>
    public static PolicyBuilder<HttpResponseMessage> OrTransientHttpStatusCode(this PolicyBuilder<HttpResponseMessage> policyBuilder)
    {
        if (policyBuilder == null)
        {
            throw new ArgumentNullException(nameof(policyBuilder));
        }

        return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
    }

    /// <summary>
    /// Configures the <see cref="PolicyBuilder{HttpResponseMessage}"/> to handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. 
    /// <para>The conditions configured to be handled are:
    /// <list type="bullet">
    /// <item><description>Network failures (as <see cref="HttpRequestException"/>)</description></item>
    /// <item><description>HTTP 5XX status codes (server errors)</description></item>
    /// <item><description>HTTP 408 status code (request timeout)</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <returns>The <see cref="PolicyBuilder{HttpResponseMessage}"/> pre-configured to handle <see cref="HttpClient"/> requests that fail with conditions indicating a transient failure. </returns>
    public static PolicyBuilder<HttpResponseMessage> OrTransientHttpError(this PolicyBuilder<HttpResponseMessage> policyBuilder)
    {
        if (policyBuilder == null)
        {
            throw new ArgumentNullException(nameof(policyBuilder));
        }

        return policyBuilder.Or<HttpRequestException>().OrTransientHttpStatusCode();
    }*/
}
