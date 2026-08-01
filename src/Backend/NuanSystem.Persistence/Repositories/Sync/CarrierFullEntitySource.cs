using System.Data;
using System.Globalization;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class CarrierFullEntitySource(ICompanyResolver companyResolver) : ISyncFullEntitySource
{
    private const string ProcedureName = "dbo.SP_NA_GET_CARRIER_SYNC_FULL";

    public string EntityCode => SyncMasterBranchEntityCodes.Carrier;

    public async Task<SyncSourcePage> ReadPageAsync(
        SyncSourceReadContext context,
        CancellationToken cancellationToken = default)
    {
        var afterId = ParseAfterId(context.LastKey);
        var pageLimit = GetPageLimit(context);
        var batchSize = Math.Clamp(pageLimit + 1, 1, 10001);
        var company = await ResolveSqlServerCompanyAsync(context.CompanyId, cancellationToken);

        await using var connection = new SqlConnection(company.ConnectionString);
        var rows = (await connection.QueryAsync<CarrierFullSourceRow>(
            new CommandDefinition(
                ProcedureName,
                new { AfterId = afterId, BatchSize = batchSize },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure))).AsList();

        var selected = rows.Take(pageLimit).ToArray();
        var records = selected.Select(row => new SyncSourceRecord(
            row.GlobalId,
            row.Code,
            !row.IsDeleted && row.IsActive,
            new CarrierSyncPayloadV1(
                row.GlobalId,
                row.Code,
                row.Name,
                row.IdentificationTypeCode,
                row.IdentificationNumber,
                row.Description,
                !row.IsDeleted && row.IsActive,
                row.IsDeleted,
                row.CreatedAt,
                row.UpdatedAt))).ToArray();

        return new SyncSourcePage(
            records,
            selected.LastOrDefault()?.Id.ToString(CultureInfo.InvariantCulture),
            rows.Count > pageLimit);
    }

    private async Task<CompanyConnectionInfo> ResolveSqlServerCompanyAsync(
        int companyId,
        CancellationToken cancellationToken)
    {
        var company = await companyResolver.ResolveByIdAsync(companyId, cancellationToken)
            ?? throw new InvalidOperationException($"La empresa {companyId} no existe.");

        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
        {
            throw new NotSupportedException("La lectura Full de Transportistas solo esta implementada para SQL Server.");
        }

        return company;
    }

    private static int? ParseAfterId(string? lastKey)
    {
        if (string.IsNullOrWhiteSpace(lastKey))
        {
            return null;
        }

        if (!int.TryParse(lastKey, NumberStyles.None, CultureInfo.InvariantCulture, out var afterId) || afterId < 0)
        {
            throw new InvalidOperationException("El cursor Full de Transportistas no es valido.");
        }

        return afterId;
    }

    private static int GetPageLimit(SyncSourceReadContext context)
    {
        var requested = context.RemainingLimit.HasValue
            ? Math.Min(context.PageSize, Math.Max(context.RemainingLimit.Value, 0))
            : context.PageSize;
        return Math.Clamp(requested, 1, 10000);
    }

    private sealed class CarrierFullSourceRow
    {
        public int Id { get; init; }
        public Guid GlobalId { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string IdentificationTypeCode { get; init; } = string.Empty;
        public string IdentificationNumber { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
