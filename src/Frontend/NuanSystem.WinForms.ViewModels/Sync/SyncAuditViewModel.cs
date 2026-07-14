using NuanSystem.Shared.Sync;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Sync;

public sealed class SyncAuditViewModel(ISyncMonitorClient client) : ViewModelBase
{
    private IReadOnlyCollection<SyncAuditItem> items = Array.Empty<SyncAuditItem>();
    private bool isBusy;

    public SyncEventStatus? Status { get; set; }
    public string? EntityName { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public bool HasErrors { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;

    public IReadOnlyCollection<SyncAuditItem> Items
    {
        get => items;
        private set => SetProperty(ref items, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            Items = await client.SearchAuditAsync(CreateFilter(), cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private SyncAuditFilter CreateFilter()
    {
        return new SyncAuditFilter
        {
            Status = Status,
            EntityName = string.IsNullOrWhiteSpace(EntityName) ? null : EntityName.Trim(),
            CreatedFrom = CreatedFrom,
            CreatedTo = CreatedTo,
            HasErrors = HasErrors ? true : null,
            Page = Page,
            PageSize = PageSize
        };
    }
}
