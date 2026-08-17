using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class ItemSubgroupSyncEventApplier(IItemSubgroupSyncApplyRepository repository) : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public bool CanApply(string entityName) =>
        string.Equals(entityName, SyncMasterBranchEntityCodes.ItemSubgroups, StringComparison.OrdinalIgnoreCase);

    public async Task<SyncEventApplyResult> ApplyAsync(SyncEventApplyContext context,
        CancellationToken cancellationToken = default)
    {
        if (context.TargetCompanyId is null)
            return new(false, "ItemSubgroup requiere sucursal destino.", "SYNC_TARGET_REQUIRED", Terminal: true);

        ItemSubgroupSyncPayload payload;
        try { payload = ReadPayload(context.PayloadJson); }
        catch (JsonException)
        {
            return new(false, "Payload ItemSubgroup no es JSON válido.", "SYNC_PAYLOAD_INVALID", Terminal: true);
        }

        if (payload.GlobalId == Guid.Empty || payload.GlobalId != context.EntityGlobalId)
            return new(false, "Payload ItemSubgroup no coincide con EntityGlobalId.",
                "SYNC_PAYLOAD_GLOBAL_ID_MISMATCH", Terminal: true);
        if (!Enum.TryParse<SyncOperation>(context.Operation, true, out var operation) || !Enum.IsDefined(operation))
            return new(false, "Operación ItemSubgroup no permitida.", "SYNC_OPERATION_INVALID", Terminal: true);
        if (payload.ItemFamilyGlobalId == Guid.Empty || string.IsNullOrWhiteSpace(payload.ItemFamilyCode)
            || string.IsNullOrWhiteSpace(payload.Code) || payload.Code.Trim().Length > 50
            || string.IsNullOrWhiteSpace(payload.Name) || payload.Name.Trim().Length > 150
            || payload.Description?.Trim().Length > 500 || payload.SortOrder < 0)
            return new(false, "Payload ItemSubgroup incumple familia, campos obligatorios, orden o longitudes.",
                "SYNC_ITEM_SUBGROUP_PAYLOAD_INVALID", Terminal: true);

        if (!await repository.ItemFamilyExistsAsync(context.TargetCompanyId.Value,
                payload.ItemFamilyGlobalId, cancellationToken))
            return new(false, $"ItemFamily {payload.ItemFamilyCode} todavía no existe en la sucursal.",
                "SYNC_ITEM_SUBGROUP_ITEM_FAMILY_PENDING", Retryable: true);

        var result = await repository.ApplyAsync(context.TargetCompanyId.Value, context, payload, operation,
            cancellationToken);
        return new(result.Applied, result.Message, result.ErrorCode, Terminal: result.TerminalConflict);
    }

    private static ItemSubgroupSyncPayload ReadPayload(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("payload", out var element))
            throw new JsonException("Payload SyncOutbox no contiene nodo payload.");
        return element.Deserialize<ItemSubgroupSyncPayload>(JsonOptions)
            ?? throw new JsonException("Payload ItemSubgroup no pudo ser deserializado.");
    }
}
