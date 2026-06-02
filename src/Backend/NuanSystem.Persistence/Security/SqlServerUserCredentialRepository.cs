using Dapper;
using NuanSystem.Application.Abstractions.Data;

namespace NuanSystem.Persistence.Security;

public sealed class SqlServerUserCredentialRepository(IMasterConnectionFactory connectionFactory) : IUserCredentialRepository
{
    public async Task<string?> GetActivePasswordHashAsync(int userId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<string?>(
            new CommandDefinition(
                "SELECT PasswordHash FROM dbo.Users WHERE Id = @UserId AND IsActive = 1 AND IsDeleted = 0;",
                new { UserId = userId },
                cancellationToken: cancellationToken));
    }

    public async Task UpdatePasswordAsync(int userId, string passwordHash, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                """
UPDATE dbo.Users
SET PasswordHash = @PasswordHash,
    MustChangePassword = 0,
    FailedAccessCount = 0,
    LockoutEndAt = NULL,
    UpdatedByUserId = @UserId,
    UpdatedByUserName = @UserName,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @UserId
  AND IsActive = 1
  AND IsDeleted = 0;
""",
                new { PasswordHash = passwordHash, UserId = userId, UserName = (string?)null },
                cancellationToken: cancellationToken));
    }
}
