using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.SriDocuments.Models;
using NuanSystem.WinForms.ViewModels.SriDocuments;

namespace NuanSystem.WinForms.Forms.SriDocuments;

public sealed partial class SriDocumentMonitorForm : XtraForm
{
    public const string FormKey = "sri-document-monitor";
    private readonly SriDocumentMonitorViewModel viewModel;

    public SriDocumentMonitorForm()
    {
        viewModel = null!;
        InitializeComponent();
    }

    public SriDocumentMonitorForm(SriDocumentMonitorViewModel viewModel, ApiSession session)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        FormStyler.ApplyBase(this);
        btnDownload.Visible = session.HasPermission(PermissionCodes.SriDocumentsDownloadXml);
        ConfigureGrids();
        WireEvents();
    }

    protected override async void OnShown(EventArgs e) { base.OnShown(e); await RefreshAsync(); }

    private void WireEvents()
    {
        btnRefresh.Click += async (_,_) => await RefreshAsync();
        btnClear.Click += async (_,_) => { cmbEnvironment.EditValue=null; cmbStatus.EditValue=null; cmbDocumentType.EditValue=null; cmbSourceType.EditValue=null; txtSearch.Text=string.Empty; dateFrom.EditValue=null; dateTo.EditValue=null; await RefreshAsync(); };
        documentGrid.FocusedRowChanged += async (_,_) => await LoadSelectedAsync();
        btnDownload.Click += async (_,_) => await DownloadAsync();
    }

    private void ConfigureGrids()
    {
        documentGrid.FormKey=FormKey;
        documentGrid.ShowPagination=true;
        documentGrid.PageSize=50;
        documentGrid.ConfigureColumns(
            new NuanGridColumnDefinition { FieldName=nameof(SriDocumentMonitorItem.QueueId),Caption="Cola",VisibleIndex=0,Width=80,Format=NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName=nameof(SriDocumentMonitorItem.Environment),Caption="Ambiente",VisibleIndex=1,Width=100 },
            new NuanGridColumnDefinition { FieldName=nameof(SriDocumentMonitorItem.DocumentTypeCode),Caption="Tipo",VisibleIndex=2,Width=70 },
            new NuanGridColumnDefinition { FieldName=nameof(SriDocumentMonitorItem.Status),Caption="Estado",VisibleIndex=3,Width=120,Format=NuanGridColumnFormat.StatusBadge },
            new NuanGridColumnDefinition { FieldName=nameof(SriDocumentMonitorItem.SourceType),Caption="Origen",VisibleIndex=4,Width=110 },
            new NuanGridColumnDefinition { FieldName=nameof(SriDocumentMonitorItem.SourceReference),Caption="Referencia",VisibleIndex=5,Width=220 },
            new NuanGridColumnDefinition { FieldName=nameof(SriDocumentMonitorItem.CreatedAt),Caption="Creado",VisibleIndex=6,Width=145,Format=NuanGridColumnFormat.DateTime },
            new NuanGridColumnDefinition { FieldName=nameof(SriDocumentMonitorItem.AuthorizationAt),Caption="Autorizado",VisibleIndex=7,Width=145,Format=NuanGridColumnFormat.DateTime },
            new NuanGridColumnDefinition { FieldName=nameof(SriDocumentMonitorItem.AttemptCount),Caption="Intentos",VisibleIndex=8,Width=80,Format=NuanGridColumnFormat.Number });
        attemptGrid.ConfigureColumns(new NuanGridColumnDefinition { FieldName=nameof(SriDocumentAttempt.AttemptNumber),Caption="Intento",VisibleIndex=0,Width=70 },new NuanGridColumnDefinition { FieldName=nameof(SriDocumentAttempt.ResultStatus),Caption="Resultado",VisibleIndex=1,Width=120 },new NuanGridColumnDefinition { FieldName=nameof(SriDocumentAttempt.StartedAt),Caption="Inicio",VisibleIndex=2,Width=150,Format=NuanGridColumnFormat.DateTime },new NuanGridColumnDefinition { FieldName=nameof(SriDocumentAttempt.DurationMs),Caption="ms",VisibleIndex=3,Width=80 });
        auditGrid.ConfigureColumns(new NuanGridColumnDefinition { FieldName=nameof(SriDocumentAudit.Action),Caption="Accion",VisibleIndex=0,Width=120 },new NuanGridColumnDefinition { FieldName=nameof(SriDocumentAudit.NewStatus),Caption="Estado",VisibleIndex=1,Width=110 },new NuanGridColumnDefinition { FieldName=nameof(SriDocumentAudit.UserName),Caption="Usuario",VisibleIndex=2,Width=140 },new NuanGridColumnDefinition { FieldName=nameof(SriDocumentAudit.CreatedAt),Caption="Fecha UTC",VisibleIndex=3,Width=150,Format=NuanGridColumnFormat.DateTime });
    }

    private async Task RefreshAsync() => await UiExceptionHandler.RunAsync(this,Text,async () =>
    {
        viewModel.Filter.Environment=Convert.ToString(cmbEnvironment.EditValue);
        viewModel.Filter.Status=Convert.ToString(cmbStatus.EditValue);
        viewModel.Filter.DocumentTypeCode=Convert.ToString(cmbDocumentType.EditValue);
        viewModel.Filter.SourceType=Convert.ToString(cmbSourceType.EditValue);
        viewModel.Filter.Search=string.IsNullOrWhiteSpace(txtSearch.Text)?null:txtSearch.Text.Trim();
        viewModel.Filter.CreatedFrom=dateFrom.EditValue as DateTime?;
        viewModel.Filter.CreatedTo=dateTo.EditValue as DateTime?;
        await viewModel.LoadAsync();
        documentGrid.SetData(viewModel.Items.ToList());
        cardTotal.ValueText=(viewModel.Summary?.Total ?? 0).ToString("N0");
        cardPending.ValueText=(viewModel.Summary?.Pending ?? 0).ToString("N0");
        cardAuthorized.ValueText=(viewModel.Summary?.Authorized ?? 0).ToString("N0");
        cardErrors.ValueText=(viewModel.Summary?.Errors ?? 0).ToString("N0");
        RenderWorkerHealth();
    });

    private void RenderWorkerHealth()
    {
        lblWorkerHealth.Text=viewModel.WorkerHealthText;
    }

    private async Task LoadSelectedAsync()
    {
        var selected=documentGrid.GetFocusedRow<SriDocumentMonitorItem>();
        if(selected is null) return;
        await UiExceptionHandler.RunAsync(this,Text,async () =>
        {
            await viewModel.LoadDetailAsync(selected.QueueId);
            var d=viewModel.Detail;
            lblDetail.Text=d is null ? "Detalle restringido por permisos." : $"Documento {d.QueueId} | {d.Status} | {d.SourceReference} | {d.SizeBytes:N0} bytes | SHA-256: {d.Sha256Hex}";
            attemptGrid.SetData(viewModel.Attempts.ToList());
            auditGrid.SetData(viewModel.Audit.ToList());
            btnDownload.Enabled=viewModel.CanDownload;
        });
    }

    private async Task DownloadAsync()
    {
        if(!viewModel.CanDownload) return;
        using var dialog=new SaveFileDialog { Filter="XML (*.xml)|*.xml",AddExtension=true,DefaultExt="xml",FileName=$"sri-{viewModel.Selected!.QueueId}.xml",OverwritePrompt=true };
        if(dialog.ShowDialog(this)!=DialogResult.OK) return;
        await UiExceptionHandler.RunAsync(this,Text,async () =>
        {
            var file=await viewModel.DownloadAsync();
            await File.WriteAllBytesAsync(dialog.FileName,file.Content);
            XtraMessageBox.Show(this,"XML descargado correctamente.",Text,MessageBoxButtons.OK,MessageBoxIcon.Information);
            await LoadSelectedAsync();
        });
    }
}
