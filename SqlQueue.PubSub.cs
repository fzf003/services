
using System.Data;
using System.Reactive.Disposables;
using System.Threading.Channels;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
public partial class SqlQueue
{


    public static async Task CreateSubscriptionAsync(SqlConnection sqlConnection, string endpoint, string topic, string queueAddress)
    {

        var CreateSubscriptionSql = string.Format(SqlConstants.CreateSubscriptionTableText, "SubscriptionTable");

        if (sqlConnection.State != ConnectionState.Open)
        {
            await sqlConnection.OpenAsync().ConfigureAwait(false);
        }

        using (var transaction = sqlConnection.BeginTransaction())
        {
            using (var Sqlcommand = new SqlCommand(CreateSubscriptionSql, sqlConnection, transaction))
            {
                await Sqlcommand.ExecuteNonQueryAsync().ConfigureAwait(false);

                await Sqlcommand.DisposeAsync().ConfigureAwait(false);
            }

            await transaction.CommitAsync().ConfigureAwait(false);
        }

    }

    public static async Task Subscribe(SqlConnection sqlConnection, string endpointName, string queueAddress, string topic, CancellationToken cancellationToken = default)
    {
        var Subscribe = string.Format(SqlConstants.SubscribeText, "SubscriptionTable");

        if (sqlConnection.State != ConnectionState.Open)
        {
            await sqlConnection.OpenAsync().ConfigureAwait(false);
        }

        using (var transaction = sqlConnection.BeginTransaction())
        {
            using (var Sqlcommand = new SqlCommand(Subscribe, sqlConnection, transaction))
            {
                Sqlcommand.AddParameter("@Endpoint", endpointName)
                          .AddParameter("@QueueAddress", queueAddress)
                          .AddParameter("@Topic", topic);

                await Sqlcommand.ExecuteNonQueryAsync().ConfigureAwait(false);

                await Sqlcommand.DisposeAsync().ConfigureAwait(false);
            }
            await transaction.CommitAsync().ConfigureAwait(false);
        }


    }

    /*
      SqlQueue.Subscribe(ProcessChannel.Reader, async (user) =>
    {
        Console.WriteLine("processconsumer:" + user);

        throw new TimeoutException("更改数据超时");

        /*

        await using var sqlConnection = new SqlConnection(connStrBldr.ConnectionString);

        await SqlQueue.AckAsync(sqlConnection, user.UserId).ConfigureAwait(false);

        await httpClient.SendAsync(MakerequestMessage(user)).ToPipe(success: response =>
        {
            logger.LogInformation(response.Content.ReadAsStringAsync().Result);
            return response;
        }, failure: p =>
        {
            Console.WriteLine(p.Message);
            return p.Message;
        }).ConfigureAwait(false);*/

    }, OnError: (ex, message) =>
    {
        Console.WriteLine("执行出错的消息:" + message);
        Console.WriteLine("执行出错" + ex.Message);
    });
    */
    public static async Task Subscribe<T>(ChannelReader<T> channelReader, Func<T, Task> func, Action<Exception, T> OnError = null, CancellationToken cancellationToken = default)
    {
        await foreach (var item in channelReader.ReadAllAsync(cancellationToken))
        {

            try
            {
                await func(item).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                //取消令牌
            }
            catch (Exception ex)
            {
                if (OnError is not null)
                {
                    OnError(ex, item);
                }
            }
            finally
            {

            }
        }
    }



    public static async Task FetchAsync(ChannelWriter<UserInfo> channelWriter, Func<SqlConnection> sqlConnectionfunc, int batchsize, CancellationToken cancellationToken = default)
    {
        //var NextQuerySql = string.Format("SELECT top ({0}) * FROM HD..Users WHERE State in(0)", batchsize);

        var NextQuerySql = string.Format(GetNextQuery, batchsize, "HD..Users");

        while (!cancellationToken.IsCancellationRequested)
        {
            using SqlConnection sqlConnection = sqlConnectionfunc();
            await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
            using (var SqlCommand = new SqlCommand(NextQuerySql, sqlConnection))
            {

                using var dataReader = await SqlCommand.ExecuteReaderAsync(System.Data.CommandBehavior.Default | CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false);

                if (dataReader.HasRows == false)
                {
                    await sqlConnection.CloseAsync().ConfigureAwait(false);

                    await Task.Delay(1000 * 2).ConfigureAwait(false);

                    Console.WriteLine("............................................");
                    continue;
                }

                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var message = await UserInfo.ReadRow(dataReader, cancellationToken: cancellationToken).ConfigureAwait(false);

                    Console.WriteLine($"Send:{message}");

                    await channelWriter.WriteAsync(message).ConfigureAwait(false);
                }

                await sqlConnection.CloseAsync().ConfigureAwait(false);

            }

        }
    }

    public static async Task ConsumeAsync(ChannelReader<UserInfo> channelReader, Func<UserInfo, Task> func, Action<Exception, UserInfo> OnError = null, CancellationToken cancellationToken = default)
    {
        await foreach (var item in channelReader.ReadAllAsync(cancellationToken))
        {
            try
            {
                await func(item).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                //取消令牌
            }
            catch (Exception ex)
            {
                if (OnError is not null)
                {
                    OnError(ex, item);
                }
            }
            finally
            {

            }
        }
    }

public static async Task FetchAsync<T>(ChannelWriter<T> channelWriter, Func<SqlConnection> sqlConnectionfunc, int batchsize, CancellationToken cancellationToken = default)
    {
        var NextQuerySql = string.Format(GetNextQuery, batchsize, "HD..Users");

        while (!cancellationToken.IsCancellationRequested)
        {
            using SqlConnection sqlConnection = sqlConnectionfunc();
            await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
            using (var SqlCommand = new SqlCommand(NextQuerySql, sqlConnection))
            {

                using var dataReader = await SqlCommand.ExecuteReaderAsync(System.Data.CommandBehavior.Default | CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false);

                if (dataReader.HasRows == false)
                {
                    await sqlConnection.CloseAsync().ConfigureAwait(false);

                    await Task.Delay(1000 * 2).ConfigureAwait(false);

                    Console.WriteLine("............................................");
                    
                    continue;
                }

                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    //var message = await UserInfo.ReadRow(dataReader, cancellationToken: cancellationToken).ConfigureAwait(false);

                    var message=  dataReader.GetRowParser<T>()(dataReader);

                    Console.WriteLine($"Send:{message}");

                    await channelWriter.WriteAsync(message).ConfigureAwait(false);
                }

                await sqlConnection.CloseAsync().ConfigureAwait(false);

            }

        }
    }

}
