using System.Data.Common;

namespace NuanSystem.SapIntegration.Hana;

public interface ISapHanaConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}
