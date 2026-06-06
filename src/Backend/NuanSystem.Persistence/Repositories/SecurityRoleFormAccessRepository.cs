using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SecurityRoleFormAccessRepository(IMasterConnectionFactory connectionFactory) : ISecurityRoleFormAccessRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const string GetFormsProcedure = "dbo.SP_NA_GET_SECURITYROLEFORMACCESS_FORMULARIOS";
    private const string GetOperationsProcedure = "dbo.SP_NA_GET_SECURITYROLEFORMACCESS_OPERACIONES";
    private const string SaveOperationsProcedure = "dbo.SP_NA_PUT_SECURITYROLEFORMACCESS_GUARDAR";
    private const string ValidateUserOperationProcedure = "dbo.SP_NA_GET_SECURITYROLEFORMACCESS_USUARIO";

    public async Task<IReadOnlyCollection<SecurityFormAccessFormDto>> GetFormsAsync(
        int? formType,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SecurityFormAccessFormDto>(
            new CommandDefinition(
                GetFormsProcedure,
                new { FormType = formType, OnlyActive = onlyActive, Search = search },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<IReadOnlyCollection<SecurityFormAccessOperationDto>> GetOperationsAsync(
        int roleId,
        int formId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SecurityFormAccessOperationDto>(
            new CommandDefinition(
                GetOperationsProcedure,
                new { RoleId = roleId, FormId = formId, OnlyActive = onlyActive, Search = search },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task SaveOperationsAsync(
        int roleId,
        int formId,
        IReadOnlyCollection<SaveSecurityFormAccessOperationData> operations,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                SaveOperationsProcedure,
                new
                {
                    RoleId = roleId,
                    FormId = formId,
                    OperationsJson = JsonSerializer.Serialize(operations, JsonOptions),
                    UpdatedByUserId = updatedByUserId,
                    UpdatedByUserName = updatedByUserName
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> ValidateUserOperationAsync(
        int userId,
        string formKey,
        string actionKey,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                ValidateUserOperationProcedure,
                new { UserId = userId, FormKey = formKey, ActionKey = actionKey },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }
}
