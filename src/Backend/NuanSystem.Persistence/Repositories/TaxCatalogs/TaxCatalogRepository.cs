using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.TaxCatalogs.Catalogs.Dtos;

namespace NuanSystem.Persistence.Repositories.TaxCatalogs;

public sealed class TaxCatalogRepository(ITenantConnectionFactory connectionFactory)
    : ITaxCatalogRepository
{
    private static readonly IReadOnlyDictionary<string, CatalogProcedures> Catalogs =
        new Dictionary<string, CatalogProcedures>(StringComparer.OrdinalIgnoreCase)
        {
            ["tax-regimes"] = new(
                "dbo.SP_NA_GET_TAXREGIMES_LISTAR",
                "dbo.SP_NA_GET_TAXREGIMES_BUSCARPORID",
                "dbo.SP_NA_GET_TAXREGIMES_LOOKUP",
                "dbo.SP_NA_GET_TAXREGIMES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_TAXREGIMES_CREAR",
                "dbo.SP_NA_PUT_TAXREGIMES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_TAXREGIMES_ELIMINAR"),
            ["taxpayer-types"] = new(
                "dbo.SP_NA_GET_TAXPAYERTYPES_LISTAR",
                "dbo.SP_NA_GET_TAXPAYERTYPES_BUSCARPORID",
                "dbo.SP_NA_GET_TAXPAYERTYPES_LOOKUP",
                "dbo.SP_NA_GET_TAXPAYERTYPES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_TAXPAYERTYPES_CREAR",
                "dbo.SP_NA_PUT_TAXPAYERTYPES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_TAXPAYERTYPES_ELIMINAR"),
            ["retention-types"] = new(
                "dbo.SP_NA_GET_RETENTIONTYPES_LISTAR",
                "dbo.SP_NA_GET_RETENTIONTYPES_BUSCARPORID",
                "dbo.SP_NA_GET_RETENTIONTYPES_LOOKUP",
                "dbo.SP_NA_GET_RETENTIONTYPES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_RETENTIONTYPES_CREAR",
                "dbo.SP_NA_PUT_RETENTIONTYPES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_RETENTIONTYPES_ELIMINAR"),
            ["tax-supports"] = new(
                "dbo.SP_NA_GET_TAXSUPPORTS_LISTAR",
                "dbo.SP_NA_GET_TAXSUPPORTS_BUSCARPORID",
                "dbo.SP_NA_GET_TAXSUPPORTS_LOOKUP",
                "dbo.SP_NA_GET_TAXSUPPORTS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_TAXSUPPORTS_CREAR",
                "dbo.SP_NA_PUT_TAXSUPPORTS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_TAXSUPPORTS_ELIMINAR"),
            ["retention-concepts"] = new(
                "dbo.SP_NA_GET_RETENTIONCONCEPTS_LISTAR",
                "dbo.SP_NA_GET_RETENTIONCONCEPTS_BUSCARPORID",
                "dbo.SP_NA_GET_RETENTIONCONCEPTS_LOOKUP",
                "dbo.SP_NA_GET_RETENTIONCONCEPTS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_RETENTIONCONCEPTS_CREAR",
                "dbo.SP_NA_PUT_RETENTIONCONCEPTS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_RETENTIONCONCEPTS_ELIMINAR")
        };

    public async Task<IReadOnlyCollection<TaxCatalogDto>> GetAllAsync(string catalogKey, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var catalogs = await connection.QueryAsync<TaxCatalogDto>(
            new CommandDefinition(GetProcedures(catalogKey).List, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return catalogs.AsList();
    }

    public async Task<IReadOnlyCollection<TaxCatalogLookupDto>> GetLookupAsync(string catalogKey, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var catalogs = await connection.QueryAsync<TaxCatalogLookupDto>(
            new CommandDefinition(GetProcedures(catalogKey).Lookup, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return catalogs.AsList();
    }

    public async Task<TaxCatalogDto?> GetByIdAsync(string catalogKey, int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<TaxCatalogDto>(
            new CommandDefinition(GetProcedures(catalogKey).GetById, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateAsync(string catalogKey, CreateTaxCatalogData catalog, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(GetProcedures(catalogKey).Create, catalog, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> ExistsByCodeAsync(string catalogKey, string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                GetProcedures(catalogKey).ExistsByCode,
                new { Code = code, ExcluirId = (int?)null },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> ExistsByCodeAsync(string catalogKey, string code, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                GetProcedures(catalogKey).ExistsByCode,
                new { Code = code, ExcluirId = excludingId },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> UpdateAsync(string catalogKey, UpdateTaxCatalogData catalog, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(GetProcedures(catalogKey).Update, catalog, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(string catalogKey, int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                GetProcedures(catalogKey).Delete,
                new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<IReadOnlyCollection<RetentionConceptDto>> GetRetentionConceptsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var concepts = await connection.QueryAsync<RetentionConceptDto>(
            new CommandDefinition(GetProcedures("retention-concepts").List, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return concepts.AsList();
    }

    public async Task<IReadOnlyCollection<RetentionConceptLookupDto>> GetRetentionConceptLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var concepts = await connection.QueryAsync<RetentionConceptLookupDto>(
            new CommandDefinition(GetProcedures("retention-concepts").Lookup, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return concepts.AsList();
    }

    public async Task<RetentionConceptDto?> GetRetentionConceptByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<RetentionConceptDto>(
            new CommandDefinition(GetProcedures("retention-concepts").GetById, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateRetentionConceptAsync(SaveRetentionConceptData concept, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(GetProcedures("retention-concepts").Create, ToRetentionConceptParameters(concept), cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateRetentionConceptAsync(SaveRetentionConceptData concept, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(GetProcedures("retention-concepts").Update, ToRetentionConceptParameters(concept), cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    private static object ToRetentionConceptParameters(SaveRetentionConceptData concept)
        => new
        {
            concept.Id,
            concept.Code,
            concept.Name,
            concept.Description,
            concept.RetentionTypeId,
            concept.SriCode,
            concept.Percent,
            concept.AppliesIva,
            concept.AppliesIncome,
            concept.IsActive,
            CreatedByUserId = concept.AuditUserId,
            CreatedByUserName = concept.AuditUserName,
            UpdatedByUserId = concept.AuditUserId,
            UpdatedByUserName = concept.AuditUserName
        };

    private static CatalogProcedures GetProcedures(string catalogKey)
    {
        if (Catalogs.TryGetValue(catalogKey, out var procedures))
        {
            return procedures;
        }

        throw new InvalidOperationException($"Tax catalog '{catalogKey}' is not configured.");
    }

    private sealed record CatalogProcedures(
        string List,
        string GetById,
        string Lookup,
        string ExistsByCode,
        string Create,
        string Update,
        string Delete);
}
