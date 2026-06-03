using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncWatermarkService(ISapSyncWatermarkRepository repository) : ISapSyncWatermarkService
{
    public Task<SapSyncWatermarkDto?> GetAsync(int companyId, string entityCode, SapSyncDirection direction, CancellationToken cancellationToken = default)
        => repository.GetAsync(companyId, entityCode, direction, cancellationToken);

    public Task UpsertSuccessAsync(int companyId, string entityCode, SapSyncDirection direction, DateTime syncedAtUtc, string? lastSapKey, string? metadataJson, CancellationToken cancellationToken = default)
        => repository.UpsertSuccessAsync(companyId, entityCode, direction, syncedAtUtc, lastSapKey, metadataJson, cancellationToken);
}
