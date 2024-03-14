using System.Threading.Channels;

internal sealed class InMemoryMessageQueue<T> : IEventBus<T> where T : IIntegrationEvent
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

    public IAsyncEnumerable<T> ReadMessageStreamAsync(CancellationToken cancellationToken = default)
    {
        return Reader.ReadAllAsync(cancellationToken);
    }
}


public interface IConsumer<T> where T : IIntegrationEvent
{
    IAsyncEnumerable<T> ReadMessageStreamAsync(CancellationToken cancellationToken = default);
}

public interface IProducer<T>  where T : IIntegrationEvent
{
    Task PublishAsync(T @event, CancellationToken token = default);
}

public interface IEventBus<T> : IProducer<T>, IConsumer<T> where T : IIntegrationEvent
{
  
}




