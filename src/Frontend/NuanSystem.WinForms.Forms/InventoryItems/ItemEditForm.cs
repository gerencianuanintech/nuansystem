using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.InventoryItems.Models;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm : BaseEditForm
{
    private readonly ItemLookups lookups;

    public ItemEditForm()
        : this(CreateDesignLookups())
    {
    }

    public ItemEditForm(ItemLookups lookups, ItemItem? item = null, bool copyMode = false)
    {
        this.lookups = lookups;
        InitializeComponent();
        BindLookups();
        LoadItem(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(codeTextEdit, "Ingrese el codigo del item.");
        isValid &= Validator.RequireText(nameTextEdit, "Ingrese la descripcion del item.");

        if (!purchaseCheckEdit.Checked && !salesCheckEdit.Checked && !inventoryCheckEdit.Checked && !serviceCheckEdit.Checked)
        {
            Validator.SetError(inventoryCheckEdit, "Seleccione al menos una clasificacion del item.");
            isValid = false;
        }

        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = EmptyRequest() with
        {
            Code = codeTextEdit.Text.Trim(),
            Name = nameTextEdit.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(descriptionMemoEdit.Text) ? null : descriptionMemoEdit.Text.Trim(),
            ItemGroupId = ToNullableInt(itemGroupSearchLookUpEdit.EditValue),
            ItemType = itemTypeComboBoxEdit.Text,
            InventoryUnitOfMeasureId = ToNullableInt(inventoryUomSearchLookUpEdit.EditValue ?? headerUomSearchLookUpEdit.EditValue),
            PurchaseUnitOfMeasureId = ToNullableInt(purchaseUomSearchLookUpEdit.EditValue),
            SalesUnitOfMeasureId = ToNullableInt(salesUomSearchLookUpEdit.EditValue),
            IsPurchaseItem = purchaseCheckEdit.Checked,
            IsSalesItem = salesCheckEdit.Checked,
            IsInventoryItem = inventoryCheckEdit.Checked,
            ManagedBy = "None",
            Remarks = string.IsNullOrWhiteSpace(remarksMemoEdit.Text) ? null : remarksMemoEdit.Text.Trim(),
            IsActive = string.Equals(statusLookUpEdit.Text, "Activo", StringComparison.OrdinalIgnoreCase)
        };
    }

    private void LoadItem(ItemItem? item, bool copyMode)
    {
        if (item is null)
        {
            return;
        }

        Text = copyMode ? "Copiar item" : "Editar item";
        lblFooterMode.Text = copyMode ? "Modo: Copia" : "Modo: Edicion";
        lblFooterRecord.Text = copyMode ? "Registro: Copia" : $"Registro: {item.Id}";
        codeTextEdit.Text = copyMode ? string.Empty : item.Code;
        nameTextEdit.Text = item.Name;
        descriptionMemoEdit.Text = item.Description;
        itemTypeComboBoxEdit.Text = item.ItemType;
        itemGroupSearchLookUpEdit.EditValue = item.ItemGroupId;
        headerUomSearchLookUpEdit.EditValue = item.InventoryUnitOfMeasureId;
        inventoryUomSearchLookUpEdit.EditValue = item.InventoryUnitOfMeasureId;
        purchaseUomSearchLookUpEdit.EditValue = item.PurchaseUnitOfMeasureId;
        salesUomSearchLookUpEdit.EditValue = item.SalesUnitOfMeasureId;
        purchaseCheckEdit.Checked = item.IsPurchaseItem;
        salesCheckEdit.Checked = item.IsSalesItem;
        inventoryCheckEdit.Checked = item.IsInventoryItem;
        serviceCheckEdit.Checked = string.Equals(item.ItemType, "Service", StringComparison.OrdinalIgnoreCase);
        remarksMemoEdit.Text = item.Remarks;
        statusLookUpEdit.EditValue = item.IsActive ? "Activo" : "Inactivo";
    }

    private void BindLookups()
    {
        BindSearchLookup(itemGroupSearchLookUpEdit, itemGroupSearchLookUpView, lookups.ItemGroups);
        BindSearchLookup(categorySearchLookUpEdit, categorySearchLookUpView, lookups.ItemGroups);
        BindSearchLookup(subCategorySearchLookUpEdit, subCategorySearchLookUpView, lookups.ItemGroups);
        BindSearchLookup(headerUomSearchLookUpEdit, headerUomSearchLookUpView, lookups.UnitOfMeasures);
        BindSearchLookup(inventoryUomSearchLookUpEdit, inventoryUomSearchLookUpView, lookups.UnitOfMeasures);
        BindSearchLookup(purchaseUomSearchLookUpEdit, purchaseUomSearchLookUpView, lookups.UnitOfMeasures);
        BindSearchLookup(salesUomSearchLookUpEdit, salesUomSearchLookUpView, lookups.UnitOfMeasures);

        brandSearchLookUpEdit.Properties.DataSource = new[] { new DesignLookup(1, "GENERAL") };
        lineSearchLookUpEdit.Properties.DataSource = new[] { new DesignLookup(1, "GENERAL") };
        manufacturerSearchLookUpEdit.Properties.DataSource = new[] { new DesignLookup(1, "NUAN INTECH") };

        if (lookups.UnitOfMeasures.FirstOrDefault() is { } uom)
        {
            headerUomSearchLookUpEdit.EditValue = uom.Id;
            inventoryUomSearchLookUpEdit.EditValue = uom.Id;
            purchaseUomSearchLookUpEdit.EditValue = uom.Id;
            salesUomSearchLookUpEdit.EditValue = uom.Id;
        }
    }

    private static void BindSearchLookup<T>(DevExpress.XtraEditors.SearchLookUpEdit control, DevExpress.XtraGrid.Views.Grid.GridView view, IReadOnlyCollection<T> dataSource)
    {
        control.Properties.DataSource = dataSource;
        view.PopulateColumns(dataSource);
        if (view.Columns["Id"] is { } idColumn)
        {
            idColumn.Visible = false;
        }

        if (view.Columns["DisplayText"] is { } displayColumn)
        {
            displayColumn.Caption = "Descripcion";
            displayColumn.VisibleIndex = 0;
        }
    }

    private static int? ToNullableInt(object? value)
    {
        if (value is null)
        {
            return null;
        }

        return int.TryParse(value.ToString(), out var parsed) ? parsed : null;
    }

    private static SaveItemRequest EmptyRequest()
    {
        return new SaveItemRequest(string.Empty, string.Empty, null, null, "Product", null, null, null, true, true, true, null, null, "MovingAverage", "None", "EveryTransaction", null, null, 0, 0, 1, 1, true, false, null, true, [], []);
    }

    private static ItemLookups CreateDesignLookups()
    {
        return new ItemLookups(
            [new ItemGroupLookupItem(1, "GENERAL", "General")],
            [new UnitOfMeasureLookupItem(1, "UND", "Unidad")],
            [new TaxLookupItem(1, "IVA15", "IVA 15%", 0.15M)],
            [new WarehouseLookupItem(1, "PRINCIPAL", "Bodega principal")]);
    }

    private sealed record DesignLookup(int Id, string DisplayText);
}
