using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;

namespace NuanSystem.Persistence.Repositories.Documents;

public sealed class SecurityDocumentSeriesRepository(ITenantConnectionFactory connectionFactory)
    : ISecurityDocumentSeriesRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_LISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_BUSCARPORID";
    private const string LookupProcedure = "dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_LOOKUP";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_BUSCARPORCODIGO";
    private const string ExistsByKeyProcedure = "dbo.SP_NA_GET_SECURITYDOCUMENTSERIES_BUSCARPORCLAVE";
    private const string CreateProcedure = "dbo.SP_NA_POST_SECURITYDOCUMENTSERIES_CREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_SECURITYDOCUMENTSERIES_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_SECURITYDOCUMENTSERIES_ELIMINAR";

    public async Task<IReadOnlyCollection<SecurityDocumentSeriesDto>> GetAllAsync(
        SecurityDocumentSeriesFilterData filter,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var series = await connection.QueryAsync<SecurityDocumentSeriesDto>(
            new CommandDefinition(
                ListProcedure,
                new
                {
                    filter.Search,
                    filter.DocumentType,
                    filter.IsActive
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return series.AsList();
    }

    public async Task<SecurityDocumentSeriesDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SecurityDocumentSeriesDto>(
            new CommandDefinition(
                GetByIdProcedure,
                new { Id = id },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    public async Task<IReadOnlyCollection<SecurityDocumentSeriesLookupDto>> GetLookupAsync(
        string? documentType,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var series = await connection.QueryAsync<SecurityDocumentSeriesLookupDto>(
            new CommandDefinition(
                LookupProcedure,
                new { DocumentType = documentType },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return series.AsList();
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        int? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var exists = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ExistsByCodeProcedure,
                new { Code = code, ExcludedId = excludedId },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return exists == 1;
    }

    public async Task<bool> ExistsBySeriesKeyAsync(
        string documentType,
        string prefix,
        string establishment,
        string emissionPoint,
        int? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var exists = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ExistsByKeyProcedure,
                new
                {
                    DocumentType = documentType,
                    Prefix = prefix,
                    Establishment = establishment,
                    EmissionPoint = emissionPoint,
                    ExcludedId = excludedId
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return exists == 1;
    }

    public async Task<int> CreateAsync(
        CreateSecurityDocumentSeriesData data,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                CreateProcedure,
                data,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateAsync(
        UpdateSecurityDocumentSeriesData data,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                UpdateProcedure,
                data,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

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
}
