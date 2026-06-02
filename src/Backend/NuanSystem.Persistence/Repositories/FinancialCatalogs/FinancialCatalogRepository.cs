using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;

namespace NuanSystem.Persistence.Repositories.FinancialCatalogs;

public sealed class FinancialCatalogRepository(ITenantConnectionFactory connectionFactory)
    : IFinancialCatalogRepository
{
    private static readonly IReadOnlyDictionary<string, CatalogProcedures> Catalogs =
        new Dictionary<string, CatalogProcedures>(StringComparer.OrdinalIgnoreCase)
        {
            ["banks"] = new(
                "dbo.SP_NA_GET_BANKS_LISTAR",
                "dbo.SP_NA_GET_BANKS_BUSCARPORID",
                "dbo.SP_NA_GET_BANKS_LOOKUP",
                "dbo.SP_NA_GET_BANKS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_BANKS_CREAR",
                "dbo.SP_NA_PUT_BANKS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_BANKS_ELIMINAR"),
            ["bank-account-types"] = new(
                "dbo.SP_NA_GET_BANKACCOUNTTYPES_LISTAR",
                "dbo.SP_NA_GET_BANKACCOUNTTYPES_BUSCARPORID",
                "dbo.SP_NA_GET_BANKACCOUNTTYPES_LOOKUP",
                "dbo.SP_NA_GET_BANKACCOUNTTYPES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_BANKACCOUNTTYPES_CREAR",
                "dbo.SP_NA_PUT_BANKACCOUNTTYPES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_BANKACCOUNTTYPES_ELIMINAR"),
            ["currencies"] = new(
                "dbo.SP_NA_GET_CURRENCIES_LISTAR",
                "dbo.SP_NA_GET_CURRENCIES_BUSCARPORID",
                "dbo.SP_NA_GET_CURRENCIES_LOOKUP",
                "dbo.SP_NA_GET_CURRENCIES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_CURRENCIES_CREAR",
                "dbo.SP_NA_PUT_CURRENCIES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_CURRENCIES_ELIMINAR"),
            ["price-lists"] = new(
                "dbo.SP_NA_GET_PRICELISTS_LISTAR",
                "dbo.SP_NA_GET_PRICELISTS_BUSCARPORID",
                "dbo.SP_NA_GET_PRICELISTS_LOOKUP",
                "dbo.SP_NA_GET_PRICELISTS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_PRICELISTS_CREAR",
                "dbo.SP_NA_PUT_PRICELISTS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_PRICELISTS_ELIMINAR"),
            ["purchasing-agents"] = new(
                "dbo.SP_NA_GET_PURCHASINGAGENTS_LISTAR",
                "dbo.SP_NA_GET_PURCHASINGAGENTS_BUSCARPORID",
                "dbo.SP_NA_GET_PURCHASINGAGENTS_LOOKUP",
                "dbo.SP_NA_GET_PURCHASINGAGENTS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_PURCHASINGAGENTS_CREAR",
                "dbo.SP_NA_PUT_PURCHASINGAGENTS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_PURCHASINGAGENTS_ELIMINAR"),
            ["accounting-payment-methods"] = new(
                "dbo.SP_NA_GET_ACCOUNTINGPAYMENTMETHODS_LISTAR",
                "dbo.SP_NA_GET_ACCOUNTINGPAYMENTMETHODS_BUSCARPORID",
                "dbo.SP_NA_GET_ACCOUNTINGPAYMENTMETHODS_LOOKUP",
                "dbo.SP_NA_GET_ACCOUNTINGPAYMENTMETHODS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_ACCOUNTINGPAYMENTMETHODS_CREAR",
                "dbo.SP_NA_PUT_ACCOUNTINGPAYMENTMETHODS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_ACCOUNTINGPAYMENTMETHODS_ELIMINAR"),
            ["payment-priorities"] = new(
                "dbo.SP_NA_GET_PAYMENTPRIORITIES_LISTAR",
                "dbo.SP_NA_GET_PAYMENTPRIORITIES_BUSCARPORID",
                "dbo.SP_NA_GET_PAYMENTPRIORITIES_LOOKUP",
                "dbo.SP_NA_GET_PAYMENTPRIORITIES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_PAYMENTPRIORITIES_CREAR",
                "dbo.SP_NA_PUT_PAYMENTPRIORITIES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_PAYMENTPRIORITIES_ELIMINAR"),
            ["approval-flows"] = new(
                "dbo.SP_NA_GET_APPROVALFLOWS_LISTAR",
                "dbo.SP_NA_GET_APPROVALFLOWS_BUSCARPORID",
                "dbo.SP_NA_GET_APPROVALFLOWS_LOOKUP",
                "dbo.SP_NA_GET_APPROVALFLOWS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_APPROVALFLOWS_CREAR",
                "dbo.SP_NA_PUT_APPROVALFLOWS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_APPROVALFLOWS_ELIMINAR"),
            ["payment-document-types"] = new(
                "dbo.SP_NA_GET_PAYMENTDOCUMENTTYPES_LISTAR",
                "dbo.SP_NA_GET_PAYMENTDOCUMENTTYPES_BUSCARPORID",
                "dbo.SP_NA_GET_PAYMENTDOCUMENTTYPES_LOOKUP",
                "dbo.SP_NA_GET_PAYMENTDOCUMENTTYPES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_PAYMENTDOCUMENTTYPES_CREAR",
                "dbo.SP_NA_PUT_PAYMENTDOCUMENTTYPES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_PAYMENTDOCUMENTTYPES_ELIMINAR"),
            ["branches"] = new(
                "dbo.SP_NA_GET_BRANCHES_LISTAR",
                "dbo.SP_NA_GET_BRANCHES_BUSCARPORID",
                "dbo.SP_NA_GET_BRANCHES_LOOKUP",
                "dbo.SP_NA_GET_BRANCHES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_BRANCHES_CREAR",
                "dbo.SP_NA_PUT_BRANCHES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_BRANCHES_ELIMINAR"),
            ["departments"] = new(
                "dbo.SP_NA_GET_DEPARTMENTS_LISTAR",
                "dbo.SP_NA_GET_DEPARTMENTS_BUSCARPORID",
                "dbo.SP_NA_GET_DEPARTMENTS_LOOKUP",
                "dbo.SP_NA_GET_DEPARTMENTS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_DEPARTMENTS_CREAR",
                "dbo.SP_NA_PUT_DEPARTMENTS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_DEPARTMENTS_ELIMINAR"),
            ["business-lines"] = new(
                "dbo.SP_NA_GET_BUSINESSLINES_LISTAR",
                "dbo.SP_NA_GET_BUSINESSLINES_BUSCARPORID",
                "dbo.SP_NA_GET_BUSINESSLINES_LOOKUP",
                "dbo.SP_NA_GET_BUSINESSLINES_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_BUSINESSLINES_CREAR",
                "dbo.SP_NA_PUT_BUSINESSLINES_ACTUALIZAR",
                "dbo.SP_NA_DELETE_BUSINESSLINES_ELIMINAR"),
            ["cost-centers"] = new(
                "dbo.SP_NA_GET_COSTCENTERS_LISTAR",
                "dbo.SP_NA_GET_COSTCENTERS_BUSCARPORID",
                "dbo.SP_NA_GET_COSTCENTERS_LOOKUP",
                "dbo.SP_NA_GET_COSTCENTERS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_COSTCENTERS_CREAR",
                "dbo.SP_NA_PUT_COSTCENTERS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_COSTCENTERS_ELIMINAR"),
            ["projects"] = new(
                "dbo.SP_NA_GET_PROJECTS_LISTAR",
                "dbo.SP_NA_GET_PROJECTS_BUSCARPORID",
                "dbo.SP_NA_GET_PROJECTS_LOOKUP",
                "dbo.SP_NA_GET_PROJECTS_BUSCARPORCODIGO",
                "dbo.SP_NA_POST_PROJECTS_CREAR",
                "dbo.SP_NA_PUT_PROJECTS_ACTUALIZAR",
                "dbo.SP_NA_DELETE_PROJECTS_ELIMINAR")
        };

    public async Task<IReadOnlyCollection<FinancialCatalogDto>> GetAllAsync(string catalogKey, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var catalogs = await connection.QueryAsync<FinancialCatalogDto>(
            new CommandDefinition(GetProcedures(catalogKey).List, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return catalogs.AsList();
    }

    public async Task<IReadOnlyCollection<FinancialCatalogLookupDto>> GetLookupAsync(string catalogKey, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var catalogs = await connection.QueryAsync<FinancialCatalogLookupDto>(
            new CommandDefinition(GetProcedures(catalogKey).Lookup, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return catalogs.AsList();
    }

    public async Task<FinancialCatalogDto?> GetByIdAsync(string catalogKey, int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<FinancialCatalogDto>(
            new CommandDefinition(GetProcedures(catalogKey).GetById, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateAsync(string catalogKey, CreateFinancialCatalogData catalog, CancellationToken cancellationToken = default)
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

    public async Task<bool> UpdateAsync(string catalogKey, UpdateFinancialCatalogData catalog, CancellationToken cancellationToken = default)
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

        throw new InvalidOperationException($"Financial catalog '{catalogKey}' is not configured.");
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
