using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class CitySyncEventApplier(
    ICitySyncApplyRepository repository) : ISyncEntityEventApplier
{
    private const string EntityName = SyncMasterBranchEntityCodes.Cities;

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
            return new SyncEventApplyResult(false, "Cities requiere sucursal destino.", "SYNC_TARGET_REQUIRED", Terminal: true);
        }

        CitySyncPayload payload;
        try
        {
            payload = ReadPayload(context.PayloadJson);
        }
        catch (JsonException)
        {
            return new SyncEventApplyResult(false, "Payload Cities no es JSON valido.", "SYNC_PAYLOAD_INVALID", Terminal: true);
        }

        if (payload.GlobalId == Guid.Empty || payload.GlobalId != context.EntityGlobalId)
        {
            return new SyncEventApplyResult(false, "Payload Cities no coincide con EntityGlobalId.", "SYNC_PAYLOAD_GLOBAL_ID_MISMATCH", Terminal: true);
        }

        if (payload.CountryGlobalId == Guid.Empty)
        {
            return new SyncEventApplyResult(false, "Payload Cities no contiene CountryGlobalId.", "SYNC_COUNTRY_GLOBAL_ID_REQUIRED", Terminal: true);
        }

        if (payload.ProvinceGlobalId == Guid.Empty)
        {
            return new SyncEventApplyResult(false, "Payload Cities no contiene ProvinceGlobalId.", "SYNC_PROVINCE_GLOBAL_ID_REQUIRED", Terminal: true);
        }

        if (!Enum.TryParse<SyncOperation>(context.Operation, true, out var operation) || !Enum.IsDefined(operation))
        {
            return new SyncEventApplyResult(false, "Operacion Cities no permitida.", "SYNC_OPERATION_INVALID", Terminal: true);
        }

        if (string.IsNullOrWhiteSpace(payload.Code)
            || payload.Code.Trim().Length > 20
            || string.IsNullOrWhiteSpace(payload.Name)
            || payload.Name.Trim().Length > 120
            || payload.ExternalSystem?.Trim().Length > 50
            || payload.ExternalCode?.Trim().Length > 100)
        {
            return new SyncEventApplyResult(false, "Payload Cities incumple campos obligatorios o longitudes.", "SYNC_CITY_PAYLOAD_INVALID", Terminal: true);
        }

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

        return new SyncEventApplyResult(result.Applied, result.Message, result.ErrorCode, Terminal: result.TerminalConflict);
    }

    private static CitySyncPayload ReadPayload(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);
        if (!document.RootElement.TryGetProperty("payload", out var payloadElement))
        {
            throw new JsonException("Payload SyncOutbox no contiene nodo payload.");
        }

        return payloadElement.Deserialize<CitySyncPayload>(JsonOptions)
            ?? throw new JsonException("Payload Cities no pudo ser deserializado.");
    }
}
