using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

namespace NuanSystem.Persistence.Repositories.GeneralInventory;

public sealed class GeneralInventoryCatalogRepository(ITenantConnectionFactory connectionFactory)
    : IGeneralInventoryCatalogRepository
{
    private static readonly IReadOnlyDictionary<string, CatalogProcedures> Catalogs =
        new Dictionary<string, CatalogProcedures>(StringComparer.OrdinalIgnoreCase)
        {
            ["warehouses"] = Procedures("WAREHOUSES"),
            ["item-subgroups"] = Procedures("ITEMSUBGROUPS"),
            ["sales-channels"] = Procedures("SALESCHANNELS"),
            ["warehouse-locations"] = Procedures("WAREHOUSELOCATIONS"),
            ["storage-zones"] = Procedures("STORAGEZONES"),
            ["storage-conditions"] = Procedures("STORAGECONDITIONS"),
            ["replenishment-methods"] = Procedures("REPLENISHMENTMETHODS"),
            ["variant-attributes"] = Procedures("VARIANTATTRIBUTES"),
            ["attachment-document-types"] = Procedures("ATTACHMENTDOCUMENTTYPES"),
            ["attachment-categories"] = Procedures("ATTACHMENTCATEGORIES")
        };

    public async Task<IReadOnlyCollection<GeneralInventoryCatalogDto>> GetAllAsync(
        string catalogKey,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var catalogs = await connection.QueryAsync<GeneralInventoryCatalogDto>(
            new CommandDefinition(GetProcedures(catalogKey).List, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return catalogs.AsList();
    }

    public async Task<IReadOnlyCollection<GeneralInventoryCatalogLookupDto>> GetLookupAsync(
        string catalogKey,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var catalogs = await connection.QueryAsync<GeneralInventoryCatalogLookupDto>(
            new CommandDefinition(GetProcedures(catalogKey).Lookup, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return catalogs.AsList();
    }

    public async Task<GeneralInventoryCatalogDto?> GetByIdAsync(
        string catalogKey,
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<GeneralInventoryCatalogDto>(
            new CommandDefinition(GetProcedures(catalogKey).GetById, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateAsync(
        string catalogKey,
        CreateGeneralInventoryCatalogData catalog,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(GetProcedures(catalogKey).Create, catalog, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> ExistsByCodeAsync(
        string catalogKey,
        string code,
        CancellationToken cancellationToken = default)
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

    public async Task<bool> ExistsByCodeAsync(
        string catalogKey,
        string code,
        int excludingId,
        CancellationToken cancellationToken = default)
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

    public async Task<bool> UpdateAsync(
        string catalogKey,
        UpdateGeneralInventoryCatalogData catalog,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(GetProcedures(catalogKey).Update, catalog, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(
        string catalogKey,
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default)
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

    private static CatalogProcedures GetProcedures(string catalogKey)
    {
        if (Catalogs.TryGetValue(catalogKey, out var procedures))
        {
            return procedures;
        }

        throw new InvalidOperationException($"GeneralInventory catalog '{catalogKey}' is not configured.");
    }

    private static CatalogProcedures Procedures(string token)
    {
        return new CatalogProcedures(
            $"dbo.SP_NA_GET_GENERAL_INVENTORY_{token}_LISTAR",
            $"dbo.SP_NA_GET_GENERAL_INVENTORY_{token}_BUSCARPORID",
            $"dbo.SP_NA_GET_GENERAL_INVENTORY_{token}_LOOKUP",
            $"dbo.SP_NA_GET_GENERAL_INVENTORY_{token}_BUSCARPORCODIGO",
            $"dbo.SP_NA_POST_GENERAL_INVENTORY_{token}_CREAR",
            $"dbo.SP_NA_PUT_GENERAL_INVENTORY_{token}_ACTUALIZAR",
            $"dbo.SP_NA_DELETE_GENERAL_INVENTORY_{token}_ELIMINAR");
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
