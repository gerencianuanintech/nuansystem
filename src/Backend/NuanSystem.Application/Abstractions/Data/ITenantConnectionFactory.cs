using System.Data;

namespace NuanSystem.Application.Abstractions.Data;

public interface ITenantConnectionFactory
{
    IDbConnection CreateConnection();
}
