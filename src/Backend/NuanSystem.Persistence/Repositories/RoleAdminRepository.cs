using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Roles.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class RoleAdminRepository(IMasterConnectionFactory connectionFactory) : IRoleAdminRepository
{
    public async Task<IReadOnlyCollection<RoleAdminDto>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT Id, Code, Name, Description, IsActive
FROM dbo.Roles
ORDER BY Name;
""";

        using var connection = connectionFactory.CreateConnection();
        var roles = (await connection.QueryAsync<RoleRecord>(
            new CommandDefinition(sql, cancellationToken: cancellationToken))).AsList();

        return await HydrateAsync(roles, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    p.Id,
    m.Code AS ModuleCode,
    m.Name AS ModuleName,
    p.Code,
    p.Name,
    p.Description,
    p.IsActive
FROM dbo.Permissions p
INNER JOIN dbo.Modules m ON m.Id = p.ModuleId
ORDER BY m.DisplayOrder, p.Code;
""";

        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<PermissionDto>(
            new CommandDefinition(sql, cancellationToken: cancellationToken))).AsList();
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM dbo.Roles WHERE Code = @Code;";

        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Code = code }, cancellationToken: cancellationToken));

        return count > 0;
    }

    public async Task<int> CreateRoleAsync(CreateRoleData role, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO dbo.Roles (Code, Name, Description, IsActive)
OUTPUT INSERTED.Id
VALUES (@Code, @Name, @Description, @IsActive);
""";

        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, role, cancellationToken: cancellationToken));
    }

    public async Task AssignPermissionAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        const string sql = """
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = @RoleId)
BEGIN
    THROW 50010, 'El rol indicado no existe.', 1;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Permissions WHERE Id = @PermissionId)
BEGIN
    THROW 50011, 'El permiso indicado no existe.', 1;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @RoleId AND PermissionId = @PermissionId)
BEGIN
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
    VALUES (@RoleId, @PermissionId);
END;
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { RoleId = roleId, PermissionId = permissionId },
            cancellationToken: cancellationToken));
    }

    public async Task<RoleAdminDto?> GetRoleByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT Id, Code, Name, Description, IsActive
FROM dbo.Roles
WHERE Id = @Id;
""";

        using var connection = connectionFactory.CreateConnection();
        var role = await connection.QuerySingleOrDefaultAsync<RoleRecord>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        if (role is null)
        {
            return null;
        }

        return (await HydrateAsync(new[] { role }, cancellationToken)).Single();
    }

    private async Task<IReadOnlyCollection<RoleAdminDto>> HydrateAsync(
        IReadOnlyCollection<RoleRecord> roles,
        CancellationToken cancellationToken)
    {
        if (roles.Count == 0)
        {
            return Array.Empty<RoleAdminDto>();
        }

        const string sql = """
SELECT rp.RoleId, p.Code
FROM dbo.RolePermissions rp
INNER JOIN dbo.Permissions p ON p.Id = rp.PermissionId
WHERE rp.RoleId IN @RoleIds
ORDER BY p.Code;
""";

        var roleIds = roles.Select(role => role.Id).ToArray();
        using var connection = connectionFactory.CreateConnection();
        var permissions = (await connection.QueryAsync<RolePermissionValue>(
            new CommandDefinition(sql, new { RoleIds = roleIds }, cancellationToken: cancellationToken)))
            .GroupBy(item => item.RoleId)
            .ToDictionary(group => group.Key, group => (IReadOnlyCollection<string>)group.Select(item => item.Code).ToArray());

        return roles.Select(role => new RoleAdminDto(
            role.Id,
            role.Code,
            role.Name,
            role.Description,
            role.IsActive,
            permissions.TryGetValue(role.Id, out var rolePermissions) ? rolePermissions : Array.Empty<string>())).ToArray();
    }

    private sealed record RoleRecord(int Id, string Code, string Name, string? Description, bool IsActive);
    private sealed record RolePermissionValue(int RoleId, string Code);
}
