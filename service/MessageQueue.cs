

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public interface ISubscribeHandler<T>
{
    public Task OnNext(MessageContext<T> context, CancellationToken cancellationToken);

    public void OnError(MessageContext<T> context);
}

public interface IMessageQueue<T> : IAsyncDisposable
{
    Task PublishAsync(T integrationEvent, CancellationToken cancellationToken = default);

    Task PublishAsync(T integrationEvent,Dictionary<string,string> headers, CancellationToken cancellationToken = default);
    
    Task SubscribeAsync(Func<MessageContext<T>, CancellationToken, Task> handler, Action<MessageContext<T>> OnError = default, CancellationToken cancellationToken = default);

    Task SubscribeAsync(ISubscribeHandler<T> handler, CancellationToken cancellationToken = default)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }
        return SubscribeAsync(handler.OnNext, handler.OnError, cancellationToken);
    }
}

public class InMemoryMessageQueue<T> : IMessageQueue<T>
{
    private readonly Channel<MessageContext<T>> _channel = Channel.CreateUnbounded<MessageContext<T>>(new UnboundedChannelOptions
    {
        SingleWriter = false,
        SingleReader = true,
        AllowSynchronousContinuations = false
    });

    internal ChannelReader<MessageContext<T>> Reader => _channel.Reader;

    internal ChannelWriter<MessageContext<T>> Writer => _channel.Writer;

    public async Task PublishAsync(T integrationEvent,Dictionary<string,string> headers, CancellationToken cancellationToken = default)
    {
        var context = MessageContext<T>.Create(integrationEvent,headers);
 
        await Writer.WriteAsync(context, cancellationToken);
    }

    public Task PublishAsync(T integrationEvent, CancellationToken cancellationToken = default)
    {
       return this.PublishAsync(integrationEvent, new Dictionary<string, string>(), cancellationToken);
    }

      

    IAsyncEnumerable<MessageContext<T>> ReadMessageStreamAsync(CancellationToken cancellationToken = default)
    {
        return Reader.ReadAllAsync(cancellationToken);
    }

    public Task SubscribeAsync(Func<MessageContext<T>, CancellationToken, Task> handler, Action<MessageContext<T>> OnError = default, CancellationToken cancellationToken = default)
    {
        return Task.Run(async () =>
        {
            await foreach (var messagecontext in this.ReadMessageStreamAsync(cancellationToken).WithCancellation(cancellationToken))
            {
                
                try
                {
                    if(handler is  null)
                    {
                        throw new ArgumentNullException(nameof(handler));
                    }
                    await handler(messagecontext, cancellationToken);
                }
                catch (System.Exception ex)
                {
                    messagecontext.SetError(ex);

                    if (OnError is not null)
                    {
                        OnError(messagecontext);
                    }
                }
            }
        });
    }
 
    public async ValueTask DisposeAsync()
    {
      
        if (_channel is not null)
        {
            if (_channel.Writer.TryComplete())
            {
                await _channel.Reader.Completion;
            }
        }
    }

  
}
 

public class MessageContext<T>
{
    public Exception Exception { get; internal set; }

    public T Message { get; internal set; }

    public bool HasError => Exception is not null;

    public IDictionary<string, string> Headers { get; internal set; }

    public MessageContext(T message, IDictionary<string, string> headers)
    {
        Message = message;
        Headers = headers;
    }

    public void SetError(Exception ex)
    {
        Exception = ex;
    }

    public void SetHeader(string key, string value)
    {
        Headers[key] = value;
    }

    public string GetHeader(string key)
    {
        return Headers[key];
    }

    public bool TryGetHeader(string key, out string value)
    {
        return Headers.TryGetValue(key, out value);
    }

    public static MessageContext<T> Create(T message)
    {
        return new MessageContext<T>(message, new Dictionary<string, string>());
    }
    public static MessageContext<T> Create(T message, Dictionary<string, string> headers)
    {
        return new MessageContext<T>(message, headers);
    }
}




public static class InMemoryMessageQueueExtension
{
    public static Task SubscribeAsync<T>(this IMessageQueue<T> messageQueue, ISubscribeHandler<T> handler, CancellationToken cancellationToken = default)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return messageQueue.SubscribeAsync(handler, cancellationToken);
    }

}


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorker<T>(this IServiceCollection services,params Assembly[] assemblies)
    {
        if(assemblies is null || assemblies.Length==0)
        {
            assemblies=new Assembly[]{ Assembly.GetExecutingAssembly() };
        }

        services.Scan(p=>{
            p.FromAssemblies(assemblies)
            .AddClasses(c=>c.AssignableTo(typeof(ISubscribeHandler<T>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime();
          });

        services.AddSingleton<IMessageQueue<T>, InMemoryMessageQueue<T>>();
        services.AddHostedService<SubcribeHostedService<T>>();
         return services;
    }
}



/********************************************宿主服务******************************************************************************/


public class SubcribeHostedService<T> : IHostedService
{

    readonly IMessageQueue<T> _messageQueue;
 

    readonly ISubscribeHandler<T> _subscribeHandler;

    readonly IServiceProvider _serviceProvider;


    public SubcribeHostedService(IMessageQueue<T> messageQueue, ISubscribeHandler<T> subscribeHandler, IServiceProvider serviceProvider)
    {
        _messageQueue = messageQueue;

        _subscribeHandler = subscribeHandler;

        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        //var tempsubscribeHandler=_serviceProvider.GetService<ISubscribeHandler<T>>();

        _messageQueue.SubscribeAsync(_subscribeHandler,cancellationToken);
  
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _messageQueue.DisposeAsync();
    }
}

/********************************************宿主服务End******************************************************************************/



public class ConsumerHandler : ISubscribeHandler<string> ,ISubscribeHandler<int> ,ISubscribeHandler<Message>
{
    public void OnError(MessageContext<string> context)
    {
        Console.WriteLine("OnError:" + context.Message + "--" + context.Exception.Message);
    }

    public Task OnNext(MessageContext<string> context, CancellationToken cancellationToken)
    {
        Console.WriteLine("OnNext:" + context.Message);
        return Task.CompletedTask;
    }


    public void OnError(MessageContext<int> context)
    {
        Console.WriteLine("OnError:" + context.Message + "--" + context.Exception.Message);
    }

 

    public Task OnNext(MessageContext<int> context, CancellationToken cancellationToken)
    {
         Console.WriteLine("SubscribeHandler--"+this.GetHashCode());
        Console.WriteLine("OnNext:" + context.Message);
        throw new Exception("OOP");
        return Task.CompletedTask;
    }

    public Task OnNext(MessageContext<Message> context, CancellationToken cancellationToken)
    {
     
        Console.WriteLine("OnNext:" + context.Message);
        throw new Exception("time out!!");
        return Task.CompletedTask;
    }

    public void OnError(MessageContext<Message> context)
    {
       Console.WriteLine("OnError:" + context.Message + "--" + context.Exception.Message);
       
    }
}
 


/**************************************************************************************************/

builder.Services.AddWorker<string>();
builder.Services.AddWorker<int>();
builder.Services.AddWorker<Message>();



