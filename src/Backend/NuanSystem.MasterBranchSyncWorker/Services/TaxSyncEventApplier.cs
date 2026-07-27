using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class TaxSyncEventApplier(ITaxSyncApplyRepository repository) : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public bool CanApply(string entityName) =>
        string.Equals(entityName, SyncMasterBranchEntityCodes.Taxes, StringComparison.OrdinalIgnoreCase);

    public async Task<SyncEventApplyResult> ApplyAsync(SyncEventApplyContext context, CancellationToken cancellationToken = default)
    {
        if (context.TargetCompanyId is null)
            return new(false, "Tax requiere sucursal destino.", "SYNC_TARGET_REQUIRED");

        using var document = JsonDocument.Parse(context.PayloadJson);
        if (!document.RootElement.TryGetProperty("payload", out var element))
            return new(false, "Payload Tax no contiene nodo payload.", "SYNC_PAYLOAD_REQUIRED");
        var payload = element.Deserialize<TaxSyncPayloadV1>(JsonOptions)
            ?? throw new InvalidOperationException("Payload Tax no pudo deserializarse.");
        if (payload.GlobalId == Guid.Empty || payload.GlobalId != context.EntityGlobalId)
            return new(false, "Payload Tax no coincide con EntityGlobalId.", "SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");

        var operation = Enum.Parse<SyncOperation>(context.Operation, true);
        var result = await repository.ApplyAsync(
            context.TargetCompanyId.Value, context, payload, operation, cancellationToken);
        return new(result.Applied, result.Message, result.ErrorCode, Terminal: result.TerminalConflict);
    }
}
