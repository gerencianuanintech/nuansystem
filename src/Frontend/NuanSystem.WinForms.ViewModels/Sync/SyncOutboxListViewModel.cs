using NuanSystem.Shared.Sync;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Sync;

public sealed class SyncOutboxListViewModel(ISyncMonitorClient client) : ViewModelBase
{
    private IReadOnlyCollection<SyncOutboxListItem> items = Array.Empty<SyncOutboxListItem>();
    private bool isBusy;
    private string? errorMessage;

    public SyncEventStatus? Status { get; set; }
    public string? EntityName { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public bool DeadLetterOnly { get; set; }
    public bool HasErrors { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;

    public IReadOnlyCollection<SyncOutboxListItem> Items
    {
        get => items;
        private set => SetProperty(ref items, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public string? ErrorMessage
    {
        get => errorMessage;
        private set => SetProperty(ref errorMessage, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            Items = await client.SearchOutboxAsync(CreateFilter(), cancellationToken);
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public Task<SyncOutboxDetail> GetDetailAsync(long id, CancellationToken cancellationToken = default)
    {
        return client.GetOutboxDetailAsync(id, cancellationToken);
    }

    public Task<IReadOnlyCollection<SyncOutboxTarget>> GetTargetsAsync(long id, CancellationToken cancellationToken = default)
    {
        return client.GetOutboxTargetsAsync(id, cancellationToken);
    }

    public Task<IReadOnlyCollection<SyncAuditItem>> GetAuditAsync(Guid eventId, Guid entityGlobalId, CancellationToken cancellationToken = default)
    {
        return client.SearchAuditAsync(new SyncAuditFilter
        {
            EventId = eventId,
            EntityGlobalId = entityGlobalId,
            PageSize = 200
        }, cancellationToken);
    }

    private SyncOutboxFilter CreateFilter()
    {
        return new SyncOutboxFilter
        {
            Status = Status,
            EntityName = string.IsNullOrWhiteSpace(EntityName) ? null : EntityName.Trim(),
            CreatedFrom = CreatedFrom,
            CreatedTo = CreatedTo,
            DeadLetterOnly = DeadLetterOnly ? true : null,
            HasErrors = HasErrors ? true : null,
            Page = Page,
            PageSize = PageSize
        };
    }
}
