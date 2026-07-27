using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class TaxSyncApplyRepository(ICompanyResolver companyResolver) : ITaxSyncApplyRepository
{
    public async Task<TaxSyncApplyResult> ApplyAsync(
        int branchCompanyId, SyncEventApplyContext context, TaxSyncPayloadV1 payload,
        SyncOperation operation, CancellationToken cancellationToken = default)
    {
        var company = await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró la sucursal destino {branchCompanyId}.");
        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
            throw new NotSupportedException($"El motor {company.DatabaseEngine} no está implementado para Sync Tax.");
        if (payload.Rate is < 0m or > 1m)
            return new(false, false, true, null, "La tasa recibida está fuera del contrato decimal 0..1.", "SYNC_TAX_RATE_OUT_OF_RANGE");

        await using var connection = new SqlConnection(company.ConnectionString);
        var result = await connection.QuerySingleAsync<ApplyResult>(new CommandDefinition(
            "dbo.SP_NA_POST_TAX_SYNC_APPLY_EVENT",
            new
            {
                context.EventId,
                context.SourceCompanyId,
                context.EntityName,
                context.EntityGlobalId,
                Operation = operation.ToString(),
                context.PayloadJson,
                payload.GlobalId,
                Code = Required(payload.Code, nameof(payload.Code), 30),
                Name = Required(payload.Name, nameof(payload.Name), 120),
                Description = Optional(payload.Description, 300),
                payload.Rate,
                IsActive = operation is not SyncOperation.Disabled and not SyncOperation.Deleted && payload.IsActive,
                IsDeleted = operation == SyncOperation.Deleted,
                ExternalSystem = Optional(payload.ExternalSystem, 50),
                ExternalCode = Optional(payload.ExternalCode, 100),
                CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
                UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow
            },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));

        return result.ResultCode switch
        {
            2 => new(true, true, false, result.TaxId, "Evento ya aplicado en SyncInbox."),
            -2 => new(false, false, true, null,
                "El código pertenece a otro GlobalId; no se realizó adopción automática.",
                "SYNC_TAX_CODE_CONFLICT"),
            -3 => new(false, false, true, null,
                "La tasa está fuera del contrato decimal 0..1.",
                "SYNC_TAX_RATE_OUT_OF_RANGE"),
            _ when result.ResultCode > 0 => new(true, false, false, result.TaxId,
                $"Impuesto sincronizado por GlobalId {payload.GlobalId}."),
            _ => new(false, false, false, null, "No se pudo aplicar Tax.", "SYNC_TAX_APPLY_FAILED")
        };
    }

    private static string Required(string value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"{field} es requerido para sincronizar Tax.");
        var normalized = value.Trim();
        if (normalized.Length > maxLength) throw new InvalidOperationException($"{field} excede la longitud permitida.");
        return normalized;
    }

    private static string? Optional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > maxLength) throw new InvalidOperationException("Un campo opcional excede la longitud permitida.");
        return normalized;
    }

    private sealed record ApplyResult(int ResultCode, int? TaxId);
}
