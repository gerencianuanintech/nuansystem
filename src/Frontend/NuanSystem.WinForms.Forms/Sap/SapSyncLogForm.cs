using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.ViewModels.Sap;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSyncLogForm : BaseCrudListForm
{
    private readonly SapSyncLogViewModel viewModel;

    public SapSyncLogForm()
    {
        viewModel = null!;
        InitializeComponent();
    }

    public SapSyncLogForm(SapSyncLogViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await viewModel.LoadAsync();
            grcSapLogs.DataSource = viewModel.Logs.ToList();
            grvSapLogs.BestFitColumns();
        });
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _ = ExecuteRefreshAsync();
    }
}
