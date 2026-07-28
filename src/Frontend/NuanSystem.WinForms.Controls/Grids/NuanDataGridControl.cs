using System.ComponentModel;
using System.Drawing.Drawing2D;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace NuanSystem.WinForms.Controls.Grids;

public sealed class NuanDataGridControl : XtraUserControl
{
    private static readonly Font BaseFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    private static readonly Font ButtonFont = new("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
    private static readonly Font GridHeaderFont = new("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
    private static readonly Font GridRowFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    private static readonly Color TextColor = Color.FromArgb(23, 32, 51);
    private static readonly Color MutedTextColor = Color.FromArgb(100, 112, 132);
    private static readonly Color SurfaceColor = Color.FromArgb(247, 248, 252);

    private readonly GridControl grcData;
    private readonly GridView grvData;
    private readonly PanelControl pnlPagination;
    private readonly SimpleButton btnFirstPage;
    private readonly SimpleButton btnPreviousPage;
    private readonly LabelControl lblPageInfo;
    private readonly SimpleButton btnNextPage;
    private readonly SimpleButton btnLastPage;
    private readonly LabelControl lblPageSize;
    private readonly ComboBoxEdit cmbPageSize;
    private readonly LabelControl lblTotal;
    private readonly LabelControl lblSelection;
    private readonly List<object> items = new();
    private readonly List<NuanGridColumnDefinition> columnDefinitions = new();
    private Func<IEnumerable<object>, object>? pageDataSourceFactory;
    private NuanGridStatusBadgeProvider statusBadgeProvider = NuanGridStatusBadges.DefaultProvider;
    private int currentPage = 1;
    private int pageSize = 20;
    private int totalItemCount;
    private bool serverPaging;
    private bool suppressPageRequests;
    private bool showPagination = true;
    private bool showFindPanel = true;
    private bool multiSelect;
    private bool showSelectionCheckBox;

    public NuanDataGridControl()
    {
        grcData = new GridControl();
        grvData = new GridView();
        pnlPagination = new PanelControl();
        btnFirstPage = new SimpleButton();
        btnPreviousPage = new SimpleButton();
        lblPageInfo = new LabelControl();
        btnNextPage = new SimpleButton();
        btnLastPage = new SimpleButton();
        lblPageSize = new LabelControl();
        cmbPageSize = new ComboBoxEdit();
        lblTotal = new LabelControl();
        lblSelection = new LabelControl();

        InitializeComponent();
        WireEvents();
        UpdatePaginationInfo();
        UpdateSelectionInfo();
    }

    public event EventHandler? FocusedRowChanged;

    public event EventHandler? RowDoubleClick;

    public event EventHandler? SelectionChanged;

    public event EventHandler<NuanGridPageRequestEventArgs>? PageRequested;

    [Browsable(false)]
    public GridControl GridControl => grcData;

    [Browsable(false)]
    public GridView GridView => grvData;

    [Browsable(false)]
    public GridControl InnerGridControl => grcData;

    [Browsable(false)]
    public GridView InnerGridView => grvData;

    [DefaultValue(true)]
    public bool ShowPagination
    {
        get => showPagination;
        set
        {
            showPagination = value;
            pnlPagination.Visible = value;
            ApplyPage();
        }
    }

    [DefaultValue(true)]
    public bool ShowFindPanel
    {
        get => showFindPanel;
        set
        {
            showFindPanel = value;
            grvData.OptionsFind.AlwaysVisible = value;
        }
    }

    [DefaultValue(false)]
    public bool MultiSelect
    {
        get => multiSelect;
        set
        {
            multiSelect = value;
            ApplySelectionOptions();
        }
    }

    [DefaultValue(false)]
    public bool ShowSelectionCheckBox
    {
        get => showSelectionCheckBox;
        set
        {
            showSelectionCheckBox = value;
            ApplySelectionOptions();
        }
    }

    [DefaultValue(20)]
    public int PageSize
    {
        get => pageSize;
        set
        {
            pageSize = Math.Max(1, value);
            currentPage = 1;
            cmbPageSize.EditValue = pageSize.ToString();
            ApplyPage();
        }
    }

    [DefaultValue("")]
    public string FormKey { get; set; } = string.Empty;

    [DefaultValue("MainGrid")]
    public string GridName { get; set; } = "MainGrid";

    [DefaultValue(false)]
    public bool EnableColumnCustomization { get; set; }

    public void ConfigureColumns(params NuanGridColumnDefinition[] columns)
    {
        columnDefinitions.Clear();
        columnDefinitions.AddRange(columns.Where(column => !string.IsNullOrWhiteSpace(column.FieldName)));
        ApplyColumnDefinitions();
    }

    public void SetStatusBadgeProvider(NuanGridStatusBadgeProvider provider)
    {
        statusBadgeProvider = provider ?? NuanGridStatusBadges.DefaultProvider;
        grvData.RefreshData();
    }

    public void SetData<T>(IEnumerable<T> data)
    {
        items.Clear();
        items.AddRange(data.Cast<object>());
        pageDataSourceFactory = pageItems => pageItems.Cast<T>().ToList();
        serverPaging = false;
        totalItemCount = items.Count;
        currentPage = 1;
        ApplyPage();
    }

    public void SetPagedData<T>(
        IEnumerable<T> data,
        int page,
        int requestedPageSize,
        int totalCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(requestedPageSize, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalCount);

        items.Clear();
        items.AddRange(data.Cast<object>());
        pageDataSourceFactory = pageItems => pageItems.Cast<T>().ToList();
        serverPaging = true;
        totalItemCount = totalCount;
        pageSize = requestedPageSize;
        currentPage = Math.Min(page, TotalPages());

        suppressPageRequests = true;
        try
        {
            EnsurePageSizeOption(requestedPageSize);
            cmbPageSize.EditValue = requestedPageSize.ToString();
        }
        finally
        {
            suppressPageRequests = false;
        }

        ApplyPage();
    }

    public void ApplyStandardGridStyle()
    {
        ApplyStandardGridStyle(grvData);
    }

    public T? GetFocusedRow<T>()
    {
        return grvData.GetFocusedRow() is T row ? row : default;
    }

    public IReadOnlyCollection<T> GetSelectedRows<T>()
    {
        var selectedRows = grvData.GetSelectedRows()
            .Where(rowHandle => rowHandle >= 0)
            .Select(rowHandle => grvData.GetRow(rowHandle))
            .OfType<T>()
            .ToArray();

        if (selectedRows.Length > 0)
        {
            return selectedRows;
        }

        return GetFocusedRow<T>() is { } focusedRow
            ? new[] { focusedRow }
            : Array.Empty<T>();
    }

    public void ExportVisibleColumns()
    {
        throw new NotSupportedException("La exportacion se implementara en una fase posterior.");
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        ((ISupportInitialize)grcData).BeginInit();
        ((ISupportInitialize)grvData).BeginInit();
        ((ISupportInitialize)pnlPagination).BeginInit();
        pnlPagination.SuspendLayout();
        ((ISupportInitialize)cmbPageSize.Properties).BeginInit();

        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Font = BaseFont;
        Name = "NuanDataGridControl";
        Size = new Size(900, 420);

        grcData.Dock = DockStyle.Fill;
        grcData.Font = BaseFont;
        grcData.Location = new Point(0, 0);
        grcData.MainView = grvData;
        grcData.Name = "grcData";
        grcData.Size = new Size(900, 376);
        grcData.TabIndex = 0;
        grcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvData });

        ApplyStandardGridStyle(grvData);
        grvData.GridControl = grcData;
        grvData.Name = "grvData";
        grvData.OptionsBehavior.Editable = false;
        grvData.OptionsFind.AlwaysVisible = true;
        grvData.OptionsFind.FindNullPrompt = "Buscar...";
        grvData.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvData.OptionsView.ColumnAutoWidth = false;
        grvData.OptionsView.ShowGroupPanel = false;

        pnlPagination.Appearance.BackColor = SurfaceColor;
        pnlPagination.Appearance.Options.UseBackColor = true;
        pnlPagination.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        pnlPagination.Controls.Add(btnFirstPage);
        pnlPagination.Controls.Add(btnPreviousPage);
        pnlPagination.Controls.Add(lblPageInfo);
        pnlPagination.Controls.Add(btnNextPage);
        pnlPagination.Controls.Add(btnLastPage);
        pnlPagination.Controls.Add(lblPageSize);
        pnlPagination.Controls.Add(cmbPageSize);
        pnlPagination.Controls.Add(lblTotal);
        pnlPagination.Controls.Add(lblSelection);
        pnlPagination.Dock = DockStyle.Bottom;
        pnlPagination.Location = new Point(0, 376);
        pnlPagination.Name = "pnlPagination";
        pnlPagination.Size = new Size(900, 44);
        pnlPagination.TabIndex = 1;

        ConfigureButton(btnFirstPage, "|<", new Point(10, 8), 0);
        ConfigureButton(btnPreviousPage, "<", new Point(50, 8), 1);
        ConfigureButton(btnNextPage, ">", new Point(178, 8), 3);
        ConfigureButton(btnLastPage, ">|", new Point(218, 8), 4);

        lblPageInfo.Appearance.Font = BaseFont;
        lblPageInfo.Appearance.ForeColor = TextColor;
        lblPageInfo.Appearance.Options.UseFont = true;
        lblPageInfo.Appearance.Options.UseForeColor = true;
        lblPageInfo.Location = new Point(96, 14);
        lblPageInfo.Name = "lblPageInfo";
        lblPageInfo.Size = new Size(72, 15);
        lblPageInfo.TabIndex = 2;
        lblPageInfo.Text = "Pagina 1 de 1";

        lblPageSize.Appearance.Font = BaseFont;
        lblPageSize.Appearance.ForeColor = TextColor;
        lblPageSize.Appearance.Options.UseFont = true;
        lblPageSize.Appearance.Options.UseForeColor = true;
        lblPageSize.Location = new Point(276, 14);
        lblPageSize.Name = "lblPageSize";
        lblPageSize.Size = new Size(54, 15);
        lblPageSize.TabIndex = 5;
        lblPageSize.Text = "Registros:";

        cmbPageSize.EditValue = "20";
        cmbPageSize.Location = new Point(344, 10);
        cmbPageSize.Name = "cmbPageSize";
        cmbPageSize.Properties.Appearance.Font = BaseFont;
        cmbPageSize.Properties.Appearance.Options.UseFont = true;
        cmbPageSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        cmbPageSize.Properties.Items.AddRange(new object[] { "10", "20", "50", "100" });
        cmbPageSize.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        cmbPageSize.Size = new Size(70, 22);
        cmbPageSize.TabIndex = 6;

        lblTotal.Appearance.Font = BaseFont;
        lblTotal.Appearance.ForeColor = TextColor;
        lblTotal.Appearance.Options.UseFont = true;
        lblTotal.Appearance.Options.UseForeColor = true;
        lblTotal.Location = new Point(438, 14);
        lblTotal.Name = "lblTotal";
        lblTotal.Size = new Size(88, 15);
        lblTotal.TabIndex = 7;
        lblTotal.Text = "Total: 0 registros";

        lblSelection.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblSelection.Appearance.Font = BaseFont;
        lblSelection.Appearance.ForeColor = MutedTextColor;
        lblSelection.Appearance.Options.UseFont = true;
        lblSelection.Appearance.Options.UseForeColor = true;
        lblSelection.Location = new Point(735, 14);
        lblSelection.Name = "lblSelection";
        lblSelection.Size = new Size(120, 15);
        lblSelection.TabIndex = 8;
        lblSelection.Text = "Seleccionados: 0 de 0";

        Controls.Add(grcData);
        Controls.Add(pnlPagination);

        ((ISupportInitialize)cmbPageSize.Properties).EndInit();
        pnlPagination.ResumeLayout(false);
        pnlPagination.PerformLayout();
        ((ISupportInitialize)pnlPagination).EndInit();
        ((ISupportInitialize)grvData).EndInit();
        ((ISupportInitialize)grcData).EndInit();
        ResumeLayout(false);
    }

    private static void ConfigureButton(SimpleButton button, string text, Point location, int tabIndex)
    {
        button.Appearance.Font = ButtonFont;
        button.Appearance.Options.UseFont = true;
        button.Location = location;
        button.Name = "btn" + tabIndex.ToString();
        button.Size = new Size(36, 28);
        button.TabIndex = tabIndex;
        button.Text = text;
    }

    private static void ApplyStandardGridStyle(GridView gridView)
    {
        gridView.Appearance.HeaderPanel.Font = GridHeaderFont;
        gridView.Appearance.HeaderPanel.ForeColor = TextColor;
        gridView.Appearance.HeaderPanel.Options.UseFont = true;
        gridView.Appearance.HeaderPanel.Options.UseForeColor = true;
        gridView.Appearance.Row.Font = GridRowFont;
        gridView.Appearance.Row.ForeColor = TextColor;
        gridView.Appearance.Row.Options.UseFont = true;
        gridView.Appearance.Row.Options.UseForeColor = true;
        gridView.Appearance.FooterPanel.Font = GridHeaderFont;
        gridView.Appearance.FooterPanel.Options.UseFont = true;
        gridView.Appearance.FilterPanel.Font = GridRowFont;
        gridView.Appearance.FilterPanel.Options.UseFont = true;
    }

    private void WireEvents()
    {
        grvData.FocusedRowChanged += (_, _) =>
        {
            UpdateSelectionInfo();
            FocusedRowChanged?.Invoke(this, EventArgs.Empty);
        };
        grvData.SelectionChanged += (_, _) =>
        {
            UpdateSelectionInfo();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        };
        grvData.DoubleClick += (_, _) => RowDoubleClick?.Invoke(this, EventArgs.Empty);
        grvData.CustomDrawCell += DrawStatusBadgeCell;

        btnFirstPage.Click += (_, _) => GoToPage(1);
        btnPreviousPage.Click += (_, _) => GoToPage(currentPage - 1);
        btnNextPage.Click += (_, _) => GoToPage(currentPage + 1);
        btnLastPage.Click += (_, _) => GoToPage(TotalPages());
        cmbPageSize.SelectedIndexChanged += (_, _) =>
        {
            if (!suppressPageRequests
                && int.TryParse(cmbPageSize.Text, out var selectedPageSize)
                && selectedPageSize > 0)
            {
                if (serverPaging)
                {
                    RequestServerPage(1, selectedPageSize);
                    return;
                }

                pageSize = selectedPageSize;
                currentPage = 1;
                ApplyPage();
            }
        };
    }

    private void ApplySelectionOptions()
    {
        grvData.OptionsSelection.MultiSelect = multiSelect || showSelectionCheckBox;
        grvData.OptionsSelection.MultiSelectMode = showSelectionCheckBox
            ? GridMultiSelectMode.CheckBoxRowSelect
            : GridMultiSelectMode.RowSelect;
        grvData.OptionsSelection.CheckBoxSelectorColumnWidth = showSelectionCheckBox ? 30 : 0;
        UpdateSelectionInfo();
    }

    private void ApplyPage()
    {
        if (pageDataSourceFactory is null)
        {
            return;
        }

        var totalPages = TotalPages();
        currentPage = Math.Max(1, Math.Min(currentPage, totalPages));
        var pageItems = showPagination && !serverPaging
            ? items.Skip((currentPage - 1) * pageSize).Take(pageSize)
            : items;

        grcData.DataSource = pageDataSourceFactory(pageItems);
        grvData.PopulateColumns();
        ApplyColumnDefinitions();
        UpdatePaginationInfo();
        UpdateSelectionInfo();
    }

    private void ApplyColumnDefinitions()
    {
        if (grvData.Columns.Count == 0)
        {
            return;
        }

        foreach (GridColumn gridColumn in grvData.Columns)
        {
            gridColumn.Visible = false;
        }

        foreach (var definition in columnDefinitions)
        {
            if (grvData.Columns[definition.FieldName] is not { } gridColumn)
            {
                continue;
            }

            gridColumn.Caption = string.IsNullOrWhiteSpace(definition.Caption)
                ? definition.FieldName
                : definition.Caption;
            gridColumn.Visible = definition.Visible;
            gridColumn.VisibleIndex = definition.VisibleIndex;
            gridColumn.Width = Math.Max(40, definition.Width);
            gridColumn.OptionsFilter.AllowFilter = definition.AllowFilter;
            gridColumn.OptionsColumn.AllowSort = definition.AllowSort
                ? DevExpress.Utils.DefaultBoolean.True
                : DevExpress.Utils.DefaultBoolean.False;
            ApplyColumnFormat(gridColumn, definition);
        }
    }

    private static void ApplyColumnFormat(GridColumn gridColumn, NuanGridColumnDefinition definition)
    {
        var alignment = definition.Alignment == HorzAlignment.Default
            ? DefaultAlignment(definition.Format)
            : definition.Alignment;

        gridColumn.AppearanceCell.TextOptions.HAlignment = alignment;
        gridColumn.AppearanceHeader.TextOptions.HAlignment = alignment == HorzAlignment.Far
            ? HorzAlignment.Far
            : HorzAlignment.Near;

        switch (definition.Format)
        {
            case NuanGridColumnFormat.Number:
                gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
                gridColumn.DisplayFormat.FormatString = "n0";
                break;
            case NuanGridColumnFormat.Decimal:
                gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
                gridColumn.DisplayFormat.FormatString = "n2";
                break;
            case NuanGridColumnFormat.Money:
                gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
                gridColumn.DisplayFormat.FormatString = "c2";
                break;
            case NuanGridColumnFormat.Percent:
                gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
                gridColumn.DisplayFormat.FormatString = "p2";
                break;
            case NuanGridColumnFormat.Date:
                gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
                gridColumn.DisplayFormat.FormatString = "dd/MM/yyyy";
                break;
            case NuanGridColumnFormat.DateTime:
                gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
                gridColumn.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                break;
            case NuanGridColumnFormat.Boolean:
            case NuanGridColumnFormat.StatusBadge:
            case NuanGridColumnFormat.Text:
            default:
                gridColumn.DisplayFormat.FormatType = FormatType.None;
                gridColumn.DisplayFormat.FormatString = string.Empty;
                break;
        }
    }

    private static HorzAlignment DefaultAlignment(NuanGridColumnFormat format)
    {
        return format switch
        {
            NuanGridColumnFormat.Number => HorzAlignment.Far,
            NuanGridColumnFormat.Decimal => HorzAlignment.Far,
            NuanGridColumnFormat.Money => HorzAlignment.Far,
            NuanGridColumnFormat.Percent => HorzAlignment.Far,
            NuanGridColumnFormat.Boolean => HorzAlignment.Center,
            NuanGridColumnFormat.StatusBadge => HorzAlignment.Center,
            _ => HorzAlignment.Near
        };
    }

    private void DrawStatusBadgeCell(object? sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
    {
        var definition = columnDefinitions.FirstOrDefault(column => column.FieldName == e.Column.FieldName);
        if (definition is null || definition.Format != NuanGridColumnFormat.StatusBadge)
        {
            return;
        }

        var text = Convert.ToString(e.CellValue) ?? string.Empty;
        var style = statusBadgeProvider(e.CellValue);
        var colors = BadgeColors(style);
        var bounds = new Rectangle(e.Bounds.X + 8, e.Bounds.Y + 4, Math.Max(44, e.Bounds.Width - 16), e.Bounds.Height - 8);
        using var path = RoundedRectangle(bounds, 4);
        using var backBrush = new SolidBrush(colors.BackColor);
        using var borderPen = new Pen(colors.BorderColor);
        using var textBrush = new SolidBrush(colors.ForeColor);
        using var textFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        e.Cache.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Cache.Graphics.FillPath(backBrush, path);
        e.Cache.Graphics.DrawPath(borderPen, path);
        e.Cache.Graphics.DrawString(text, GridRowFont, textBrush, bounds, textFormat);
        e.Handled = true;
    }

    private static (Color BackColor, Color ForeColor, Color BorderColor) BadgeColors(NuanGridBadgeStyle style)
    {
        return style switch
        {
            NuanGridBadgeStyle.Info => (Color.FromArgb(219, 234, 254), Color.FromArgb(29, 78, 216), Color.FromArgb(191, 219, 254)),
            NuanGridBadgeStyle.Success => (Color.FromArgb(220, 252, 231), Color.FromArgb(22, 101, 52), Color.FromArgb(187, 247, 208)),
            NuanGridBadgeStyle.Warning => (Color.FromArgb(254, 243, 199), Color.FromArgb(146, 64, 14), Color.FromArgb(253, 230, 138)),
            NuanGridBadgeStyle.Error => (Color.FromArgb(254, 226, 226), Color.FromArgb(185, 28, 28), Color.FromArgb(254, 202, 202)),
            NuanGridBadgeStyle.Critical => (Color.FromArgb(255, 228, 230), Color.FromArgb(159, 18, 57), Color.FromArgb(254, 205, 211)),
            _ => (Color.FromArgb(241, 245, 249), Color.FromArgb(51, 65, 85), Color.FromArgb(226, 232, 240))
        };
    }

    private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private void GoToPage(int page)
    {
        var requestedPage = Math.Max(1, Math.Min(page, TotalPages()));
        if (requestedPage == currentPage)
        {
            return;
        }

        if (serverPaging)
        {
            RequestServerPage(requestedPage, pageSize);
            return;
        }

        currentPage = requestedPage;
        ApplyPage();
    }

    private void RequestServerPage(int requestedPage, int requestedPageSize)
    {
        PageRequested?.Invoke(
            this,
            new NuanGridPageRequestEventArgs(requestedPage, requestedPageSize));
    }

    private int TotalPages()
    {
        if (!showPagination)
        {
            return 1;
        }

        var itemCount = serverPaging ? totalItemCount : items.Count;
        return Math.Max(1, (int)Math.Ceiling(itemCount / (double)pageSize));
    }

    private void UpdatePaginationInfo()
    {
        var totalPages = TotalPages();
        lblPageInfo.Text = $"Pagina {currentPage} de {totalPages}";
        var itemCount = serverPaging ? totalItemCount : items.Count;
        lblTotal.Text = $"Total: {itemCount:N0} registros";

        btnFirstPage.Enabled = showPagination && currentPage > 1;
        btnPreviousPage.Enabled = showPagination && currentPage > 1;
        btnNextPage.Enabled = showPagination && currentPage < totalPages;
        btnLastPage.Enabled = showPagination && currentPage < totalPages;
    }

    private void UpdateSelectionInfo()
    {
        var selectedRows = grvData.GetSelectedRows().Count(rowHandle => rowHandle >= 0);
        lblSelection.Text = $"Seleccionados: {selectedRows:N0} de {items.Count:N0}";
    }

    private void EnsurePageSizeOption(int requestedPageSize)
    {
        if (!cmbPageSize.Properties.Items.Contains(requestedPageSize.ToString()))
        {
            cmbPageSize.Properties.Items.Add(requestedPageSize.ToString());
        }
    }
}
