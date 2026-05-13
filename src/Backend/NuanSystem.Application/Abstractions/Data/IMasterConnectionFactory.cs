using System.Data;

namespace NuanSystem.Application.Abstractions.Data;

public interface IMasterConnectionFactory
{
    IDbConnection CreateConnection();
}
