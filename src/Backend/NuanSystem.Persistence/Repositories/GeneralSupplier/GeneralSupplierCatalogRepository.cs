using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

namespace NuanSystem.Persistence.Repositories.GeneralSupplier;

public sealed class GeneralSupplierCatalogRepository(ITenantConnectionFactory connectionFactory)
    : IGeneralSupplierCatalogRepository
{
    private static readonly IReadOnlyDictionary<string, CatalogProcedures> Catalogs =
        new Dictionary<string, CatalogProcedures>(StringComparer.OrdinalIgnoreCase)
        {
            ["supplier-groups"] = new(
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLIERGROUPS_LISTAR",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLIERGROUPS_BUSCARPORID",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLIERGROUPS_LOOKUP",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLIERGROUPS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_GENERAL_SUPPLIER_SUPPLIERGROUPS_CREAR",
                "dbo.SP_NA_PUT_GENERAL_SUPPLIER_SUPPLIERGROUPS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_GENERAL_SUPPLIER_SUPPLIERGROUPS_ELIMINAR"),
            ["supplier-classes"] = new(
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLIERCLASSES_LISTAR",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLIERCLASSES_BUSCARPORID",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLIERCLASSES_LOOKUP",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLIERCLASSES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_GENERAL_SUPPLIER_SUPPLIERCLASSES_CREAR",
                "dbo.SP_NA_PUT_GENERAL_SUPPLIER_SUPPLIERCLASSES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_GENERAL_SUPPLIER_SUPPLIERCLASSES_ELIMINAR"),
            ["economic-activities"] = new(
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_ECONOMICACTIVITIES_LISTAR",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_ECONOMICACTIVITIES_BUSCARPORID",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_ECONOMICACTIVITIES_LOOKUP",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_ECONOMICACTIVITIES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_GENERAL_SUPPLIER_ECONOMICACTIVITIES_CREAR",
                "dbo.SP_NA_PUT_GENERAL_SUPPLIER_ECONOMICACTIVITIES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_GENERAL_SUPPLIER_ECONOMICACTIVITIES_ELIMINAR"),
            ["zones"] = new(
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_ZONES_LISTAR",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_ZONES_BUSCARPORID",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_ZONES_LOOKUP",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_ZONES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_GENERAL_SUPPLIER_ZONES_CREAR",
                "dbo.SP_NA_PUT_GENERAL_SUPPLIER_ZONES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_GENERAL_SUPPLIER_ZONES_ELIMINAR"),
            ["supply-methods"] = new(
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLYMETHODS_LISTAR",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLYMETHODS_BUSCARPORID",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLYMETHODS_LOOKUP",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_SUPPLYMETHODS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_GENERAL_SUPPLIER_SUPPLYMETHODS_CREAR",
                "dbo.SP_NA_PUT_GENERAL_SUPPLIER_SUPPLYMETHODS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_GENERAL_SUPPLIER_SUPPLYMETHODS_ELIMINAR"),
            ["contact-types"] = new(
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_CONTACTTYPES_LISTAR",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_CONTACTTYPES_BUSCARPORID",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_CONTACTTYPES_LOOKUP",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_CONTACTTYPES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_GENERAL_SUPPLIER_CONTACTTYPES_CREAR",
                "dbo.SP_NA_PUT_GENERAL_SUPPLIER_CONTACTTYPES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_GENERAL_SUPPLIER_CONTACTTYPES_ELIMINAR"),
            ["contact-channels"] = new(
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_CONTACTCHANNELS_LISTAR",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_CONTACTCHANNELS_BUSCARPORID",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_CONTACTCHANNELS_LOOKUP",
                "dbo.SP_NA_GET_GENERAL_SUPPLIER_CONTACTCHANNELS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_GENERAL_SUPPLIER_CONTACTCHANNELS_CREAR",
                "dbo.SP_NA_PUT_GENERAL_SUPPLIER_CONTACTCHANNELS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_GENERAL_SUPPLIER_CONTACTCHANNELS_ELIMINAR")
        };

    public async Task<IReadOnlyCollection<GeneralSupplierCatalogDto>> GetAllAsync(
        string catalogKey,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var catalogs = await connection.QueryAsync<GeneralSupplierCatalogDto>(
            new CommandDefinition(GetProcedures(catalogKey).List, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return catalogs.AsList();
    }

    public async Task<IReadOnlyCollection<GeneralSupplierCatalogLookupDto>> GetLookupAsync(
        string catalogKey,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var catalogs = await connection.QueryAsync<GeneralSupplierCatalogLookupDto>(
            new CommandDefinition(GetProcedures(catalogKey).Lookup, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return catalogs.AsList();
    }

    public async Task<GeneralSupplierCatalogDto?> GetByIdAsync(
        string catalogKey,
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<GeneralSupplierCatalogDto>(
            new CommandDefinition(GetProcedures(catalogKey).GetById, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateAsync(
        string catalogKey,
        CreateGeneralSupplierCatalogData catalog,
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
        UpdateGeneralSupplierCatalogData catalog,
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

        throw new InvalidOperationException($"GeneralSupplier catalog '{catalogKey}' is not configured.");
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

