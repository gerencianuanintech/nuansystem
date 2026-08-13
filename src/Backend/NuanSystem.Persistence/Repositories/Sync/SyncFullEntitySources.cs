using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
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
                IsDeleted,
                ExternalSystem,
                ExternalCode,
                CreatedAt,
                UpdatedAt
            FROM dbo.Countries
            WHERE (@LastKey IS NULL OR Code > @LastKey)
            ORDER BY Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<CountrySourceRow>(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            !row.IsDeleted && row.IsActive,
            new CountrySyncPayload(
                row.GlobalId,
                row.Code,
                row.Name,
                row.Iso2,
                row.Iso3,
                row.PhonePrefix,
                !row.IsDeleted && row.IsActive,
                row.IsDeleted,
                row.ExternalSystem,
                row.ExternalCode,
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
        bool IsDeleted,
        string? ExternalSystem,
        string? ExternalCode,
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
                province.IsDeleted,
                province.ExternalSystem,
                province.ExternalCode,
                province.CreatedAt,
                province.UpdatedAt,
                CONCAT(country.Code, N'|', province.Code) AS EntityKey
            FROM dbo.Provinces AS province
            INNER JOIN dbo.Countries AS country ON country.CountryId = province.CountryId
            WHERE country.IsDeleted = 0
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
            !row.IsDeleted && row.IsActive,
            new ProvinceSyncPayload(
                row.GlobalId,
                row.CountryGlobalId,
                row.CountryCode,
                row.Code,
                row.Name,
                !row.IsDeleted && row.IsActive,
                row.IsDeleted,
                row.ExternalSystem,
                row.ExternalCode,
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
        bool IsDeleted,
        string? ExternalSystem,
        string? ExternalCode,
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
                city.IsDeleted,
                city.ExternalSystem,
                city.ExternalCode,
                city.CreatedAt,
                city.UpdatedAt,
                CONCAT(country.Code, N'|', province.Code, N'|', city.Code) AS EntityKey
            FROM dbo.Cities AS city
            INNER JOIN dbo.Countries AS country ON country.CountryId = city.CountryId
            INNER JOIN dbo.Provinces AS province ON province.ProvinceId = city.ProvinceId
            WHERE country.IsDeleted = 0
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
            !row.IsDeleted && row.IsActive,
            new CitySyncPayload(
                row.GlobalId,
                row.CountryGlobalId,
                row.CountryCode,
                row.ProvinceGlobalId,
                row.ProvinceCode,
                row.Code,
                row.Name,
                !row.IsDeleted && row.IsActive,
                row.IsDeleted,
                row.ExternalSystem,
                row.ExternalCode,
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
        bool IsDeleted,
        string? ExternalSystem,
        string? ExternalCode,
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
                SortOrder,
                IsSystem,
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
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                row.SortOrder,
                row.IsSystem,
                null,
                null,
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
        int SortOrder,
        bool IsSystem,
        string? SapGroupCode,
        string? SapCode,
        bool IsActive,
        string? ExternalSystem,
        string? ExternalCode,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}

public sealed class ItemFamilyFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.ItemFamilies;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                family.GlobalId,
                itemGroup.GlobalId AS ItemGroupGlobalId,
                itemGroup.Code AS ItemGroupCode,
                family.Code,
                family.Name,
                family.Description,
                family.SortOrder,
                family.IsActive,
                family.SapFamilyCode,
                family.SapCode,
                family.ExternalSystem,
                family.ExternalCode,
                family.CreatedAt,
                family.UpdatedAt,
                CONCAT(itemGroup.Code, N'|', family.Code) AS EntityKey
            FROM dbo.ItemFamilies AS family
            INNER JOIN dbo.ItemGroups AS itemGroup ON itemGroup.Id = family.ItemGroupId
            WHERE family.IsDeleted = 0
              AND itemGroup.IsDeleted = 0
              AND (@LastKey IS NULL OR CONCAT(itemGroup.Code, N'|', family.Code) > @LastKey)
            ORDER BY itemGroup.Code, family.Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(
            companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<ItemFamilySourceRow>(
            new CommandDefinition(
                sql,
                SyncFullEntitySourceHelpers.ReadParameters(context),
                cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context))
            .Select(row => new SyncSourceRecord(
                row.GlobalId,
                row.EntityKey,
                row.IsActive,
                new ItemFamilySyncPayload(
                    row.GlobalId,
                    row.ItemGroupGlobalId,
                    row.ItemGroupCode,
                    row.Code,
                    row.Name,
                    row.Description,
                    row.IsActive,
                    row.SapFamilyCode,
                    row.SapCode,
                    row.ExternalSystem,
                    row.ExternalCode,
                    row.CreatedAt,
                    row.UpdatedAt,
                    row.SortOrder)))
            .ToArray();

        return new SyncSourcePage(
            limited,
            limited.LastOrDefault()?.EntityKey,
            rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record ItemFamilySourceRow(
        Guid GlobalId,
        Guid ItemGroupGlobalId,
        string ItemGroupCode,
        string Code,
        string Name,
        string? Description,
        int SortOrder,
        bool IsActive,
        string? SapFamilyCode,
        string? SapCode,
        string? ExternalSystem,
        string? ExternalCode,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string EntityKey);
}

public sealed class ItemBrandFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.ItemBrands;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                GlobalId, Code, Name, Description, SortOrder, IsActive, IsDeleted,
                COALESCE(UpdatedAt, CreatedAt) AS EffectiveUpdatedAt
            FROM dbo.ItemBrands
            WHERE (@LastKey IS NULL OR Code > @LastKey)
            ORDER BY Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(
            companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<ItemBrandSourceRow>(new CommandDefinition(
            sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row =>
            new SyncSourceRecord(row.GlobalId, row.Code, !row.IsDeleted && row.IsActive,
                new ItemBrandSyncPayload(row.GlobalId, row.Code, row.Name, row.Description,
                    row.SortOrder, !row.IsDeleted && row.IsActive, row.IsDeleted, row.EffectiveUpdatedAt)))
            .ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey,
            rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record ItemBrandSourceRow(Guid GlobalId, string Code, string Name,
        string? Description, int SortOrder, bool IsActive, bool IsDeleted, DateTime EffectiveUpdatedAt);
}

public sealed class UnitMeasureFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.UnitOfMeasures;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT TOP (@Take)
                GlobalId,Code,Name,Description,Symbol,MagnitudeCode,SortOrder,IsActive,IsDeleted,
                COALESCE(UpdatedAt,CreatedAt) AS EffectiveUpdatedAt
            FROM dbo.UnitOfMeasures
            WHERE (@LastKey IS NULL OR Code>@LastKey)
            ORDER BY Code;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(
            companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<UnitMeasureSourceRow>(new CommandDefinition(
            sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken))).AsList();
        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context)).Select(row =>
            new SyncSourceRecord(row.GlobalId, row.Code, !row.IsDeleted && row.IsActive,
                new UnitMeasureSyncPayload(row.GlobalId, row.Code, row.Name, row.Description, row.Symbol,
                    row.MagnitudeCode, row.SortOrder, !row.IsDeleted && row.IsActive,
                    row.IsDeleted, row.EffectiveUpdatedAt))).ToArray();
        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey,
            rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    private sealed record UnitMeasureSourceRow(Guid GlobalId, string Code, string Name,
        string? Description, string? Symbol, string MagnitudeCode, int SortOrder,
        bool IsActive, bool IsDeleted, DateTime EffectiveUpdatedAt);
}

public sealed class ProductTypeFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.ProductTypes;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context, CancellationToken cancellationToken = default)
    {
        var afterId = int.TryParse(context.LastKey, out var parsed) ? parsed : (int?)null;
        var take = SyncFullEntitySourceHelpers.GetPageLimit(context);
        var requested = Math.Clamp(take + 1, 1, 10001);
        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(
            companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<ProductTypeSourceRow>(new CommandDefinition(
            "dbo.SP_NA_GET_PRODUCT_TYPE_SYNC_FULL",
            new { AfterId = afterId, BatchSize = requested },
            cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();

        var limited = rows.Take(take).Select(row =>
            new SyncSourceRecord(row.GlobalId, row.Code, !row.IsDeleted && row.IsActive,
                new ProductTypeSyncPayload(row.GlobalId, row.Code, row.Name, row.Description,
                    row.NatureCode, row.SortOrder, row.IsSystem, !row.IsDeleted && row.IsActive,
                    row.IsDeleted, row.UpdatedAt ?? row.CreatedAt))).ToArray();
        return new SyncSourcePage(limited, rows.Take(take).LastOrDefault()?.Id.ToString(), rows.Count > take);
    }

    private sealed record ProductTypeSourceRow(int Id, Guid GlobalId, string Code, string Name,
        string? Description, string NatureCode, int SortOrder, bool IsSystem, bool IsActive,
        bool IsDeleted, DateTime CreatedAt, DateTime? UpdatedAt);
}

public sealed class ItemLineFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.ItemLines;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context, CancellationToken cancellationToken = default)
    {
        var afterId = int.TryParse(context.LastKey, out var parsed) ? parsed : (int?)null;
        var take = SyncFullEntitySourceHelpers.GetPageLimit(context);
        var requested = Math.Clamp(take + 1, 1, 10001);
        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(
            companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<ItemLineSourceRow>(new CommandDefinition(
            "dbo.SP_NA_GET_ITEM_LINE_SYNC_FULL",
            new { AfterId = afterId, BatchSize = requested },
            cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();

        var limited = rows.Take(take).Select(row =>
            new SyncSourceRecord(row.GlobalId, row.Code, !row.IsDeleted && row.IsActive,
                new ItemLineSyncPayload(row.GlobalId, row.Code, row.Name, row.Description,
                    row.SortOrder, !row.IsDeleted && row.IsActive, row.IsDeleted,
                    row.UpdatedAt ?? row.CreatedAt))).ToArray();
        return new SyncSourcePage(limited, rows.Take(take).LastOrDefault()?.Id.ToString(), rows.Count > take);
    }

    private sealed record ItemLineSourceRow(int Id, Guid GlobalId, string Code, string Name,
        string? Description, int SortOrder, bool IsActive, bool IsDeleted,
        DateTime CreatedAt, DateTime? UpdatedAt);
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
                ig.GlobalId AS ItemGroupGlobalId,
                ig.Code AS ItemGroupCode,
                itemFamily.GlobalId AS ItemFamilyGlobalId,
                itemFamily.Code AS ItemFamilyCode,
                inventoryUom.GlobalId AS InventoryUnitOfMeasureGlobalId,
                inventoryUom.Code AS InventoryUnitOfMeasureCode,
                purchaseUom.GlobalId AS PurchaseUnitOfMeasureGlobalId,
                purchaseUom.Code AS PurchaseUnitOfMeasureCode,
                salesUom.GlobalId AS SalesUnitOfMeasureGlobalId,
                salesUom.Code AS SalesUnitOfMeasureCode,
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
            LEFT JOIN dbo.ItemFamilies itemFamily ON itemFamily.Id = i.ItemFamilyId
            LEFT JOIN dbo.UnitOfMeasures inventoryUom ON inventoryUom.Id = i.InventoryUnitOfMeasureId
            LEFT JOIN dbo.UnitOfMeasures purchaseUom ON purchaseUom.Id = i.PurchaseUnitOfMeasureId
            LEFT JOIN dbo.UnitOfMeasures salesUom ON salesUom.Id = i.SalesUnitOfMeasureId
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
                row.ItemGroupGlobalId,
                row.ItemGroupCode,
                row.ItemFamilyGlobalId,
                row.ItemFamilyCode,
                row.InventoryUnitOfMeasureGlobalId,
                row.InventoryUnitOfMeasureCode,
                row.PurchaseUnitOfMeasureGlobalId,
                row.PurchaseUnitOfMeasureCode,
                row.SalesUnitOfMeasureGlobalId,
                row.SalesUnitOfMeasureCode,
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
        Guid? ItemGroupGlobalId,
        string? ItemGroupCode,
        Guid? ItemFamilyGlobalId,
        string? ItemFamilyCode,
        Guid? InventoryUnitOfMeasureGlobalId,
        string? InventoryUnitOfMeasureCode,
        Guid? PurchaseUnitOfMeasureGlobalId,
        string? PurchaseUnitOfMeasureCode,
        Guid? SalesUnitOfMeasureGlobalId,
        string? SalesUnitOfMeasureCode,
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
