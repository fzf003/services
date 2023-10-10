
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

/*
   
var Dispatcher = new OutboxDispatcher(() => (SqlConnection)factory.CreateConnection("HD"));

var source = Observable.FromAsync(async () => await PollingQueue.DequeueAsync(CancellationToken.None));

Polling.Poll(source)
       .SelectMany(p => p)
       .Select(MessageAckAsync).Retry(2)
       .Switch()
       .OnErrorResumeNext(Observable.Return(UserInfo.CreateUser("", "S", DateTime.Now)))
       .Subscribe(Console.WriteLine, err => Console.WriteLine(err));



static IObservable<UserInfo> MessageAckAsync(UserInfo user)
{
    return Observable.FromAsync(async () =>
      {
          using var sqlConnection = new SqlConnection(connectionString);
          await sqlConnection.OpenAsync().ConfigureAwait(false);
          await SqlQueue.AckAsync(sqlConnection, user.UserId).ConfigureAwait(false);
          return user;
      });
}
*/
