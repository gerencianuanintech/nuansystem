using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Sync;

public sealed class SyncMonitorViewModel(ISyncMonitorClient client) : ViewModelBase
{
    private SyncDashboard? dashboard;
    private SyncSummary? summary;
    private bool isBusy;
    private string? errorMessage;

    public SyncDashboard? Dashboard
    {
        get => dashboard;
        private set => SetProperty(ref dashboard, value);
    }

    public SyncSummary? Summary
    {
        get => summary;
        private set => SetProperty(ref summary, value);
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
            Dashboard = await client.GetDashboardAsync(10, cancellationToken);
            Summary = await client.GetSummaryAsync(cancellationToken);
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
}
