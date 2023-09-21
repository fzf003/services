

using System.Data;
using System.Runtime.CompilerServices;
using System.Transactions;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Crmf;
using SCZS.Persistence.Dapper;
/**
    await factory.ExecuteTranAsync("HD", async (sqlConnection, transaction) =>
    {
        var currtransaction = (SqlTransaction)transaction;
        
        var connection = (SqlConnection)sqlConnection;

        var count= await SqlQueue.TryPeek(connection,currtransaction).ConfigureAwait(false);

        Console.WriteLine(count);

        Console.WriteLine(sqlConnection.State);

        await foreach (var item in SqlQueue.ReaderStreamAsync(sqlConnection: connection, transaction: currtransaction).WithCancellation(CancellationToken.None))
        {
            Console.WriteLine(item);
        }

        Console.WriteLine(sqlConnection.State);

        await SqlQueue.SendAsync(connection, currtransaction, UserInfo.CreateUser($"fzf-{index}", "男", DateTime.Now)).ConfigureAwait(false);

        Console.WriteLine(sqlConnection.State);

        Console.WriteLine("=============================================");

        Console.WriteLine(sqlConnection.State);
        
        transaction.Commit();
 
    });
**/
public class SqlQueue
{

    #region Sql
    public static readonly string CreateQueueText = @"
                            IF EXISTS (
                                SELECT *
                                FROM {1}.sys.objects
                                WHERE object_id = OBJECT_ID(N'{0}')
                                    AND type in (N'U'))
                            RETURN

                            EXEC sp_getapplock @Resource = '{0}_lock', @LockMode = 'Exclusive'

                            IF EXISTS (
                                SELECT *
                                FROM {1}.sys.objects
                                WHERE object_id = OBJECT_ID(N'{0}')
                                    AND type in (N'U'))
                            BEGIN
                                EXEC sp_releaseapplock @Resource = '{0}_lock'
                                RETURN
                            END

                            BEGIN TRY
                                CREATE TABLE {0} (
                                    Id uniqueidentifier NOT NULL,
                                    CorrelationId varchar(255),
                                    ReplyToAddress varchar(255),
                                    Recoverable bit NOT NULL,
                                    Expires datetime,
                                    Headers nvarchar(max) NOT NULL,
                                    Body varbinary(max),
                                    RowVersion bigint IDENTITY(1,1) NOT NULL
                                );

                                CREATE NONCLUSTERED INDEX Index_RowVersion ON {0}
                                (
                                    [RowVersion] ASC
                                )

                                CREATE NONCLUSTERED INDEX Index_Expires ON {0}
                                (
                                    Expires
                                )
                                INCLUDE
                                (
                                    Id,
                                    RowVersion
                                )
                                WHERE
                                    Expires IS NOT NULL
                            END TRY
                            BEGIN CATCH
                                EXEC sp_releaseapplock @Resource = '{0}_lock';
                                THROW;
                            END CATCH;

                            EXEC sp_releaseapplock @Resource = '{0}_lock'";
    public static readonly string PeekText = @"
SELECT isnull(cast(max([RowVersion]) - min([RowVersion]) + 1 AS int), 0) Id FROM {0} WITH (nolock)";

    public static readonly string QueueName = "InputQueue";


    public static readonly string SendSqlText = "INSERT INTO {0} (UserName,Sex,DueAfter) values(@UserName,@Sex,@DueAfter)";
    #endregion

    static IServiceProvider GetServiceProvider(Action<ServiceCollection> action = null)
    {
        const string connectionString = "Data Source=10.32.3.13;Initial Catalog=HD;Integrated Security=False;User ID=sa;Password=sczs_dev2020; Max Pool Size=1000;Connect Timeout=3000;TrustServerCertificate=True";
        var services = new ServiceCollection();

        if (action is not null)
        {
            action(services);
        }

        services.AddDapperPersistence(dic =>
        {
            dic.Add("HD", connectionString);
        });

        return services.BuildServiceProvider();
    }



    public static async Task<int> SendAsync(SqlConnection sqlConnection, UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        int Affected = 0;
        using (var transaction = sqlConnection.BeginTransaction())
        {
            Affected = await SendAsync(sqlConnection, transaction, userInfo, cancellationToken);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        return Affected;
    }

    public static async Task<int> SendAsync(SqlConnection sqlConnection, SqlTransaction transaction, UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        int Affected = 0;

        {
            var sendCommand = string.Format(SendSqlText, "HD..Users");

            using (var SqlCommand = new SqlCommand(sendCommand, sqlConnection, transaction))
            {
                SqlCommand.AddParameter(nameof(userInfo.UserName), SqlDbType.NVarChar, userInfo.UserName);
                SqlCommand.AddParameter(nameof(userInfo.Sex), SqlDbType.NVarChar, userInfo.Sex);
                SqlCommand.AddParameter(nameof(userInfo.DueAfter), SqlDbType.DateTime, userInfo.DueAfter);

                Affected = await SqlCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            //await transaction.CommitAsync(cancellationToken).ConfigureAwait(false); ;
        }

        return Affected;
    }





    public static async Task<int> TryPeek(SqlConnection sqlConnection, int? timeoutInSeconds = null, CancellationToken cancellationToken = default)
    {
        int numberOfMessages = 0;
        using (var transaction = sqlConnection.BeginTransaction())
        {
            numberOfMessages = await TryPeek(sqlConnection, transaction, timeoutInSeconds, cancellationToken).ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false); ;
        }

        return numberOfMessages;
    }

    public static async Task<int> TryPeek(SqlConnection connection, SqlTransaction transaction, int? timeoutInSeconds = null, CancellationToken cancellationToken = default)
    {
        var peekCommand = string.Format(PeekText, QueueName);
        using (var command = new SqlCommand(peekCommand, connection, transaction)
        {
            CommandTimeout = timeoutInSeconds ?? 30
        })
        {
            var numberOfMessages = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            return (int)numberOfMessages;
        }
    }



    public static async Task CreateQueue(SqlConnection sqlConnection, CancellationToken cancellationToken = default)
    {

        var sql = string.Format(CreateQueueText, QueueName, "HD");
        try
        {
            using (var transaction = sqlConnection.BeginTransaction())
            {

                using (var command = new SqlCommand(sql, sqlConnection, transaction)
                {
                    CommandType = CommandType.Text
                })
                {
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                }

                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (SqlException e) when (e.Number is 2714 or 1913)
        {
            e.Data["Sql"] = sql;
            throw e;
        }
    }

    public static async IAsyncEnumerable<UserInfo> ReaderStreamAsync(SqlConnection sqlConnection, string sql = "SELECT * FROM HD..Users where UserId>@UserId", int Id = 0, CancellationToken cancellationToken = default)
    {
        using (var transaction = sqlConnection.BeginTransaction())
        {
            await foreach (var message in ReaderStreamAsync(sqlConnection, transaction, sql, Id, cancellationToken).ConfigureAwait(false))
            {
               yield return message;
            }
        }
    }


    public static async IAsyncEnumerable<UserInfo> ReaderStreamAsync(SqlConnection sqlConnection, SqlTransaction transaction, string sql = "SELECT * FROM HD..Users where UserId>@UserId", int Id = 0, CancellationToken cancellationToken = default)
    {

        using (var SqlCommand = new SqlCommand(sql, sqlConnection, transaction))
        {
            SqlCommand.AddParameter("UserId", SqlDbType.Int, Id);

            using (var dataReader = await SqlCommand.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess | System.Data.CommandBehavior.Default).ConfigureAwait(false))
            {
                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var message = await UserInfo.ReadRow(dataReader, cancellationToken: CancellationToken.None).ConfigureAwait(false);
                    yield return message;
                }
            }
        }
    }


    public static async Task ReaderCreateQueue(SqlConnection sqlConnection, string sql = "SELECT * FROM HD..Users where UserId>@UserId", int Id = 0)
    {
        // var sqlConnection = sqlConnection;
        /*  if (sqlConnection.State == ConnectionState.Closed)
          {
              await sqlConnection.OpenAsync().ConfigureAwait(false);
          }*/
        using (var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        using (var Transaction = sqlConnection.BeginTransaction())
        using (var SqlCommand = new SqlCommand(sql, sqlConnection, transaction: Transaction))//sqlConnection.CreateCommand()
        {
            //SqlCommand.CommandText = sql;
            //SqlCommand.Transaction = (SqlTransaction)Transaction;
            #region add Parameters
            SqlCommand.AddParameter("UserId", SqlDbType.Int, Id);
            //SqlCommand.Parameters.AddWithValue("UserId", 0);
            #endregion

            using (var dataReader = await SqlCommand.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection).ConfigureAwait(false))
            {
                int headersIndex = 1;
                while (await dataReader.ReadAsync().ConfigureAwait(false))
                {
                    var message = await UserInfo.ReadRow(dataReader, cancellationToken: CancellationToken.None).ConfigureAwait(false);
                    // var message = await dataReader.GetMessageAsync(headersIndex, CancellationToken.None);
                    Console.WriteLine(message);
                }
            }

            scope.Complete();
        }

    }








}



public record UserInfo
{
    public static async Task<UserInfo> ReadRow(SqlDataReader dataReader, CancellationToken cancellationToken = default)
    {
        return new UserInfo
        {
            UserId = await dataReader.GetMessageIdAsync(0, cancellationToken).ConfigureAwait(false),
            UserName = await dataReader.GetStringAsync(1, cancellationToken).ConfigureAwait(false),
            Sex = await dataReader.GetStringAsync(2, cancellationToken).ConfigureAwait(false),
            DueAfter = await dataReader.GetDataTimeAsync(3, cancellationToken).ConfigureAwait(false)
        };

    }

    public static UserInfo CreateUser(string UserName, string Sex, DateTime DueAfter)
    {
        return new UserInfo
        {
            UserName = UserName,
            Sex = Sex,
            DueAfter = DueAfter
        };
    }

    public int UserId { get; set; }
    public string UserName { get; set; }

    public string Sex { get; set; }

    public DateTime DueAfter { get; set; }
}


public static class SqlQueueExtensions
{

    public static void AddParameter(this SqlCommand command, string name, SqlDbType type, object value)
    {
        command.Parameters.Add(name, type).Value = value ?? DBNull.Value;
    }

    public static void AddParameter(this SqlCommand command, string name, SqlDbType type, object value, int size)
    {
        command.Parameters.Add(name, type, size).Value = value ?? DBNull.Value;
    }

    public static async Task<int> GetMessageIdAsync(this SqlDataReader dataReader, int headersIndex, CancellationToken cancellationToken = default)
    {
        if (await dataReader.IsDBNullAsync(headersIndex, cancellationToken).ConfigureAwait(false))
        {
            return default;
        }
        return await dataReader.GetFieldValueAsync<int>(headersIndex, cancellationToken).ConfigureAwait(false);
    }


    public static async Task<string> GetStringAsync(this SqlDataReader dataReader, int headersIndex, CancellationToken cancellationToken = default)
    {
        if (await dataReader.IsDBNullAsync(headersIndex, cancellationToken).ConfigureAwait(false))
        {
            return default;
        }
        return await dataReader.GetFieldValueAsync<string>(headersIndex, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<DateTime> GetDataTimeAsync(this SqlDataReader dataReader, int headersIndex, CancellationToken cancellationToken = default)
    {
        if (await dataReader.IsDBNullAsync(headersIndex, cancellationToken).ConfigureAwait(false))
        {
            return default;
        }

        return await dataReader.GetFieldValueAsync<DateTime>(headersIndex, cancellationToken).ConfigureAwait(false);
    }


    public static async Task<string> GetMessageAsync(this SqlDataReader dataReader, int messageIndex, CancellationToken cancellationToken = default)
    {
        if (await dataReader.IsDBNullAsync(messageIndex, cancellationToken).ConfigureAwait(false))
        {
            return default;
        }

        using (var textReader = dataReader.GetTextReader(messageIndex))
        {
            return await textReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
