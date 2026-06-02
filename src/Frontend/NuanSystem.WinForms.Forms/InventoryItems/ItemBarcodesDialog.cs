using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.InventoryItems;

internal sealed class ItemBarcodesDialog : XtraForm
{
    private readonly BindingList<BarcodeRow> rows;
    private readonly LabelControl lblTitle = new();
    private readonly LabelControl lblBarcode = new();
    private readonly TextEdit txtBarcode = new();
    private readonly SimpleButton btnAdd = new();
    private readonly GridControl grcBarcodes = new();
    private readonly GridView grvBarcodes = new();
    private readonly GridColumn colBarcode = new();
    private readonly SimpleButton btnRemove = new();
    private readonly SimpleButton btnAccept = new();
    private readonly SimpleButton btnCancel = new();

    public ItemBarcodesDialog(IEnumerable<string> barcodes)
    {
        rows = new BindingList<BarcodeRow>(
            barcodes
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(code => code.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(code => new BarcodeRow { Code = code })
                .ToList());

        InitializeComponent();
    }

    public IReadOnlyList<string> Barcodes => rows.Select(row => row.Code).ToArray();

    private void InitializeComponent()
    {
        SuspendLayout();
        ((ISupportInitialize)txtBarcode.Properties).BeginInit();
        ((ISupportInitialize)grcBarcodes).BeginInit();
        ((ISupportInitialize)grvBarcodes).BeginInit();

        FormStyler.ApplyBase(this);
        Text = "Codigos de barras";
        ClientSize = new Size(520, 390);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        lblTitle.Text = "Codigos de barras";
        lblTitle.Location = new Point(18, 16);
        lblTitle.Appearance.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblTitle.Appearance.ForeColor = BrandResources.Primary;
        lblTitle.Appearance.Options.UseFont = true;
        lblTitle.Appearance.Options.UseForeColor = true;

        lblBarcode.Text = "Codigo";
        lblBarcode.Location = new Point(20, 58);
        lblBarcode.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblBarcode.Appearance.Options.UseFont = true;

        txtBarcode.Location = new Point(90, 54);
        txtBarcode.Size = new Size(290, 22);
        txtBarcode.Properties.Appearance.Font = FormStyler.LabelFont;
        txtBarcode.Properties.Appearance.Options.UseFont = true;
        txtBarcode.KeyDown += BarcodeKeyDown;

        btnAdd.Text = "Agregar";
        btnAdd.Location = new Point(392, 52);
        btnAdd.Size = new Size(96, 26);
        btnAdd.Appearance.BackColor = BrandResources.Primary;
        btnAdd.Appearance.ForeColor = Color.White;
        btnAdd.AppearanceHovered.BackColor = BrandResources.PrimaryHover;
        btnAdd.AppearanceHovered.ForeColor = Color.White;
        btnAdd.Appearance.Options.UseBackColor = true;
        btnAdd.Appearance.Options.UseForeColor = true;
        btnAdd.AppearanceHovered.Options.UseBackColor = true;
        btnAdd.AppearanceHovered.Options.UseForeColor = true;
        btnAdd.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAdd.LookAndFeel.UseDefaultLookAndFeel = false;
        btnAdd.Click += AddBarcodeClick;

        grcBarcodes.Location = new Point(20, 95);
        grcBarcodes.MainView = grvBarcodes;
        grcBarcodes.Name = "grcBarcodes";
        grcBarcodes.Size = new Size(468, 210);
        grcBarcodes.DataSource = rows;
        grcBarcodes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvBarcodes });

        grvBarcodes.Columns.AddRange(new GridColumn[] { colBarcode });
        grvBarcodes.GridControl = grcBarcodes;
        grvBarcodes.OptionsBehavior.Editable = true;
        grvBarcodes.OptionsView.ShowGroupPanel = false;
        grvBarcodes.OptionsView.ShowIndicator = false;
        grvBarcodes.Appearance.HeaderPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grvBarcodes.Appearance.Row.Font = FormStyler.LabelFont;
        grvBarcodes.Appearance.HeaderPanel.Options.UseFont = true;
        grvBarcodes.Appearance.Row.Options.UseFont = true;

        colBarcode.Caption = "Codigo de barras";
        colBarcode.FieldName = nameof(BarcodeRow.Code);
        colBarcode.Visible = true;
        colBarcode.VisibleIndex = 0;

        btnRemove.Text = "Quitar seleccionado";
        btnRemove.Location = new Point(20, 322);
        btnRemove.Size = new Size(130, 30);
        btnRemove.Click += RemoveBarcodeClick;

        btnAccept.Text = "Aceptar";
        btnAccept.Location = new Point(282, 322);
        btnAccept.Size = new Size(96, 30);
        btnAccept.DialogResult = DialogResult.OK;
        btnAccept.Appearance.BackColor = BrandResources.Primary;
        btnAccept.Appearance.ForeColor = Color.White;
        btnAccept.AppearanceHovered.BackColor = BrandResources.PrimaryHover;
        btnAccept.AppearanceHovered.ForeColor = Color.White;
        btnAccept.Appearance.Options.UseBackColor = true;
        btnAccept.Appearance.Options.UseForeColor = true;
        btnAccept.AppearanceHovered.Options.UseBackColor = true;
        btnAccept.AppearanceHovered.Options.UseForeColor = true;
        btnAccept.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAccept.LookAndFeel.UseDefaultLookAndFeel = false;
        btnAccept.Click += AcceptClick;

        btnCancel.Text = "Cancelar";
        btnCancel.Location = new Point(392, 322);
        btnCancel.Size = new Size(96, 30);
        btnCancel.DialogResult = DialogResult.Cancel;

        AcceptButton = btnAccept;
        CancelButton = btnCancel;

        Controls.Add(lblTitle);
        Controls.Add(lblBarcode);
        Controls.Add(txtBarcode);
        Controls.Add(btnAdd);
        Controls.Add(grcBarcodes);
        Controls.Add(btnRemove);
        Controls.Add(btnAccept);
        Controls.Add(btnCancel);

        ((ISupportInitialize)grvBarcodes).EndInit();
        ((ISupportInitialize)grcBarcodes).EndInit();
        ((ISupportInitialize)txtBarcode.Properties).EndInit();
        ResumeLayout(false);
    }

    private void BarcodeKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        AddBarcode();
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    private void AddBarcodeClick(object? sender, EventArgs e)
    {
        AddBarcode();
    }

    private void AddBarcode()
    {
        var code = txtBarcode.Text.Trim();
        if (string.IsNullOrWhiteSpace(code))
        {
            XtraMessageBox.Show(this, "Ingrese un codigo de barras.", "Codigos de barras", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtBarcode.Focus();
            return;
        }

        if (rows.Any(row => row.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            XtraMessageBox.Show(this, "Este codigo de barras ya esta agregado.", "Codigos de barras", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtBarcode.SelectAll();
            txtBarcode.Focus();
            return;
        }

        rows.Add(new BarcodeRow { Code = code });
        txtBarcode.Text = string.Empty;
        txtBarcode.Focus();
    }

    private void RemoveBarcodeClick(object? sender, EventArgs e)
    {
        if (grvBarcodes.GetFocusedRow() is not BarcodeRow row)
        {
            return;
        }

        rows.Remove(row);
    }

    private void AcceptClick(object? sender, EventArgs e)
    {
        grvBarcodes.PostEditor();
        grvBarcodes.UpdateCurrentRow();

        var normalizedRows = rows
            .Where(row => !string.IsNullOrWhiteSpace(row.Code))
            .Select(row => row.Code.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(code => new BarcodeRow { Code = code })
            .ToList();

        rows.Clear();
        foreach (var row in normalizedRows)
        {
            rows.Add(row);
        }
    }

    private sealed class BarcodeRow
    {
        public string Code { get; set; } = string.Empty;
    }
}
