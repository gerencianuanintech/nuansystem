using ClosedXML.Excel;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.GridColumnSettings.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Text.Json;
using System.Xml;

namespace NuanSystem.WinForms.Forms.Common;

public partial class BaseGridCrudListForm : BaseCrudListForm
{
    private const float PdfPageWidth = 747F;
    private const float PdfLandscapePageWidth = 1089F;
    private const string ExportFontName = "Segoe UI";
    private NuanDataGridControl nuanGrid = null!;
    private PanelControl paginationPanel = null!;
    private SimpleButton firstPageButton = null!;
    private SimpleButton previousPageButton = null!;
    private LabelControl pageInfoLabel = null!;
    private SimpleButton nextPageButton = null!;
    private SimpleButton lastPageButton = null!;
    private LabelControl pageSizeLabel = null!;
    private ComboBoxEdit pageSizeCombo = null!;
    private LabelControl totalInfoLabel = null!;
    private LabelControl selectionInfoLabel = null!;
    private PanelControl auditPanel = null!;
    private LabelControl auditCreatedLabel = null!;
    private LabelControl auditUpdatedLabel = null!;
    private readonly List<object> items = new();
    private IGridColumnSettingsClient? columnSettingsClient;
    private string? columnSettingsFormKey;
    private string columnSettingsGridName = "MainGrid";
    private Func<IEnumerable<object>, object>? pageDataSourceFactory;
    private object? batchSelectedItem;
    private int currentPage = 1;
    private int pageSize = 20;

    public BaseGridCrudListForm()
    {
        InitializeComponent();
        WireGridEvents();
        UpdatePaginationInfo();
        UpdateSelectionInfo();
        UpdateAuditInfo();
    }

    private GridControl gridControl => nuanGrid.InnerGridControl;

    private GridView gridView => nuanGrid.InnerGridView;

    protected GridControl GridControl => gridControl;

    protected GridView GridView => gridView;

    protected NuanDataGridControl NuanGrid => nuanGrid;

    public void ExportVisibleColumnsToExcel(string userName, string companyName, byte[]? companyLogoImage)
    {
        var columns = GetVisibleGridColumns();

        if (columns.Length == 0)
        {
            ShowWarning("No hay columnas visibles para exportar.");
            return;
        }

        if (items.Count == 0)
        {
            ShowWarning("No hay registros para exportar.");
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "Excel (*.xlsx)|*.xlsx",
            FileName = $"{SanitizeFileName(Text)}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        using var workbook = new XLWorkbook();
        workbook.Style.Font.FontName = ExportFontName;
        workbook.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        var worksheet = workbook.Worksheets.Add(SanitizeWorksheetName(Text));
        var totalColumns = columns.Length + 1;
        var lastColumn = Math.Max(totalColumns, 7);

        BuildExcelTable(worksheet, columns, totalColumns);
        BuildExcelHeader(worksheet, lastColumn, userName, companyName, companyLogoImage);

        worksheet.SheetView.FreezeRows(5);
        workbook.SaveAs(dialog.FileName);
        ShowSuccess("Listado exportado correctamente.");
    }

    public void ExportVisibleColumnsToPdf(string userName, string companyName, byte[]? companyLogoImage)
    {
        var columns = GetVisibleGridColumns();
        if (columns.Length == 0)
        {
            ShowWarning("No hay columnas visibles para exportar.");
            return;
        }

        if (items.Count == 0)
        {
            ShowWarning("No hay registros para exportar.");
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "PDF (*.pdf)|*.pdf",
            FileName = $"{SanitizeFileName(Text)}_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        using var report = BuildPdfReport(columns, userName, companyName, companyLogoImage);
        report.ExportToPdf(dialog.FileName);
        ShowSuccess("Listado exportado correctamente.");

        try
        {
            Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
        }
        catch
        {
            // El archivo ya fue generado; si Windows no puede abrirlo, dejamos el flujo sin interrumpir.
        }
    }

    public void ExportVisibleColumnsToJson()
    {
        var columns = GetVisibleGridColumns();
        if (!CanExportRawData(columns))
        {
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "JSON (*.json)|*.json",
            FileName = $"{SanitizeFileName(Text)}_{DateTime.Now:yyyyMMdd_HHmm}.json"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var data = items
            .Select(item => columns.ToDictionary(
                column => column.FieldName,
                column => NormalizeRawExportValue(ReadValue(item, column.FieldName))))
            .ToArray();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(data, options));
        ShowSuccess("Listado exportado correctamente.");
    }

    public void ExportVisibleColumnsToXml()
    {
        var columns = GetVisibleGridColumns();
        if (!CanExportRawData(columns))
        {
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "XML (*.xml)|*.xml",
            FileName = $"{SanitizeFileName(Text)}_{DateTime.Now:yyyyMMdd_HHmm}.xml"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var settings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = System.Text.Encoding.UTF8
        };

        using var writer = XmlWriter.Create(dialog.FileName, settings);
        writer.WriteStartDocument();
        writer.WriteStartElement("Listado");

        foreach (var item in items)
        {
            writer.WriteStartElement("Registro");
            foreach (var column in columns)
            {
                writer.WriteStartElement(SanitizeXmlElementName(column.FieldName));
                WriteXmlValue(writer, NormalizeRawExportValue(ReadValue(item, column.FieldName)));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
        ShowSuccess("Listado exportado correctamente.");
    }

    protected void SetGridData<TItem>(IEnumerable<TItem> source)
        where TItem : class
    {
        items.Clear();
        items.AddRange(source);
        pageDataSourceFactory = pageItems => pageItems.Cast<TItem>().ToList();
        currentPage = 1;
        ApplyPage();
    }

    protected void EnableServerPaging(int initialPageSize = 50)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(initialPageSize, 1);
        paginationPanel.Visible = false;
        nuanGrid.ShowPagination = true;
        nuanGrid.PageSize = initialPageSize;
    }

    protected void SetPagedGridData<TItem>(
        IEnumerable<TItem> source,
        int page,
        int requestedPageSize,
        int totalCount)
        where TItem : class
    {
        var pageItems = source.ToList();
        items.Clear();
        items.AddRange(pageItems);
        pageDataSourceFactory = values => values.Cast<TItem>().ToList();
        nuanGrid.SetPagedData(pageItems, page, requestedPageSize, totalCount);
        ConfigureGridColumns();
        ConfigureSelectionColumn();
        UpdateAuditInfo();
    }

    protected TItem? SelectedGridItem<TItem>()
        where TItem : class
    {
        if (batchSelectedItem is TItem batchItem)
        {
            return batchItem;
        }

        return gridView.GetFocusedRow() as TItem;
    }

    protected IReadOnlyCollection<TItem> SelectedGridItems<TItem>()
        where TItem : class
    {
        return GetSelectedRowHandles()
            .Select(rowHandle => gridView.GetRow(rowHandle))
            .OfType<TItem>()
            .ToArray();
    }

    protected override async Task ExecuteEditCoreAsync()
    {
        var rowHandles = GetSelectedRowHandles();
        if (rowHandles.Count <= 1)
        {
            await base.ExecuteEditCoreAsync();
            return;
        }

        await ExecuteForEachSelectedItemAsync(rowHandles, EditAsync);
    }

    protected override async Task ExecuteCopyCoreAsync()
    {
        var rowHandles = GetSelectedRowHandles();
        if (rowHandles.Count <= 1)
        {
            await base.ExecuteCopyCoreAsync();
            return;
        }

        await ExecuteForEachSelectedItemAsync(rowHandles, CopyAsync);
    }

    protected override async Task ExecuteDeleteCoreAsync()
    {
        var rowHandles = GetSelectedRowHandles();
        if (rowHandles.Count <= 1)
        {
            await base.ExecuteDeleteCoreAsync();
            return;
        }

        await ExecuteForEachSelectedItemAsync(rowHandles, DeleteAsync);
    }

    protected override async Task ExecuteConsultCoreAsync()
    {
        var rowHandles = GetSelectedRowHandles();
        if (rowHandles.Count <= 1)
        {
            await base.ExecuteConsultCoreAsync();
            return;
        }

        await ExecuteForEachSelectedItemAsync(rowHandles, ConsultAsync);
    }

    protected override async Task ExecuteHistoryCoreAsync()
    {
        var rowHandles = GetSelectedRowHandles();
        if (rowHandles.Count <= 1)
        {
            await base.ExecuteHistoryCoreAsync();
            return;
        }

        await ExecuteForEachSelectedItemAsync(rowHandles, HistoryAsync);
    }

    protected virtual void ConfigureGridColumns()
    {
        HideColumn("CreatedByUserId");
        HideColumn("CreatedByUserName");
        HideColumn("CreatedAt");
        HideColumn("UpdatedByUserId");
        HideColumn("UpdatedByUserName");
        HideColumn("UpdatedAt");
        HideColumn("DeletedByUserId");
        HideColumn("DeletedByUserName");
        HideColumn("DeletedAt");
    }

    protected void ConfigureColumnPersonalization(
        IGridColumnSettingsClient? client,
        string formKey,
        string gridName = "MainGrid")
    {
        columnSettingsClient = client;
        columnSettingsFormKey = formKey;
        columnSettingsGridName = gridName;
    }

    protected async Task ApplyColumnSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (columnSettingsClient is null || string.IsNullOrWhiteSpace(columnSettingsFormKey))
        {
            return;
        }

        var settings = await columnSettingsClient.GetAsync(columnSettingsFormKey, columnSettingsGridName, cancellationToken);
        ApplyColumnSettings(settings);
    }

    protected override async Task CustomizeColumnsAsync()
    {
        if (columnSettingsClient is null || string.IsNullOrWhiteSpace(columnSettingsFormKey))
        {
            await base.CustomizeColumnsAsync();
            return;
        }

        var currentColumns = CaptureCurrentColumnSettings();
        using var form = new GridColumnSettingsForm(currentColumns);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await columnSettingsClient.SaveAsync(columnSettingsFormKey, columnSettingsGridName, form.Request);
        ApplyColumnSettings(form.Request.Select(item => new GridColumnSettingItem(
            item.FieldName,
            item.DefaultCaption,
            item.Caption,
            item.IsVisible,
            item.VisibleIndex,
            item.Width)).ToArray());
        ShowSuccess("Columnas guardadas correctamente.");
    }

    protected void HideColumn(string fieldName)
    {
        if (gridView.Columns[fieldName] is { } column)
        {
            column.Visible = false;
        }
    }

    private void WireGridEvents()
    {
        gridView.SelectionChanged += (_, _) => UpdateSelectionInfo();
        gridView.FocusedRowChanged += (_, _) =>
        {
            UpdateSelectionInfo();
            UpdateAuditInfo();
        };

        firstPageButton.Click += (_, _) => GoToPage(1);
        previousPageButton.Click += (_, _) => GoToPage(currentPage - 1);
        nextPageButton.Click += (_, _) => GoToPage(currentPage + 1);
        lastPageButton.Click += (_, _) => GoToPage(TotalPages());
        pageSizeCombo.SelectedIndexChanged += (_, _) =>
        {
            if (int.TryParse(pageSizeCombo.Text, out var selectedPageSize) && selectedPageSize > 0)
            {
                pageSize = selectedPageSize;
                currentPage = 1;
                ApplyPage();
            }
        };
    }

    private void ApplyPage()
    {
        var totalPages = TotalPages();
        currentPage = Math.Max(1, Math.Min(currentPage, totalPages));

        var pageItems = items
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize);

        gridControl.DataSource = pageDataSourceFactory is null
            ? pageItems.ToList()
            : pageDataSourceFactory(pageItems);

        gridView.PopulateColumns();

        ConfigureGridColumns();
        ConfigureSelectionColumn();
        UpdatePaginationInfo();
        UpdateSelectionInfo();
        UpdateAuditInfo();
    }

    private void ConfigureSelectionColumn()
    {
        gridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private XtraReport BuildPdfReport(IReadOnlyList<DevExpress.XtraGrid.Columns.GridColumn> columns, string userName, string companyName, byte[]? companyLogoImage)
    {
        var useLandscape = ShouldUseLandscapePdf(columns);
        var pageWidth = useLandscape ? PdfLandscapePageWidth : PdfPageWidth;
        var report = new XtraReport
        {
            DataSource = items.ToArray(),
            PaperKind = DXPaperKind.A4,
            Landscape = useLandscape,
            Margins = new Margins(40, 40, 20, 20),
            Font = new DXFont(ExportFontName, 8F)
        };

        var exportColumns = BuildPdfColumns(columns, pageWidth);
        report.Bands.AddRange(new Band[]
        {
            BuildPdfHeader(report, exportColumns, userName, companyName, companyLogoImage, pageWidth),
            BuildPdfDetail(report, exportColumns),
            BuildPdfFooter(pageWidth),
            BuildPdfPageFooter(companyName, pageWidth)
        });

        return report;
    }

    private PageHeaderBand BuildPdfHeader(XtraReport report, IReadOnlyList<PdfColumn> columns, string userName, string companyName, byte[]? companyLogoImage, float pageWidth)
    {
        var band = new PageHeaderBand { HeightF = 114F };

        var logoPanel = new XRPanel
        {
            LocationF = new PointF(0, 0),
            SizeF = new SizeF(130, 80),
            BackColor = Color.White,
            BorderWidth = 0,
            Borders = BorderSide.None
        };

        if (!TryAddPdfLogo(logoPanel, companyLogoImage))
        {
            logoPanel.Controls.Add(new XRLabel
            {
                Text = companyName,
                LocationF = new PointF(8, 20),
                SizeF = new SizeF(114, 22),
                ForeColor = PdfColor("#00B894"),
                Font = new DXFont(ExportFontName, 10F, DXFontStyle.Bold),
                TextAlignment = TextAlignment.MiddleCenter
            });
            logoPanel.Controls.Add(new XRLabel
            {
                Text = "Sistema de Gestion",
                LocationF = new PointF(8, 44),
                SizeF = new SizeF(114, 16),
                ForeColor = PdfColor("#00B894"),
                Font = new DXFont(ExportFontName, 7F),
                TextAlignment = TextAlignment.MiddleCenter
            });
        }

        var title = new XRLabel
        {
            Text = Text,
            LocationF = new PointF(140, 4),
            SizeF = new SizeF(pageWidth - 140, 28),
            ForeColor = PdfColor("#00B894"),
            Font = new DXFont(ExportFontName, 14F, DXFontStyle.Bold),
            TextAlignment = TextAlignment.MiddleLeft
        };

        var subtitle = new XRLabel
        {
            Text = "Reporte generado automaticamente por el sistema",
            LocationF = new PointF(140, 30),
            SizeF = new SizeF(pageWidth - 140, 16),
            ForeColor = PdfColor("#666666"),
            Font = new DXFont(ExportFontName, 7F),
            TextAlignment = TextAlignment.MiddleLeft
        };

        var filter = string.IsNullOrWhiteSpace(gridView.FindFilterText) ? "Todos" : gridView.FindFilterText;
        var metaFecha = CreatePdfMetaLabel("Fecha:", 140, 50, 60);
        var metaFechaVal = CreatePdfMetaValue(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 200, 50, 115);
        var metaUsuario = CreatePdfMetaLabel("Usuario:", 324, 50, 55);
        var metaUsuarioVal = CreatePdfMetaValue(userName, 378, 50, 105);
        var metaFiltro = CreatePdfMetaLabel("Filtro:", 492, 50, 40);
        var metaFiltroVal = CreatePdfMetaValue(filter, 534, 50, 190);
        var metaRegs = CreatePdfMetaLabel("Registros:", 140, 66, 60);
        var metaRegsVal = CreatePdfMetaValue(items.Count.ToString("N0"), 200, 66, 80);

        var line = new XRLine
        {
            LocationF = new PointF(0, 82),
            SizeF = new SizeF(pageWidth, 4),
            ForeColor = PdfColor("#00B894"),
            LineWidth = 4
        };

        var headerRow = BuildPdfHeaderRow(columns, pageWidth);
        headerRow.LocationF = new PointF(0, 88);

        band.Controls.AddRange(new XRControl[]
        {
            logoPanel, title, subtitle,
            metaFecha, metaFechaVal, metaUsuario, metaUsuarioVal, metaFiltro, metaFiltroVal, metaRegs, metaRegsVal,
            line, headerRow
        });

        return band;
    }

    private DetailBand BuildPdfDetail(XtraReport report, IReadOnlyList<PdfColumn> columns)
    {
        var band = new DetailBand { HeightF = 18F };
        var labels = new List<XRLabel>();
        var x = 0F;

        foreach (var column in columns)
        {
            var label = new XRLabel
            {
                LocationF = new PointF(x, 0),
                SizeF = new SizeF(column.Width, 18F),
                Font = new DXFont(ExportFontName, 8F),
                ForeColor = PdfColor("#222222"),
                TextAlignment = column.Alignment,
                Padding = new PaddingInfo(4, 4, 0, 0),
                Borders = BorderSide.Bottom,
                BorderColor = PdfColor("#E8E8E8"),
                BorderWidth = 0.5F,
                CanGrow = false
            };
            labels.Add(label);
            band.Controls.Add(label);
            x += column.Width;
        }

        var rowNumber = 0;
        band.BeforePrint += (_, _) =>
        {
            var item = report.GetCurrentRow();
            band.BackColor = rowNumber % 2 != 0 ? PdfColor("#E6FBF6") : Color.White;

            for (var index = 0; index < columns.Count; index++)
            {
                var value = index == 0 ? rowNumber + 1 : item is null ? null : ReadValue(item, columns[index].FieldName);
                labels[index].Text = FormatPdfValue(value);
            }

            rowNumber++;
        };

        return band;
    }

    private ReportFooterBand BuildPdfFooter(float pageWidth)
    {
        var band = new ReportFooterBand { HeightF = 22F };
        var line = new XRLine
        {
            LocationF = new PointF(0, 0),
            SizeF = new SizeF(pageWidth, 2),
            ForeColor = PdfColor("#00B894"),
            LineWidth = 2
        };
        var label = new XRLabel
        {
            Text = "Total registros:",
            LocationF = new PointF(0, 2),
            SizeF = new SizeF(pageWidth - 90, 20),
            Font = new DXFont(ExportFontName, 8F, DXFontStyle.Bold),
            ForeColor = PdfColor("#003D30"),
            BackColor = PdfColor("#E6FBF6"),
            TextAlignment = TextAlignment.MiddleRight,
            Padding = new PaddingInfo(0, 6, 0, 0)
        };
        var total = new XRLabel
        {
            Text = items.Count.ToString("N0"),
            LocationF = new PointF(pageWidth - 90, 2),
            SizeF = new SizeF(90, 20),
            Font = new DXFont(ExportFontName, 8F, DXFontStyle.Bold),
            ForeColor = PdfColor("#003D30"),
            BackColor = PdfColor("#E6FBF6"),
            TextAlignment = TextAlignment.MiddleRight,
            Padding = new PaddingInfo(0, 4, 0, 0)
        };

        band.Controls.AddRange(new XRControl[] { line, label, total });
        return band;
    }

    private PageFooterBand BuildPdfPageFooter(string companyName, float pageWidth)
    {
        var band = new PageFooterBand
        {
            HeightF = 28F,
            BackColor = PdfColor("#F9F9F9")
        };

        var line = new XRLine
        {
            LocationF = new PointF(0, 0),
            SizeF = new SizeF(pageWidth, 1),
            ForeColor = PdfColor("#E0E0E0")
        };
        var company = new XRLabel
        {
            Text = companyName,
            LocationF = new PointF(0, 4),
            SizeF = new SizeF(220, 20),
            Font = new DXFont(ExportFontName, 7F),
            ForeColor = PdfColor("#888888"),
            TextAlignment = TextAlignment.MiddleLeft
        };
        var generated = new XRLabel
        {
            Text = "Documento generado el " + DateTime.Now.ToString("dd/MM/yyyy"),
            LocationF = new PointF(220, 4),
            SizeF = new SizeF(250, 20),
            Font = new DXFont(ExportFontName, 7F),
            ForeColor = PdfColor("#888888"),
            TextAlignment = TextAlignment.MiddleCenter
        };
        var page = new XRPageInfo
        {
            PageInfo = PageInfo.NumberOfTotal,
            Format = "Pagina {0} de {1}",
            LocationF = new PointF(pageWidth - 277, 4),
            SizeF = new SizeF(277, 20),
            Font = new DXFont(ExportFontName, 7F),
            ForeColor = PdfColor("#888888"),
            TextAlignment = TextAlignment.MiddleRight
        };

        band.Controls.AddRange(new XRControl[] { line, company, generated, page });
        return band;
    }

    private XRPanel BuildPdfHeaderRow(IReadOnlyList<PdfColumn> columns, float pageWidth)
    {
        var panel = new XRPanel
        {
            SizeF = new SizeF(pageWidth, 22F),
            BackColor = PdfColor("#00B894"),
            Borders = BorderSide.None
        };

        var x = 0F;
        foreach (var column in columns)
        {
            panel.Controls.Add(new XRLabel
            {
                Text = column.Caption,
                LocationF = new PointF(x, 0),
                SizeF = new SizeF(column.Width, 22F),
                ForeColor = Color.White,
                Font = new DXFont(ExportFontName, 8F, DXFontStyle.Bold),
                TextAlignment = column.Alignment,
                Padding = new PaddingInfo(4, 4, 0, 0),
                CanGrow = false
            });
            x += column.Width;
        }

        return panel;
    }

    private IReadOnlyList<PdfColumn> BuildPdfColumns(IReadOnlyList<DevExpress.XtraGrid.Columns.GridColumn> columns, float pageWidth)
    {
        var sourceWidths = columns.Select(column => Math.Max(40, column.Width)).ToArray();
        var numberWidth = 30F;
        var availableWidth = pageWidth - numberWidth;
        var totalSourceWidth = sourceWidths.Sum();
        var exportColumns = new List<PdfColumn>
        {
            new("#", "#", numberWidth, TextAlignment.MiddleCenter)
        };

        for (var index = 0; index < columns.Count; index++)
        {
            var width = totalSourceWidth <= 0
                ? availableWidth / columns.Count
                : sourceWidths[index] * availableWidth / totalSourceWidth;
            width = Math.Max(45F, width);

            var caption = string.IsNullOrWhiteSpace(columns[index].Caption) ? columns[index].FieldName : columns[index].Caption;
            exportColumns.Add(new PdfColumn(columns[index].FieldName, caption, width, ResolvePdfAlignment(columns[index])));
        }

        var excess = exportColumns.Sum(column => column.Width) - pageWidth;
        if (excess > 0 && exportColumns.Count > 1)
        {
            var adjustable = exportColumns.Skip(1).ToArray();
            var subtract = excess / adjustable.Length;
            foreach (var column in adjustable)
            {
                column.Width = Math.Max(35F, column.Width - subtract);
            }
        }

        return exportColumns;
    }

    private static bool ShouldUseLandscapePdf(IReadOnlyList<DevExpress.XtraGrid.Columns.GridColumn> columns)
    {
        var totalVisibleWidth = columns.Sum(column => Math.Max(40, column.Width));
        return columns.Count >= 8 || totalVisibleWidth > 760;
    }

    private static TextAlignment ResolvePdfAlignment(DevExpress.XtraGrid.Columns.GridColumn column)
    {
        var type = column.ColumnType;
        if (type == typeof(int) || type == typeof(long) || type == typeof(decimal) || type == typeof(double) || type == typeof(float))
        {
            return TextAlignment.MiddleRight;
        }

        return type == typeof(bool)
            ? TextAlignment.MiddleCenter
            : TextAlignment.MiddleLeft;
    }

    private static bool TryAddPdfLogo(XRPanel logoPanel, byte[]? companyLogoImage)
    {
        if (companyLogoImage is null || companyLogoImage.Length == 0)
        {
            return false;
        }

        try
        {
            using var stream = new MemoryStream(companyLogoImage);
            using var sourceImage = Image.FromStream(stream);
            var logoImage = new Bitmap(sourceImage);
            var logoSize = FitSize(logoImage.Width, logoImage.Height, 120, 68);
            logoPanel.Controls.Add(new XRPictureBox
            {
                Image = logoImage,
                LocationF = new PointF((130F - logoSize.Width) / 2F, (80F - logoSize.Height) / 2F),
                SizeF = new SizeF(logoSize.Width, logoSize.Height),
                Sizing = ImageSizeMode.StretchImage
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static XRLabel CreatePdfMetaLabel(string text, float x, float y, float width)
    {
        return new XRLabel
        {
            Text = text,
            LocationF = new PointF(x, y),
            SizeF = new SizeF(width, 14),
            ForeColor = PdfColor("#888888"),
            Font = new DXFont(ExportFontName, 7F),
            TextAlignment = TextAlignment.MiddleLeft
        };
    }

    private static XRLabel CreatePdfMetaValue(string text, float x, float y, float width)
    {
        return new XRLabel
        {
            Text = text,
            LocationF = new PointF(x, y),
            SizeF = new SizeF(width, 14),
            ForeColor = PdfColor("#333333"),
            Font = new DXFont(ExportFontName, 7F, DXFontStyle.Bold),
            TextAlignment = TextAlignment.MiddleLeft
        };
    }

    private static string FormatPdfValue(object? value)
    {
        return value switch
        {
            null => string.Empty,
            DateTime dateTime => dateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            bool boolean => boolean ? "Si" : "No",
            decimal decimalValue => decimalValue.ToString("#,##0.00"),
            double doubleValue => doubleValue.ToString("#,##0.00"),
            float floatValue => floatValue.ToString("#,##0.00"),
            _ => Convert.ToString(value) ?? string.Empty
        };
    }

    private bool CanExportRawData(IReadOnlyCollection<DevExpress.XtraGrid.Columns.GridColumn> columns)
    {
        if (columns.Count == 0)
        {
            ShowWarning("No hay columnas visibles para exportar.");
            return false;
        }

        if (items.Count == 0)
        {
            ShowWarning("No hay registros para exportar.");
            return false;
        }

        return true;
    }

    private static object? NormalizeRawExportValue(object? value)
    {
        return value switch
        {
            DateTime dateTime => dateTime.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"),
            byte[] bytes => Convert.ToBase64String(bytes),
            _ => value
        };
    }

    private static void WriteXmlValue(XmlWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                writer.WriteAttributeString("nil", "true");
                break;
            case bool boolean:
                writer.WriteString(boolean ? "true" : "false");
                break;
            case IFormattable formattable:
                writer.WriteString(formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture));
                break;
            default:
                writer.WriteString(Convert.ToString(value) ?? string.Empty);
                break;
        }
    }

    private static string SanitizeXmlElementName(string value)
    {
        var name = string.IsNullOrWhiteSpace(value) ? "Campo" : value.Trim();
        var builder = new System.Text.StringBuilder(name.Length + 1);
        for (var index = 0; index < name.Length; index++)
        {
            var character = name[index];
            var valid = index == 0
                ? XmlConvert.IsStartNCNameChar(character)
                : XmlConvert.IsNCNameChar(character);
            builder.Append(valid ? character : '_');
        }

        if (builder.Length == 0 || !XmlConvert.IsStartNCNameChar(builder[0]))
        {
            builder.Insert(0, "Campo_");
        }

        return builder.ToString();
    }

    private static Color PdfColor(string html)
    {
        return ColorTranslator.FromHtml(html);
    }

    private void BuildExcelHeader(IXLWorksheet worksheet, int lastColumn, string userName, string companyName, byte[]? companyLogoImage)
    {
        worksheet.Row(1).Height = 40;
        worksheet.Row(2).Height = 18;
        worksheet.Row(3).Height = 18;

        worksheet.Range(1, 1, 3, 2).Merge();
        worksheet.Cell(1, 1).Value = $"{companyName}\nSistema de Gestion";
        worksheet.Cell(1, 1).Style
            .Fill.SetBackgroundColor(XLColor.White)
            .Font.SetFontColor(XLColor.FromHtml("#00B894"))
            .Font.SetFontSize(13)
            .Font.SetBold(true)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Alignment.SetWrapText(true);

        worksheet.Range(1, 3, 1, lastColumn).Merge();
        worksheet.Cell(1, 3).Value = Text;
        worksheet.Cell(1, 3).Style
            .Fill.SetBackgroundColor(XLColor.FromHtml("#E6FBF6"))
            .Font.SetFontColor(XLColor.FromHtml("#00B894"))
            .Font.SetFontSize(14)
            .Font.SetBold(true)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

        worksheet.Cell(2, 3).Value = "Fecha:";
        worksheet.Cell(2, 4).Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        worksheet.Cell(2, 5).Value = "Usuario:";
        worksheet.Cell(2, 6).Value = userName;
        worksheet.Cell(3, 3).Value = "Total registros:";
        worksheet.Cell(3, 4).Value = items.Count;
        worksheet.Cell(3, 5).Value = "Filtro:";
        worksheet.Cell(3, 6).Value = string.IsNullOrWhiteSpace(gridView.FindFilterText) ? "Todos" : gridView.FindFilterText;

        worksheet.Range(2, 3, 3, lastColumn).Style
            .Fill.SetBackgroundColor(XLColor.FromHtml("#F9F9F9"))
            .Font.SetFontSize(10)
            .Font.SetFontColor(XLColor.FromHtml("#555555"));

        worksheet.Row(4).Height = 6;
        worksheet.Range(4, 1, 4, lastColumn).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F9F9F9"));

        var logoAdded = TryAddCompanyLogo(worksheet, companyLogoImage);
        if (logoAdded)
        {
            worksheet.Cell(1, 1).Value = string.Empty;
        }
    }

    private static bool TryAddCompanyLogo(IXLWorksheet worksheet, byte[]? companyLogoImage)
    {
        if (companyLogoImage is null || companyLogoImage.Length == 0)
        {
            return false;
        }

        try
        {
            using var imageStream = new MemoryStream(companyLogoImage);
            using var sourceImage = Image.FromStream(imageStream);
            var logoSize = FitSize(sourceImage.Width, sourceImage.Height, 120, 68);
            var blockWidth = ExcelWidthToPixels(worksheet.Column(1).Width) + ExcelWidthToPixels(worksheet.Column(2).Width);
            var blockHeight = PointsToPixels(worksheet.Row(1).Height + worksheet.Row(2).Height + worksheet.Row(3).Height);
            var offsetX = Math.Max(0, (blockWidth - logoSize.Width) / 2);
            var offsetY = Math.Max(0, (blockHeight - logoSize.Height) / 2);

            using var stream = new MemoryStream(companyLogoImage);
            worksheet.AddPicture(stream, "LogoEmpresa")
                .MoveTo(worksheet.Cell(1, 1), offsetX, offsetY)
                .WithSize(logoSize.Width, logoSize.Height);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static int ExcelWidthToPixels(double width)
    {
        return width <= 0 ? 0 : (int)Math.Truncate(width * 7 + 5);
    }

    private static int PointsToPixels(double points)
    {
        return (int)Math.Round(points * 96 / 72);
    }

    private static Size FitSize(int sourceWidth, int sourceHeight, int maxWidth, int maxHeight)
    {
        if (sourceWidth <= 0 || sourceHeight <= 0)
        {
            return new Size(maxWidth, maxHeight);
        }

        var ratio = Math.Min(maxWidth / (double)sourceWidth, maxHeight / (double)sourceHeight);
        return new Size(
            Math.Max(1, (int)Math.Round(sourceWidth * ratio)),
            Math.Max(1, (int)Math.Round(sourceHeight * ratio)));
    }

    private void BuildExcelTable(IXLWorksheet worksheet, IReadOnlyList<DevExpress.XtraGrid.Columns.GridColumn> columns, int totalColumns)
    {
        worksheet.Cell(5, 1).Value = "#";
        ApplyExcelHeaderStyle(worksheet.Cell(5, 1));

        for (var index = 0; index < columns.Count; index++)
        {
            var cell = worksheet.Cell(5, index + 2);
            cell.Value = string.IsNullOrWhiteSpace(columns[index].Caption) ? columns[index].FieldName : columns[index].Caption;
            ApplyExcelHeaderStyle(cell);
        }

        var rowIndex = 6;
        for (var row = 0; row < items.Count; row++)
        {
            var item = items[row];
            var background = row % 2 != 0 ? XLColor.FromHtml("#E6FBF6") : XLColor.White;
            worksheet.Cell(rowIndex, 1).Value = row + 1;

            for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                var value = ReadValue(item, columns[columnIndex].FieldName);
                WriteExcelValue(worksheet.Cell(rowIndex, columnIndex + 2), value);
            }

            worksheet.Range(rowIndex, 1, rowIndex, totalColumns).Style.Fill.SetBackgroundColor(background);
            rowIndex++;
        }

        worksheet.Range(rowIndex, 1, rowIndex, totalColumns - 1).Merge();
        worksheet.Cell(rowIndex, 1).Value = "Total registros:";
        worksheet.Cell(rowIndex, totalColumns).Value = items.Count;
        worksheet.Range(rowIndex, 1, rowIndex, totalColumns).Style
            .Fill.SetBackgroundColor(XLColor.FromHtml("#E6FBF6"))
            .Font.SetFontColor(XLColor.FromHtml("#003D30"))
            .Font.SetBold(true)
            .Border.SetTopBorder(XLBorderStyleValues.Medium)
            .Border.SetTopBorderColor(XLColor.FromHtml("#00B894"));

        worksheet.Column(1).Width = 6;
        for (var index = 0; index < columns.Count; index++)
        {
            worksheet.Column(index + 2).Width = Math.Max(10, Math.Min(45, columns[index].Width / 7.0));
        }

        worksheet.Row(5).Height = 18;
        worksheet.Range(5, 1, Math.Max(5, rowIndex), totalColumns).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
        worksheet.Range(5, 1, Math.Max(5, rowIndex), totalColumns).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
    }

    private static void ApplyExcelHeaderStyle(IXLCell cell)
    {
        cell.Style
            .Fill.SetBackgroundColor(XLColor.FromHtml("#00B894"))
            .Font.SetFontColor(XLColor.White)
            .Font.SetBold(true)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }

    private static void WriteExcelValue(IXLCell cell, object? value)
    {
        switch (value)
        {
            case null:
                cell.Value = string.Empty;
                break;
            case DateTime dateTime:
                cell.Value = dateTime.ToLocalTime();
                cell.Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                break;
            case bool boolean:
                cell.Value = boolean ? "Si" : "No";
                break;
            case decimal decimalValue:
                cell.Value = decimalValue;
                cell.Style.NumberFormat.Format = "#,##0.00";
                break;
            case double doubleValue:
                cell.Value = doubleValue;
                cell.Style.NumberFormat.Format = "#,##0.00";
                break;
            case float floatValue:
                cell.Value = floatValue;
                cell.Style.NumberFormat.Format = "#,##0.00";
                break;
            case int intValue:
                cell.Value = intValue;
                break;
            case long longValue:
                cell.Value = longValue;
                break;
            default:
                cell.Value = Convert.ToString(value) ?? string.Empty;
                break;
        }
    }

    private static object? ReadValue(object item, string propertyName)
    {
        return item.GetType().GetProperty(propertyName)?.GetValue(item);
    }

    private DevExpress.XtraGrid.Columns.GridColumn[] GetVisibleGridColumns()
    {
        return gridView.Columns
            .Cast<DevExpress.XtraGrid.Columns.GridColumn>()
            .Where(column => column.Visible && !string.IsNullOrWhiteSpace(column.FieldName))
            .OrderBy(column => column.VisibleIndex)
            .ToArray();
    }

    private sealed class PdfColumn(string fieldName, string caption, float width, TextAlignment alignment)
    {
        public string FieldName { get; } = fieldName;

        public string Caption { get; } = caption;

        public float Width { get; set; } = width;

        public TextAlignment Alignment { get; } = alignment;
    }

    private static string SanitizeFileName(string value)
    {
        var sanitized = string.Join("_", value.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim();
        return string.IsNullOrWhiteSpace(sanitized) ? "Listado" : sanitized;
    }

    private static string SanitizeWorksheetName(string value)
    {
        var invalidChars = new[] { ':', '\\', '/', '?', '*', '[', ']' };
        var sanitized = string.Join("_", value.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).Trim();
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            return "Listado";
        }

        return sanitized.Length > 31 ? sanitized[..31] : sanitized;
    }

    private IReadOnlyCollection<GridColumnSettingItem> CaptureCurrentColumnSettings()
    {
        return gridView.Columns
            .Cast<DevExpress.XtraGrid.Columns.GridColumn>()
            .Where(column => !string.IsNullOrWhiteSpace(column.FieldName))
            .Select(column => new GridColumnSettingItem(
                column.FieldName,
                string.IsNullOrWhiteSpace(column.Caption) ? column.FieldName : column.Caption,
                string.IsNullOrWhiteSpace(column.Caption) ? column.FieldName : column.Caption,
                column.Visible,
                column.VisibleIndex < 0 ? 999 : column.VisibleIndex,
                column.Width <= 0 ? 100 : column.Width))
            .OrderBy(column => column.VisibleIndex)
            .ThenBy(column => column.FieldName)
            .ToArray();
    }

    private void ApplyColumnSettings(IReadOnlyCollection<GridColumnSettingItem> settings)
    {
        if (settings.Count == 0)
        {
            return;
        }

        foreach (var setting in settings)
        {
            if (gridView.Columns[setting.FieldName] is not { } column)
            {
                continue;
            }

            column.Caption = string.IsNullOrWhiteSpace(setting.Caption)
                ? setting.DefaultCaption
                : setting.Caption;
            column.Width = Math.Max(40, setting.Width);

            if (setting.IsVisible)
            {
                column.Visible = true;
                column.VisibleIndex = setting.VisibleIndex;
            }
            else
            {
                column.Visible = false;
            }
        }
    }

    private IReadOnlyCollection<int> GetSelectedRowHandles()
    {
        var selectedRows = gridView.GetSelectedRows()
            .Where(rowHandle => rowHandle >= 0)
            .ToArray();

        if (selectedRows.Length > 0)
        {
            return selectedRows;
        }

        return gridView.FocusedRowHandle >= 0
            ? new[] { gridView.FocusedRowHandle }
            : Array.Empty<int>();
    }

    private async Task ExecuteForEachSelectedItemAsync(IReadOnlyCollection<int> rowHandles, Func<Task> action)
    {
        var rows = rowHandles
            .Select(rowHandle => gridView.GetRow(rowHandle))
            .Where(row => row is not null)
            .ToArray();

        foreach (var row in rows)
        {
            batchSelectedItem = row;
            try
            {
                await action();
            }
            finally
            {
                batchSelectedItem = null;
            }
        }
    }

    private void GoToPage(int page)
    {
        currentPage = Math.Max(1, Math.Min(page, TotalPages()));
        ApplyPage();
    }

    private int TotalPages()
    {
        return Math.Max(1, (int)Math.Ceiling(items.Count / (double)pageSize));
    }

    private void UpdatePaginationInfo()
    {
        var totalPages = TotalPages();
        pageInfoLabel.Text = $"Pagina {currentPage} de {totalPages}";
        totalInfoLabel.Text = $"Total: {items.Count:N0} registros";

        firstPageButton.Enabled = currentPage > 1;
        previousPageButton.Enabled = currentPage > 1;
        nextPageButton.Enabled = currentPage < totalPages;
        lastPageButton.Enabled = currentPage < totalPages;
    }

    private void UpdateSelectionInfo()
    {
        var selectedRows = gridView.GetSelectedRows().Count(rowHandle => rowHandle >= 0);
        selectionInfoLabel.Text = $"Seleccionados: {selectedRows:N0} de {items.Count:N0}";
    }

    private void UpdateAuditInfo()
    {
        var selectedItem = gridView.GetFocusedRow();
        if (selectedItem is null)
        {
            auditCreatedLabel.Text = "Creado por: -";
            auditUpdatedLabel.Text = "Modificado por: -";
            return;
        }

        auditCreatedLabel.Text = $"Creado por: {FormatUser(selectedItem, "CreatedByUserName", "CreatedByUserId")} | {FormatDate(selectedItem, "CreatedAt")}";
        auditUpdatedLabel.Text = TryGetDate(selectedItem, "UpdatedAt", out var updatedAt)
            ? $"Modificado por: {FormatUser(selectedItem, "UpdatedByUserName", "UpdatedByUserId")} | {FormatDate(updatedAt)}"
            : "Modificado por: -";
    }

    private static string FormatUser(object item, string userNameProperty, string userIdProperty)
    {
        var userName = ReadValue<string>(item, userNameProperty);
        if (!string.IsNullOrWhiteSpace(userName))
        {
            return userName;
        }

        var userId = ReadValue<int?>(item, userIdProperty);
        return userId.HasValue ? $"Usuario {userId.Value}" : "Sistema";
    }

    private static string FormatDate(object item, string propertyName)
    {
        return TryGetDate(item, propertyName, out var value) ? FormatDate(value) : "-";
    }

    private static string FormatDate(DateTime value)
    {
        return value.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
    }

    private static bool TryGetDate(object item, string propertyName, out DateTime value)
    {
        var property = item.GetType().GetProperty(propertyName);
        var rawValue = property?.GetValue(item);
        if (rawValue is DateTime dateTime)
        {
            value = dateTime;
            return true;
        }

        value = default;
        return false;
    }

    private static T? ReadValue<T>(object item, string propertyName)
    {
        var property = item.GetType().GetProperty(propertyName);
        var value = property?.GetValue(item);
        return value is T typedValue ? typedValue : default;
    }
}
