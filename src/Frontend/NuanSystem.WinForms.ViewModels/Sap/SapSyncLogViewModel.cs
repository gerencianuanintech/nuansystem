using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sap.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Sap;

public sealed class SapSyncLogViewModel : ViewModelBase
{
    private readonly ISapClient sapClient;
    private IReadOnlyCollection<SapSyncLogItem> logs = Array.Empty<SapSyncLogItem>();
    private bool isBusy;

    public SapSyncLogViewModel(ISapClient sapClient)
    {
        this.sapClient = sapClient;
    }

    public IReadOnlyCollection<SapSyncLogItem> Logs
    {
        get => logs;
        private set => SetProperty(ref logs, value);
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
            Logs = await sapClient.GetSyncLogsAsync(cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
