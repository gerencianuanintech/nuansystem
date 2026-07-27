using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.SriTxtImports.Models;
using NuanSystem.WinForms.ViewModels.SriTxtImports;

namespace NuanSystem.WinForms.Forms.SriTxtImports;

public sealed partial class SriTxtImportForm : XtraForm
{
    public const string FormKey = "sri-txt-imports";
    private readonly SriTxtImportViewModel viewModel;
    private readonly Action<long> openQueue;
    private readonly bool canEnqueue;
    private readonly bool canOpenQueue;
    private bool busy;

    public SriTxtImportForm()
    {
        viewModel = null!;
        openQueue = null!;
        InitializeComponent();
    }

    public SriTxtImportForm(
        SriTxtImportViewModel viewModel,
        ApiSession session,
        Action<long> openQueue)
    {
        this.viewModel = viewModel;
        this.openQueue = openQueue;
        canEnqueue = session.HasPermission(PermissionCodes.SriTxtImportsEnqueue);
        canOpenQueue = session.HasPermission(PermissionCodes.SriDocumentsView);
        InitializeComponent();
        FormStyler.ApplyBase(this);
        ConfigureGrids();
        WireEvents();
        btnEnqueue.Visible = canEnqueue;
        btnOpenQueue.Visible = canOpenQueue;
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await RefreshAsync();
    }

    private void ConfigureGrids()
    {
        importGrid.FormKey = FormKey;
        importGrid.GridName = "Imports";
        importGrid.ShowPagination = false;
        importGrid.ShowFindPanel = false;
        importGrid.ConfigureColumns(
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportListItem.Id), Caption = "Carga", VisibleIndex = 0, Width = 75, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportListItem.OriginalFileName), Caption = "Archivo", VisibleIndex = 1, Width = 240 },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportListItem.Status), Caption = "Estado", VisibleIndex = 2, Width = 145, Format = NuanGridColumnFormat.StatusBadge },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportListItem.TotalRows), Caption = "Filas", VisibleIndex = 3, Width = 75, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportListItem.ValidRows), Caption = "Válidas", VisibleIndex = 4, Width = 75, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportListItem.InvalidRows), Caption = "Inválidas", VisibleIndex = 5, Width = 75, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportListItem.StagedRows), Caption = "Preparadas", VisibleIndex = 6, Width = 90, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportListItem.PendingRows), Caption = "Pendientes", VisibleIndex = 7, Width = 90, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportListItem.CreatedAt), Caption = "Creada UTC", VisibleIndex = 8, Width = 145, Format = NuanGridColumnFormat.DateTime });

        rowGrid.FormKey = FormKey;
        rowGrid.GridName = "Rows";
        rowGrid.ShowPagination = false;
        rowGrid.ShowFindPanel = false;
        rowGrid.ConfigureColumns(
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.LineNumber), Caption = "Línea", VisibleIndex = 0, Width = 70, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.MaskedAccessKey), Caption = "Clave protegida", VisibleIndex = 1, Width = 145 },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.DocumentTypeName), Caption = "Documento", VisibleIndex = 2, Width = 120 },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.IssuerLegalName), Caption = "Emisor", VisibleIndex = 3, Width = 220 },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.DocumentSeries), Caption = "Serie", VisibleIndex = 4, Width = 145 },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.Environment), Caption = "Ambiente", VisibleIndex = 5, Width = 95 },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.TotalAmount), Caption = "Total", VisibleIndex = 6, Width = 100, Format = NuanGridColumnFormat.Money },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.ValidationStatus), Caption = "Validación", VisibleIndex = 7, Width = 110, Format = NuanGridColumnFormat.StatusBadge },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.QueueStatusDisplayName), Caption = "Cola", VisibleIndex = 8, Width = 145, Format = NuanGridColumnFormat.StatusBadge },
            new NuanGridColumnDefinition { FieldName = nameof(SriTxtImportRow.QueueId), Caption = "QueueId", VisibleIndex = 9, Width = 85, Format = NuanGridColumnFormat.Number });
    }

    private void WireEvents()
    {
        btnRefresh.Click += async (_, _) => await RefreshAsync();
        btnClear.Click += async (_, _) => await ClearAsync();
        btnImportPrevious.Click += async (_, _) => await MoveImportPageAsync(-1);
        btnImportNext.Click += async (_, _) => await MoveImportPageAsync(1);
        btnRowPrevious.Click += async (_, _) => await MoveRowPageAsync(-1);
        btnRowNext.Click += async (_, _) => await MoveRowPageAsync(1);
        importGrid.FocusedRowChanged += async (_, _) => await LoadSelectedAsync();
        rowGrid.FocusedRowChanged += (_, _) => UpdateActionState();
        cmbValidity.SelectedIndexChanged += async (_, _) => await ChangeValidityAsync();
        btnEnqueue.Click += async (_, _) => await EnqueueAsync();
        btnOpenQueue.Click += (_, _) => OpenSelectedQueue();
    }

    private async Task RefreshAsync()
    {
        if (busy)
            return;

        ApplyFilters();
        await RunBusyAsync(async () =>
        {
            await viewModel.LoadAsync();
            RenderPage();
            await SelectFirstVisibleAsync();
        });
    }

    private async Task ClearAsync()
    {
        dateFrom.EditValue = null;
        dateTo.EditValue = null;
        cmbStatus.EditValue = null;
        cmbEnvironment.EditValue = null;
        txtFileName.Text = string.Empty;
        cmbValidity.EditValue = "All";
        viewModel.ResetPaging();
        await RefreshAsync();
    }

    private void ApplyFilters()
    {
        viewModel.Filter.CreatedFrom = dateFrom.EditValue as DateTime?;
        viewModel.Filter.CreatedTo = (dateTo.EditValue as DateTime?)?.Date.AddDays(1).AddTicks(-1);
        viewModel.Filter.Status = Convert.ToString(cmbStatus.EditValue);
        viewModel.Filter.Environment = Convert.ToString(cmbEnvironment.EditValue);
        viewModel.Filter.FileName = string.IsNullOrWhiteSpace(txtFileName.Text) ? null : txtFileName.Text.Trim();
    }

    private async Task LoadSelectedAsync()
    {
        if (busy || importGrid.GetFocusedRow<SriTxtImportListItem>() is not { } selected)
            return;

        await RunBusyAsync(async () =>
        {
            await viewModel.SelectAsync(selected);
            RenderDetailAndRows();
        });
    }

    private async Task ChangeValidityAsync()
    {
        if (busy || viewModel.SelectedImport is null)
            return;

        viewModel.RowValidity = Convert.ToString(cmbValidity.EditValue) ?? "All";
        viewModel.ResetRowPaging();
        await RunBusyAsync(async () =>
        {
            await viewModel.LoadSelectedAsync();
            RenderDetailAndRows();
        });
    }

    private async Task MoveImportPageAsync(int delta)
    {
        if (busy || (delta < 0 && !viewModel.CanMoveImportPrevious) || (delta > 0 && !viewModel.CanMoveImportNext))
            return;

        await RunBusyAsync(async () =>
        {
            await viewModel.MoveImportPageAsync(delta);
            RenderPage();
            await SelectFirstVisibleAsync();
        });
    }

    private async Task MoveRowPageAsync(int delta)
    {
        if (busy || (delta < 0 && !viewModel.CanMoveRowsPrevious) || (delta > 0 && !viewModel.CanMoveRowsNext))
            return;

        await RunBusyAsync(async () =>
        {
            await viewModel.MoveRowPageAsync(delta);
            RenderDetailAndRows();
        });
    }

    private async Task EnqueueAsync()
    {
        if (!canEnqueue || viewModel.Detail is not { StagedRows: > 0 } detail)
            return;

        var confirmation = XtraMessageBox.Show(
            this,
            $"¿Encolar los {detail.StagedRows:N0} documentos preparados de esta importación?",
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirmation != DialogResult.Yes)
            return;

        await RunBusyAsync(async () =>
        {
            await viewModel.EnqueueAsync();
            RenderPage();
            RenderDetailAndRows();
        });
    }

    private void OpenSelectedQueue()
    {
        if (!canOpenQueue || rowGrid.GetFocusedRow<SriTxtImportRow>()?.QueueId is not long queueId)
            return;
        openQueue(queueId);
    }

    private void RenderPage()
    {
        importGrid.SetData(viewModel.Page.Items.ToList());
        cardTotal.ValueText = viewModel.Page.Summary.TotalRows.ToString("N0");
        cardValid.ValueText = viewModel.Page.Summary.ValidRows.ToString("N0");
        cardInvalid.ValueText = viewModel.Page.Summary.InvalidRows.ToString("N0");
        cardLinked.ValueText = viewModel.Page.Summary.LinkedRows.ToString("N0");
        cardStaged.ValueText = viewModel.Page.Summary.StagedRows.ToString("N0");
        cardPending.ValueText = viewModel.Page.Summary.PendingRows.ToString("N0");
        lblImportPage.Text = PageText(viewModel.Filter.Page, viewModel.Filter.PageSize, viewModel.Page.TotalCount);
        btnImportPrevious.Enabled = viewModel.CanMoveImportPrevious;
        btnImportNext.Enabled = viewModel.CanMoveImportNext;
        if (viewModel.Page.Items.Count == 0)
        {
            lblDetail.Text = "No existen importaciones para los filtros seleccionados.";
            rowGrid.SetData(Array.Empty<SriTxtImportRow>());
        }
        UpdateActionState();
    }

    private void RenderDetailAndRows()
    {
        var detail = viewModel.Detail;
        lblDetail.Text = detail is null
            ? "Seleccione una importación."
            : $"Carga {detail.Id} | {detail.OriginalFileName} | {detail.EncodingCode} | {detail.Status} | "
              + $"{detail.FileSizeBytes:N0} bytes | SHA-256: {detail.FileSha256Hex}";
        rowGrid.SetData(viewModel.Rows.Items.ToList());
        lblRowPage.Text = PageText(viewModel.RowPage, viewModel.RowPageSize, viewModel.Rows.TotalCount);
        btnRowPrevious.Enabled = viewModel.CanMoveRowsPrevious;
        btnRowNext.Enabled = viewModel.CanMoveRowsNext;
        UpdateActionState();
    }

    private async Task SelectFirstVisibleAsync()
    {
        if (viewModel.Page.Items.FirstOrDefault() is not { } first)
            return;
        await viewModel.SelectAsync(first);
        RenderDetailAndRows();
    }

    private void UpdateActionState()
    {
        btnEnqueue.Enabled = !busy && canEnqueue && viewModel.Detail is { StagedRows: > 0 };
        btnOpenQueue.Enabled = !busy
            && canOpenQueue
            && rowGrid.GetFocusedRow<SriTxtImportRow>()?.QueueId is not null;
    }

    private async Task RunBusyAsync(Func<Task> action)
    {
        busy = true;
        btnRefresh.Enabled = false;
        btnClear.Enabled = false;
        UpdateActionState();
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, action);
        }
        finally
        {
            busy = false;
            btnRefresh.Enabled = true;
            btnClear.Enabled = true;
            UpdateActionState();
        }
    }

    private static string PageText(int page, int pageSize, int totalCount)
    {
        var pageCount = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        return $"Página {page:N0} de {pageCount:N0} | {totalCount:N0} registros";
    }
}
