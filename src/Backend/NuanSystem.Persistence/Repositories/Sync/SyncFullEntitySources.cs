using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class CountryFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.Countries;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                GlobalId,
                Code,
                Name,
                Iso2,
                Iso3,
                PhonePrefix,
                IsActive,
                CreatedAt,
                UpdatedAt
            FROM dbo.Countries
            WHERE IsDeleted = 0
              AND (@LastKey IS NULL OR Code > @LastKey)
            ORDER BY Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<CountrySourceRow>(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            row.IsActive,
            new CountrySyncPayload(
                row.GlobalId,
                row.Code,
                row.Name,
                row.Iso2,
                row.Iso3,
                row.PhonePrefix,
                row.IsActive,
                row.CreatedAt,
                row.UpdatedAt))).ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey, rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record CountrySourceRow(
        Guid GlobalId,
        string Code,
        string Name,
        string? Iso2,
        string? Iso3,
        string? PhonePrefix,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}

public sealed class ProvinceFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.Provinces;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                province.GlobalId,
                country.GlobalId AS CountryGlobalId,
                country.Code AS CountryCode,
                province.Code,
                province.Name,
                province.IsActive,
                province.CreatedAt,
                province.UpdatedAt,
                CONCAT(country.Code, N'|', province.Code) AS EntityKey
            FROM dbo.Provinces AS province
            INNER JOIN dbo.Countries AS country ON country.CountryId = province.CountryId
            WHERE province.IsDeleted = 0
              AND country.IsDeleted = 0
              AND (@LastKey IS NULL OR CONCAT(country.Code, N'|', province.Code) > @LastKey)
            ORDER BY country.Code, province.Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<ProvinceSourceRow>(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.EntityKey,
            row.IsActive,
            new ProvinceSyncPayload(
                row.GlobalId,
                row.CountryGlobalId,
                row.CountryCode,
                row.Code,
                row.Name,
                row.IsActive,
                row.CreatedAt,
                row.UpdatedAt))).ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey, rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record ProvinceSourceRow(
        Guid GlobalId,
        Guid CountryGlobalId,
        string CountryCode,
        string Code,
        string Name,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string EntityKey);
}

public sealed class CityFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.Cities;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                city.GlobalId,
                country.GlobalId AS CountryGlobalId,
                country.Code AS CountryCode,
                province.GlobalId AS ProvinceGlobalId,
                province.Code AS ProvinceCode,
                city.Code,
                city.Name,
                city.IsActive,
                city.CreatedAt,
                city.UpdatedAt,
                CONCAT(country.Code, N'|', province.Code, N'|', city.Code) AS EntityKey
            FROM dbo.Cities AS city
            INNER JOIN dbo.Countries AS country ON country.CountryId = city.CountryId
            INNER JOIN dbo.Provinces AS province ON province.ProvinceId = city.ProvinceId
            WHERE city.IsDeleted = 0
              AND country.IsDeleted = 0
              AND province.IsDeleted = 0
              AND province.CountryId = country.CountryId
              AND (@LastKey IS NULL OR CONCAT(country.Code, N'|', province.Code, N'|', city.Code) > @LastKey)
            ORDER BY country.Code, province.Code, city.Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<CitySourceRow>(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.EntityKey,
            row.IsActive,
            new CitySyncPayload(
                row.GlobalId,
                row.CountryGlobalId,
                row.CountryCode,
                row.ProvinceGlobalId,
                row.ProvinceCode,
                row.Code,
                row.Name,
                row.IsActive,
                row.CreatedAt,
                row.UpdatedAt))).ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey, rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record CitySourceRow(
        Guid GlobalId,
        Guid CountryGlobalId,
        string CountryCode,
        Guid ProvinceGlobalId,
        string ProvinceCode,
        string Code,
        string Name,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string EntityKey);
}

public sealed class CurrencyFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.Currencies;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                GlobalId,
                Code,
                Name,
                Symbol,
                Description,
                IsBaseCurrency,
                IsActive,
                ExternalSystem,
                ExternalCode,
                CreatedAt,
                UpdatedAt
            FROM dbo.Currencies
            WHERE IsDeleted = 0
              AND (@LastKey IS NULL OR Code > @LastKey)
            ORDER BY Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<CurrencySourceRow>(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            row.IsActive,
            new CurrencySyncPayload(
                row.GlobalId,
                row.Code,
                row.Name,
                row.Symbol,
                row.Description,
                row.IsBaseCurrency,
                row.IsActive,
                row.ExternalSystem,
                row.ExternalCode,
                row.CreatedAt,
                row.UpdatedAt))).ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey, rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record CurrencySourceRow(
        Guid GlobalId,
        string Code,
        string Name,
        string? Symbol,
        string? Description,
        bool IsBaseCurrency,
        bool IsActive,
        string? ExternalSystem,
        string? ExternalCode,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}

public sealed class BusinessPartnerFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.BusinessPartner;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                bp.GlobalId,
                bp.Code,
                bp.Name,
                bp.CommercialName,
                bp.PartnerType,
                it.Code AS IdentificationTypeCode,
                bp.IdentificationNumber,
                bp.Email,
                bp.Phone,
                bp.IsActive,
                bp.ExternalSystem,
                bp.ExternalCode
            FROM dbo.BusinessPartners bp
            LEFT JOIN dbo.BusinessPartnerIdentificationTypes it ON it.Id = bp.IdentificationTypeId
            WHERE bp.IsDeleted = 0
              AND (@LastKey IS NULL OR bp.Code > @LastKey)
            ORDER BY bp.Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<BusinessPartnerSourceRow>(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            row.IsActive,
            new BusinessPartnerSyncPayload(
                row.GlobalId,
                row.Code,
                row.Name,
                row.CommercialName,
                row.PartnerType,
                row.IdentificationTypeCode,
                row.IdentificationNumber,
                row.Email,
                row.Phone,
                row.IsActive,
                row.ExternalSystem,
                row.ExternalCode))).ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey, rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record BusinessPartnerSourceRow(
        Guid GlobalId,
        string Code,
        string Name,
        string? CommercialName,
        string PartnerType,
        string? IdentificationTypeCode,
        string IdentificationNumber,
        string? Email,
        string? Phone,
        bool IsActive,
        string? ExternalSystem,
        string? ExternalCode);
}

public sealed class ItemGroupFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.ItemGroups;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                GlobalId,
                Code,
                Name,
                Description,
                InventoryAccountCode,
                CostOfSalesAccountCode,
                SalesAccountCode,
                PurchaseAccountCode,
                SapGroupCode,
                SapCode,
                IsActive,
                ExternalSystem,
                ExternalCode,
                CreatedAt,
                UpdatedAt
            FROM dbo.ItemGroups
            WHERE IsDeleted = 0
              AND (@LastKey IS NULL OR Code > @LastKey)
            ORDER BY Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<ItemGroupSourceRow>(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            row.IsActive,
            new ItemGroupSyncPayload(
                row.GlobalId,
                row.Code,
                row.Name,
                row.Description,
                row.InventoryAccountCode,
                row.CostOfSalesAccountCode,
                row.SalesAccountCode,
                row.PurchaseAccountCode,
                row.SapGroupCode,
                row.SapCode,
                row.IsActive,
                row.ExternalSystem,
                row.ExternalCode,
                row.CreatedAt,
                row.UpdatedAt))).ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey, rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record ItemGroupSourceRow(
        Guid GlobalId,
        string Code,
        string Name,
        string? Description,
        string? InventoryAccountCode,
        string? CostOfSalesAccountCode,
        string? SalesAccountCode,
        string? PurchaseAccountCode,
        string? SapGroupCode,
        string? SapCode,
        bool IsActive,
        string? ExternalSystem,
        string? ExternalCode,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}

public sealed class ItemFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.Item;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                i.GlobalId,
                i.Code,
                i.Name,
                i.Description,
                i.ItemType,
                i.ItemGroupId,
                ig.Code AS ItemGroupCode,
                CAST(NULL AS int) AS ItemFamilyId,
                CAST(NULL AS nvarchar(50)) AS ItemFamilyCode,
                i.InventoryUnitOfMeasureId,
                uom.Code AS InventoryUnitOfMeasureCode,
                barcode.Barcode,
                i.IsInventoryItem,
                i.IsSalesItem,
                i.IsPurchaseItem,
                i.IsActive,
                i.ExternalSystem,
                i.ExternalCode,
                i.SapCode
            FROM dbo.Items i
            LEFT JOIN dbo.ItemGroups ig ON ig.Id = i.ItemGroupId
            LEFT JOIN dbo.UnitOfMeasures uom ON uom.Id = i.InventoryUnitOfMeasureId
            OUTER APPLY (
                SELECT TOP (1) b.Barcode
                FROM dbo.ItemBarcodes b
                WHERE b.ItemId = i.Id AND b.IsDeleted = 0
                ORDER BY b.IsMain DESC, b.Id
            ) barcode
            WHERE i.IsDeleted = 0
              AND (@LastKey IS NULL OR i.Code > @LastKey)
            ORDER BY i.Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<ItemSourceRow>(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            row.IsActive,
            new ItemSyncPayload(
                row.GlobalId,
                row.Code,
                row.Name,
                row.Description,
                row.ItemType,
                row.ItemGroupId,
                row.ItemGroupCode,
                row.ItemFamilyId,
                row.ItemFamilyCode,
                row.InventoryUnitOfMeasureId,
                row.InventoryUnitOfMeasureCode,
                row.Barcode,
                row.IsInventoryItem,
                row.IsSalesItem,
                row.IsPurchaseItem,
                row.IsActive,
                row.ExternalSystem,
                row.ExternalCode,
                row.SapCode))).ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey, rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record ItemSourceRow(
        Guid GlobalId,
        string Code,
        string Name,
        string? Description,
        string ItemType,
        int? ItemGroupId,
        string? ItemGroupCode,
        int? ItemFamilyId,
        string? ItemFamilyCode,
        int? InventoryUnitOfMeasureId,
        string? InventoryUnitOfMeasureCode,
        string? Barcode,
        bool IsInventoryItem,
        bool IsSalesItem,
        bool IsPurchaseItem,
        bool IsActive,
        string? ExternalSystem,
        string? ExternalCode,
        string? SapCode);
}

public sealed class WarehouseFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.Warehouse;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                GlobalId,
                Code,
                Name,
                Description,
                BranchCode,
                Address,
                City,
                Province,
                Country,
                Phone,
                Email,
                ManagerName,
                AllowsSales,
                AllowsPurchases,
                AllowsTransfers,
                AllowsProduction,
                IsDefault,
                IsActive,
                ExternalSystem,
                ExternalCode,
                SapCode,
                CreatedAt,
                UpdatedAt
            FROM dbo.Warehouses
            WHERE IsDeleted = 0
              AND (@LastKey IS NULL OR Code > @LastKey)
            ORDER BY Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<WarehouseSourceRow>(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            row.IsActive,
            new WarehouseSyncPayload(
                row.GlobalId,
                row.Code,
                row.Name,
                row.Description,
                row.BranchCode,
                row.Address,
                row.City,
                row.Province,
                row.Country,
                row.Phone,
                row.Email,
                row.ManagerName,
                row.AllowsSales,
                row.AllowsPurchases,
                row.AllowsTransfers,
                row.AllowsProduction,
                row.IsDefault,
                row.IsActive,
                row.ExternalSystem,
                row.ExternalCode,
                row.SapCode,
                row.CreatedAt,
                row.UpdatedAt))).ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey, rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record WarehouseSourceRow(
        Guid GlobalId,
        string Code,
        string Name,
        string? Description,
        string? BranchCode,
        string? Address,
        string? City,
        string? Province,
        string? Country,
        string? Phone,
        string? Email,
        string? ManagerName,
        bool AllowsSales,
        bool AllowsPurchases,
        bool AllowsTransfers,
        bool AllowsProduction,
        bool IsDefault,
        bool IsActive,
        string? ExternalSystem,
        string? ExternalCode,
        string? SapCode,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}

file static class SyncFullEntitySourceHelpers
{
    public static int GetPageLimit(SyncSourceReadContext context)
    {
        return context.RemainingLimit.HasValue
            ? Math.Min(context.PageSize, Math.Max(context.RemainingLimit.Value, 0))
            : context.PageSize;
    }

    public static object ReadParameters(SyncSourceReadContext context)
    {
        var requestedTake = context.RemainingLimit.HasValue
            ? Math.Min(context.PageSize + 1, context.RemainingLimit.Value + 1)
            : context.PageSize + 1;

        return new
        {
            LastKey = string.IsNullOrWhiteSpace(context.LastKey) ? null : context.LastKey,
            Take = Math.Clamp(requestedTake, 1, 10001)
        };
    }

    public static async Task<CompanyConnectionInfo> ResolveSqlServerCompanyAsync(
        ICompanyResolver companyResolver,
        int companyId,
        CancellationToken cancellationToken)
    {
        var company = await companyResolver.ResolveByIdAsync(companyId, cancellationToken)
            ?? throw new InvalidOperationException($"La empresa {companyId} no existe.");

        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
        {
            throw new NotSupportedException("La lectura Full de sincronizacion solo esta implementada para SQL Server.");
        }

        return company;
    }
}
