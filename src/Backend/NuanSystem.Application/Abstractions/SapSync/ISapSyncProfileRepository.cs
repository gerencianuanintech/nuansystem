using NuanSystem.Application.Features.SapSync.Profiles;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncProfileRepository
{
    Task<SapSyncPagedResult<SapSyncProfileListItemDto>> SearchAsync(
        SapSyncProfileFilter filter,
        CancellationToken cancellationToken = default);

    Task<SapSyncProfileDetailDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SapSyncHandlerCapabilityDto>> GetHandlerCapabilitiesAsync(
        bool activeOnly,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SapSyncProfileCompanyAccessDto>> GetCompanyAccessAsync(
        int userId,
        int? companyId = null,
        CancellationToken cancellationToken = default);

    Task<SapSyncProfileWriteResult> CreateAsync(
        SapSyncProfileAggregate profile,
        CancellationToken cancellationToken = default);

    Task<SapSyncProfileWriteResult> UpdateAsync(
        SapSyncProfileAggregate profile,
        CancellationToken cancellationToken = default);

    Task<SapSyncProfileWriteResult> SetActiveAsync(
        long id,
        bool isActive,
        byte[] expectedRowVersion,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);

    Task<SapSyncProfileWriteResult> DeleteAsync(
        long id,
        byte[] expectedRowVersion,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
