using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SecurityRoleFormFieldAccessRepository(IMasterConnectionFactory connectionFactory) : ISecurityRoleFormFieldAccessRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const string GetFieldsProcedure = "dbo.SP_NA_GET_SECURITYROLEFORMFIELDS_CAMPOS";
    private const string SaveFieldsProcedure = "dbo.SP_NA_PUT_SECURITYROLEFORMFIELDS_GUARDAR";
    private const string GetDocumentSeriesFieldsProcedure = "dbo.SP_NA_GET_SECURITYROLEDOCUMENTSERIESFIELDS_CAMPOS";
    private const string SaveDocumentSeriesFieldsProcedure = "dbo.SP_NA_PUT_SECURITYROLEDOCUMENTSERIESFIELDS_GUARDAR";
    private const string GetEffectiveDocumentSeriesFieldsProcedure = "dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_FIELDS_USUARIO";

    public async Task<IReadOnlyCollection<SecurityFormFieldAccessDto>> GetFieldsAsync(
        int roleId,
        int formId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SecurityFormFieldAccessDto>(
            new CommandDefinition(
                GetFieldsProcedure,
                new { RoleId = roleId, FormId = formId, OnlyActive = onlyActive, Search = search },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task SaveFieldsAsync(
        int roleId,
        int formId,
        IReadOnlyCollection<SaveSecurityFormFieldAccessData> fields,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                SaveFieldsProcedure,
                new
                {
                    RoleId = roleId,
                    FormId = formId,
                    FieldsJson = JsonSerializer.Serialize(fields, JsonOptions),
                    UpdatedByUserId = updatedByUserId,
                    UpdatedByUserName = updatedByUserName
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    public async Task<IReadOnlyCollection<SecurityFormFieldAccessDto>> GetDocumentSeriesFieldsAsync(
        int roleId,
        string companyCode,
        int formId,
        string documentType,
        int securityDocumentSeriesId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SecurityFormFieldAccessDto>(
            new CommandDefinition(
                GetDocumentSeriesFieldsProcedure,
                new
                {
                    RoleId = roleId,
                    CompanyCode = companyCode,
                    FormId = formId,
                    DocumentType = documentType,
                    SecurityDocumentSeriesId = securityDocumentSeriesId,
                    OnlyActive = onlyActive,
                    Search = search
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task SaveDocumentSeriesFieldsAsync(
        int roleId,
        string companyCode,
        int formId,
        string documentType,
        int securityDocumentSeriesId,
        IReadOnlyCollection<SaveSecurityFormFieldAccessData> fields,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                SaveDocumentSeriesFieldsProcedure,
                new
                {
                    RoleId = roleId,
                    CompanyCode = companyCode,
                    FormId = formId,
                    DocumentType = documentType,
                    SecurityDocumentSeriesId = securityDocumentSeriesId,
                    FieldsJson = JsonSerializer.Serialize(fields, JsonOptions),
                    UpdatedByUserId = updatedByUserId,
                    UpdatedByUserName = updatedByUserName
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    public async Task<IReadOnlyCollection<SecurityFormFieldAccessDto>> GetEffectiveDocumentSeriesFieldsForUserAsync(
        int userId,
        string companyCode,
        string formKey,
        string documentType,
        int securityDocumentSeriesId,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SecurityFormFieldAccessDto>(
            new CommandDefinition(
                GetEffectiveDocumentSeriesFieldsProcedure,
                new
                {
                    UserId = userId,
                    CompanyCode = companyCode,
                    FormKey = formKey,
                    DocumentType = documentType,
                    SecurityDocumentSeriesId = securityDocumentSeriesId
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure))).AsList();
    }
}
