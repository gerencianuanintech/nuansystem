using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class CountrySyncApplyRepository(ICompanyResolver companyResolver) : ICountrySyncApplyRepository
{
    private const string ApplyProcedure = "dbo.SP_NA_POST_COUNTRY_SYNC_APPLY_EVENT";

    public Task<CountrySyncApplyResult> UpsertFromSyncAsync(int branchCompanyId, SyncEventApplyContext context, CountrySyncPayload payload, SyncOperation operation, CancellationToken cancellationToken = default) =>
        ApplyAsync(branchCompanyId, context, payload, operation, markDeleted: false, cancellationToken);

    public Task<CountrySyncApplyResult> DisableFromSyncAsync(int branchCompanyId, SyncEventApplyContext context, CountrySyncPayload payload, bool markDeleted, CancellationToken cancellationToken = default) =>
        ApplyAsync(branchCompanyId, context, payload, markDeleted ? SyncOperation.Deleted : SyncOperation.Disabled, markDeleted, cancellationToken);

    private async Task<CountrySyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CountrySyncPayload payload,
        SyncOperation operation,
        bool markDeleted,
        CancellationToken cancellationToken)
    {
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
        var isDeleted = markDeleted || operation == SyncOperation.Deleted || payload.IsDeleted;
        var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;

        var result = await connection.QuerySingleAsync<ApplyResult>(new CommandDefinition(
            ApplyProcedure,
            new
            {
                context.EventId,
                context.SourceCompanyId,
                context.EntityName,
                context.EntityGlobalId,
                Operation = operation.ToString(),
                context.PayloadJson,
                payload.GlobalId,
                Code = Required(payload.Code, nameof(payload.Code), 10),
                Name = Required(payload.Name, nameof(payload.Name), 120),
                Iso2 = Optional(payload.Iso2, nameof(payload.Iso2), 2),
                Iso3 = Optional(payload.Iso3, nameof(payload.Iso3), 3),
                PhonePrefix = Optional(payload.PhonePrefix, nameof(payload.PhonePrefix), 10),
                IsActive = isActive,
                IsDeleted = isDeleted,
                ExternalSystem = Optional(payload.ExternalSystem, nameof(payload.ExternalSystem), 50),
                ExternalCode = Optional(payload.ExternalCode, nameof(payload.ExternalCode), 100),
                CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
                UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow
            },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));

        return result.ResultCode switch
        {
            2 => new(true, true, false, result.CountryId, "Evento ya aplicado en SyncInbox."),
            -2 => new(false, false, true, null, "El codigo de Country pertenece a otro GlobalId; no se realizo adopcion automatica.", "SYNC_COUNTRY_CODE_CONFLICT"),
            -3 => new(false, false, true, null, "La referencia externa de Country pertenece a otro GlobalId.", "SYNC_COUNTRY_EXTERNAL_CONFLICT"),
            > 0 => new(true, false, false, result.CountryId, $"Pais sincronizado por GlobalId {payload.GlobalId}."),
            _ => new(false, false, false, null, "No se pudo aplicar Country.", "SYNC_COUNTRY_APPLY_FAILED")
        };
    }

    private async Task<CompanyConnectionInfo> ResolveBranchAsync(int branchCompanyId, CancellationToken cancellationToken) =>
        await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
        ?? throw new InvalidOperationException($"No se encontro la sucursal destino {branchCompanyId}.");

    private static SqlConnection CreateSqlConnection(CompanyConnectionInfo company) =>
        company.DatabaseEngine == DatabaseEngine.SqlServer
            ? new SqlConnection(company.ConnectionString)
            : throw new NotSupportedException($"El motor {company.DatabaseEngine} no esta implementado para Sync Countries.");

    private static string Required(string value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"{field} es requerido para sincronizar Countries.");
        var normalized = value.Trim();
        if (normalized.Length > maxLength) throw new InvalidOperationException($"{field} excede la longitud permitida.");
        return normalized;
    }

    private static string? Optional(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > maxLength) throw new InvalidOperationException($"{field} excede la longitud permitida.");
        return normalized;
    }

    private sealed class ApplyResult
    {
        public int ResultCode { get; set; }
        public int? CountryId { get; set; }
    }
}
