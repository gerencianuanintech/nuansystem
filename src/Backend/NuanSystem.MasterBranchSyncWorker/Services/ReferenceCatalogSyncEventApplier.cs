using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class ReferenceCatalogSyncEventApplier(IReferenceCatalogSyncApplyRepository repository)
    : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
    {
        SyncMasterBranchEntityCodes.Taxes,
        SyncMasterBranchEntityCodes.UnitOfMeasures,
        SyncMasterBranchEntityCodes.PriceLists,
        SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms
    };

    public bool CanApply(string entityName) => Supported.Contains(entityName);

    public async Task<SyncEventApplyResult> ApplyAsync(SyncEventApplyContext context, CancellationToken cancellationToken = default)
    {
        if (context.TargetCompanyId is null)
            return new(false, $"{context.EntityName} requiere sucursal destino.", "SYNC_TARGET_REQUIRED");

        using var document = JsonDocument.Parse(context.PayloadJson);
        if (!document.RootElement.TryGetProperty("payload", out var element))
            throw new InvalidOperationException("Payload SyncOutbox no contiene nodo payload.");
        var payload = element.Deserialize<ReferenceCatalogSyncPayload>(JsonOptions)
            ?? throw new InvalidOperationException($"Payload {context.EntityName} no pudo deserializarse.");
        if (payload.GlobalId == Guid.Empty || payload.GlobalId != context.EntityGlobalId)
            return new(false, $"Payload {context.EntityName} no coincide con EntityGlobalId.", "SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");

        var operation = Enum.Parse<SyncOperation>(context.Operation, true);
        var result = await repository.ApplyAsync(context.TargetCompanyId.Value, context.EntityName, context, payload, operation, cancellationToken);
        return new(result.Applied, result.Message);
    }
}
