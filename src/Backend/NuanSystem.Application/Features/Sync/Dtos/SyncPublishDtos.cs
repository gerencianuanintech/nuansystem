using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record SyncPublishRequest(
    int CompanyId,
    string EntityName,
    Guid EntityGlobalId,
    string? EntityCode,
    SyncOperation Operation,
    object Payload,
    string? SourceSystem,
    string? SourceReference,
    int? SyncProfileId = null,
    string? CorrelationId = null);

public sealed record SyncPublishResult(
    bool Published,
    long? OutboxId,
    string Reason);

public sealed record ReplicableEntityMetadata(
    int CompanyId,
    bool IsMaster,
    bool SyncEnabled,
    string EntityName,
    bool IsConfigured,
    bool IsEnabled,
    SyncDirection? Direction);
