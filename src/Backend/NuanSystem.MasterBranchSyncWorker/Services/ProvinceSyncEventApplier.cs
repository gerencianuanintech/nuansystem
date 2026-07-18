using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class ProvinceSyncEventApplier(
    IProvinceSyncApplyRepository repository) : ISyncEntityEventApplier
{
    private const string EntityName = SyncMasterBranchEntityCodes.Provinces;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public bool CanApply(string entityName)
    {
        return string.Equals(entityName, EntityName, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<SyncEventApplyResult> ApplyAsync(
        SyncEventApplyContext context,
        CancellationToken cancellationToken = default)
    {
        if (context.TargetCompanyId is null)
        {
            return new SyncEventApplyResult(false, "Provinces requiere sucursal destino.", "SYNC_TARGET_REQUIRED");
        }

        var payload = ReadPayload(context.PayloadJson);
        if (payload.GlobalId == Guid.Empty || payload.GlobalId != context.EntityGlobalId)
        {
            return new SyncEventApplyResult(false, "Payload Provinces no coincide con EntityGlobalId.", "SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");
        }

        if (payload.CountryGlobalId == Guid.Empty)
        {
            return new SyncEventApplyResult(false, "Payload Provinces no contiene CountryGlobalId.", "SYNC_PARENT_GLOBAL_ID_REQUIRED");
        }

        var operation = Enum.Parse<SyncOperation>(context.Operation, ignoreCase: true);
        var result = operation switch
        {
            SyncOperation.Disabled => await repository.DisableFromSyncAsync(
                context.TargetCompanyId.Value,
                context,
                payload,
                markDeleted: false,
                cancellationToken),

            SyncOperation.Deleted => await repository.DisableFromSyncAsync(
                context.TargetCompanyId.Value,
                context,
                payload,
                markDeleted: true,
                cancellationToken),

            _ => await repository.UpsertFromSyncAsync(
                context.TargetCompanyId.Value,
                context,
                payload,
                operation,
                cancellationToken)
        };

        return new SyncEventApplyResult(result.Applied, result.Message);
    }

    private static ProvinceSyncPayload ReadPayload(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);
        if (!document.RootElement.TryGetProperty("payload", out var payloadElement))
        {
            throw new InvalidOperationException("Payload SyncOutbox no contiene nodo payload.");
        }

        return payloadElement.Deserialize<ProvinceSyncPayload>(JsonOptions)
            ?? throw new InvalidOperationException("Payload Provinces no pudo ser deserializado.");
    }
}
