using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemTypes;

public sealed partial class ItemTypeEditForm : BaseEditForm
{
    private static readonly BehaviorOption[] Behaviors =
    [
        new("Product", "Producto"), new("Service", "Servicio"),
        new("Supply", "Insumo"), new("Asset", "Activo"), new("Kit", "Kit")
    ];

    public ItemTypeEditForm()
    {
        InitializeComponent();
        ConfigureBehaviorLookup();
        chkIsActive.Checked = true;
        chkDefaultPurchase.Checked = true;
        chkDefaultSales.Checked = true;
        chkDefaultInventory.Checked = true;
        lueBehavior.EditValue = "Product";
        Request = EmptyRequest();
    }

    public ItemTypeEditForm(ItemTypeItem item, bool copyMode = false) : this()
    {
        var editingSystemItem = item.IsSystem && !copyMode;
        Text = copyMode ? "Copiar tipo de ítem" : "Editar tipo de ítem";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        lueBehavior.EditValue = item.BehaviorCode;
        chkDefaultPurchase.Checked = item.DefaultIsPurchaseItem;
        chkDefaultSales.Checked = item.DefaultIsSalesItem;
        chkDefaultInventory.Checked = item.DefaultIsInventoryItem;
        spnSortOrder.Value = item.SortOrder;
        chkIsSystem.Checked = editingSystemItem;
        chkIsActive.Checked = item.IsActive;
        txtCode.Properties.ReadOnly = editingSystemItem;
        lueBehavior.Properties.ReadOnly = editingSystemItem;
    }

    public SaveItemTypeRequest Request { get; private set; }

    protected override bool ValidateForm()
    {
        var valid = Validator.RequireText(txtCode, "Ingrese el código.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        if (lueBehavior.EditValue is null)
        {
            Validator.SetError(lueBehavior, "Seleccione el comportamiento.");
            valid = false;
        }
        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveItemTypeRequest(
            txtCode.Text.Trim(), txtName.Text.Trim(), Normalize(memDescription.Text),
            Convert.ToString(lueBehavior.EditValue) ?? string.Empty,
            chkDefaultPurchase.Checked, chkDefaultSales.Checked, chkDefaultInventory.Checked,
            Convert.ToInt32(spnSortOrder.Value), chkIsActive.Checked);
    }

    private void ConfigureBehaviorLookup()
    {
        lueBehavior.Properties.DataSource = Behaviors;
        lueBehavior.Properties.DisplayMember = nameof(BehaviorOption.Name);
        lueBehavior.Properties.ValueMember = nameof(BehaviorOption.Code);
        lueBehavior.Properties.Columns.Clear();
        lueBehavior.Properties.Columns.Add(new LookUpColumnInfo(nameof(BehaviorOption.Name), "Comportamiento"));
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SaveItemTypeRequest EmptyRequest() => new(string.Empty, string.Empty, null, "Product", true, true, true, 0, true);
    private sealed record BehaviorOption(string Code, string Name);
}
