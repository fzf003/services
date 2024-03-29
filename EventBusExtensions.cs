


public static class EventBusExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
         
        //services.AddSingleton(typeof(InMemoryMessageQueue<>));
         
        services.AddProducer<UserCreateOrderIntegrationEvent>();
                //.AddConsumer<UserCreateOrderIntegrationEvent>();

        services.AddProducer<RegisterProductRequest>();
               // .AddConsumer<RegisterProductRequest>();
 
        
        services.AddHostedService<IntegrationEventProcessorJob>();

        services.AddHostedService<SCMEventProcessorJob>();

        return services;
    }

    static IServiceCollection AddProducer<T>(this IServiceCollection services) where T:IIntegrationEvent
    {
        var channel= new InMemoryMessageQueue<T>();
        services.AddSingleton<IProducer<T>>(channel);
        services.AddSingleton<IConsumer<T>>(channel);
        //services.AddSingleton<IProducer<T>>(sp =>sp.GetRequiredService<InMemoryMessageQueue<T>>());
        return services;
    }

    static IServiceCollection AddConsumer<T>(this IServiceCollection services) where T:IIntegrationEvent
    {
        services.AddSingleton<IConsumer<T>>(sp => sp.GetRequiredService<InMemoryMessageQueue<T>>());
        return services;
    }
}




public static class ReadWriteExtensions
{
 
    public static Task WithCancellation(this Task task, CancellationToken cancellationToken)
    {
        if (task.IsCompleted || !cancellationToken.CanBeCanceled)
            return task;

        var tcs = new TaskCompletionSource<object>();
        var r = cancellationToken.Register(() => { tcs.SetCanceled(); }, false);

        return Task.WhenAny(task, tcs.Task)

            .ContinueWith(_ => { r.Dispose(); }, TaskContinuationOptions.ExecuteSynchronously)

            .ContinueWith(_ => { }, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }
    
 

    public static IObservable<T> ToObservable<T>(this IAsyncEnumerable<T> asyncEnumerable, CancellationToken cancellation = default)
    {
    

        return Observable.Create<T>(async observer =>
       {
           while (!cancellation.IsCancellationRequested)
           {
               await foreach (var item in asyncEnumerable)
               {
                   observer.OnNext(item);
               }
               observer.OnCompleted();
           }

       });
    }
 
    public static IDisposable ToConsumerSubscribe<T>(this IConsumer<T> consumer,Action<T> action, CancellationToken cancellation = default) where T:IIntegrationEvent
    {
         return consumer.ReadMessageStreamAsync().ToObservable(cancellation).Subscribe(action);
    }

    public static IDisposable ToConsumerSubscribe<T>(this IConsumer<T> consumer,Action<T> action,Action OnComple, CancellationToken cancellation = default) where T:IIntegrationEvent
    {
         return consumer.ReadMessageStreamAsync().ToObservable(cancellation).Subscribe(action,OnComple);
    }

    public static IObservable<T> ToObservable<T>(this ChannelReader<T> channelReader, CancellationToken cancellation = default)
    {
        return Observable.Create<T>(async observer =>
        {
            while (!cancellation.IsCancellationRequested)
            {
                await foreach (var item in channelReader.ReadAllAsync(cancellation))
                {
                    observer.OnNext(item);
                }
                observer.OnCompleted();
            }

        });
    }
  
}
