using System.Data.Common;

namespace NuanSystem.SapIntegration.Hana;

public interface ISapHanaQueryClient
{
    Task<IReadOnlyCollection<T>> QueryAsync<T>(
        int companyId,
        string sql,
        Func<DbDataReader, T> map,
        IReadOnlyDictionary<string, object?>? parameters = null,
        CancellationToken cancellationToken = default);
}
