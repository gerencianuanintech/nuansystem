using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class CountrySyncEventApplier(
    ICountrySyncApplyRepository repository) : ISyncEntityEventApplier
{
    private const string EntityName = SyncMasterBranchEntityCodes.Countries;

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
            return new SyncEventApplyResult(false, "Countries requiere sucursal destino.", "SYNC_TARGET_REQUIRED", Terminal: true);
        }

        CountrySyncPayload payload;
        try
        {
            payload = ReadPayload(context.PayloadJson);
        }
        catch (JsonException)
        {
            return new SyncEventApplyResult(false, "Payload Countries no es JSON valido.", "SYNC_PAYLOAD_INVALID", Terminal: true);
        }

        if (payload.GlobalId == Guid.Empty || payload.GlobalId != context.EntityGlobalId)
        {
            return new SyncEventApplyResult(false, "Payload Countries no coincide con EntityGlobalId.", "SYNC_PAYLOAD_GLOBAL_ID_MISMATCH", Terminal: true);
        }

        if (!Enum.TryParse<SyncOperation>(context.Operation, ignoreCase: true, out var operation) || !Enum.IsDefined(operation))
        {
            return new SyncEventApplyResult(false, "Operacion Countries no permitida.", "SYNC_OPERATION_INVALID", Terminal: true);
        }

        if (string.IsNullOrWhiteSpace(payload.Code) || payload.Code.Trim().Length > 10 ||
            string.IsNullOrWhiteSpace(payload.Name) || payload.Name.Trim().Length > 120 ||
            payload.Iso2?.Trim().Length > 2 || payload.Iso3?.Trim().Length > 3 ||
            payload.PhonePrefix?.Trim().Length > 10 || payload.ExternalSystem?.Trim().Length > 50 ||
            payload.ExternalCode?.Trim().Length > 100)
        {
            return new SyncEventApplyResult(false, "Payload Countries incumple campos obligatorios o longitudes.", "SYNC_COUNTRY_PAYLOAD_INVALID", Terminal: true);
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

    private static CountrySyncPayload ReadPayload(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);
        if (!document.RootElement.TryGetProperty("payload", out var payloadElement))
        {
            throw new JsonException("Payload SyncOutbox no contiene nodo payload.");
        }

        return payloadElement.Deserialize<CountrySyncPayload>(JsonOptions)
            ?? throw new JsonException("Payload Countries no pudo ser deserializado.");
    }
}
