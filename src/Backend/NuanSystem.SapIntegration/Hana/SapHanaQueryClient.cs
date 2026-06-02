using System.Data.Common;

namespace NuanSystem.SapIntegration.Hana;

public sealed class SapHanaQueryClient(ISapHanaConnectionFactory connectionFactory) : ISapHanaQueryClient
{
    public async Task<IReadOnlyCollection<T>> QueryAsync<T>(
        int companyId,
        string sql,
        Func<DbDataReader, T> map,
        IReadOnlyDictionary<string, object?>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        ValidateReadOnlySql(sql);

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(companyId, cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = 60;

        AddParameters(command, parameters);

        var items = new List<T>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(map(reader));
        }

        return items;
    }

    private static void ValidateReadOnlySql(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            throw new InvalidOperationException("La consulta HANA no puede estar vacia.");
        }

        var normalized = sql.TrimStart();
        if (!normalized.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("El cliente HANA solo permite consultas SELECT.");
        }

        if (normalized.Contains(';'))
        {
            throw new InvalidOperationException("El cliente HANA solo permite una sentencia SELECT por consulta.");
        }
    }

    private static void AddParameters(
        DbCommand command,
        IReadOnlyDictionary<string, object?>? parameters)
    {
        if (parameters is null)
        {
            return;
        }

        foreach (var parameter in parameters)
        {
            var dbParameter = command.CreateParameter();
            dbParameter.ParameterName = parameter.Key;
            dbParameter.Value = parameter.Value ?? DBNull.Value;
            command.Parameters.Add(dbParameter);
        }
    }
}
