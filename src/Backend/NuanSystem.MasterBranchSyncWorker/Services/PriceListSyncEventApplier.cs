using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class PriceListSyncEventApplier(IPriceListSyncApplyRepository repository)
    : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public bool CanApply(string entityName) =>
        string.Equals(entityName, SyncMasterBranchEntityCodes.PriceLists, StringComparison.OrdinalIgnoreCase);

    public async Task<SyncEventApplyResult> ApplyAsync(SyncEventApplyContext context, CancellationToken cancellationToken = default)
    {
        if (context.TargetCompanyId is null)
        {
            return new(false, "PriceList requiere sucursal destino.", "SYNC_TARGET_REQUIRED");
        }

        using var document = JsonDocument.Parse(context.PayloadJson);
        if (!document.RootElement.TryGetProperty("payload", out var payloadElement))
        {
            return new(false, "Payload PriceList no contiene nodo payload.", "SYNC_PAYLOAD_REQUIRED");
        }

        var payload = payloadElement.Deserialize<PriceListSyncPayloadV2>(JsonOptions)
            ?? throw new InvalidOperationException("Payload PriceList no pudo deserializarse.");
        if (payload.GlobalId == Guid.Empty || payload.GlobalId != context.EntityGlobalId)
        {
            return new(false, "Payload PriceList no coincide con EntityGlobalId.", "SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");
        }

        var operation = Enum.Parse<SyncOperation>(context.Operation, true);
        var result = await repository.ApplyAsync(
            context.TargetCompanyId.Value, context, payload, operation, cancellationToken);
        return new(
            result.Applied,
            result.Message,
            result.ErrorCode,
            Retryable: string.Equals(
                result.ErrorCode,
                "SYNC_PRICELIST_CURRENCY_DEPENDENCY",
                StringComparison.Ordinal),
            Terminal: result.TerminalConflict);
    }
}
