using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncEntityDefinitionRepository
{
    Task<PagedResultDto<SyncEntityDefinitionRecord>> SearchAsync(
        SyncEntityDefinitionListFilter filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncEntityDefinitionRecord>> ListAsync(
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<SyncEntityDefinitionDetailRecord?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<SyncEntityDefinitionDetailRecord?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityChangeDto>> GetHistoryAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncEntityDefinitionDetailRecord>> GetLookupAsync(
        int? includeId,
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<SyncEntityDefinitionMutationResult> CreateAsync(
        CreateSyncEntityDefinitionData definition,
        CancellationToken cancellationToken = default);

    Task<SyncEntityDefinitionMutationResult> UpdateAsync(
        UpdateSyncEntityDefinitionData definition,
        CancellationToken cancellationToken = default);

    Task<SyncEntityDefinitionMutationResult> DeleteAsync(
        int id,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
