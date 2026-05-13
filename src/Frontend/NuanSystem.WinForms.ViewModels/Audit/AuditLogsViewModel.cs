using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Audit;

public sealed class AuditLogsViewModel : ViewModelBase
{
    private readonly IAuditClient auditClient;
    private IReadOnlyCollection<AuditLogItem> logs = Array.Empty<AuditLogItem>();
    private bool isBusy;

    public AuditLogsViewModel(IAuditClient auditClient)
    {
        this.auditClient = auditClient;
    }

    public IReadOnlyCollection<AuditLogItem> Logs
    {
        get => logs;
        private set => SetProperty(ref logs, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public async Task LoadAsync(int take = 200, CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            Logs = await auditClient.GetLogsAsync(take, cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
