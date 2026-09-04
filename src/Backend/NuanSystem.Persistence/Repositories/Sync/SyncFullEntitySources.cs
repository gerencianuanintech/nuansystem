using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;
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
            DECLARE @Page TABLE
            (
                Id int NOT NULL PRIMARY KEY,
                GlobalId uniqueidentifier NOT NULL,
                Code nvarchar(50) NOT NULL,
                Name nvarchar(200) NOT NULL,
                CommercialName nvarchar(200) NULL,
                PartnerType nvarchar(20) NOT NULL,
                IdentificationTypeCode nvarchar(30) NOT NULL,
                IdentificationNumber nvarchar(50) NOT NULL,
                NormalizedIdentificationNumber nvarchar(50) NOT NULL,
                Email nvarchar(256) NULL,
                Phone nvarchar(50) NULL,
                SapCardCode nvarchar(50) NULL,
                CanonicalVersion bigint NOT NULL,
                IsActive bit NOT NULL
            );

            INSERT @Page
            SELECT TOP (@Take)
                bp.Id,
                bp.GlobalId,
                bp.Code,
                bp.Name,
                bp.CommercialName,
                bp.PartnerType,
                it.Code AS IdentificationTypeCode,
                bp.IdentificationNumber,
                bp.NormalizedIdentificationNumber,
                bp.Email,
                bp.Phone,
                mapping.SapCardCode,
                bp.CanonicalVersion,
                bp.IsActive
            FROM dbo.BusinessPartners bp
            INNER JOIN dbo.BusinessPartnerIdentificationTypes it ON it.Id = bp.IdentificationTypeId
            LEFT JOIN dbo.BusinessPartnerSapMapping mapping ON mapping.BusinessPartnerId = bp.Id
            WHERE bp.IsDeleted = 0
              AND bp.MasterSyncStatus = 'Accepted'
              AND bp.CanonicalVersion > 0
              AND (@LastKey IS NULL OR bp.Code > @LastKey)
            ORDER BY bp.Code;

            SELECT
                GlobalId,
                Code,
                Name,
                CommercialName,
                PartnerType,
                IdentificationTypeCode,
                IdentificationNumber,
                NormalizedIdentificationNumber,
                Email,
                Phone,
                SapCardCode,
                CanonicalVersion,
                IsActive
            FROM @Page
            ORDER BY Code;

            SELECT
                page.GlobalId AS BusinessPartnerGlobalId,
                addressItem.GlobalId,
                addressItem.AddressType,
                addressItem.Line1,
                addressItem.Line2,
                COALESCE(country.Code, addressItem.CountryCode) AS CountryCode,
                province.Code AS ProvinceCode,
                city.Code AS CityCode,
                addressItem.PostalCode,
                addressItem.Latitude,
                addressItem.Longitude,
                addressItem.IsPrimary,
                addressItem.IsActive
            FROM dbo.BusinessPartnerAddresses addressItem
            INNER JOIN @Page page ON page.Id = addressItem.BusinessPartnerId
            LEFT JOIN dbo.Countries country ON country.CountryId = addressItem.CountryId
            LEFT JOIN dbo.Provinces province ON province.ProvinceId = addressItem.ProvinceId
            LEFT JOIN dbo.Cities city ON city.CityId = addressItem.CityId
            WHERE addressItem.IsDeleted = 0
            ORDER BY page.Code, addressItem.GlobalId;

            SELECT
                page.GlobalId AS BusinessPartnerGlobalId,
                contactItem.GlobalId,
                contactType.Code AS ContactTypeCode,
                contactChannel.Code AS ContactChannelCode,
                contactItem.Name,
                contactItem.Position,
                contactItem.Department,
                contactItem.Phone,
                contactItem.Extension,
                contactItem.Mobile,
                contactItem.Email,
                contactItem.[Language],
                contactItem.ReceivesNotifications,
                contactItem.IsPrimary,
                contactItem.IsActive,
                contactItem.Notes
            FROM dbo.BusinessPartnerContacts contactItem
            INNER JOIN @Page page ON page.Id = contactItem.BusinessPartnerId
            LEFT JOIN dbo.ContactTypes contactType ON contactType.ContactTypeId = contactItem.ContactTypeId
            LEFT JOIN dbo.ContactChannels contactChannel ON contactChannel.ContactChannelId = contactItem.ContactChannelId
            WHERE contactItem.IsDeleted = 0
            ORDER BY page.Code, contactItem.GlobalId;
            """;

        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(sql, SyncFullEntitySourceHelpers.ReadParameters(context), cancellationToken: cancellationToken));
        var rows = (await grid.ReadAsync<BusinessPartnerSourceRow>()).AsList();
        var addresses = (await grid.ReadAsync<BusinessPartnerAddressSourceRow>()).ToLookup(row => row.BusinessPartnerGlobalId);
        var contacts = (await grid.ReadAsync<BusinessPartnerContactSourceRow>()).ToLookup(row => row.BusinessPartnerGlobalId);

        var limited = rows.Take(SyncFullEntitySourceHelpers.GetPageLimit(context))
            .Select(row => CreateRecord(row, addresses[row.GlobalId], contacts[row.GlobalId]))
            .ToArray();

        return new SyncSourcePage(limited, limited.LastOrDefault()?.EntityKey, rows.Count > SyncFullEntitySourceHelpers.GetPageLimit(context));
    }

    internal static SyncSourceRecord CreateRecord(
        BusinessPartnerSourceRow row,
        IEnumerable<BusinessPartnerAddressSourceRow> addresses,
        IEnumerable<BusinessPartnerContactSourceRow> contacts)
    {
        var addressSnapshots = addresses
            .OrderBy(item => item.GlobalId)
            .Select(item => new BusinessPartnerAddressSnapshot(
                item.GlobalId,
                item.AddressType,
                item.Line1,
                item.Line2,
                item.CountryCode,
                item.ProvinceCode,
                item.CityCode,
                item.PostalCode,
                item.Latitude,
                item.Longitude,
                item.IsPrimary,
                item.IsActive))
            .ToArray();
        var contactSnapshots = contacts
            .OrderBy(item => item.GlobalId)
            .Select(item => new BusinessPartnerContactSnapshot(
                item.GlobalId,
                item.ContactTypeCode,
                item.ContactChannelCode,
                item.Name,
                item.Position,
                item.Department,
                item.Phone,
                item.Extension,
                item.Mobile,
                item.Email,
                item.Language,
                item.ReceivesNotifications,
                item.IsPrimary,
                item.IsActive,
                item.Notes))
            .ToArray();
        var snapshot = new BusinessPartnerCanonicalSnapshot(
            row.GlobalId,
            row.Code,
            row.Name,
            row.CommercialName,
            row.PartnerType,
            row.IdentificationTypeCode,
            row.IdentificationNumber,
            row.NormalizedIdentificationNumber,
            row.Email,
            row.Phone,
            row.SapCardCode,
            row.IsActive,
            addressSnapshots,
            contactSnapshots);

        return new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            row.IsActive,
            new BusinessPartnerCanonicalPayloadV2(
                BusinessPartnerSyncSchemaVersions.Canonical,
                row.CanonicalVersion,
                OriginCompanyId: null,
                CausationEventId: null,
                snapshot));
    }

    internal sealed record BusinessPartnerSourceRow(
        Guid GlobalId,
        string Code,
        string Name,
        string? CommercialName,
        string PartnerType,
        string IdentificationTypeCode,
        string IdentificationNumber,
        string NormalizedIdentificationNumber,
        string? Email,
        string? Phone,
        string? SapCardCode,
        long CanonicalVersion,
        bool IsActive);

    internal sealed record BusinessPartnerAddressSourceRow(
        Guid BusinessPartnerGlobalId,
        Guid GlobalId,
        string AddressType,
        string Line1,
        string? Line2,
        string? CountryCode,
        string? ProvinceCode,
        string? CityCode,
        string? PostalCode,
        decimal? Latitude,
        decimal? Longitude,
        bool IsPrimary,
        bool IsActive);

    internal sealed record BusinessPartnerContactSourceRow(
        Guid BusinessPartnerGlobalId,
        Guid GlobalId,
        string? ContactTypeCode,
        string? ContactChannelCode,
        string Name,
        string? Position,
        string? Department,
        string? Phone,
        string? Extension,
        string? Mobile,
        string? Email,
        string? Language,
        bool ReceivesNotifications,
        bool IsPrimary,
        bool IsActive,
        string? Notes);
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

public sealed class ItemOriginFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.ItemOrigins;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context, CancellationToken cancellationToken = default)
    {
        var afterId = int.TryParse(context.LastKey, out var parsed) ? parsed : (int?)null;
        var take = SyncFullEntitySourceHelpers.GetPageLimit(context);
        var requested = Math.Clamp(take + 1, 1, 10001);
        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(
            companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<ItemOriginSourceRow>(new CommandDefinition(
            "dbo.SP_NA_GET_ITEM_ORIGIN_SYNC_FULL",
            new { AfterId = afterId, BatchSize = requested },
            cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();

        var limited = rows.Take(take).Select(row =>
            new SyncSourceRecord(row.GlobalId, row.Code, !row.IsDeleted && row.IsActive,
                new ItemOriginSyncPayload(row.GlobalId, row.Code, row.Name, row.Description,
                    row.SortOrder, !row.IsDeleted && row.IsActive, row.IsDeleted,
                    row.UpdatedAt ?? row.CreatedAt))).ToArray();
        return new SyncSourcePage(limited, rows.Take(take).LastOrDefault()?.Id.ToString(), rows.Count > take);
    }

    private sealed record ItemOriginSourceRow(int Id, Guid GlobalId, string Code, string Name,
        string? Description, int SortOrder, bool IsActive, bool IsDeleted,
        DateTime CreatedAt, DateTime? UpdatedAt);
}

public sealed class ReplenishmentMethodFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.ReplenishmentMethods;
    public async Task<SyncSourcePage> ReadPageAsync(SyncSourceReadContext context,CancellationToken cancellationToken=default)
    {
        var afterId=int.TryParse(context.LastKey,out var parsed)?parsed:(int?)null;
        var take=SyncFullEntitySourceHelpers.GetPageLimit(context);var requested=Math.Clamp(take+1,1,10001);
        var company=await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(companyResolver,context.CompanyId,cancellationToken);
        await using var connection=new SqlConnection(company.ConnectionString);
        var rows=(await connection.QueryAsync<Row>(new CommandDefinition("dbo.SP_NA_GET_REPLENISHMENT_METHOD_SYNC_FULL",new{AfterId=afterId,BatchSize=requested},cancellationToken:cancellationToken,commandType:CommandType.StoredProcedure))).AsList();
        var limited=rows.Take(take).Select(row=>new SyncSourceRecord(row.GlobalId,row.Code,!row.IsDeleted&&row.IsActive,new ReplenishmentMethodSyncPayload(row.GlobalId,row.Code,row.Name,row.Description,row.SortOrder,!row.IsDeleted&&row.IsActive,row.IsDeleted,row.UpdatedAt??row.CreatedAt))).ToArray();
        return new SyncSourcePage(limited,rows.Take(take).LastOrDefault()?.Id.ToString(),rows.Count>take);
    }
    private sealed record Row(int Id,Guid GlobalId,string Code,string Name,string? Description,int SortOrder,bool IsActive,bool IsDeleted,DateTime CreatedAt,DateTime? UpdatedAt);
}

public sealed class StorageConditionFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.StorageConditions;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        var afterId = int.TryParse(context.LastKey, out var parsed) ? parsed : (int?)null;
        var take = SyncFullEntitySourceHelpers.GetPageLimit(context);
        var requested = Math.Clamp(take + 1, 1, 10001);
        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(
            companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<StorageConditionSourceRow>(new CommandDefinition(
            "dbo.SP_NA_GET_STORAGE_CONDITION_SYNC_FULL",
            new { AfterId = afterId, BatchSize = requested },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure))).AsList();

        var limited = rows.Take(take).Select(row =>
            new SyncSourceRecord(
                row.GlobalId,
                row.Code,
                !row.IsDeleted && row.IsActive,
                new StorageConditionSyncPayload(
                    row.GlobalId,
                    row.Code,
                    row.Name,
                    row.Description,
                    row.SortOrder,
                    !row.IsDeleted && row.IsActive,
                    row.IsDeleted,
                    row.UpdatedAt ?? row.CreatedAt))).ToArray();

        return new SyncSourcePage(
            limited,
            rows.Take(take).LastOrDefault()?.Id.ToString(),
            rows.Count > take);
    }

    private sealed record StorageConditionSourceRow(
        int Id,
        Guid GlobalId,
        string Code,
        string Name,
        string? Description,
        int SortOrder,
        bool IsActive,
        bool IsDeleted,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}

public sealed class ItemSubgroupFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.ItemSubgroups;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context, CancellationToken cancellationToken = default)
    {
        var afterId = int.TryParse(context.LastKey, out var parsed) ? parsed : (int?)null;
        var take = SyncFullEntitySourceHelpers.GetPageLimit(context);
        var requested = Math.Clamp(take + 1, 1, 10001);
        var company = await SyncFullEntitySourceHelpers.ResolveSqlServerCompanyAsync(
            companyResolver, context.CompanyId, cancellationToken);
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<ItemSubgroupSourceRow>(new CommandDefinition(
            "dbo.SP_NA_GET_ITEM_SUBGROUP_SYNC_FULL",
            new { AfterId = afterId, BatchSize = requested },
            cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();

        var limited = rows.Take(take).Select(row =>
            new SyncSourceRecord(row.GlobalId, $"{row.ItemFamilyCode}|{row.Code}",
                !row.IsDeleted && row.IsActive,
                new ItemSubgroupSyncPayload(row.GlobalId, row.ItemFamilyGlobalId, row.ItemFamilyCode,
                    row.Code, row.Name, row.Description, row.SortOrder,
                    !row.IsDeleted && row.IsActive, row.IsDeleted,
                    row.CreatedAt, row.UpdatedAt))).ToArray();
        return new SyncSourcePage(limited, rows.Take(take).LastOrDefault()?.Id.ToString(), rows.Count > take);
    }

    private sealed record ItemSubgroupSourceRow(int Id, Guid GlobalId, Guid ItemFamilyGlobalId,
        string ItemFamilyCode, string Code, string Name, string? Description, int SortOrder,
        bool IsActive, bool IsDeleted, DateTime CreatedAt, DateTime? UpdatedAt);
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
