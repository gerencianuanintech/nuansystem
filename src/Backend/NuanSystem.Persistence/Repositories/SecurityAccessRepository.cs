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
        return (await connection.QueryAsync<NavigationMenuDto>(
            new CommandDefinition(NavigationProcedure, new { UserId = userId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<IReadOnlyCollection<FormOperationAccessDto>> GetFormOperationsAsync(int userId, string formKey, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<FormOperationAccessDto>(
            new CommandDefinition(FormOperationsProcedure, new { UserId = userId, FormKey = formKey }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<RoleAccessDto> GetRoleAccessAsync(int roleId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(GetRoleAccessProcedure, new { RoleId = roleId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        var menus = (await multi.ReadAsync<RoleAccessMenuDto>()).AsList();
        var operations = (await multi.ReadAsync<RoleAccessOperationDto>()).AsList();
        return new RoleAccessDto(menus, operations);
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
}
