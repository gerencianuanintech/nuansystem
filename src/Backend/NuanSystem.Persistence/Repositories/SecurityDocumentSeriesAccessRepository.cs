using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SecurityDocumentSeriesAccessRepository(IMasterConnectionFactory connectionFactory)
    : ISecurityDocumentSeriesAccessRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const string GetSelectedProcedure = "dbo.SP_NA_GET_SECURITYROLEDOCUMENTSERIES_SELECCIONADAS";
    private const string GetOperationsProcedure = "dbo.SP_NA_GET_SECURITYROLEDOCUMENTSERIES_OPERACIONES";
    private const string SaveProcedure = "dbo.SP_NA_PUT_SECURITYROLEDOCUMENTSERIES_GUARDAR";
    private const string GetAuthorizedSeriesForUserProcedure = "dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_USUARIO_AUTORIZADAS";
    private const string ValidateUserOperationProcedure = "dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_VALIDAROPERACIONUSUARIO";

    public async Task<IReadOnlySet<int>> GetSelectedSeriesIdsAsync(
        int roleId,
        string companyCode,
        string formKey,
        string? documentType,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var ids = await connection.QueryAsync<int>(
            new CommandDefinition(
                GetSelectedProcedure,
                new { RoleId = roleId, CompanyCode = companyCode, FormKey = formKey, DocumentType = documentType },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return ids.ToHashSet();
    }

    public async Task<IReadOnlyCollection<SecurityDocumentSeriesOperationAccessDto>> GetOperationsAsync(
        int roleId,
        string companyCode,
        string formKey,
        string documentType,
        int securityDocumentSeriesId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SecurityDocumentSeriesOperationAccessDto>(
            new CommandDefinition(
                GetOperationsProcedure,
                new
                {
                    RoleId = roleId,
                    CompanyCode = companyCode,
                    FormKey = formKey,
                    DocumentType = documentType,
                    SecurityDocumentSeriesId = securityDocumentSeriesId,
                    OnlyActive = onlyActive,
                    Search = search
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task SaveAsync(
        int roleId,
        string companyCode,
        string formKey,
        string documentType,
        int securityDocumentSeriesId,
        bool isSelected,
        IReadOnlyCollection<SaveSecurityDocumentSeriesOperationAccessData> operations,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                SaveProcedure,
                new
                {
                    RoleId = roleId,
                    CompanyCode = companyCode,
                    FormKey = formKey,
                    DocumentType = documentType,
                    SecurityDocumentSeriesId = securityDocumentSeriesId,
                    IsSelected = isSelected,
                    OperationsJson = JsonSerializer.Serialize(operations, JsonOptions),
                    UpdatedByUserId = updatedByUserId,
                    UpdatedByUserName = updatedByUserName
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    public async Task<IReadOnlySet<int>> GetAuthorizedSeriesIdsForUserAsync(
        int userId,
        string companyCode,
        string formKey,
        string documentType,
        string actionKey,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var ids = await connection.QueryAsync<int>(
            new CommandDefinition(
                GetAuthorizedSeriesForUserProcedure,
                new
                {
                    UserId = userId,
                    CompanyCode = companyCode,
                    FormKey = formKey,
                    DocumentType = documentType,
                    ActionKey = actionKey
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return ids.ToHashSet();
    }

    public async Task<bool> ValidateUserOperationAsync(
        int userId,
        string companyCode,
        string formKey,
        string documentType,
        int securityDocumentSeriesId,
        string actionKey,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                ValidateUserOperationProcedure,
                new
                {
                    UserId = userId,
                    CompanyCode = companyCode,
                    FormKey = formKey,
                    DocumentType = documentType,
                    SecurityDocumentSeriesId = securityDocumentSeriesId,
                    ActionKey = actionKey
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }
}
