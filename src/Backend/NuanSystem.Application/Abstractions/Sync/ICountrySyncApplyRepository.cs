using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ICountrySyncApplyRepository
{
    Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        Guid globalId,
        CancellationToken cancellationToken = default);

    Task<CountrySyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CountrySyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<CountrySyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CountrySyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record CountrySyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    int? CountryId,
    string Message);
