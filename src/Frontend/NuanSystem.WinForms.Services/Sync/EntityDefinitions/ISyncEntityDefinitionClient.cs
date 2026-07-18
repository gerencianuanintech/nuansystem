using NuanSystem.WinForms.Services.Sync.EntityDefinitions.Models;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.Services.Audit.Models;

namespace NuanSystem.WinForms.Services.Sync.EntityDefinitions;

public interface ISyncEntityDefinitionClient
{
    Task<PagedResult<SyncEntityDefinitionListItem>> SearchAsync(
        SyncEntityDefinitionListFilter filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncEntityDefinitionLookupItem>> GetLookupAsync(
        int? includeId = null,
        CancellationToken cancellationToken = default);

    Task<SyncEntityDefinitionDetail> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityChangeItem>> GetHistoryAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<SyncEntityDefinitionDetail> CreateAsync(
        CreateSyncEntityDefinitionRequest request,
        CancellationToken cancellationToken = default);

    Task<SyncEntityDefinitionDetail> UpdateAsync(
        int id,
        UpdateSyncEntityDefinitionRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public sealed class SyncEntityDefinitionListFilter
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
