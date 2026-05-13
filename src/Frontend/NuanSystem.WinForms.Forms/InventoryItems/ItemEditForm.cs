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
        LoadInventoryDemoData();
        LoadPurchasesDemoData();
        LoadSalesDemoData();
        LoadCostsDemoData();
        LoadSapDemoData();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtItemCode, "Ingrese el codigo del item.");
        isValid &= Validator.RequireText(txtDescription, "Ingrese la descripcion del item.");

        if (!chkPurchaseItem.Checked && !chkSalesItem.Checked && !chkInventoryItem.Checked)
        {
            Validator.SetError(chkInventoryItem, "Seleccione al menos una clasificacion del item.");
            isValid = false;
        }

        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = EmptyRequest() with
        {
            Code = txtItemCode.Text.Trim(),
            Name = txtDescription.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(memLongDescription.Text) ? null : memLongDescription.Text.Trim(),
            ItemGroupId = ToNullableInt(sleItemGroup.EditValue),
            ItemType = lueItemType.Text,
            InventoryUnitOfMeasureId = ToNullableInt(sleInventoryUom.EditValue ?? sleHeaderUom.EditValue),
            PurchaseUnitOfMeasureId = ToNullableInt(slePurchaseUom.EditValue),
            SalesUnitOfMeasureId = ToNullableInt(sleSalesUom.EditValue),
            IsPurchaseItem = chkPurchaseItem.Checked,
            IsSalesItem = chkSalesItem.Checked,
            IsInventoryItem = chkInventoryItem.Checked,
            ManagedBy = "None",
            Remarks = string.IsNullOrWhiteSpace(memGeneralNotes.Text) ? null : memGeneralNotes.Text.Trim(),
            IsActive = string.Equals(lueStatus.Text, "Activo", StringComparison.OrdinalIgnoreCase)
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
        txtItemCode.Text = copyMode ? string.Empty : item.Code;
        txtDescription.Text = item.Name;
        memLongDescription.Text = item.Description;
        lueItemType.Text = item.ItemType;
        sleItemGroup.EditValue = item.ItemGroupId;
        sleHeaderUom.EditValue = item.InventoryUnitOfMeasureId;
        sleInventoryUom.EditValue = item.InventoryUnitOfMeasureId;
        slePurchaseUom.EditValue = item.PurchaseUnitOfMeasureId;
        sleSalesUom.EditValue = item.SalesUnitOfMeasureId;
        chkPurchaseItem.Checked = item.IsPurchaseItem;
        chkSalesItem.Checked = item.IsSalesItem;
        chkInventoryItem.Checked = item.IsInventoryItem;
        memGeneralNotes.Text = item.Remarks;
        lueStatus.EditValue = item.IsActive ? "Activo" : "Inactivo";
    }

    private void LoadInventoryDemoData()
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("WarehouseCode", typeof(string));
        table.Columns.Add("WarehouseName", typeof(string));
        table.Columns.Add("Stock", typeof(decimal));
        table.Columns.Add("Committed", typeof(decimal));
        table.Columns.Add("Ordered", typeof(decimal));
        table.Columns.Add("Available", typeof(decimal));
        table.Columns.Add("BinLocation", typeof(string));

        table.Rows.Add("01", "Principal", 900m, 80m, 50m, 770m, "Pasillo 1 - Estante A - Nivel 2");
        table.Rows.Add("02", "Sucursal Norte", 250m, 20m, 20m, 210m, "Pasillo 3 - Estante B - Nivel 1");
        table.Rows.Add("03", "Sucursal Sur", 100m, 20m, 10m, 70m, "Pasillo 2 - Estante C - Nivel 1");

        grcStock.DataSource = table;
    }

    private void LoadPurchasesDemoData()
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("Priority", typeof(int));
        table.Columns.Add("VendorName", typeof(string));
        table.Columns.Add("VendorCode", typeof(string));
        table.Columns.Add("Price", typeof(decimal));
        table.Columns.Add("Currency", typeof(string));
        table.Columns.Add("DeliveryDays", typeof(int));
        table.Columns.Add("Active", typeof(bool));

        table.Rows.Add(1, "DISTRIBUCIONES ANDINAS S.A.S.", "P000002", 1.19m, "USD", 2, true);
        table.Rows.Add(2, "IMPORTADORA DEL SUR S.A.", "P000003", 1.21m, "USD", 3, true);
        table.Rows.Add(3, "SUMINISTROS ALIMENTICIOS S.A.S.", "P000004", 1.22m, "USD", 3, true);

        grcVendors.DataSource = table;
    }

    private void LoadSalesDemoData()
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("PriceListName", typeof(string));
        table.Columns.Add("Price", typeof(decimal));
        table.Columns.Add("Currency", typeof(string));
        table.Columns.Add("Margin", typeof(decimal));
        table.Columns.Add("StartDate", typeof(System.DateTime));
        table.Columns.Add("EndDate", typeof(System.DateTime));
        table.Columns.Add("Active", typeof(bool));

        table.Rows.Add("LISTA GENERAL", 1.60m, "USD", 28.00m, new System.DateTime(2024, 1, 1), System.DBNull.Value, true);
        table.Rows.Add("LISTA MAYORISTA", 1.48m, "USD", 18.40m, new System.DateTime(2024, 1, 1), System.DBNull.Value, true);
        table.Rows.Add("LISTA DISTRIBUIDOR", 1.41m, "USD", 14.00m, new System.DateTime(2024, 1, 1), System.DBNull.Value, true);
        table.Rows.Add("LISTA PROMOCIONES", 1.29m, "USD", 4.00m, new System.DateTime(2026, 5, 1), new System.DateTime(2026, 5, 31), true);
        table.Rows.Add("LISTA INSTITUCIONAL", 1.53m, "USD", 22.00m, new System.DateTime(2024, 1, 1), System.DBNull.Value, true);

        grcPrices.DataSource = table;
    }

    private void LoadCostsDemoData()
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("CostDate", typeof(System.DateTime));
        table.Columns.Add("Document", typeof(string));
        table.Columns.Add("Vendor", typeof(string));
        table.Columns.Add("Quantity", typeof(decimal));
        table.Columns.Add("PreviousCost", typeof(decimal));
        table.Columns.Add("NewCost", typeof(decimal));
        table.Columns.Add("Currency", typeof(string));
        table.Columns.Add("UserName", typeof(string));

        table.Rows.Add(new System.DateTime(2026, 5, 10), "OC-000452", "INDUSTRIAL XYZ S.A.", 5000m, 1.22m, 1.24m, "USD", "admin");
        table.Rows.Add(new System.DateTime(2026, 4, 18), "OC-000398", "INDUSTRIAL XYZ S.A.", 5000m, 1.20m, 1.22m, "USD", "admin");
        table.Rows.Add(new System.DateTime(2026, 3, 25), "OC-000347", "AGROALIMENTOS DEL SUR S.A.", 4800m, 1.18m, 1.20m, "USD", "admin");
        table.Rows.Add(new System.DateTime(2026, 3, 1), "OC-000299", "AGROALIMENTOS DEL SUR S.A.", 5000m, 1.15m, 1.18m, "USD", "admin");
        table.Rows.Add(new System.DateTime(2026, 2, 12), "OC-000250", "AGROALIMENTOS DEL SUR S.A.", 5000m, 1.12m, 1.15m, "USD", "admin");

        grcCosts.DataSource = table;
    }

    private void LoadSapDemoData()
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("Field", typeof(string));
        table.Columns.Add("Description", typeof(string));
        table.Columns.Add("LocalValue", typeof(string));
        table.Columns.Add("SapValue", typeof(string));
        table.Columns.Add("Status", typeof(string));

        table.Rows.Add("U_NUAN_MARCA", "Marca del Producto", "FLOR", "FLOR", "Sincronizado");
        table.Rows.Add("U_NUAN_LINEA", "Linea / Familia", "ALIMENTOS", "ALIMENTOS", "Sincronizado");
        table.Rows.Add("U_NUAN_CATEGORIA", "Categoria de Producto", "GRANOS", "GRANOS", "Sincronizado");
        table.Rows.Add("U_NUAN_PRESENTACION", "Presentacion", "BOLSA 2 KG", "BOLSA 2 KG", "Sincronizado");
        table.Rows.Add("U_NUAN_ORIGEN", "Pais de Origen", "ECUADOR", "ECUADOR", "Sincronizado");

        grcSapUdf.DataSource = table;
    }

    private void BindLookups()
    {
        BindSearchLookup(sleItemGroup, grvItemGroupLookup, lookups.ItemGroups);
        BindSearchLookup(sleCategory, grvCategoryLookup, lookups.ItemGroups);
        BindSearchLookup(sleSubCategory, grvSubCategoryLookup, lookups.ItemGroups);
        BindSearchLookup(sleHeaderUom, grvHeaderUomLookup, lookups.UnitOfMeasures);
        BindSearchLookup(sleInventoryUom, grvInventoryUomLookup, lookups.UnitOfMeasures);
        BindSearchLookup(slePurchaseUom, grvPurchaseUomLookup, lookups.UnitOfMeasures);
        BindSearchLookup(sleSalesUom, grvSalesUomLookup, lookups.UnitOfMeasures);

        sleBrand.Properties.DataSource = new[] { new DesignLookup(1, "GENERAL") };
        sleLine.Properties.DataSource = new[] { new DesignLookup(1, "GENERAL") };
        sleManufacturer.Properties.DataSource = new[] { new DesignLookup(1, "NUAN INTECH") };

        if (lookups.UnitOfMeasures.FirstOrDefault() is { } uom)
        {
            sleHeaderUom.EditValue = uom.Id;
            sleInventoryUom.EditValue = uom.Id;
            slePurchaseUom.EditValue = uom.Id;
            sleSalesUom.EditValue = uom.Id;
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


