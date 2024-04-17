
public interface ISubscribeHandler<T>
{
    public Task OnNext(T message, CancellationToken cancellationToken);

    public void OnError(T message, Exception ex);
}

public interface IMessageQueue<T>:IAsyncDisposable
{
    Task PublishAsync(T integrationEvent, CancellationToken cancellationToken = default);
    
    Task SubscribeAsync(Func<T, CancellationToken, Task> handler, Action<T, Exception> OnError = default, CancellationToken cancellationToken = default);

    Task SubscribeAsync(ISubscribeHandler<T> handler, CancellationToken cancellationToken = default)
    {
        if(handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }
        return SubscribeAsync(handler.OnNext, handler.OnError, cancellationToken);
    }
}

public class InMemoryMessageQueue<T> : IMessageQueue<T>
{
    private readonly Channel<T> _channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
    {
        SingleWriter = false,
        SingleReader = true,
        AllowSynchronousContinuations = false
    });

    internal ChannelReader<T> Reader => _channel.Reader;

    internal ChannelWriter<T> Writer => _channel.Writer;


    public async Task PublishAsync(T integrationEvent, CancellationToken cancellationToken = default)
    {
        await Writer.WriteAsync(integrationEvent, cancellationToken);
    }

    IAsyncEnumerable<T> ReadMessageStreamAsync(CancellationToken cancellationToken = default)
    {
        return Reader.ReadAllAsync(cancellationToken);
    }

    public Task SubscribeAsync(Func<T, CancellationToken, Task> handler, Action<T, Exception> OnError = default, CancellationToken cancellationToken = default)
    {
        return Task.Run(async () =>
        {
            await foreach (var message in this.ReadMessageStreamAsync(cancellationToken).WithCancellation(cancellationToken))
            {
                try
                {
                    await handler(message, cancellationToken);
                }
                catch (System.Exception ex)
                {

                    if (OnError is not null)
                    {
                        OnError(message, ex);
                    }

                }
            }
        });

    }

    public async ValueTask DisposeAsync()
    {
         if(_channel is not null)
        {
            if(_channel.Writer.TryComplete())
            {
              await  _channel.Reader.Completion;
            }
        }
    }
}



public static class InMemoryMessageQueueExtension
{
    public static Task SubscribeAsync<T>(this IMessageQueue<T> messageQueue, ISubscribeHandler<T> handler, CancellationToken cancellationToken = default)
    {
        if(handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return messageQueue.SubscribeAsync(handler, cancellationToken);
    }
   
}


