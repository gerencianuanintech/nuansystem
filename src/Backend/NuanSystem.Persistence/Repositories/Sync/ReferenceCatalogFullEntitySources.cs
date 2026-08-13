using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class TaxFullEntitySource(ICompanyResolver resolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.Taxes;
    public Task<SyncSourcePage> ReadPageAsync(SyncSourceReadContext context, CancellationToken cancellationToken = default) =>
        ReferenceCatalogFullSource.ReadAsync(resolver, context, EntityCode, cancellationToken);
}

public sealed class PriceListFullEntitySource(ICompanyResolver resolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.PriceLists;
    public Task<SyncSourcePage> ReadPageAsync(SyncSourceReadContext context, CancellationToken cancellationToken = default) =>
        PriceListFullSource.ReadAsync(resolver, context, cancellationToken);
}

public sealed class PaymentTermFullEntitySource(ICompanyResolver resolver) : ISyncFullEntitySource
{
    public string EntityCode => SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms;
    public Task<SyncSourcePage> ReadPageAsync(SyncSourceReadContext context, CancellationToken cancellationToken = default) =>
        ReferenceCatalogFullSource.ReadAsync(resolver, context, EntityCode, cancellationToken);
}

internal static class PriceListFullSource
{
    public static async Task<SyncSourcePage> ReadAsync(
        ICompanyResolver resolver,
        SyncSourceReadContext context,
        CancellationToken cancellationToken)
    {
        var company = await resolver.ResolveByIdAsync(context.CompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"La empresa {context.CompanyId} no existe.");
        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
        {
            throw new NotSupportedException("La lectura Full de PriceList solo está implementada para SQL Server.");
        }

        const string sql = """
SELECT TOP (@Take)
    p.PriceListId Id,
    p.GlobalId,
    p.Code,
    p.Name,
    p.Description,
    c.GlobalId CurrencyGlobalId,
    p.CurrencyCode,
    p.AppliesTo,
    p.IsDefault,
    p.IsActive,
    p.ExternalSystem,
    p.ExternalCode,
    p.SapCode,
    p.CreatedAt,
    p.UpdatedAt
FROM dbo.PriceLists p
INNER JOIN dbo.Currencies c
    ON c.Code = p.CurrencyCode
   AND c.IsDeleted = 0
WHERE p.IsDeleted = 0
  AND (@LastKey IS NULL OR p.Code > @LastKey)
ORDER BY p.Code;
""";

        var take = context.RemainingLimit.HasValue
            ? Math.Min(context.PageSize + 1, context.RemainingLimit.Value + 1)
            : context.PageSize + 1;
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<Row>(new CommandDefinition(
            sql,
            new
            {
                LastKey = string.IsNullOrWhiteSpace(context.LastKey) ? null : context.LastKey,
                Take = Math.Clamp(take, 1, 10001)
            },
            cancellationToken: cancellationToken))).AsList();
        var limit = context.RemainingLimit.HasValue
            ? Math.Min(context.PageSize, Math.Max(context.RemainingLimit.Value, 0))
            : context.PageSize;
        var records = rows.Take(limit).Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            row.IsActive,
            new PriceListSyncPayloadV2(
                row.GlobalId, row.Code, row.Name, row.Description, row.CurrencyGlobalId,
                row.CurrencyCode, row.AppliesTo, row.IsDefault, row.IsActive,
                row.ExternalSystem, row.ExternalCode, row.SapCode, row.CreatedAt, row.UpdatedAt)))
            .ToArray();
        return new(records, records.LastOrDefault()?.EntityKey, rows.Count > limit);
    }

    private sealed record Row(
        int Id,
        Guid GlobalId,
        string Code,
        string Name,
        string? Description,
        Guid CurrencyGlobalId,
        string CurrencyCode,
        string AppliesTo,
        bool IsDefault,
        bool IsActive,
        string? ExternalSystem,
        string? ExternalCode,
        string? SapCode,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}

internal static class ReferenceCatalogFullSource
{
    public static async Task<SyncSourcePage> ReadAsync(ICompanyResolver resolver, SyncSourceReadContext context, string entityCode, CancellationToken cancellationToken)
    {
        var (table, id, projection) = entityCode switch
        {
            SyncMasterBranchEntityCodes.Taxes => ("Taxes", "Id", "Description,Rate,CAST(NULL AS nvarchar(10)) CurrencyCode,CAST(NULL AS nvarchar(30)) AppliesTo,CAST(0 AS bit) IsDefault"),
            SyncMasterBranchEntityCodes.UnitOfMeasures => ("UnitOfMeasures", "Id", "Description,CAST(NULL AS decimal(18,6)) Rate,CAST(NULL AS nvarchar(10)) CurrencyCode,CAST(NULL AS nvarchar(30)) AppliesTo,CAST(0 AS bit) IsDefault"),
            SyncMasterBranchEntityCodes.PriceLists => ("PriceLists", "PriceListId", "Description,CAST(NULL AS decimal(18,6)) Rate,CurrencyCode,AppliesTo,IsDefault"),
            SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms => ("BusinessPartnerPaymentTerms", "Id", "CAST(NULL AS nvarchar(500)) Description,CAST(NULL AS decimal(18,6)) Rate,CAST(NULL AS nvarchar(10)) CurrencyCode,CAST(NULL AS nvarchar(30)) AppliesTo,CAST(0 AS bit) IsDefault,Days,IsCredit"),
            _ => throw new InvalidOperationException($"Fuente Full no soportada: {entityCode}.")
        };
        var sql = $"""
            SELECT TOP (@Take) {id} Id,GlobalId,Code,Name,{projection},IsActive,ExternalSystem,ExternalCode,CreatedAt,UpdatedAt
            FROM dbo.{table}
            WHERE IsDeleted=0 AND (@LastKey IS NULL OR Code>@LastKey)
            ORDER BY Code;
            """;
        var company = await resolver.ResolveByIdAsync(context.CompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"La empresa {context.CompanyId} no existe.");
        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
            throw new NotSupportedException("La lectura Full de catalogos de referencia solo esta implementada para SQL Server.");
        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<Row>(new CommandDefinition(sql,
            ReadParameters(context), cancellationToken: cancellationToken))).AsList();
        var limit = GetPageLimit(context);
        var records = rows.Take(limit).Select(row => new SyncSourceRecord(row.GlobalId, row.Code, row.IsActive,
            new ReferenceCatalogSyncPayload(row.GlobalId,row.Code,row.Name,row.Description,row.Rate,row.CurrencyCode,row.AppliesTo,row.IsDefault,
                row.IsActive,row.ExternalSystem,row.ExternalCode,row.CreatedAt,row.UpdatedAt,row.Days,row.IsCredit))).ToArray();
        return new(records, records.LastOrDefault()?.EntityKey, rows.Count > limit);
    }

    private static int GetPageLimit(SyncSourceReadContext context) => context.RemainingLimit.HasValue
        ? Math.Min(context.PageSize, Math.Max(context.RemainingLimit.Value, 0)) : context.PageSize;
    private static object ReadParameters(SyncSourceReadContext context)
    {
        var take = context.RemainingLimit.HasValue ? Math.Min(context.PageSize + 1, context.RemainingLimit.Value + 1) : context.PageSize + 1;
        return new { LastKey = string.IsNullOrWhiteSpace(context.LastKey) ? null : context.LastKey, Take = Math.Clamp(take, 1, 10001) };
    }

    private sealed record Row(int Id, Guid GlobalId, string Code, string Name, string? Description, decimal? Rate,
        string? CurrencyCode, string? AppliesTo, bool IsDefault, bool IsActive, string? ExternalSystem, string? ExternalCode,
        DateTime CreatedAt, DateTime? UpdatedAt, int? Days = null, bool? IsCredit = null);
}
