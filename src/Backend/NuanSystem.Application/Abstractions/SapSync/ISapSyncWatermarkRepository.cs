using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncWatermarkRepository
{
    Task<SapSyncWatermarkDto?> GetAsync(int companyId, string entityCode, SapSyncDirection direction, CancellationToken cancellationToken = default);
    Task UpsertSuccessAsync(int companyId, string entityCode, SapSyncDirection direction, DateTime syncedAtUtc, string? lastSapKey, string? metadataJson, CancellationToken cancellationToken = default);
}
