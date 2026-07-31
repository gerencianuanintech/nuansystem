using System.Text.Json;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Warehouses.Contracts;

namespace NuanSystem.Application.Features.SapSync.Warehouses.Services;

public sealed class SapWarehouseExecutionRetryProcessor(
    SapWarehouseRecordProcessor recordProcessor) : ISapSyncExecutionRetryProcessor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public string ApprovedSnapshotType => SapSyncApprovedSnapshotTypes.WarehouseV1;

    public async Task<SapSyncExecutionRetryProcessResult> ProcessAsync(
        SapSyncExecutionDetailClaim claim,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(claim.ApprovedSnapshotType, ApprovedSnapshotType, StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(claim.ApprovedSnapshotJson))
        {
            return Invalid("El snapshot de bodega no corresponde al tipo aprobado.");
        }

        SapWarehouseSnapshot? snapshot;
        try
        {
            snapshot = JsonSerializer.Deserialize<SapWarehouseSnapshot>(claim.ApprovedSnapshotJson, JsonOptions);
        }
        catch (JsonException)
        {
            return Invalid("El snapshot de bodega no tiene un formato valido.");
        }

        if (snapshot is null
            || !string.Equals(Normalize(snapshot.WarehouseCode), Normalize(claim.SourceRecordKey),
                StringComparison.OrdinalIgnoreCase))
        {
            return Invalid("La identidad del snapshot no coincide con el detalle reclamado.");
        }

        var result = await recordProcessor.ProcessAsync(
            snapshot, null, "SAP Sync Retry", cancellationToken);
        return new(
            result.Action,
            result.Status,
            result.LocalWarehouseId,
            result.LocalGlobalId,
            result.ResultCode,
            result.SafeMessage);
    }

    private static SapSyncExecutionRetryProcessResult Invalid(string message) =>
        new(
            SapSyncExecutionDetailActions.Skip,
            SapSyncExecutionDetailStatuses.Conflict,
            null,
            null,
            SapWarehouseResultCodes.SnapshotInvalid,
            message);

    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;
}
