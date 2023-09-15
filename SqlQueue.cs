

using System.Data;
using System.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using SCZS.Persistence.Dapper;

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
    #endregion

    static void AddParameter(SqlCommand command, string name, SqlDbType type, object value)
    {
        command.Parameters.Add(name, type).Value = value ?? DBNull.Value;
    }

    static void AddParameter(SqlCommand command, string name, SqlDbType type, object value, int size)
    {
        command.Parameters.Add(name, type, size).Value = value ?? DBNull.Value;
    }



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



    public static async Task<int> TryPeek(SqlConnection sqlConnection, int? timeoutInSeconds = null, CancellationToken cancellationToken = default)
    {
        int numberOfMessages = 0;
        using (var transaction = sqlConnection.BeginTransaction())
        {
            numberOfMessages = await TryPeekCore(sqlConnection,transaction,timeoutInSeconds, cancellationToken).ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false); ;
        }

        return numberOfMessages;
    }

    static async Task<int> TryPeekCore(SqlConnection connection, SqlTransaction transaction, int? timeoutInSeconds = null, CancellationToken cancellationToken = default)
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


    public static async Task ReaderCreateQueue(Func<SqlConnection> sqlConnectionfuc, string sql = "SELECT * FROM HD..Users where UserId>@UserId")
    {
        var sqlConnection = sqlConnectionfuc();
        if (sqlConnection.State == ConnectionState.Closed)
        {
            await sqlConnection.OpenAsync().ConfigureAwait(false);
        }
        using (var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        using (var Transaction = sqlConnection.BeginTransaction())
        using (var SqlCommand = new SqlCommand(sql, sqlConnection, transaction: Transaction))//sqlConnection.CreateCommand()
        {
            //SqlCommand.CommandText = sql;
            //SqlCommand.Transaction = (SqlTransaction)Transaction;
            #region add Parameters
            //AddParameter(SqlCommand, "UserId", SqlDbType.Int, 0);
            SqlCommand.Parameters.AddWithValue("UserId", 0);
            #endregion

            using (var dataReader = await SqlCommand.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection).ConfigureAwait(false))
            {
                int headersIndex = 1;
                while (await dataReader.ReadAsync().ConfigureAwait(false))
                {

                    var message = await GetMessageAsync(dataReader, headersIndex, CancellationToken.None);
                    Console.WriteLine(message);
                }
            }

            scope.Complete();
        }
    }


    public static async Task<string> GetMessageAsync(SqlDataReader dataReader, int headersIndex, CancellationToken cancellationToken)
    {
        if (await dataReader.IsDBNullAsync(headersIndex, cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        using (var textReader = dataReader.GetTextReader(headersIndex))
        {
            return await textReader.ReadToEndAsync().ConfigureAwait(false);
        }
    }

}
