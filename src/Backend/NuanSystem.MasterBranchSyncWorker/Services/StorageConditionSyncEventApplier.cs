using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class StorageConditionSyncEventApplier(IStorageConditionSyncApplyRepository repository) : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public bool CanApply(string entityName) => string.Equals(entityName, SyncMasterBranchEntityCodes.StorageConditions, StringComparison.OrdinalIgnoreCase);
    public async Task<SyncEventApplyResult> ApplyAsync(SyncEventApplyContext context, CancellationToken cancellationToken=default)
    {
        if (context.TargetCompanyId is null) return new(false,"StorageCondition requiere sucursal destino.","SYNC_TARGET_REQUIRED",Terminal:true);
        StorageConditionSyncPayload payload;
        try { using var document=JsonDocument.Parse(context.PayloadJson); if(!document.RootElement.TryGetProperty("payload",out var element)) throw new JsonException(); payload=element.Deserialize<StorageConditionSyncPayload>(JsonOptions)??throw new JsonException(); }
        catch(JsonException) { return new(false,"Payload StorageCondition no es JSON valido.","SYNC_PAYLOAD_INVALID",Terminal:true); }
        if(payload.GlobalId==Guid.Empty||payload.GlobalId!=context.EntityGlobalId) return new(false,"Payload no coincide con EntityGlobalId.","SYNC_PAYLOAD_GLOBAL_ID_MISMATCH",Terminal:true);
        if(!Enum.TryParse<SyncOperation>(context.Operation,true,out var operation)||!Enum.IsDefined(operation)) return new(false,"Operacion no permitida.","SYNC_OPERATION_INVALID",Terminal:true);
        if(string.IsNullOrWhiteSpace(payload.Code)||payload.Code.Trim().Length>50||string.IsNullOrWhiteSpace(payload.Name)||payload.Name.Trim().Length>150||payload.Description?.Trim().Length>500||payload.SortOrder<0) return new(false,"Payload StorageCondition invalido.","SYNC_STORAGE_CONDITION_PAYLOAD_INVALID",Terminal:true);
        var result=await repository.ApplyAsync(context.TargetCompanyId.Value,context,payload,operation,cancellationToken);
        return new(result.Applied,result.Message,result.ErrorCode,Terminal:result.TerminalConflict);
    }
}


