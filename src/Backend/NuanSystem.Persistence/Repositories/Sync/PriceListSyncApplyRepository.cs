using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class PriceListSyncApplyRepository(ICompanyResolver companyResolver) : IPriceListSyncApplyRepository
{
    public async Task<PriceListSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        PriceListSyncPayloadV2 payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        var company = await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró la sucursal destino {branchCompanyId}.");
        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
        {
            throw new NotSupportedException($"El motor {company.DatabaseEngine} no está implementado para Sync PriceList.");
        }

        await using var connection = new SqlConnection(company.ConnectionString);
        var result = await connection.QuerySingleAsync<ApplyResult>(new CommandDefinition(
            "dbo.SP_NA_POST_PRICELIST_SYNC_APPLY_EVENT",
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
                payload.CurrencyGlobalId,
                CurrencyCodeEvidence = Required(payload.CurrencyCode, nameof(payload.CurrencyCode), 3),
                AppliesTo = Required(payload.AppliesTo, nameof(payload.AppliesTo), 20),
                payload.IsDefault,
                IsActive = operation is not SyncOperation.Disabled and not SyncOperation.Deleted && payload.IsActive,
                IsDeleted = operation == SyncOperation.Deleted,
                ExternalSystem = Optional(payload.ExternalSystem, 50),
                ExternalCode = Optional(payload.ExternalCode, 100),
                SapCode = Optional(payload.SapCode, 100),
                CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
                UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow
            },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));

        return result.ResultCode switch
        {
            2 => new(true, true, false, result.PriceListId, "Evento ya aplicado en SyncInbox."),
            -2 => new(false, false, true, null,
                "El código pertenece a otro GlobalId; no se realizó adopción automática.",
                "SYNC_PRICELIST_CODE_CONFLICT"),
            -3 => new(false, false, false, null,
                "La moneda dependiente todavía no existe en la sucursal.",
                "SYNC_PRICELIST_CURRENCY_DEPENDENCY"),
            -4 => new(false, false, true, null,
                "La lista predeterminada entra en conflicto con el ámbito existente.",
                "SYNC_PRICELIST_DEFAULT_CONFLICT"),
            _ when result.ResultCode > 0 => new(true, false, false, result.PriceListId,
                $"Lista de precios sincronizada por GlobalId {payload.GlobalId}."),
            _ => new(false, false, false, null, "No se pudo aplicar PriceList.", "SYNC_PRICELIST_APPLY_FAILED")
        };
    }

    private static string Required(string value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{field} es requerido para sincronizar PriceList.");
        }

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
        {
            throw new InvalidOperationException($"{field} excede la longitud permitida.");
        }

        return normalized;
    }

    private static string? Optional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
        {
            throw new InvalidOperationException("Un campo opcional excede la longitud permitida.");
        }

        return normalized;
    }

    private sealed record ApplyResult(int ResultCode, int? PriceListId);
}
