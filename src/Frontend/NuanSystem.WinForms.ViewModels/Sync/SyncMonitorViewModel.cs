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
    private IReadOnlyCollection<BusinessPartnerSyncConflict> businessPartnerConflicts = [];
    private IReadOnlyCollection<BusinessPartnerSyncConflictGridRow> businessPartnerConflictRows = [];

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

    public IReadOnlyCollection<BusinessPartnerSyncConflict> BusinessPartnerConflicts
    {
        get => businessPartnerConflicts;
        private set
        {
            if (SetProperty(ref businessPartnerConflicts, value))
            {
                BusinessPartnerConflictRows = FlattenConflicts(value);
            }
        }
    }

    public IReadOnlyCollection<BusinessPartnerSyncConflictGridRow> BusinessPartnerConflictRows
    {
        get => businessPartnerConflictRows;
        private set => SetProperty(ref businessPartnerConflictRows, value);
    }

    private static IReadOnlyCollection<BusinessPartnerSyncConflictGridRow> FlattenConflicts(
        IReadOnlyCollection<BusinessPartnerSyncConflict> conflicts) =>
        conflicts
            .SelectMany(conflict => conflict.Differences.Select(difference => new BusinessPartnerSyncConflictGridRow(
                conflict.Id,
                conflict.BusinessPartnerGlobalId,
                string.IsNullOrWhiteSpace(conflict.Code)
                    ? conflict.Name ?? conflict.BusinessPartnerGlobalId.ToString("D")
                    : $"{conflict.Code} - {conflict.Name}",
                conflict.OriginCompanyId,
                conflict.BaseCanonicalVersion,
                conflict.CurrentCanonicalVersion,
                difference.FieldPath,
                difference.ProposedValue,
                difference.CentralValue,
                conflict.CreatedAt,
                conflict.Status,
                conflict.RowVersion)))
            .ToArray();

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

    public async Task LoadBusinessPartnerConflictsAsync(CancellationToken cancellationToken = default)
    {
        BusinessPartnerConflicts = await client.GetBusinessPartnerConflictsAsync("Open", cancellationToken);
    }

    public async Task ResolveBusinessPartnerConflictAsync(
        long conflictId,
        string resolution,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (resolution is not ("AcceptBranch" or "KeepCentral"))
        {
            throw new ArgumentException("La resolución del conflicto no es válida.", nameof(resolution));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("El motivo es obligatorio.", nameof(reason));
        }

        var conflict = BusinessPartnerConflicts.SingleOrDefault(item => item.Id == conflictId)
            ?? throw new InvalidOperationException("El conflicto seleccionado ya no está disponible.");

        await client.ResolveBusinessPartnerConflictAsync(
            conflictId,
            new(conflictId, resolution, reason.Trim(), conflict.RowVersion),
            cancellationToken);
        await LoadBusinessPartnerConflictsAsync(cancellationToken);
    }
}
