using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Security;

public sealed class UserSecurityStateService(MasterConnectionFactory connectionFactory) : IUserSecurityStateService
{
    public async Task<string?> GetSecurityStampAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var userState = await GetUserStateAsync(connection, userId, cancellationToken);
        if (userState is null || !userState.IsActive || userState.IsLocked || userState.LockoutEndAt > DateTime.UtcNow)
        {
            return null;
        }

        var roles = await GetRolesAsync(connection, userId, cancellationToken);
        var permissions = await GetPermissionsAsync(connection, userId, cancellationToken);

        var material = string.Join('|',
            userId.ToString(),
            userState.MustChangePassword ? "1" : "0",
            userState.UpdatedAt?.Ticks.ToString() ?? string.Empty,
            string.Join(',', roles),
            string.Join(',', permissions));

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(material)));
    }

    private static async Task<UserSecurityState?> GetUserStateAsync(
        SqlConnection connection,
        int userId,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
SELECT TOP (1)
    IsActive,
    IsLocked,
    MustChangePassword,
    LockoutEndAt,
    UpdatedAt
FROM dbo.Users
WHERE Id = @userId
  AND IsDeleted = 0;
""";
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new UserSecurityState(
            reader.GetBoolean(reader.GetOrdinal("IsActive")),
            reader.GetBoolean(reader.GetOrdinal("IsLocked")),
            reader.GetBoolean(reader.GetOrdinal("MustChangePassword")),
            reader.IsDBNull(reader.GetOrdinal("LockoutEndAt")) ? null : reader.GetDateTime(reader.GetOrdinal("LockoutEndAt")),
            reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")));
    }

    private static async Task<IReadOnlyCollection<string>> GetRolesAsync(
        SqlConnection connection,
        int userId,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
SELECT r.Code
FROM dbo.UserRoles ur
INNER JOIN dbo.Roles r ON r.Id = ur.RoleId
WHERE ur.UserId = @userId
  AND r.IsActive = 1
ORDER BY r.Code;
""";
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        var roles = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            roles.Add(reader.GetString(0));
        }

        return roles;
    }

    private static async Task<IReadOnlyCollection<string>> GetPermissionsAsync(
        SqlConnection connection,
        int userId,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
SELECT DISTINCT p.Code
FROM dbo.UserRoles ur
INNER JOIN dbo.Roles r ON r.Id = ur.RoleId AND r.IsActive = 1
INNER JOIN dbo.RolePermissions rp ON rp.RoleId = r.Id
INNER JOIN dbo.Permissions p ON p.Id = rp.PermissionId AND p.IsActive = 1
WHERE ur.UserId = @userId
ORDER BY p.Code;
""";
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        var permissions = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            permissions.Add(reader.GetString(0));
        }

        return permissions;
    }

    private sealed record UserSecurityState(
        bool IsActive,
        bool IsLocked,
        bool MustChangePassword,
        DateTime? LockoutEndAt,
        DateTime? UpdatedAt);
}
