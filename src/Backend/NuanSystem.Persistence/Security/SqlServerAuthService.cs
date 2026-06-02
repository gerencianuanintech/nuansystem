using System.Data;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Security;

public sealed class SqlServerAuthService(
    MasterConnectionFactory connectionFactory,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IUserSecurityStateService userSecurityStateService) : IAuthService
{
    public async Task<AuthResult?> LoginAsync(
        string userNameOrEmail,
        string password,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var user = await FindUserAsync(connection, userNameOrEmail, cancellationToken);
        if (user is null)
        {
            return null;
        }

        if (user.LockoutEndAt > DateTime.UtcNow)
        {
            return null;
        }

        if (!passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            await RegisterFailedLoginAsync(connection, user.Id, cancellationToken);
            return null;
        }

        var roles = await GetRolesAsync(connection, user.Id, cancellationToken);
        var permissions = await GetPermissionsAsync(connection, user.Id, cancellationToken);
        var companies = await GetCompaniesForUserAsync(connection, user.Id, cancellationToken);
        var securityStamp = await userSecurityStateService.GetSecurityStampAsync(user.Id, cancellationToken);
        if (string.IsNullOrWhiteSpace(securityStamp))
        {
            return null;
        }

        var token = jwtTokenService.CreateToken(
            user.Id,
            user.UserName,
            user.DisplayName,
            user.MustChangePassword,
            securityStamp,
            roles,
            permissions);

        await RegisterSuccessfulLoginAsync(connection, user.Id, cancellationToken);

        return new AuthResult(
            user.Id,
            user.UserName,
            user.DisplayName,
            token.AccessToken,
            token.ExpiresAtUtc,
            user.MustChangePassword,
            roles,
            permissions,
            companies);
    }

    public async Task<IReadOnlyCollection<AuthCompanyDto>> GetCompaniesForUserAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        return await GetCompaniesForUserAsync(connection, userId, cancellationToken);
    }

    private static async Task<UserRecord?> FindUserAsync(
        SqlConnection connection,
        string userNameOrEmail,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
SELECT TOP (1)
    Id,
    UserName,
    DisplayName,
    PasswordHash,
    MustChangePassword,
    LockoutEndAt,
    IsLocked
FROM dbo.Users
WHERE IsActive = 1
  AND IsLocked = 0
  AND IsDeleted = 0
  AND (NormalizedUserName = UPPER(@userNameOrEmail) OR NormalizedEmail = UPPER(@userNameOrEmail));
""";
        command.Parameters.Add("@userNameOrEmail", SqlDbType.NVarChar, 256).Value = userNameOrEmail.Trim();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new UserRecord(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("UserName")),
            reader.GetString(reader.GetOrdinal("DisplayName")),
            reader.GetString(reader.GetOrdinal("PasswordHash")),
            reader.GetBoolean(reader.GetOrdinal("MustChangePassword")),
            reader.IsDBNull(reader.GetOrdinal("LockoutEndAt")) ? null : reader.GetDateTime(reader.GetOrdinal("LockoutEndAt")),
            reader.GetBoolean(reader.GetOrdinal("IsLocked")));
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

    private static async Task<IReadOnlyCollection<AuthCompanyDto>> GetCompaniesForUserAsync(
        SqlConnection connection,
        int userId,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
SELECT c.Id,
       c.Code,
       c.CommercialName,
       c.LogoImage,
       c.LogoImageContentType,
       c.LogoImageFileName
FROM dbo.UserCompanies uc
INNER JOIN dbo.Companies c ON c.Id = uc.CompanyId
WHERE uc.UserId = @userId
  AND uc.IsActive = 1
  AND c.IsActive = 1
ORDER BY c.CommercialName;
""";
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        var companies = new List<AuthCompanyDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            companies.Add(new AuthCompanyDto(
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("Code")),
                reader.GetString(reader.GetOrdinal("CommercialName")),
                reader.IsDBNull(reader.GetOrdinal("LogoImage")) ? null : (byte[])reader["LogoImage"],
                reader.IsDBNull(reader.GetOrdinal("LogoImageContentType")) ? null : reader.GetString(reader.GetOrdinal("LogoImageContentType")),
                reader.IsDBNull(reader.GetOrdinal("LogoImageFileName")) ? null : reader.GetString(reader.GetOrdinal("LogoImageFileName"))));
        }

        return companies;
    }

    private static async Task RegisterSuccessfulLoginAsync(
        SqlConnection connection,
        int userId,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
UPDATE dbo.Users
SET LastLoginAt = SYSUTCDATETIME(),
    FailedAccessCount = 0,
    LockoutEndAt = NULL
WHERE Id = @userId;
""";
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task RegisterFailedLoginAsync(
        SqlConnection connection,
        int userId,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
UPDATE dbo.Users
SET FailedAccessCount = FailedAccessCount + 1,
    LockoutEndAt = CASE
        WHEN FailedAccessCount + 1 >= 5 THEN DATEADD(MINUTE, 15, SYSUTCDATETIME())
        ELSE LockoutEndAt
    END
WHERE Id = @userId;
""";
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private sealed record UserRecord(
        int Id,
        string UserName,
        string DisplayName,
        string PasswordHash,
        bool MustChangePassword,
        DateTime? LockoutEndAt,
        bool IsLocked);
}
