

using System.Data;
using System.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using SCZS.Persistence.Dapper;

public class SqlQueue
{
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

    /*
       while (true)
    {
        await SqlQueue.CreateQueue(() => (SqlConnection)factory.CreateConnection("HD"));
        Console.ReadKey();
    }

    */
    public static async Task CreateQueue(Func<SqlConnection> sqlConnectionfuc, string sql = "SELECT * FROM HD..Users where UserId>@UserId")
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
                while (await dataReader.ReadAsync())
                {

                    var message = await GetMessageAsync(dataReader, headersIndex, CancellationToken.None);
                    Console.WriteLine(message);
                }
            }

            scope.Complete();
        }
    }

    static async Task<string> GetMessageAsync(SqlDataReader dataReader, int headersIndex, CancellationToken cancellationToken)
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
