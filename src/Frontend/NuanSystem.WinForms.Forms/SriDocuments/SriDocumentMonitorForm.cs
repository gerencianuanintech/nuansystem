using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.SriDocuments.Models;
using NuanSystem.WinForms.ViewModels.SriDocuments;

namespace NuanSystem.WinForms.Forms.SriDocuments;

public sealed partial class SriDocumentMonitorForm : BaseCrudListForm
{
    public const string FormKey = "sri-document-monitor";
    private readonly SriDocumentMonitorViewModel viewModel;
    private readonly long? initialQueueId;
    private readonly bool canDownload;
    private bool initialQueueLoaded;
    private bool busy;

    public SriDocumentMonitorForm()
    {
        viewModel = null!;
        InitializeComponent();
    }

    public SriDocumentMonitorForm(
        SriDocumentMonitorViewModel viewModel,
        ApiSession session,
        long? initialQueueId = null)
    {
        this.viewModel = viewModel;
        this.initialQueueId = initialQueueId;
        canDownload = session.HasPermission(PermissionCodes.SriDocumentsDownloadXml);
        InitializeComponent();
        FormStyler.ApplyBase(this);
        ConfigureGrids();
        WireEvents();
    }

    private void WireEvents()
    {
        documentGrid.PageRequested += async (_,args) => await GoToDocumentPageAsync(args);
        documentGrid.FocusedRowChanged += async (_,_) => await LoadSelectedAsync();
        documentGrid.GridView.DoubleClick += async (_,_) => await ExecuteConsultAsync();
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

    protected override async Task LoadDataAsync()
    {
        if (busy || viewModel is null)
            return;

        await RunBusyAsync(async () =>
        {
            await viewModel.LoadAsync();
            RenderMonitor();
            if (!initialQueueLoaded && initialQueueId is long queueId)
            {
                initialQueueLoaded = true;
                await LoadQueueCoreAsync(queueId, direct: true);
            }
            else if (viewModel.Items.FirstOrDefault() is { } first)
            {
                await LoadQueueCoreAsync(first.QueueId, direct: false);
            }
        });
    }

    protected override async Task ConsultAsync()
    {
        if (documentGrid.GetFocusedRow<SriDocumentMonitorItem>() is not { } selected)
        {
            ShowWarning("Seleccione un documento SRI.");
            return;
        }

        await RunBusyAsync(() => LoadQueueCoreAsync(selected.QueueId, direct: false));
    }

    private void RenderMonitor()
    {
        var totalCount=viewModel.Items.FirstOrDefault()?.TotalCount ?? 0;
        documentGrid.SetPagedData(
            viewModel.Items.ToList(),
            viewModel.Filter.Page,
            viewModel.Filter.PageSize,
            ToGridTotalCount(totalCount));
        cardTotal.ValueText=(viewModel.Summary?.Total ?? 0).ToString("N0");
        cardPending.ValueText=(viewModel.Summary?.Pending ?? 0).ToString("N0");
        cardAuthorized.ValueText=(viewModel.Summary?.Authorized ?? 0).ToString("N0");
        cardErrors.ValueText=(viewModel.Summary?.Errors ?? 0).ToString("N0");
        RenderWorkerHealth();
        if (viewModel.Items.Count==0)
        {
            lblDetail.Text="No existen documentos para los filtros seleccionados.";
            attemptGrid.SetData(Array.Empty<SriDocumentAttempt>());
            auditGrid.SetData(Array.Empty<SriDocumentAudit>());
        }
    }

    private async Task GoToDocumentPageAsync(NuanGridPageRequestEventArgs args)
    {
        if (busy)
            return;

        await RunBusyAsync(async () =>
        {
            await viewModel.GoToPageAsync(args.Page,args.PageSize);
            RenderMonitor();
            if (viewModel.Items.FirstOrDefault() is { } first)
                await LoadQueueCoreAsync(first.QueueId,direct:false);
        });
    }

    private void RenderWorkerHealth()
    {
        lblWorkerHealth.Text=viewModel.WorkerHealthText;
    }

    private async Task LoadSelectedAsync()
    {
        if (busy)
            return;

        var selected=documentGrid.GetFocusedRow<SriDocumentMonitorItem>();
        if(selected is null) return;
        await RunBusyAsync(() => LoadQueueCoreAsync(selected.QueueId, direct: false));
    }

    private async Task LoadQueueCoreAsync(long queueId, bool direct)
    {
        if (direct)
            await viewModel.LoadDirectAsync(queueId);
        else
            await viewModel.LoadDetailAsync(queueId);

        var detail=viewModel.Detail;
        lblDetail.Text=detail is null
            ? "Detalle restringido por permisos."
            : $"Documento {detail.QueueId} | {detail.Status} | {detail.SourceReference} | {detail.SizeBytes:N0} bytes | SHA-256: {detail.Sha256Hex}";
        attemptGrid.SetData(viewModel.Attempts.ToList());
        auditGrid.SetData(viewModel.Audit.ToList());
    }

    private async Task DownloadAsync()
    {
        if(busy || !canDownload || !viewModel.CanDownload) return;
        using var dialog=new SaveFileDialog { Filter="XML (*.xml)|*.xml",AddExtension=true,DefaultExt="xml",FileName=$"sri-{viewModel.Selected!.QueueId}.xml",OverwritePrompt=true };
        if(dialog.ShowDialog(this)!=DialogResult.OK) return;
        await RunBusyAsync(async () =>
        {
            var file=await viewModel.DownloadAsync();
            await File.WriteAllBytesAsync(dialog.FileName,file.Content);
            XtraMessageBox.Show(this,"XML descargado correctamente.",Text,MessageBoxButtons.OK,MessageBoxIcon.Information);
            if (viewModel.Selected is { } selected)
                await LoadQueueCoreAsync(selected.QueueId, direct: false);
        });
    }

    private async Task OpenFiltersAsync()
    {
        if (busy)
            return;

        using var dialog=new SriDocumentMonitorFilterDialog(viewModel.Filter);
        if (dialog.ShowDialog(this)!=DialogResult.OK)
            return;

        viewModel.Filter.Environment=dialog.EnvironmentCode;
        viewModel.Filter.Status=dialog.Status;
        viewModel.Filter.DocumentTypeCode=dialog.DocumentTypeCode;
        viewModel.Filter.SourceType=dialog.SourceType;
        viewModel.Filter.Search=dialog.Search;
        viewModel.Filter.CreatedFrom=dialog.CreatedFrom;
        viewModel.Filter.CreatedTo=dialog.CreatedTo?.Date.AddDays(1).AddTicks(-1);
        viewModel.Filter.Page=1;
        await LoadDataAsync();
    }

    public override bool CanExecuteCustomOperation(string operationKey)
    {
        return IsCustomOperation(operationKey,"filter","filters","filtro","filtros")
            ? !busy
            : IsCustomOperation(operationKey,"downloadxml","download-xml","descargarxml")
                ? !busy && canDownload && viewModel.CanDownload
                : base.CanExecuteCustomOperation(operationKey);
    }

    public override Task ExecuteCustomOperationAsync(string operationKey)
    {
        if (IsCustomOperation(operationKey,"filter","filters","filtro","filtros"))
            return OpenFiltersAsync();

        if (IsCustomOperation(operationKey,"downloadxml","download-xml","descargarxml"))
            return DownloadAsync();

        return base.ExecuteCustomOperationAsync(operationKey);
    }

    private async Task RunBusyAsync(Func<Task> action)
    {
        busy=true;
        await RunWithBusyStateAsync(async () =>
        {
            try
            {
                await action();
            }
            finally
            {
                busy=false;
            }
        });
    }

    private static bool IsCustomOperation(string operationKey,params string[] aliases)
    {
        var normalized=NormalizeOperation(operationKey);
        return aliases.Select(NormalizeOperation)
            .Any(alias=>string.Equals(normalized,alias,StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeOperation(string operationKey)
    {
        return operationKey
            .Replace("ACTION.",string.Empty,StringComparison.OrdinalIgnoreCase)
            .Replace("SRI_DOCUMENTS.",string.Empty,StringComparison.OrdinalIgnoreCase)
            .Replace("_",string.Empty,StringComparison.OrdinalIgnoreCase)
            .Replace("-",string.Empty,StringComparison.OrdinalIgnoreCase)
            .Replace(" ",string.Empty,StringComparison.OrdinalIgnoreCase);
    }

    private static int ToGridTotalCount(long totalCount) =>
        totalCount >= int.MaxValue ? int.MaxValue : Math.Max(0,(int)totalCount);
}
