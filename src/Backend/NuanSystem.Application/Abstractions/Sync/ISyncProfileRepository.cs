using NuanSystem.Application.Features.Sync.Configuration.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncProfileRepository
{
    Task<PagedResultDto<SyncProfileListItemDto>> SearchAsync(
        SyncProfileListFilter filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncProfileSummaryDto>> ListAsync(
        int? companyId,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<SyncProfileDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<SyncProfileDetailDto?> GetByCodeAsync(
        int companyId,
        string code,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(SyncProfileAggregate profile, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(SyncProfileAggregate profile, CancellationToken cancellationToken = default);

    Task<bool> SetActiveAsync(
        int id,
        bool isActive,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default);

    Task<bool> HasOperationalHistoryAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncCompanyLookupRecord>> GetCompanyLookupsAsync(
        int? userId,
        CancellationToken cancellationToken = default);

    Task RecordAuditAsync(
        int? profileId,
        string action,
        string? fieldName,
        string? oldValue,
        string? newValue,
        int? userId,
        string? userName,
        CancellationToken cancellationToken = default);
}
