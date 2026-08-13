using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class ProductTypeSyncEventApplier(IProductTypeSyncApplyRepository repository) : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public bool CanApply(string entityName) =>
        string.Equals(entityName, SyncMasterBranchEntityCodes.ProductTypes, StringComparison.OrdinalIgnoreCase);

    public async Task<SyncEventApplyResult> ApplyAsync(SyncEventApplyContext context,
        CancellationToken cancellationToken = default)
    {
        if (context.TargetCompanyId is null)
            return new(false, "ProductType requiere sucursal destino.", "SYNC_TARGET_REQUIRED", Terminal: true);

        ProductTypeSyncPayload payload;
        try { payload = ReadPayload(context.PayloadJson); }
        catch (JsonException) { return new(false, "Payload ProductType no es JSON valido.", "SYNC_PAYLOAD_INVALID", Terminal: true); }

        if (payload.GlobalId == Guid.Empty || payload.GlobalId != context.EntityGlobalId)
            return new(false, "Payload ProductType no coincide con EntityGlobalId.",
                "SYNC_PAYLOAD_GLOBAL_ID_MISMATCH", Terminal: true);
        if (!Enum.TryParse<SyncOperation>(context.Operation, true, out var operation) || !Enum.IsDefined(operation))
            return new(false, "Operacion ProductType no permitida.", "SYNC_OPERATION_INVALID", Terminal: true);
        if (string.IsNullOrWhiteSpace(payload.Code) || payload.Code.Trim().Length > 50
            || string.IsNullOrWhiteSpace(payload.Name) || payload.Name.Trim().Length > 150
            || payload.Description?.Trim().Length > 500
            || string.IsNullOrWhiteSpace(payload.NatureCode)
            || !ProductTypeNatureCodes.All.Contains(payload.NatureCode.Trim()) || payload.SortOrder < 0)
            return new(false, "Payload ProductType incumple campos obligatorios, naturaleza o longitudes.",
                "SYNC_PRODUCT_TYPE_PAYLOAD_INVALID", Terminal: true);

        var result = await repository.ApplyAsync(context.TargetCompanyId.Value, context, payload, operation, cancellationToken);
        return new(result.Applied, result.Message, result.ErrorCode, Terminal: result.TerminalConflict);
    }

    private static ProductTypeSyncPayload ReadPayload(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("payload", out var element))
            throw new JsonException("Payload SyncOutbox no contiene nodo payload.");
        return element.Deserialize<ProductTypeSyncPayload>(JsonOptions)
            ?? throw new JsonException("Payload ProductType no pudo ser deserializado.");
    }
}
