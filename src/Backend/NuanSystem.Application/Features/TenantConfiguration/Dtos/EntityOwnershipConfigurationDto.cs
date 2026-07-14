using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.TenantConfiguration.Dtos;

public sealed record EntityOwnershipConfigurationDto(
    string EntityName,
    EntitySourceOfTruth SourceOfTruth,
    EntitySyncDirection SyncDirection,
    bool IsEnabled,
    DateTime? CreatedAt,
    DateTime? UpdatedAt);

