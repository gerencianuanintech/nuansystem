using DevExpress.XtraEditors;
using NuanSystem.WinForms.ViewModels.Audit;

namespace NuanSystem.WinForms.Forms.Audit;

public sealed partial class AuditLogsForm : XtraForm
{
    private readonly AuditLogsViewModel viewModel;

    public AuditLogsForm()
    {
        viewModel = null!;
        InitializeComponent();
        WireEvents();
    }

    public AuditLogsForm(AuditLogsViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        WireEvents();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadLogsAsync();
    }

    private void WireEvents()
    {
        Common.FormStyler.ApplyBase(this);
        btnActualizar.Click += async (_, _) => await LoadLogsAsync();
    }

    private async Task LoadLogsAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        btnActualizar.Enabled = false;
        try
        {
            await viewModel.LoadAsync((int)sedRegistros.Value);
            grcAuditoria.DataSource = viewModel.Logs.ToList();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "Auditoria", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            btnActualizar.Enabled = true;
        }
    }
}
