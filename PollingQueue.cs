public interface IPollingQueue
{
    Task<List<UserInfo>> DequeueAsync(CancellationToken cancellationToken);
}


/**
1. Observable.Timer(TimeSpan.Zero,TimeSpan.FromMilliseconds(1000))
          .Select(p=>Observable.FromAsync(async ()=>await PollingQueue.DequeueAsync(CancellationToken.None).ConfigureAwait(false)))
          .Switch().SelectMany(p=>p)
          .Subscribe(Console.WriteLine);

2. var PollingQueue = new PollingQueue(() => (SqlConnection)factory.CreateConnection("HD"));
   var source = Observable.FromAsync(() => PollingQueue.DequeueAsync(CancellationToken.None));

  Polling.Poll(source)
         .SelectMany(p => p)
         .Select(MessageAckAsync).Retry(3)
         .Switch()
         .OnErrorResumeNext(Observable.Return(UserInfo.CreateUser("Error", "S", DateTime.Now)))
         .Subscribe(Console.WriteLine, err => Console.WriteLine(err));
**/
////拉取数据库队列
public class PollingQueue : IPollingQueue
{
    readonly Func<SqlConnection> _sqlConnection;
    readonly string TableName = "HD..users";

    readonly int batchCount = 1;
 
    public PollingQueue(Func<SqlConnection> sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<List<UserInfo>> DequeueAsync(CancellationToken cancellationToken)
    {
        using var sqlConnection = _sqlConnection();
 
        var result = await SqlQueue.GetNextAsync(sqlConnection: sqlConnection, TableName: TableName, batchCount: batchCount, cancellationToken: cancellationToken);

        while (result.Count == 0 && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(200, cancellationToken).ConfigureAwait(false);

            result = await SqlQueue.GetNextAsync(sqlConnection, TableName, batchCount: batchCount, cancellationToken: cancellationToken);
        }

        Console.WriteLine(result.Count);

        return result;
    }
}
