using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SecurityRoles.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SecurityRoleRepository(IMasterConnectionFactory connectionFactory) : ISecurityRoleRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_ROLSEGURIDADLISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_ROLSEGURIDADBUSCARPORID";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_ROLSEGURIDADBUSCARPORCODIGO";
    private const string ExistsByNameProcedure = "dbo.SP_NA_GET_ROLSEGURIDADBUSCARPORNOMBRE";
    private const string CreateProcedure = "dbo.SP_NA_POST_ROLSEGURIDADCREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_ROLSEGURIDADACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_ROLSEGURIDADELIMINAR";

    public async Task<IReadOnlyCollection<SecurityRoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var roles = (await connection.QueryAsync<SecurityRoleRecord>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();

        return roles.Select(MapRole).ToArray();
    }

    public async Task<SecurityRoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var role = await connection.QuerySingleOrDefaultAsync<SecurityRoleRecord>(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return role is null ? null : MapRole(role);
    }

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return ExistsByCodeCoreAsync(code, null, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default)
    {
        return ExistsByCodeCoreAsync(code, excludingId, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return ExistsByNameCoreAsync(name, null, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, int excludingId, CancellationToken cancellationToken = default)
    {
        return ExistsByNameCoreAsync(name, excludingId, cancellationToken);
    }

    public async Task<int> CreateAsync(CreateSecurityRoleData role, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, role, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateAsync(UpdateSecurityRoleData role, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, role, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                DeleteProcedure,
                new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    private async Task<bool> ExistsByCodeCoreAsync(string code, int? excludingId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ExistsByCodeProcedure,
                new { Code = code, ExcluirId = excludingId },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    private async Task<bool> ExistsByNameCoreAsync(string name, int? excludingId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ExistsByNameProcedure,
                new { Name = name, ExcluirId = excludingId },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    private static SecurityRoleDto MapRole(SecurityRoleRecord role)
    {
        return new SecurityRoleDto(
            role.Id,
            role.Code,
            role.Name,
            role.Description,
            role.DisplayOrder,
            role.IsSystemRole,
            role.IsAssignable,
            role.IsActive,
            SplitText(role.PermissionsText),
            role.CreatedByUserId,
            role.CreatedByUserName,
            role.CreatedAt,
            role.UpdatedByUserId,
            role.UpdatedByUserName,
            role.UpdatedAt,
            role.DeletedByUserId,
            role.DeletedByUserName,
            role.DeletedAt);
    }

    private static IReadOnlyCollection<string> SplitText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Array.Empty<string>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private sealed record SecurityRoleRecord(
        int Id,
        string Code,
        string Name,
        string? Description,
        int DisplayOrder,
        bool IsSystemRole,
        bool IsAssignable,
        bool IsActive,
        string? PermissionsText,
        int? CreatedByUserId,
        string? CreatedByUserName,
        DateTime CreatedAt,
        int? UpdatedByUserId,
        string? UpdatedByUserName,
        DateTime? UpdatedAt,
        int? DeletedByUserId,
        string? DeletedByUserName,
        DateTime? DeletedAt);
}
