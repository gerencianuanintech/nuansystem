using Dapper;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Security;

public sealed class UserSecurityStateService(MasterConnectionFactory connectionFactory) : IUserSecurityStateService
{
    public async Task<string?> GetSecurityStampAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<string?>(new CommandDefinition(
            """
SELECT CONVERT(nvarchar(36), SecurityStamp)
FROM dbo.Users
WHERE Id = @UserId
  AND IsDeleted = 0
  AND IsActive = 1
  AND IsLocked = 0
  AND (LockoutEndAt IS NULL OR LockoutEndAt <= SYSUTCDATETIME());
""",
            new { UserId = userId },
            cancellationToken: cancellationToken));
    }
}
