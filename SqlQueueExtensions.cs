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


    public static async Task<string> GetBodyAsync(this SqlDataReader dataReader, int Index, CancellationToken cancellationToken = default)
    {
        if (await dataReader.IsDBNullAsync(Index, cancellationToken).ConfigureAwait(false))
        {
            return null;
        }
        return await dataReader.GetFieldValueAsync<string>(Index, cancellationToken).ConfigureAwait(false);
    }


    public static async Task<string> GetMessageAsync(this SqlDataReader dataReader, int headersIndex, CancellationToken cancellationToken = default)
    {
        if (await dataReader.IsDBNullAsync(headersIndex, cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        using (var textReader = dataReader.GetTextReader(headersIndex))
        {
            return await textReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
