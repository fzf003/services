
public static partial class Polling
{
    public static IObservable<Unit> DefaultPoller = Observable
        .Timer(TimeSpan.FromMilliseconds(100))
        .Select(_ => Unit.Default);

    internal static IObservable<T> Poll<T>(this IObservable<T> query, IObservable<Unit> poller) => poller
        .SelectMany(_ => query)
        .Repeat();

    internal static IObservable<T> Poll<T>(this IObservable<T> query) => query.Poll(DefaultPoller);
}
