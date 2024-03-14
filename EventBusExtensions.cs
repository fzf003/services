


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