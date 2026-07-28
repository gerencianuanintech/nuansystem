using DevExpress.XtraEditors;
using System.ComponentModel;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.SriTxtImports.Models;
using NuanSystem.WinForms.ViewModels.SriTxtImports;

namespace NuanSystem.WinForms.Forms.SriTxtImports;

public sealed partial class SriTxtImportForm : BaseCrudListForm
{
    public const string FormKey = "sri-txt-imports";
    private const long MaxUploadSizeBytes = 10L * 1024L * 1024L;
    private SriTxtImportViewModel? viewModel;
    private ApiSession? session;
    private Action<long>? openQueue;
    private readonly bool canUpload;
    private readonly bool canEnqueue;
    private readonly bool canOpenQueue;
    private bool busy;

    public SriTxtImportForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SriTxtImportForm(
        SriTxtImportViewModel viewModel,
        ApiSession session,
        Action<long> openQueue)
        : this()
    {
        this.viewModel = viewModel;
        this.session = session;
        this.openQueue = openQueue;
        canUpload = session.HasPermission(PermissionCodes.SriTxtImportsUpload);
        canEnqueue = session.HasPermission(PermissionCodes.SriTxtImportsEnqueue);
        canOpenQueue = session.HasPermission(PermissionCodes.SriDocumentsView);
        ConfigureGrids();
        WireEvents();
    }

    private SriTxtImportViewModel ViewModel =>
        viewModel ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyección de dependencias.");

    private ApiSession Session =>
        session ?? throw new InvalidOperationException("La sesión no está configurada.");

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
        btnImportPrevious.Click += async (_, _) => await MoveImportPageAsync(-1);
        btnImportNext.Click += async (_, _) => await MoveImportPageAsync(1);
        btnRowPrevious.Click += async (_, _) => await MoveRowPageAsync(-1);
        btnRowNext.Click += async (_, _) => await MoveRowPageAsync(1);
        importGrid.FocusedRowChanged += async (_, _) => await LoadSelectedAsync();
        importGrid.GridView.DoubleClick += async (_, _) => await ExecuteConsultAsync();
        rowGrid.GridView.DoubleClick += (_, _) => OpenSelectedQueue();
    }

    protected override async Task LoadDataAsync()
    {
        if (busy || IsInDesignMode() || viewModel is null)
            return;

        await RunBusyAsync(async () =>
        {
            await ViewModel.LoadAsync();
            RenderPage();
            await SelectFirstVisibleAsync();
        });
    }

    protected override async Task ConsultAsync()
    {
        if (importGrid.GetFocusedRow<SriTxtImportListItem>() is not { } selected)
        {
            ShowWarning("Seleccione una importación.");
            return;
        }

        await RunBusyAsync(async () =>
        {
            await ViewModel.SelectAsync(selected);
            RenderDetailAndRows();
        });
    }

    private async Task LoadSelectedAsync()
    {
        if (busy || importGrid.GetFocusedRow<SriTxtImportListItem>() is not { } selected)
            return;

        await RunBusyAsync(async () =>
        {
            await ViewModel.SelectAsync(selected);
            RenderDetailAndRows();
        });
    }

    private async Task OpenFiltersAsync()
    {
        if (busy)
            return;

        using var dialog = new SriTxtImportFilterDialog(ViewModel.Filter, ViewModel.RowValidity);
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        ViewModel.Filter.CreatedFrom = dialog.CreatedFrom;
        ViewModel.Filter.CreatedTo = dialog.CreatedTo?.Date.AddDays(1).AddTicks(-1);
        ViewModel.Filter.Status = dialog.Status;
        ViewModel.Filter.Environment = dialog.EnvironmentCode;
        ViewModel.Filter.FileName = dialog.FileNameFilter;
        ViewModel.RowValidity = dialog.RowValidity;
        ViewModel.ResetPaging();
        await LoadDataAsync();
    }

    private async Task UploadAsync()
    {
        if (busy || !canUpload)
            return;

        using var dialog = new OpenFileDialog
        {
            AddExtension = true,
            CheckFileExists = true,
            CheckPathExists = true,
            DefaultExt = "txt",
            Filter = "Archivos TXT (*.txt)|*.txt",
            Multiselect = false,
            RestoreDirectory = true,
            Title = "Seleccionar archivo TXT del SRI"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        var file = new FileInfo(dialog.FileName);
        if (file.Length <= 0)
        {
            ShowWarning("El archivo seleccionado está vacío.");
            return;
        }

        if (file.Length > MaxUploadSizeBytes)
        {
            ShowWarning("El archivo seleccionado supera el límite de 10 MiB.");
            return;
        }

        await RunBusyAsync(async () =>
        {
            await using var stream = new FileStream(
                file.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true);
            await ViewModel.UploadAsync(stream, file.Name);
            RenderPage();
            RenderDetailAndRows();
            ShowSuccess($"El archivo {file.Name} fue cargado y quedó preparado para revisión.");
        });
    }

    private async Task MoveImportPageAsync(int delta)
    {
        if (busy || (delta < 0 && !ViewModel.CanMoveImportPrevious) || (delta > 0 && !ViewModel.CanMoveImportNext))
            return;

        await RunBusyAsync(async () =>
        {
            await ViewModel.MoveImportPageAsync(delta);
            RenderPage();
            await SelectFirstVisibleAsync();
        });
    }

    private async Task MoveRowPageAsync(int delta)
    {
        if (busy || (delta < 0 && !ViewModel.CanMoveRowsPrevious) || (delta > 0 && !ViewModel.CanMoveRowsNext))
            return;

        await RunBusyAsync(async () =>
        {
            await ViewModel.MoveRowPageAsync(delta);
            RenderDetailAndRows();
        });
    }

    private async Task EnqueueAsync()
    {
        if (!canEnqueue)
            return;

        if (ViewModel.Detail is not { StagedRows: > 0 } detail)
        {
            ShowWarning("Seleccione una importación con documentos preparados.");
            return;
        }

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
            await ViewModel.EnqueueAsync();
            RenderPage();
            RenderDetailAndRows();
        });
    }

    private void OpenSelectedQueue()
    {
        if (!canOpenQueue)
            return;

        if (rowGrid.GetFocusedRow<SriTxtImportRow>()?.QueueId is not long queueId)
        {
            ShowWarning("Seleccione una fila vinculada a la cola SRI.");
            return;
        }

        openQueue?.Invoke(queueId);
    }

    private void RenderPage()
    {
        importGrid.SetData(ViewModel.Page.Items.ToList());
        cardTotal.ValueText = ViewModel.Page.Summary.TotalRows.ToString("N0");
        cardValid.ValueText = ViewModel.Page.Summary.ValidRows.ToString("N0");
        cardInvalid.ValueText = ViewModel.Page.Summary.InvalidRows.ToString("N0");
        cardLinked.ValueText = ViewModel.Page.Summary.LinkedRows.ToString("N0");
        cardStaged.ValueText = ViewModel.Page.Summary.StagedRows.ToString("N0");
        cardPending.ValueText = ViewModel.Page.Summary.PendingRows.ToString("N0");
        lblImportPage.Text = PageText(ViewModel.Filter.Page, ViewModel.Filter.PageSize, ViewModel.Page.TotalCount);
        btnImportPrevious.Enabled = ViewModel.CanMoveImportPrevious;
        btnImportNext.Enabled = ViewModel.CanMoveImportNext;
        if (ViewModel.Page.Items.Count == 0)
        {
            lblDetail.Text = "No existen importaciones para los filtros seleccionados.";
            rowGrid.SetData(Array.Empty<SriTxtImportRow>());
        }
    }

    private void RenderDetailAndRows()
    {
        var detail = ViewModel.Detail;
        lblDetail.Text = detail is null
            ? "Seleccione una importación."
            : $"Carga {detail.Id} | {detail.OriginalFileName} | {detail.EncodingCode} | {detail.Status} | "
              + $"{detail.FileSizeBytes:N0} bytes | SHA-256: {detail.FileSha256Hex}";
        rowGrid.SetData(ViewModel.Rows.Items.ToList());
        lblRowPage.Text = PageText(ViewModel.RowPage, ViewModel.RowPageSize, ViewModel.Rows.TotalCount);
        btnRowPrevious.Enabled = ViewModel.CanMoveRowsPrevious;
        btnRowNext.Enabled = ViewModel.CanMoveRowsNext;
    }

    private async Task SelectFirstVisibleAsync()
    {
        if (ViewModel.Page.Items.FirstOrDefault() is not { } first)
            return;
        await ViewModel.SelectAsync(first);
        RenderDetailAndRows();
    }

    public override bool CanExecuteCustomOperation(string operationKey)
    {
        return IsCustomOperation(operationKey, "upload", "load", "cargar")
            ? !busy && canUpload
            : IsCustomOperation(operationKey, "filter", "filters", "filtro", "filtros")
                ? !busy && Session.HasPermission(PermissionCodes.SriTxtImportsView)
                : IsCustomOperation(operationKey, "enqueue", "encolar")
                    ? !busy && canEnqueue
                    : IsCustomOperation(operationKey, "openqueue", "open-queue", "abrircola")
                        ? !busy && canOpenQueue
                        : base.CanExecuteCustomOperation(operationKey);
    }

    public override Task ExecuteCustomOperationAsync(string operationKey)
    {
        if (IsCustomOperation(operationKey, "upload", "load", "cargar"))
            return UploadAsync();

        if (IsCustomOperation(operationKey, "filter", "filters", "filtro", "filtros"))
            return OpenFiltersAsync();

        if (IsCustomOperation(operationKey, "enqueue", "encolar"))
            return EnqueueAsync();

        if (IsCustomOperation(operationKey, "openqueue", "open-queue", "abrircola"))
        {
            OpenSelectedQueue();
            return Task.CompletedTask;
        }

        return base.ExecuteCustomOperationAsync(operationKey);
    }

    private async Task RunBusyAsync(Func<Task> action)
    {
        busy = true;
        await RunWithBusyStateAsync(async () =>
        {
            try
            {
                await action();
            }
            finally
            {
                busy = false;
            }
        });
    }

    private static string PageText(int page, int pageSize, int totalCount)
    {
        var pageCount = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        return $"Página {page:N0} de {pageCount:N0} | {totalCount:N0} registros";
    }

    private static bool IsCustomOperation(string operationKey, params string[] aliases)
    {
        var normalized = NormalizeOperation(operationKey);
        return aliases.Select(NormalizeOperation)
            .Any(alias => string.Equals(normalized, alias, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeOperation(string operationKey)
    {
        return operationKey
            .Replace("ACTION.", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("SRI_TXT_IMPORTS.", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsInDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime
            || DesignMode
            || Site?.DesignMode == true;
    }
}
