using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class CarrierSyncEventApplier(ICarrierSyncApplyRepository repository) : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public bool CanApply(string entityName) =>
        string.Equals(entityName, SyncMasterBranchEntityCodes.Carrier, StringComparison.OrdinalIgnoreCase);

    public async Task<SyncEventApplyResult> ApplyAsync(
        SyncEventApplyContext context,
        CancellationToken cancellationToken = default)
    {
        if (context.TargetCompanyId is null)
        {
            return new(false, "Transportistas requiere sucursal destino.", "SYNC_TARGET_REQUIRED", Terminal: true);
        }

        CarrierSyncPayloadV1 payload;
        try
        {
            payload = ReadPayload(context.PayloadJson);
        }
        catch (JsonException)
        {
            return new(false, "Payload Transportistas no es JSON valido.", "SYNC_PAYLOAD_INVALID", Terminal: true);
        }

        if (payload.GlobalId == Guid.Empty || payload.GlobalId != context.EntityGlobalId)
        {
            return new(
                false,
                "Payload Transportistas no coincide con EntityGlobalId.",
                "SYNC_PAYLOAD_GLOBAL_ID_MISMATCH",
                Terminal: true);
        }

        if (!Enum.TryParse<SyncOperation>(context.Operation, ignoreCase: true, out var operation) ||
            !Enum.IsDefined(operation))
        {
            return new(false, "Operacion Transportistas no permitida.", "SYNC_OPERATION_INVALID", Terminal: true);
        }

        if (!IsValid(payload, out var validationMessage))
        {
            return new(false, validationMessage, "SYNC_CARRIER_PAYLOAD_INVALID", Terminal: true);
        }

        var result = await repository.ApplyAsync(
            context.TargetCompanyId.Value,
            context,
            payload,
            operation,
            cancellationToken);

        return new(
            result.Applied,
            result.Message,
            result.ErrorCode,
            Terminal: result.TerminalConflict);
    }

    private static CarrierSyncPayloadV1 ReadPayload(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);
        if (!document.RootElement.TryGetProperty("payload", out var payloadElement))
        {
            throw new JsonException("Payload SyncOutbox no contiene nodo payload.");
        }

        return payloadElement.Deserialize<CarrierSyncPayloadV1>(JsonOptions)
            ?? throw new JsonException("Payload Transportistas no pudo ser deserializado.");
    }

    private static bool IsValid(CarrierSyncPayloadV1 payload, out string message)
    {
        if (string.IsNullOrWhiteSpace(payload.Code) || payload.Code.Trim().Length > 50 ||
            string.IsNullOrWhiteSpace(payload.Name) || payload.Name.Trim().Length > 150 ||
            string.IsNullOrWhiteSpace(payload.IdentificationNumber) ||
            payload.IdentificationNumber.Trim().Length > 30 ||
            payload.Description?.Trim().Length > 500 ||
            payload.IdentificationTypeCode is not ("04" or "05" or "06"))
        {
            message = "Payload Transportistas incumple campos obligatorios, longitudes o tipo de identificacion.";
            return false;
        }

        message = string.Empty;
        return true;
    }
}
