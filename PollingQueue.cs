
using Microsoft.Data.SqlClient;
 

public interface IPollingQueue
{
    Task<List<UserInfo>> DequeueAsync(CancellationToken cancellationToken);
}
/**
Observable.Timer(TimeSpan.Zero,TimeSpan.FromMilliseconds(1000))
          .Select(p=>Observable.FromAsync(async ()=>await PollingQueue.DequeueAsync(CancellationToken.None).ConfigureAwait(false)))
          .Switch().SelectMany(p=>p)
          .Subscribe(Console.WriteLine);
**/
public class PollingQueue : IPollingQueue
{
    readonly Func<SqlConnection> _sqlConnection;

    readonly string TableName = "HD..users";

    readonly int batchCount=10;
    int index=1;
    public PollingQueue(Func<SqlConnection> sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<List<UserInfo>> DequeueAsync(CancellationToken cancellationToken)
    {
        var count=batchCount*index;

        using var sqlConnection = _sqlConnection();

        var result = await SqlQueue.GetNextAsync(sqlConnection, TableName,batchCount:count, cancellationToken: cancellationToken);

        while (result.Count == 0 && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            result = await SqlQueue.GetNextAsync(sqlConnection, TableName,batchCount:count, cancellationToken: cancellationToken);
        }

        index++;

        return result;
    }
}
