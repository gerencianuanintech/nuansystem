using System.Text.Json;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Features.SapSync.Cities.Services;

public sealed class SapCityExecutionRetryProcessor(SapCityRecordProcessor recordProcessor)
    : ISapSyncExecutionRetryProcessor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public string ApprovedSnapshotType => SapSyncApprovedSnapshotTypes.CityV1;

    public async Task<SapSyncExecutionRetryProcessResult> ProcessAsync(
        SapSyncExecutionDetailClaim claim, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(claim.ApprovedSnapshotType, ApprovedSnapshotType, StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(claim.ApprovedSnapshotJson))
            return Invalid("El snapshot de ciudad no corresponde al tipo aprobado.");
        SapCitySnapshot? snapshot;
        try { snapshot = JsonSerializer.Deserialize<SapCitySnapshot>(claim.ApprovedSnapshotJson, JsonOptions); }
        catch (JsonException) { return Invalid("El snapshot de ciudad no tiene un formato valido."); }
        if (snapshot is null || !string.Equals(snapshot.ExternalCode, claim.SourceRecordKey,
                StringComparison.OrdinalIgnoreCase))
            return Invalid("La identidad del snapshot no coincide con el detalle reclamado.");
        var result = await recordProcessor.ProcessAsync(snapshot, null, "SAP Sync Retry", cancellationToken);
        return new(result.Action, result.Status, result.LocalCityId, result.LocalGlobalId,
            result.ResultCode, result.SafeMessage);
    }

    private static SapSyncExecutionRetryProcessResult Invalid(string message) =>
        new(SapSyncExecutionDetailActions.Skip, SapSyncExecutionDetailStatuses.Conflict,
            null, null, SapCityResultCodes.SnapshotInvalid, message);
}
