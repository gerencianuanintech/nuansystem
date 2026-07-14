using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SecurityAccessRepository(IMasterConnectionFactory connectionFactory) : ISecurityAccessRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const string NavigationProcedure = "dbo.SP_NA_GET_SEGURIDADNAVEGACIONUSUARIO";
    private const string FormOperationsProcedure = "dbo.SP_NA_GET_SEGURIDADOPERACIONESUSUARIO";
    private const string GetRoleAccessProcedure = "dbo.SP_NA_GET_ACCESOROLCARGAR";
    private const string SaveRoleAccessProcedure = "dbo.SP_NA_PUT_ACCESOROLGUARDAR";

    public async Task<IReadOnlyCollection<NavigationMenuDto>> GetNavigationAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var menus = (await connection.QueryAsync<NavigationMenuRecord>(
            new CommandDefinition(NavigationProcedure, new { UserId = userId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();

        return menus.Select(MapNavigationMenu).ToArray();
    }

    public async Task<IReadOnlyCollection<FormOperationAccessDto>> GetFormOperationsAsync(int userId, string formKey, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var operations = (await connection.QueryAsync<FormOperationAccessRecord>(
            new CommandDefinition(FormOperationsProcedure, new { UserId = userId, FormKey = formKey }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();

        return operations.Select(MapFormOperation).ToArray();
    }

    public async Task<RoleAccessDto> GetRoleAccessAsync(int roleId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(GetRoleAccessProcedure, new { RoleId = roleId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        var menus = (await multi.ReadAsync<RoleAccessMenuRecord>()).AsList();
        var operations = (await multi.ReadAsync<RoleAccessOperationRecord>()).AsList();
        return new RoleAccessDto(
            menus.Select(MapRoleAccessMenu).ToArray(),
            operations.Select(MapRoleAccessOperation).ToArray());
    }

    public async Task SaveRoleAccessAsync(
        int roleId,
        IReadOnlyCollection<SaveRoleAccessMenuData> menus,
        IReadOnlyCollection<SaveRoleAccessOperationData> operations,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            SaveRoleAccessProcedure,
            new
            {
                RoleId = roleId,
                MenusJson = JsonSerializer.Serialize(menus, JsonOptions),
                OperationsJson = JsonSerializer.Serialize(operations, JsonOptions),
                UpdatedByUserId = updatedByUserId,
                UpdatedByUserName = updatedByUserName
            },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));
    }

    private static NavigationMenuDto MapNavigationMenu(NavigationMenuRecord menu)
    {
        return new NavigationMenuDto(
            menu.Id,
            menu.ParentId,
            menu.Code,
            menu.Name,
            menu.Description,
            menu.MenuType,
            menu.FormKey,
            menu.IconLarge,
            menu.IconSmall,
            menu.DisplayOrder,
            menu.IsVisible,
            menu.IsActive);
    }

    private static FormOperationAccessDto MapFormOperation(FormOperationAccessRecord operation)
    {
        return new FormOperationAccessDto(
            operation.OperationId,
            operation.Code,
            operation.Name,
            operation.Description,
            operation.ActionKey,
            operation.RibbonPageName,
            operation.RibbonGroupName,
            operation.IconLarge,
            operation.IconSmall,
            operation.DisplayOrder,
            operation.IsAllowed);
    }

    private static RoleAccessMenuDto MapRoleAccessMenu(RoleAccessMenuRecord menu)
    {
        return new RoleAccessMenuDto(
            menu.MenuId,
            menu.ParentId,
            menu.Code,
            menu.Name,
            menu.MenuType,
            menu.FormKey,
            menu.IsAllowed);
    }

    private static RoleAccessOperationDto MapRoleAccessOperation(RoleAccessOperationRecord operation)
    {
        return new RoleAccessOperationDto(
            operation.FormId,
            operation.FormCode,
            operation.FormName,
            operation.FormKey,
            operation.OperationId,
            operation.OperationCode,
            operation.OperationName,
            operation.ActionKey,
            operation.IsAllowed);
    }

    private sealed class NavigationMenuRecord
    {
        public int Id { get; init; }
        public int? ParentId { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int MenuType { get; init; }
        public string? FormKey { get; init; }
        public string? IconLarge { get; init; }
        public string? IconSmall { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsVisible { get; init; }
        public bool IsActive { get; init; }
    }

    private sealed class FormOperationAccessRecord
    {
        public int OperationId { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? ActionKey { get; init; }
        public string? RibbonPageName { get; init; }
        public string? RibbonGroupName { get; init; }
        public string? IconLarge { get; init; }
        public string? IconSmall { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsAllowed { get; init; }
    }

    private sealed class RoleAccessMenuRecord
    {
        public int MenuId { get; init; }
        public int? ParentId { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public int MenuType { get; init; }
        public string? FormKey { get; init; }
        public bool IsAllowed { get; init; }
    }

    private sealed class RoleAccessOperationRecord
    {
        public int FormId { get; init; }
        public string FormCode { get; init; } = string.Empty;
        public string FormName { get; init; } = string.Empty;
        public string FormKey { get; init; } = string.Empty;
        public int OperationId { get; init; }
        public string OperationCode { get; init; } = string.Empty;
        public string OperationName { get; init; } = string.Empty;
        public string? ActionKey { get; init; }
        public bool IsAllowed { get; init; }
    }
}
