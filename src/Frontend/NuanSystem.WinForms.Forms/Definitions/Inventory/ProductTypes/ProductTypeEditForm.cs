using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ProductTypes;

public sealed partial class ProductTypeEditForm : BaseEditForm
{
    private static readonly NatureOption[] NatureOptions =
    [
        new("Merchandise", "Mercadería"),
        new("FinishedGood", "Producto terminado"),
        new("RawMaterial", "Materia prima"),
        new("SemiFinished", "Semielaborado"),
        new("Supply", "Insumo"),
        new("Packaging", "Empaque"),
        new("ByProduct", "Subproducto"),
        new("Other", "Otro")
    ];

    public ProductTypeEditForm()
    {
        InitializeComponent();
        ConfigureNatureLookup();
        chkIsActive.Checked = true;
        lueNature.EditValue = "Merchandise";
    }

    public ProductTypeEditForm(ProductTypeItem item, bool copyMode = false) : this()
    {
        var editingSystemItem = item.IsSystem && !copyMode;
        Text = copyMode ? "Copiar tipo de producto" : "Tipo de producto";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        lueNature.EditValue = item.NatureCode;
        spnSortOrder.Value = item.SortOrder;
        chkIsSystem.Checked = editingSystemItem;
        chkIsActive.Checked = item.IsActive;
        txtCode.Properties.ReadOnly = editingSystemItem;
        lueNature.Properties.ReadOnly = editingSystemItem;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveProductTypeRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var valid = Validator.RequireText(txtCode, "Ingrese el código del tipo de producto.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre del tipo de producto.");
        if (lueNature.EditValue is null)
        {
            Validator.SetError(lueNature, "Seleccione la naturaleza del producto.");
            valid = false;
        }

        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveProductTypeRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            Optional(memDescription.Text),
            Convert.ToString(lueNature.EditValue) ?? string.Empty,
            Convert.ToInt32(spnSortOrder.Value),
            chkIsActive.Checked);
    }

    private void ConfigureNatureLookup()
    {
        lueNature.Properties.DataSource = NatureOptions;
        lueNature.Properties.DisplayMember = nameof(NatureOption.Name);
        lueNature.Properties.ValueMember = nameof(NatureOption.Code);
        lueNature.Properties.Columns.Clear();
        lueNature.Properties.Columns.Add(new LookUpColumnInfo(nameof(NatureOption.Name), "Naturaleza"));
    }

    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SaveProductTypeRequest EmptyRequest() => new(string.Empty, string.Empty, null, "Merchandise", 0, true);
    private sealed record NatureOption(string Code, string Name);
}
