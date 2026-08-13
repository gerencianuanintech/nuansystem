using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.UnitMeasures;

public sealed partial class UnitMeasureEditForm : BaseEditForm
{
    private static readonly MagnitudeOption[] Magnitudes =
    [
        new("Quantity", "Cantidad"),
        new("Packaging", "Empaque"),
        new("Mass", "Masa"),
        new("Volume", "Volumen"),
        new("Length", "Longitud"),
        new("Area", "Área"),
        new("Time", "Tiempo"),
        new("Other", "Otro")
    ];

    public UnitMeasureEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public UnitMeasureEditForm(UnitMeasureItem item, bool copyMode = false)
        : this()
    {
        LoadItem(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveUnitMeasureRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese el código de la unidad de medida.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre de la unidad de medida.");
        valid &= Validator.RequireText(cmbMagnitude, "Seleccione el tipo de magnitud.");
        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveUnitMeasureRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            Optional(memDescription.Text),
            Optional(txtSymbol.Text),
            (cmbMagnitude.SelectedItem as MagnitudeOption)?.Code ?? string.Empty,
            Convert.ToInt32(spnSortOrder.Value),
            chkIsActive.Checked,
            Optional(cmbExternalSystem.Text),
            Optional(txtExternalCode.Text));
    }

    private void ConfigureForm()
    {
        cmbMagnitude.Properties.Items.AddRange(Magnitudes);
        cmbMagnitude.SelectedItem = Magnitudes[0];
        chkIsActive.Checked = true;
        cmbExternalSystem.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
    }

    private void LoadItem(UnitMeasureItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar unidad de medida" : "Unidad de medida";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        txtSymbol.Text = item.Symbol;
        cmbMagnitude.SelectedItem = Magnitudes.FirstOrDefault(option =>
            string.Equals(option.Code, item.MagnitudeCode, StringComparison.OrdinalIgnoreCase)) ?? Magnitudes[^1];
        spnSortOrder.Value = item.SortOrder;
        chkIsActive.Checked = item.IsActive;
        cmbExternalSystem.Text = item.ExternalSystem;
        txtExternalCode.Text = item.ExternalCode;
    }

    private static string? Optional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SaveUnitMeasureRequest EmptyRequest() =>
        new(string.Empty, string.Empty, null, null, "Quantity", 0, true, null, null);

    private sealed record MagnitudeOption(string Code, string Caption)
    {
        public override string ToString() => Caption;
    }
}
