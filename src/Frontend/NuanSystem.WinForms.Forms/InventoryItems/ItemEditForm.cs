using System.ComponentModel;
using System.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.InventoryItems.Models;
using NuanSystem.WinForms.ViewModels.InventoryItems;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm : BaseEditForm
{
    private ItemLookups? lookups;
    private ItemItem? currentItem;
    private Func<string, bool>? canCreateRelatedCatalog;
    private Func<string, Form?>? relatedCatalogFormFactory;
    private Func<CancellationToken, Task<ItemLookups>>? reloadLookupsAsync;
    private bool dirtyTrackingEnabled;

    private const string UnitMeasuresFormKey = "inventory-unit-measures";
    private const string WarehousesFormKey = "inventory-warehouses";
    private const string ItemBrandsFormKey = "inventory-item-brands";
    private const string ProductTypesFormKey = "inventory-product-types";
    private const string ItemLinesFormKey = "inventory-item-lines";
    private const string ItemSubgroupsFormKey = "inventory-item-subgroups";
    private const string WarehouseLocationsFormKey = "inventory-warehouse-locations";
    private const string ReplenishmentMethodsFormKey = "inventory-replenishment-methods";

    public ItemEditForm()
    {
        Request = EmptyRequest();
        InitializeComponent();
        ConfigureRuntimeBehavior();
    }

    public ItemEditForm(
        ItemLookups lookups,
        ItemItem? item = null,
        bool copyMode = false,
        bool canCreateItemGroups = false,
        Func<SaveItemGroupRequest, Task<ItemGroupLookupItem>>? createItemGroupAsync = null,
        bool canCreateItemFamilies = false,
        Func<SaveItemFamilyRequest, Task<ItemFamilyLookupItem>>? createItemFamilyAsync = null,
        Func<string, bool>? canCreateRelatedCatalog = null,
        Func<string, Form?>? relatedCatalogFormFactory = null,
        Func<CancellationToken, Task<ItemLookups>>? reloadLookupsAsync = null)
        : this()
    {
        this.lookups = lookups;
        currentItem = copyMode ? null : item;
        this.canCreateRelatedCatalog = canCreateRelatedCatalog;
        this.relatedCatalogFormFactory = relatedCatalogFormFactory;
        this.reloadLookupsAsync = reloadLookupsAsync;

        Text = item is null || copyMode
            ? "Maestro de ítems / Artículos - Nuevo"
            : "Maestro de ítems / Artículos";

        BindLookups(lookups);
        ConfigureRelatedLookupButtons();

        if (item is null || copyMode)
        {
            LoadNewItemDefaults(item, copyMode);
        }
        else
        {
            LoadItem(item);
        }

        UpdateIntegrationWarning();
        EnableDirtyTracking();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemRequest Request { get; private set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemEditState EditState { get; private set; } = new();

    private void ConfigureRuntimeBehavior()
    {
        EnsurePresentationColumns();
        EnsurePresentationBarcodeColumns();
        EnsureWarehouseColumns();
        EnsureOperationalAlertColumns();
        EnsureAttachmentColumns();
        EnsureSapFieldColumns();
        grdSapFieldMapping.DataSource = sapFieldsTable;

        btnSave.DialogResult = DialogResult.None;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnSave.Click += (_, _) => Save();
        btnAddItemPresentation.Click -= AddItemPresentationClick;
        btnAddItemPresentation.Click += AddItemPresentationClick;
        btnUpdateItemPresentation.Click -= UpdateItemPresentationClick;
        btnUpdateItemPresentation.Click += UpdateItemPresentationClick;
        btnRemoveItemPresentation.Click -= RemoveItemPresentationClick;
        btnRemoveItemPresentation.Click += RemoveItemPresentationClick;
        btnSetMainItemPresentation.Click -= SetMainItemPresentationClick;
        btnSetMainItemPresentation.Click += SetMainItemPresentationClick;
        btnAddBarcode.Click -= AddBarcodeClick;
        btnAddBarcode.Click += AddBarcodeClick;
        btnUpdateBarcode.Click -= UpdateBarcodeClick;
        btnUpdateBarcode.Click += UpdateBarcodeClick;
        btnRemoveBarcode.Click -= RemoveBarcodeClick;
        btnRemoveBarcode.Click += RemoveBarcodeClick;
        btnSetMainBarcode.Click -= SetMainBarcodeClick;
        btnSetMainBarcode.Click += SetMainBarcodeClick;
        btnAddWarehouseStock.Click -= AddWarehouseStockClick;
        btnAddWarehouseStock.Click += AddWarehouseStockClick;
        btnUpdateWarehouseStock.Click -= UpdateWarehouseStockClick;
        btnUpdateWarehouseStock.Click += UpdateWarehouseStockClick;
        btnRemoveWarehouseStock.Click -= RemoveWarehouseStockClick;
        btnRemoveWarehouseStock.Click += RemoveWarehouseStockClick;
        btnSetMainWarehouseStock.Click -= SetMainWarehouseStockClick;
        btnSetMainWarehouseStock.Click += SetMainWarehouseStockClick;
        btnAddOperationalAlert.Click -= AddOperationalAlertClick;
        btnAddOperationalAlert.Click += AddOperationalAlertClick;
        btnUpdateOperationalAlert.Click -= UpdateOperationalAlertClick;
        btnUpdateOperationalAlert.Click += UpdateOperationalAlertClick;
        btnRemoveOperationalAlert.Click -= RemoveOperationalAlertClick;
        btnRemoveOperationalAlert.Click += RemoveOperationalAlertClick;
        btnClearOperationalAlert.Click -= ClearOperationalAlertsClick;
        btnClearOperationalAlert.Click += ClearOperationalAlertsClick;
        btnAddAttachment.Click -= AddAttachmentClick;
        btnAddAttachment.Click += AddAttachmentClick;
        btnUpdateAttachment.Click -= UpdateAttachmentClick;
        btnUpdateAttachment.Click += UpdateAttachmentClick;
        btnRemoveAttachment.Click -= RemoveAttachmentClick;
        btnRemoveAttachment.Click += RemoveAttachmentClick;
        btnSetMainAttachment.Click -= SetMainAttachmentClick;
        btnSetMainAttachment.Click += SetMainAttachmentClick;
        btnOpenAttachment.Click -= OpenAttachmentClick;
        btnOpenAttachment.Click += OpenAttachmentClick;
        btnDownloadAttachment.Click -= DownloadAttachmentClick;
        btnDownloadAttachment.Click += DownloadAttachmentClick;
        btnAddSapField.Click -= AddSapFieldClick;
        btnAddSapField.Click += AddSapFieldClick;
        btnUpdateSapField.Click -= UpdateSapFieldClick;
        btnUpdateSapField.Click += UpdateSapFieldClick;
        btnRemoveSapField.Click -= RemoveSapFieldClick;
        btnRemoveSapField.Click += RemoveSapFieldClick;
        btnClearSapFields.Click -= ClearSapFieldsClick;
        btnClearSapFields.Click += ClearSapFieldsClick;
    }

    private void EnableDirtyTracking()
    {
        foreach (var editor in EnumerateControls(this).OfType<BaseEdit>())
        {
            editor.EditValueChanged += EditorEditValueChanged;
        }

        foreach (var table in GetEditableTables())
        {
            table.RowChanged += EditableTableChanged;
            table.RowDeleted += EditableTableChanged;
            table.TableNewRow += EditableTableNewRow;
        }

        txtSapLastError.EditValueChanged += (_, _) => UpdateIntegrationWarning();
        dirtyTrackingEnabled = true;
        lblUnsavedIndicator.Visible = false;
        lblValidationIndicator.Visible = false;
    }

    private static IEnumerable<Control> EnumerateControls(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            yield return child;

            foreach (var descendant in EnumerateControls(child))
            {
                yield return descendant;
            }
        }
    }

    private IEnumerable<DataTable> GetEditableTables()
    {
        yield return itemPresentationsTable;
        yield return presentationBarcodesTable;
        yield return warehouseStockTable;
        yield return purchasesPresentationsTable;
        yield return itemSuppliersTable;
        yield return costComponentsTable;
        yield return taxMatrixTable;
        yield return variantAttributesTable;
        yield return registeredVariantsTable;
        yield return sapCompanySyncTable;
        yield return sapFieldsTable;
        yield return attachmentsTable;
        yield return operationalAlertsTable;
        yield return allowedLocationsTable;
    }

    private void EditorEditValueChanged(object? sender, EventArgs e)
    {
        MarkAsDirty();
    }

    private void EditableTableChanged(object? sender, DataRowChangeEventArgs e)
    {
        MarkAsDirty();
    }

    private void EditableTableNewRow(object? sender, DataTableNewRowEventArgs e)
    {
        MarkAsDirty();
    }

    private void MarkAsDirty()
    {
        if (dirtyTrackingEnabled)
        {
            lblUnsavedIndicator.Visible = true;
        }
    }

    private void UpdateIntegrationWarning()
    {
        var hasWarning = !string.IsNullOrWhiteSpace(txtSapLastError.Text);
        tabSap.Text = hasWarning ? "Integración ⚠" : "Integración";
        tabSap.Appearance.Header.ForeColor = hasWarning
            ? BrandResources.WarningText
            : Color.Empty;
        tabSap.Appearance.Header.Options.UseForeColor = hasWarning;
    }

    private void ConfigureRelatedLookupButtons()
    {
        RegisterRelatedLookup(lueBrand, ItemBrandsFormKey);
        RegisterRelatedLookup(lueBaseUnit, UnitMeasuresFormKey);
        RegisterRelatedLookup(lueInventoryUnit, UnitMeasuresFormKey);
        RegisterRelatedLookup(luePurchaseUnit, UnitMeasuresFormKey);
        RegisterRelatedLookup(lueSalesUnit, UnitMeasuresFormKey);
        RegisterRelatedLookup(lueWeightUnit, UnitMeasuresFormKey);
        RegisterRelatedLookup(lueVolumeUnit, UnitMeasuresFormKey);
        RegisterRelatedLookup(lueProductType, ProductTypesFormKey);
        RegisterRelatedLookup(lueLine, ItemLinesFormKey);
        RegisterRelatedLookup(lueSubGroup, ItemSubgroupsFormKey);
        RegisterRelatedLookup(lueReplenishmentMethod, ReplenishmentMethodsFormKey);
        RegisterRelatedLookup(slueMainWarehouse, WarehousesFormKey);
        RegisterRelatedLookup(slueDefaultBinLocation, WarehouseLocationsFormKey);
    }

    private void RegisterRelatedLookup(ButtonEdit editor, string formKey)
    {
        var button = editor.Properties.Buttons
            .Cast<EditorButton>()
            .FirstOrDefault(editorButton => editorButton.Kind == ButtonPredefines.Plus);

        if (button is null)
        {
            return;
        }

        var canOpen = CanOpenRelatedCatalog(formKey);
        button.Enabled = canOpen;
        button.Visible = canOpen;

        editor.Properties.ButtonClick += async (_, args) =>
        {
            if (!ReferenceEquals(args.Button, button))
            {
                return;
            }

            await OpenRelatedCatalogAsync(formKey);
        };
    }

    private bool CanOpenRelatedCatalog(string formKey)
    {
        return relatedCatalogFormFactory is not null
            && (canCreateRelatedCatalog?.Invoke(formKey) ?? false);
    }

    private async Task OpenRelatedCatalogAsync(string formKey)
    {
        if (!CanOpenRelatedCatalog(formKey))
        {
            return;
        }

        try
        {
            using var form = relatedCatalogFormFactory?.Invoke(formKey);
            if (form is null)
            {
                return;
            }

            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);

            if (reloadLookupsAsync is not null)
            {
                lookups = await reloadLookupsAsync(CancellationToken.None);
                BindLookups(lookups);
            }
        }
        catch
        {
            XtraMessageBox.Show(
                this,
                "No fue posible abrir o actualizar el catalogo relacionado.",
                "Maestro de ítems",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void BindLookups(ItemLookups source)
    {
        BindFixedLookup(lueItemType, new[]
        {
            new LookupOption("Product", "Producto terminado"),
            new LookupOption("Merchandise", "Mercaderia"),
            new LookupOption("Service", "Servicio"),
            new LookupOption("Supply", "Insumo"),
            new LookupOption("Kit", "Kit")
        });

        BindFixedLookup(lueProductType, new[]
        {
            new LookupOption("Merchandise", "Mercaderia"),
            new LookupOption("FinishedProduct", "Producto terminado"),
            new LookupOption("RawMaterial", "Materia prima"),
            new LookupOption("Service", "Servicio"),
            new LookupOption("Kit", "Kit")
        });

        BindFixedLookup(lueValuationMethod, new[]
        {
            new LookupOption("MovingAverage", "Promedio ponderado"),
            new LookupOption("Standard", "Costo estandar"),
            new LookupOption("FIFO", "FIFO"),
            new LookupOption("BatchSerial", "Por lote / serie")
        });

        BindFixedLookup(lueNegativeStockPolicy, new[]
        {
            new LookupOption("None", "No permitido"),
            new LookupOption("Warning", "Permitir con advertencia"),
            new LookupOption("Allowed", "Permitido")
        });

        BindFixedLookup(lueOrigin, new[]
        {
            new LookupOption("Local", "Local"),
            new LookupOption("Imported", "Importado"),
            new LookupOption("Mixed", "Mixto")
        });

        BindFixedLookup(lueSupplyMethod, new[]
        {
            new LookupOption("Purchase", "Comprar"),
            new LookupOption("Produce", "Fabricar"),
            new LookupOption("Transfer", "Transferir"),
            new LookupOption("Consignment", "Consignacion")
        });

        BindLookup(lueItemGroup, source.ItemGroups, "DisplayText", "Id");
        BindLookup(lueItemFamily, source.ItemFamilies, "DisplayText", "Id");
        BindCatalogLookup(lueBrand, source.Brands, nameof(GeneralInventoryCatalogLookupItem.Id));
        BindCatalogLookup(lueProductType, source.ProductTypes, keepCurrentWhenEmpty: true);
        BindCatalogLookup(lueLine, source.ItemLines);
        BindCatalogLookup(lueSubGroup, source.ItemSubgroups);
        BindCatalogLookup(lueReplenishmentMethod, source.ReplenishmentMethods);
        BindLookup(lueBaseUnit, source.UnitOfMeasures, "DisplayText", "Id");
        BindLookup(lueInventoryUnit, source.UnitOfMeasures, "DisplayText", "Id");
        BindLookup(luePurchaseUnit, source.UnitOfMeasures, "DisplayText", "Id");
        BindLookup(lueSalesUnit, source.UnitOfMeasures, "DisplayText", "Id");
        BindLookup(lueWeightUnit, source.UnitOfMeasures, "DisplayText", "Code");
        BindLookup(lueVolumeUnit, source.UnitOfMeasures, "DisplayText", "Code");
        BindLookup(luePurchaseVat, source.Taxes, "DisplayText", "Id");
        BindLookup(lueTaxesSalesVat, source.Taxes, "DisplayText", "Id");
        BindWarehouseLookup(source.Warehouses);
        BindSearchCatalogLookup(slueDefaultBinLocation, source.WarehouseLocations);
    }

    private static void BindLookup<T>(LookUpEdit lookup, IReadOnlyCollection<T> items, string displayMember, string valueMember)
    {
        lookup.Properties.DataSource = items.ToList();
        lookup.Properties.DisplayMember = displayMember;
        lookup.Properties.ValueMember = valueMember;
        lookup.Properties.NullText = string.Empty;
        ConfigureLookupColumns(lookup, typeof(T));
    }

    private static void BindCatalogLookup(
        LookUpEdit lookup,
        IReadOnlyCollection<GeneralInventoryCatalogLookupItem> items,
        string valueMember = nameof(GeneralInventoryCatalogLookupItem.Code),
        bool keepCurrentWhenEmpty = false)
    {
        if (items.Count == 0 && keepCurrentWhenEmpty)
        {
            return;
        }

        BindLookup(
            lookup,
            items,
            nameof(GeneralInventoryCatalogLookupItem.DisplayText),
            valueMember);
    }

    private static void BindFixedLookup(LookUpEdit lookup, IReadOnlyCollection<LookupOption> items)
    {
        lookup.Properties.DataSource = items.ToList();
        lookup.Properties.DisplayMember = nameof(LookupOption.Display);
        lookup.Properties.ValueMember = nameof(LookupOption.Value);
        lookup.Properties.NullText = string.Empty;
        lookup.Properties.ShowHeader = false;
    }

    private static void ConfigureLookupColumns(LookUpEdit lookup, Type itemType)
    {
        lookup.Properties.Columns.Clear();

        if (itemType.GetProperty("Code") is not null)
        {
            lookup.Properties.Columns.Add(new LookUpColumnInfo("Code", "Codigo", 80));
        }

        if (itemType.GetProperty("Name") is not null)
        {
            lookup.Properties.Columns.Add(new LookUpColumnInfo("Name", "Nombre", 180));
        }

        lookup.Properties.ShowHeader = lookup.Properties.Columns.Count > 1;
    }

    private static void SetLookupValue(LookUpEdit lookup, object? value)
    {
        if (value is null)
        {
            lookup.EditValue = null;
            return;
        }

        lookup.EditValue = value;
    }

    private void BindWarehouseLookup(IReadOnlyCollection<WarehouseLookupItem> warehouses)
    {
        slueMainWarehouse.Properties.DataSource = warehouses.ToList();
        slueMainWarehouse.Properties.DisplayMember = nameof(WarehouseLookupItem.DisplayText);
        slueMainWarehouse.Properties.ValueMember = nameof(WarehouseLookupItem.Id);
        slueMainWarehouse.Properties.NullText = string.Empty;
        ConfigureSearchLookupGrid(slueMainWarehouse);
    }

    private static void BindSearchCatalogLookup(
        SearchLookUpEdit lookup,
        IReadOnlyCollection<GeneralInventoryCatalogLookupItem> items)
    {
        lookup.Properties.DataSource = items.ToList();
        lookup.Properties.DisplayMember = nameof(GeneralInventoryCatalogLookupItem.DisplayText);
        lookup.Properties.ValueMember = nameof(GeneralInventoryCatalogLookupItem.Code);
        lookup.Properties.NullText = string.Empty;
        ConfigureSearchLookupGrid(lookup);
    }

    private static void ConfigureSearchLookupGrid(SearchLookUpEdit lookup)
    {
        var view = lookup.Properties.PopupView;
        if (view is null)
        {
            return;
        }

        view.Columns.Clear();
        view.Columns.AddVisible("Code", "Codigo");
        view.Columns.AddVisible("Name", "Nombre");
        if (view is GridView gridView)
        {
            gridView.OptionsView.ShowGroupPanel = false;
        }
    }

    private void LoadNewItemDefaults(ItemItem? sourceItem, bool copyMode)
    {
        txtItemCode.Text = string.Empty;
        txtDescription.Text = sourceItem?.Name ?? string.Empty;
        txtCommercialName.Text = sourceItem?.Name ?? string.Empty;

        lueItemType.EditValue = sourceItem?.ItemType ?? "Product";
        lueItemGroup.EditValue = sourceItem?.ItemGroupId;
        lueItemFamily.EditValue = sourceItem?.ItemFamilyId;
        lueBaseUnit.EditValue = sourceItem?.InventoryUnitOfMeasureId;
        lueInventoryUnit.EditValue = sourceItem?.InventoryUnitOfMeasureId;
        luePurchaseUnit.EditValue = sourceItem?.PurchaseUnitOfMeasureId;
        lueSalesUnit.EditValue = sourceItem?.SalesUnitOfMeasureId;
        luePurchaseVat.EditValue = sourceItem?.PurchaseTaxId;
        lueTaxesSalesVat.EditValue = sourceItem?.SalesTaxId;
        lueValuationMethod.EditValue = sourceItem?.ValuationMethod ?? "MovingAverage";
        lueNegativeStockPolicy.EditValue = sourceItem?.AllowSaleWithoutStock == true ? "Allowed" : "None";

        tglPurchaseActive.IsOn = sourceItem?.IsPurchaseItem ?? true;
        tglSalesActive.IsOn = sourceItem?.IsSalesItem ?? true;
        tglAffectsInventory.IsOn = sourceItem?.IsInventoryItem ?? true;
        tglGeneralAllowDiscount.IsOn = sourceItem?.AllowDiscount ?? true;

        spnBaseSalesPrice.Value = sourceItem?.BaseSalesPrice ?? 0;
        spnAnalysisBasePrice.Value = sourceItem?.BaseSalesPrice ?? 0;
        spnAverageCost.Value = sourceItem?.ReferenceCost ?? 0;
        spnLastCost.Value = sourceItem?.ReferenceCost ?? 0;
        memGeneralNotes.Text = sourceItem?.Remarks ?? string.Empty;

        itemPresentationsTable.Clear();
        warehouseStockTable.Clear();

        if (copyMode && sourceItem is not null)
        {
            LoadBarcodes(sourceItem.Barcodes);
            LoadWarehouses(sourceItem.Warehouses);
        }

        SetActiveBadge(true);
        BuildRequest();
    }

    private void LoadItem(ItemItem item)
    {
        txtItemCode.Text = item.Code;
        txtDescription.Text = item.Name;
        txtCommercialName.Text = item.Name;
        lueItemType.EditValue = item.ItemType;
        lueItemGroup.EditValue = item.ItemGroupId;
        lueItemFamily.EditValue = item.ItemFamilyId;
        lueBaseUnit.EditValue = item.InventoryUnitOfMeasureId;
        lueInventoryUnit.EditValue = item.InventoryUnitOfMeasureId;
        luePurchaseUnit.EditValue = item.PurchaseUnitOfMeasureId;
        lueSalesUnit.EditValue = item.SalesUnitOfMeasureId;
        luePurchaseVat.EditValue = item.PurchaseTaxId;
        lueTaxesSalesVat.EditValue = item.SalesTaxId;
        lueValuationMethod.EditValue = item.ValuationMethod;
        lueNegativeStockPolicy.EditValue = item.AllowSaleWithoutStock ? "Allowed" : "None";

        tglPurchaseActive.IsOn = item.IsPurchaseItem;
        tglSalesActive.IsOn = item.IsSalesItem;
        tglAffectsInventory.IsOn = item.IsInventoryItem;
        tglGeneralAllowDiscount.IsOn = item.AllowDiscount;
        tglGeneralBatchManaged.IsOn = item.ManagedBy.Equals("Batch", StringComparison.OrdinalIgnoreCase);
        tglGeneralSerialManaged.IsOn = item.ManagedBy.Equals("Serial", StringComparison.OrdinalIgnoreCase);

        spnBaseSalesPrice.Value = item.BaseSalesPrice;
        spnAnalysisBasePrice.Value = item.BaseSalesPrice;
        spnAverageCost.Value = item.ReferenceCost;
        spnLastCost.Value = item.ReferenceCost;
        memGeneralNotes.Text = item.Remarks ?? string.Empty;

        LoadBarcodes(item.Barcodes);
        LoadWarehouses(item.Warehouses);
        ApplyMasterData(item.MasterData);
        SetActiveBadge(item.IsActive);
        BuildRequest();
    }

    private void LoadBarcodes(IReadOnlyCollection<ItemBarcodeItem> barcodes)
    {
        itemPresentationsTable.Clear();

        foreach (var barcode in barcodes.OrderByDescending(x => x.IsMain).ThenBy(x => x.Id))
        {
            var unitCode = ResolveUnitCode(barcode.UnitOfMeasureId);
            itemPresentationsTable.Rows.Add(
                unitCode is null ? "Presentacion" : $"{unitCode} ({barcode.ConversionFactor:0.##})",
                unitCode ?? string.Empty,
                barcode.ConversionFactor,
                barcode.Barcode,
                true,
                true,
                true,
                barcode.IsMain,
                barcode.IsActive);
        }
    }

    private void LoadWarehouses(IReadOnlyCollection<ItemWarehouseItem> warehouses)
    {
        warehouseStockTable.Clear();

        foreach (var warehouse in warehouses.OrderByDescending(x => x.IsDefaultWarehouse).ThenBy(x => x.WarehouseCode))
        {
            warehouseStockTable.Rows.Add(
                warehouse.WarehouseCode ?? string.Empty,
                warehouse.WarehouseName ?? string.Empty,
                0m,
                0m,
                0m,
                0m,
                warehouse.MinimumStock,
                warehouse.MaximumStock,
                warehouse.ReorderPoint,
                warehouse.IsActive ? "Activo" : "Inactivo",
                warehouse.WarehouseId,
                warehouse.RequiredStock,
                warehouse.DefaultLocationCode ?? string.Empty,
                warehouse.WarehouseCost,
                warehouse.IsDefaultWarehouse,
                warehouse.IsLocked);

            if (warehouse.IsDefaultWarehouse)
            {
                slueMainWarehouse.EditValue = warehouse.WarehouseId;
            }
        }
    }

    private void AddItemPresentationClick(object? sender, EventArgs e)
    {
        if (!TryGetUnits(out var unitLookups))
        {
            return;
        }

        using var dialog = new ItemPresentationEditDialog(unitLookups);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (!ValidatePresentationRow(dialog.Row))
        {
            return;
        }

        if (dialog.Row.IsMain)
        {
            ClearMainPresentation();
        }

        AddPresentationRow(dialog.Row);
    }

    private void UpdateItemPresentationClick(object? sender, EventArgs e)
    {
        if (!TryGetUnits(out var unitLookups))
        {
            return;
        }

        var dataRow = GetFocusedPresentationDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione una presentacion para actualizar.");
            return;
        }

        var originalBarcode = Convert.ToString(dataRow["CodigoBarras"]);
        using var dialog = new ItemPresentationEditDialog(unitLookups, CreatePresentationRow(dataRow));
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (!ValidatePresentationRow(dialog.Row, originalBarcode))
        {
            return;
        }

        if (dialog.Row.IsMain)
        {
            ClearMainPresentation(dataRow);
        }

        ApplyPresentationRow(dataRow, dialog.Row);
    }

    private void RemoveItemPresentationClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedPresentationDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione una presentacion para quitar.");
            return;
        }

        var answer = XtraMessageBox.Show(
            this,
            "Desea quitar la presentacion seleccionada?",
            "Presentaciones",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        itemPresentationsTable.Rows.Remove(dataRow);
    }

    private void SetMainItemPresentationClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedPresentationDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione una presentacion para marcar como principal.");
            return;
        }

        ClearMainPresentation(dataRow);
        dataRow["Principal"] = true;
    }

    private void AddBarcodeClick(object? sender, EventArgs e)
    {
        if (!TryGetUnits(out var unitLookups))
        {
            return;
        }

        using var dialog = new ItemBarcodeEditDialog(unitLookups);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (!ValidateBarcodeRow(dialog.Row))
        {
            return;
        }

        if (dialog.Row.IsMain)
        {
            ClearMainBarcode();
        }

        AddBarcodeRow(dialog.Row);
    }

    private void UpdateBarcodeClick(object? sender, EventArgs e)
    {
        if (!TryGetUnits(out var unitLookups))
        {
            return;
        }

        var dataRow = GetFocusedBarcodeDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione un codigo de barras para actualizar.");
            return;
        }

        var originalBarcode = Convert.ToString(dataRow["CodigoBarras"]);
        using var dialog = new ItemBarcodeEditDialog(unitLookups, CreateBarcodeRow(dataRow));
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (!ValidateBarcodeRow(dialog.Row, originalBarcode))
        {
            return;
        }

        if (dialog.Row.IsMain)
        {
            ClearMainBarcode(dataRow);
        }

        ApplyBarcodeRow(dataRow, dialog.Row);
    }

    private void RemoveBarcodeClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedBarcodeDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione un codigo de barras para quitar.");
            return;
        }

        var answer = XtraMessageBox.Show(
            this,
            "Desea quitar el codigo de barras seleccionado?",
            "Codigos de barras",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        presentationBarcodesTable.Rows.Remove(dataRow);
    }

    private void SetMainBarcodeClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedBarcodeDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione un codigo de barras para marcar como principal.");
            return;
        }

        ClearMainBarcode(dataRow);
        dataRow["Principal"] = true;
    }

    private void AddWarehouseStockClick(object? sender, EventArgs e)
    {
        if (!TryGetWarehouses(out var warehouseLookups))
        {
            return;
        }

        using var dialog = new ItemWarehouseEditDialog(warehouseLookups);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (!ValidateWarehouseRow(dialog.Row))
        {
            return;
        }

        if (dialog.Row.IsDefaultWarehouse)
        {
            ClearMainWarehouse();
        }

        AddWarehouseRow(dialog.Row);
    }

    private void UpdateWarehouseStockClick(object? sender, EventArgs e)
    {
        if (!TryGetWarehouses(out var warehouseLookups))
        {
            return;
        }

        var dataRow = GetFocusedWarehouseDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione una bodega para actualizar.");
            return;
        }

        var originalWarehouseId = ToInt(dataRow["WarehouseId"]);
        using var dialog = new ItemWarehouseEditDialog(warehouseLookups, CreateWarehouseRow(dataRow));
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (!ValidateWarehouseRow(dialog.Row, originalWarehouseId))
        {
            return;
        }

        if (dialog.Row.IsDefaultWarehouse)
        {
            ClearMainWarehouse(dataRow);
        }

        ApplyWarehouseRow(dataRow, dialog.Row);
    }

    private void RemoveWarehouseStockClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedWarehouseDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione una bodega para quitar.");
            return;
        }

        var answer = XtraMessageBox.Show(
            this,
            "Desea quitar la bodega seleccionada?",
            "Bodegas",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        warehouseStockTable.Rows.Remove(dataRow);
    }

    private void SetMainWarehouseStockClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedWarehouseDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione una bodega para marcar como principal.");
            return;
        }

        ClearMainWarehouse(dataRow);
        dataRow["Principal"] = true;
        slueMainWarehouse.EditValue = ToInt(dataRow["WarehouseId"]);
    }

    protected override bool ValidateForm()
    {
        var isValid = true;

        if (string.IsNullOrWhiteSpace(txtItemCode.Text))
        {
            Validator.SetError(txtItemCode, "Ingrese el código del artículo.");
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(txtDescription.Text))
        {
            Validator.SetError(txtDescription, "Ingrese la descripción del artículo.");
            isValid = false;
        }

        if (lueItemType.EditValue is null)
        {
            Validator.SetError(lueItemType, "Seleccione el tipo de ítem.");
            isValid = false;
        }

        if (lueBaseUnit.EditValue is null && tglAffectsInventory.IsOn)
        {
            Validator.SetError(lueBaseUnit, "Seleccione la unidad base del artículo.");
            isValid = false;
        }

        lblValidationIndicator.Visible = !isValid;
        if (!isValid)
        {
            tabMain.SelectedTabPage = tabGeneral;
        }

        return isValid;
    }

    private bool ValidatePresentationRow(ItemPresentationRow row, string? originalBarcode = null)
    {
        if (!string.IsNullOrWhiteSpace(row.Barcode))
        {
            var exists = itemPresentationsTable
                .AsEnumerable()
                .Any(dataRow =>
                    !string.Equals(Convert.ToString(dataRow["CodigoBarras"]), originalBarcode, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(Convert.ToString(dataRow["CodigoBarras"]), row.Barcode, StringComparison.OrdinalIgnoreCase)) ||
                presentationBarcodesTable
                    .AsEnumerable()
                    .Any(dataRow =>
                        !string.Equals(Convert.ToString(dataRow["CodigoBarras"]), originalBarcode, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(Convert.ToString(dataRow["CodigoBarras"]), row.Barcode, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                ShowValidationMessage("El codigo de barra ya esta registrado en otra presentacion.");
                return false;
            }
        }

        return true;
    }

    private bool ValidateBarcodeRow(ItemBarcodeRow row, string? originalBarcode = null)
    {
        var exists = presentationBarcodesTable
            .AsEnumerable()
            .Any(dataRow =>
                !string.Equals(Convert.ToString(dataRow["CodigoBarras"]), originalBarcode, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Convert.ToString(dataRow["CodigoBarras"]), row.Barcode, StringComparison.OrdinalIgnoreCase)) ||
            itemPresentationsTable
                .AsEnumerable()
                .Any(dataRow =>
                    !string.Equals(Convert.ToString(dataRow["CodigoBarras"]), originalBarcode, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(Convert.ToString(dataRow["CodigoBarras"]), row.Barcode, StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            ShowValidationMessage("El codigo de barras ya esta registrado.");
            return false;
        }

        return true;
    }

    private bool ValidateWarehouseRow(ItemWarehouseRow row, int? originalWarehouseId = null)
    {
        var exists = warehouseStockTable
            .AsEnumerable()
            .Any(dataRow =>
                ToInt(dataRow["WarehouseId"]) != originalWarehouseId &&
                ToInt(dataRow["WarehouseId"]) == row.WarehouseId);

        if (exists)
        {
            ShowValidationMessage("La bodega ya esta registrada para este item.");
            return false;
        }

        return true;
    }

    private void AddOperationalAlertClick(object? sender, EventArgs e)
    {
        using var dialog = new ItemOperationalAlertEditDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        AddOperationalAlertRow(dialog.Row);
    }

    private void UpdateOperationalAlertClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedOperationalAlertDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione una alerta operativa para actualizar.");
            return;
        }

        using var dialog = new ItemOperationalAlertEditDialog(CreateOperationalAlertRow(dataRow));
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        ApplyOperationalAlertRow(dataRow, dialog.Row);
    }

    private void RemoveOperationalAlertClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedOperationalAlertDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione una alerta operativa para quitar.");
            return;
        }

        operationalAlertsTable.Rows.Remove(dataRow);
    }

    private void ClearOperationalAlertsClick(object? sender, EventArgs e)
    {
        operationalAlertsTable.Rows.Clear();
    }

    private void AddAttachmentClick(object? sender, EventArgs e)
    {
        using var dialog = new ItemAttachmentEditDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (dialog.Row.IsMain)
        {
            ClearMainAttachment();
        }

        AddAttachmentRow(dialog.Row);
    }

    private void UpdateAttachmentClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedAttachmentDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione un anexo para actualizar.");
            return;
        }

        using var dialog = new ItemAttachmentEditDialog(CreateAttachmentRow(dataRow));
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (dialog.Row.IsMain)
        {
            ClearMainAttachment(dataRow);
        }

        ApplyAttachmentRow(dataRow, dialog.Row);
    }

    private void RemoveAttachmentClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedAttachmentDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione un anexo para quitar.");
            return;
        }

        attachmentsTable.Rows.Remove(dataRow);
    }

    private void SetMainAttachmentClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedAttachmentDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione un anexo para marcar como principal.");
            return;
        }

        ClearMainAttachment(dataRow);
        dataRow["Principal"] = true;
    }

    private void OpenAttachmentClick(object? sender, EventArgs e)
    {
        ShowValidationMessage("La apertura de archivos se habilitara cuando el almacenamiento de anexos este conectado.");
    }

    private void DownloadAttachmentClick(object? sender, EventArgs e)
    {
        ShowValidationMessage("La descarga de archivos se habilitara cuando el almacenamiento de anexos este conectado.");
    }

    private void AddSapFieldClick(object? sender, EventArgs e)
    {
        using var dialog = new ItemSapFieldMappingEditDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        AddSapFieldRow(dialog.Row);
    }

    private void UpdateSapFieldClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedSapFieldDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione un campo SAP para actualizar.");
            return;
        }

        using var dialog = new ItemSapFieldMappingEditDialog(CreateSapFieldRow(dataRow));
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        ApplySapFieldRow(dataRow, dialog.Row);
    }

    private void RemoveSapFieldClick(object? sender, EventArgs e)
    {
        var dataRow = GetFocusedSapFieldDataRow();
        if (dataRow is null)
        {
            ShowValidationMessage("Seleccione un campo SAP para quitar.");
            return;
        }

        sapFieldsTable.Rows.Remove(dataRow);
    }

    private void ClearSapFieldsClick(object? sender, EventArgs e)
    {
        sapFieldsTable.Rows.Clear();
    }

    private void ShowValidationMessage(string message)
    {
        XtraMessageBox.Show(this, message, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    protected override void BuildRequest()
    {
        var inventoryUnitId = GetLookupInt(lueInventoryUnit) ?? GetLookupInt(lueBaseUnit);
        var purchaseUnitId = GetLookupInt(luePurchaseUnit) ?? inventoryUnitId;
        var salesUnitId = GetLookupInt(lueSalesUnit) ?? inventoryUnitId;
        var price = spnBaseSalesPrice.Value > 0 ? spnBaseSalesPrice.Value : spnAnalysisBasePrice.Value;
        var cost = spnAverageCost.Value > 0 ? spnAverageCost.Value : spnLastCost.Value;
        var mainFactor = GetMainPresentationFactor();
        EditState = BuildEditState();

        Request = new SaveItemRequest(
            txtItemCode.Text.Trim(),
            txtDescription.Text.Trim(),
            NullIfWhiteSpace(txtCommercialName.Text),
            GetLookupInt(lueItemGroup),
            GetLookupInt(lueItemFamily),
            GetLookupString(lueItemType) ?? "Product",
            inventoryUnitId,
            purchaseUnitId,
            salesUnitId,
            tglPurchaseActive.IsOn,
            tglSalesActive.IsOn,
            tglAffectsInventory.IsOn,
            GetLookupInt(luePurchaseVat),
            GetLookupInt(lueTaxesSalesVat),
            GetLookupString(lueValuationMethod) ?? "MovingAverage",
            ResolveManagedBy(),
            "EveryTransaction",
            NullIfWhiteSpace(slueSupplierSku.Text),
            NullIfWhiteSpace(txtManufacturerReference.Text),
            price,
            cost,
            mainFactor,
            mainFactor,
            tglGeneralAllowDiscount.IsOn || tglAllowSalesDiscount.IsOn,
            string.Equals(GetLookupString(lueNegativeStockPolicy), "Allowed", StringComparison.OrdinalIgnoreCase),
            NullIfWhiteSpace(memGeneralNotes.Text),
            currentItem?.IsActive ?? true,
            BuildBarcodeRequests(),
            BuildWarehouseRequests(),
            ToMasterData(EditState));
    }

    private ItemEditState BuildEditState()
    {
        var inventoryUnitId = GetLookupInt(lueInventoryUnit) ?? GetLookupInt(lueBaseUnit);
        var purchaseUnitId = GetLookupInt(luePurchaseUnit) ?? inventoryUnitId;
        var salesUnitId = GetLookupInt(lueSalesUnit) ?? inventoryUnitId;

        var state = new ItemEditState
        {
            Header = new ItemHeaderState
            {
                Code = txtItemCode.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                CommercialName = NullIfWhiteSpace(txtCommercialName.Text),
                ItemType = GetLookupString(lueItemType) ?? "Product",
                ItemGroupId = GetLookupInt(lueItemGroup),
                ItemFamilyId = GetLookupInt(lueItemFamily),
                BrandId = GetLookupInt(lueBrand),
                BaseUnitOfMeasureId = GetLookupInt(lueBaseUnit),
                IsActive = currentItem?.IsActive ?? true,
                AverageCost = spnAverageCost.Value,
                SalesPrice = spnBaseSalesPrice.Value,
                LastPurchaseDate = ToDate(DateTime.Today),
                SapStatus = lblKpiSapValue.Text.Trim()
            },
            General = new ItemGeneralState
            {
                AlternateCode = NullIfWhiteSpace(txtAlternateCode.Text),
                SupplierSku = NullIfWhiteSpace(slueSupplierSku.Text),
                LongDescription = NullIfWhiteSpace(memLongDescription.Text),
                ProductType = GetLookupString(lueProductType),
                Origin = GetLookupString(lueOrigin),
                Line = GetLookupString(lueLine),
                SubGroup = GetLookupString(lueSubGroup),
                Model = NullIfWhiteSpace(txtModel.Text),
                Reference = NullIfWhiteSpace(txtReference.Text),
                SalesActive = tglSalesActive.IsOn,
                PurchaseActive = tglPurchaseActive.IsOn,
                ManageInventory = tglAffectsInventory.IsOn,
                IsService = string.Equals(GetLookupString(lueItemType), "Service", StringComparison.OrdinalIgnoreCase),
                IsKit = string.Equals(GetLookupString(lueProductType), "Kit", StringComparison.OrdinalIgnoreCase),
                BatchManaged = tglGeneralBatchManaged.IsOn,
                SerialManaged = tglGeneralSerialManaged.IsOn,
                Perishable = tglGeneralPerishable.IsOn,
                ExpirationManaged = tglGeneralExpirationManaged.IsOn,
                RequiresScale = tglGeneralRequiresScale.IsOn,
                AllowDiscount = tglGeneralAllowDiscount.IsOn,
                AffectsInventory = tglAffectsInventory.IsOn
            },
            Units = new ItemUnitsState
            {
                InventoryUnitOfMeasureId = inventoryUnitId,
                PurchaseUnitOfMeasureId = purchaseUnitId,
                SalesUnitOfMeasureId = salesUnitId,
                PurchaseFactor = GetMainPresentationFactor(),
                SalesFactor = GetMainPresentationFactor(),
                NetWeight = spnNetWeight.Value,
                GrossWeight = spnGrossWeight.Value,
                Volume = spnVolume.Value,
                WeightUnit = GetLookupString(lueWeightUnit),
                VolumeUnit = GetLookupString(lueVolumeUnit),
                QuantityRounding = 0.01m,
                AllowFractions = true
            },
            Inventory = new ItemInventoryState
            {
                ManageInventory = tglAffectsInventory.IsOn,
                ValuationMethod = GetLookupString(lueValuationMethod) ?? "MovingAverage",
                NegativeStockPolicy = GetLookupString(lueNegativeStockPolicy) ?? "None",
                AutoReplenishment = tglAutoReplenishment.IsOn,
                ManageLocations = tglManageLocations.IsOn,
                RequiresCycleCount = tglRequiresCycleCount.IsOn,
                CoverageDays = decimal.ToInt32(spnCoverageDays.Value),
                GlobalMinimumStock = spnGlobalMinStock.Value,
                GlobalMaximumStock = spnGlobalMaxStock.Value,
                GlobalReorderPoint = spnGlobalReorderPoint.Value,
                LeadTimeDays = decimal.ToInt32(spnLeadTimeDays.Value),
                MainWarehouseId = GetSearchLookupInt(slueMainWarehouse),
                SupplyMethod = GetLookupString(lueSupplyMethod),
                ReplenishmentMethod = GetLookupString(lueReplenishmentMethod),
                AbcClassification = GetLookupString(lueAbcClassification),
                DefaultLocationCode = NullIfWhiteSpace(slueDefaultBinLocation.Text),
                BatchRequired = tglGeneralBatchManaged.IsOn,
                SerialRequired = tglGeneralSerialManaged.IsOn,
                AllowTransfers = !tglBlockedForMovements.IsOn,
                Storable = tglAffectsInventory.IsOn,
                OperationNote = NullIfWhiteSpace(memInventoryOperationNote.Text)
            },
            Purchasing = new ItemPurchasingState
            {
                PurchaseEnabled = tglPurchaseActive.IsOn,
                MainSupplierCode = NullIfWhiteSpace(slueSupplierSku.Text),
                PurchaseUnitOfMeasureId = purchaseUnitId,
                PurchaseMultiple = GetMainPresentationFactor(),
                MinimumOrderQuantity = spnSuggestedPurchaseQty.Value,
                LeadTimeDays = decimal.ToInt32(spnLeadTimeDays.Value),
                AllowBackorder = tglSupplierBackorderAllowed.IsOn,
                RequiresPurchaseApproval = tglPurchaseApprovalRequired.IsOn,
                LastPurchaseCost = spnLastCost.Value,
                StandardPurchaseCost = spnStandardCost.Value,
                PurchaseTaxId = GetLookupInt(luePurchaseVat),
                PurchaseExpenseAccountCode = NullIfWhiteSpace(sluePurchaseExpenseAccount.Text),
                ReturnPolicy = NullIfWhiteSpace(memPurchasePolicy.Text)
            },
            Sales = new ItemSalesState
            {
                SalesEnabled = tglSalesActive.IsOn,
                SalesUnitOfMeasureId = salesUnitId,
                BasePrice = spnBaseSalesPrice.Value,
                MainPriceList = GetLookupString(lueMainPriceList),
                AllowDiscount = tglAllowSalesDiscount.IsOn,
                MaximumDiscountPercent = spnMaxDiscount.Value,
                MinimumMarginPercent = spnMinimumMargin.Value,
                MinimumSaleQuantity = spnMinimumSale.Value,
                SalesMultiple = spnSalesMultiple.Value,
                CommissionPercent = spnSalesCommission.Value,
                SalesTaxId = GetLookupInt(lueTaxesSalesVat),
                ExciseTax = GetLookupString(lueExciseTax),
                SuggestedRetention = GetLookupString(lueTaxesSuggestedWithholding),
                TaxableProduct = tglTaxableGoods.IsOn,
                AffectsPromotions = tglAffectsPromotions.IsOn,
                AllowsReturns = true,
                BlockedForEcommerce = tglBlockedEcommerce.IsOn,
                CommercialPolicy = NullIfWhiteSpace(memSalesNotes.Text)
            },
            Costs = new ItemCostsState
            {
                AverageCost = spnAverageCost.Value,
                LastCost = spnLastCost.Value,
                StandardCost = spnStandardCost.Value,
                ReplacementCost = spnReplacementCost.Value,
                CostCurrency = GetLookupString(lueCostCurrency),
                CostUpdatedAt = ToDate(dtCostUpdatedAt.EditValue),
                CostingMethod = GetLookupString(lueValuationMethod) ?? "MovingAverage",
                BasePrice = spnAnalysisBasePrice.Value,
                SuggestedPrice = spnSuggestedPrice.Value,
                GrossMargin = ToDecimal(lblGrossMarginValue.Text),
                GrossMarginPercent = ToDecimal(lblGrossMarginPercentValue.Text.Replace("%", string.Empty)),
                MinimumAllowedMarginPercent = spnMinimumMarginPercent.Value,
                TwelveMonthProfitabilityPercent = ToDecimal(lblProfitability12mValue.Text.Replace("%", string.Empty)),
                PriceUpdatedAt = ToDate(dtPriceUpdatedAt.EditValue)
            },
            Accounting = new ItemAccountingState
            {
                InventoryAccountCode = NullIfWhiteSpace(slueInventoryAccount.Text),
                IncomeAccountCode = NullIfWhiteSpace(slueRevenueAccount.Text),
                CostOfSalesAccountCode = NullIfWhiteSpace(slueCostOfGoodsSoldAccount.Text),
                SalesReturnAccountCode = NullIfWhiteSpace(slueSalesReturnAccount.Text),
                PurchaseReturnAccountCode = NullIfWhiteSpace(sluePurchaseReturnAccount.Text),
                CostVarianceAccountCode = NullIfWhiteSpace(slueCostVarianceAccount.Text),
                InventoryAdjustmentAccountCode = NullIfWhiteSpace(slueInventoryAdjustmentAccount.Text),
                PurchaseExpenseAccountCode = NullIfWhiteSpace(sluePurchaseExpenseAccount.Text),
                AllowDocumentOverride = true,
                RequiresDimensionInMovements = false,
                GeneratesInventoryEntry = tglGenerateInventoryJournal.IsOn,
                UsesWarehouseAccount = tglUseWarehouseAccount.IsOn,
                UsesGroupAccount = tglUseGroupAccount.IsOn,
                AllowsCompensation = tglAllowCompensation.IsOn,
                AccountingBlocked = tglAccountingBlocked.IsOn,
                ReconciliationDays = decimal.ToInt32(spnReconciliationDays.Value),
                AccountingIntegrationMethod = GetLookupString(lueAccountingIntegrationMethod),
                AccountingNotes = NullIfWhiteSpace(memAccountingNotes.Text)
            },
            Taxes = new ItemTaxesState
            {
                FiscalItemType = GetLookupString(lueFiscalItemType),
                PurchaseVatId = GetLookupInt(luePurchaseVat),
                SalesVatId = GetLookupInt(lueTaxesSalesVat),
                ExciseTax = GetLookupString(lueExciseTax),
                TaxableService = tglTaxableService.IsOn,
                ExemptGood = tglTaxExemptGoods.IsOn,
                SuggestedRetention = GetLookupString(lueTaxesSuggestedWithholding),
                TaxSupport = GetLookupString(lueTaxSupport),
                FiscalCode = NullIfWhiteSpace(txtFiscalCode.Text),
                FiscalCountry = GetLookupString(lueFiscalCountry),
                AppliesToPurchases = tglPurchaseActive.IsOn,
                AppliesToSales = tglSalesActive.IsOn,
                AffectsRetention = GetLookupString(lueTaxesSuggestedWithholding) is not null,
                TariffCode = NullIfWhiteSpace(txtTariffCode.Text)
            },
            Traceability = new ItemTraceabilityState
            {
                BatchControl = tglGeneralBatchManaged.IsOn,
                SerialControl = tglGeneralSerialManaged.IsOn,
                RequiresExpiration = tglRequiresExpiration.IsOn,
                ExpirationRequired = tglExpirationMandatory.IsOn,
                ExpirationAlertDays = decimal.ToInt32(spnExpirationAlertDays.Value),
                QuarantineDays = decimal.ToInt32(spnQuarantineDays.Value),
                GeneratesBatchAutomatically = tglAutoGenerateBatch.IsOn,
                BatchPrefix = NullIfWhiteSpace(txtBatchPrefix.Text),
                SerialLength = decimal.ToInt32(spnSerialLength.Value),
                FefoFifoMethod = GetLookupString(lueIssueMethod),
                AllowsMultipleLotsPerDocument = tglAllowMultipleBatches.IsOn,
                AllowsExpiredLotSale = tglAllowExpiredBatchSale.IsOn,
                RequiresLotInTransfers = tglGeneralBatchManaged.IsOn,
                RequiresSerialInDispatch = tglGeneralSerialManaged.IsOn,
                OperationNote = NullIfWhiteSpace(memLotOperationalNotes.Text)
            },
            Sap = new ItemSapState
            {
                IsSynchronized = string.Equals(lblKpiSapValue.Text, "Sincronizado", StringComparison.OrdinalIgnoreCase),
                SapItemCode = txtItemCode.Text.Trim(),
                LastSynchronizationAt = ToDate(txtSapLastSync.Text),
                SynchronizationStatus = GetLookupString(lueSapSyncStatus),
                SapCompany = NullIfWhiteSpace(lueSapCompany.Text),
                LastError = NullIfWhiteSpace(txtSapLastError.Text),
                SynchronizeItem = string.Equals(GetLookupString(lueSapEnabled), "Si", StringComparison.OrdinalIgnoreCase),
                SapGroup = GetLookupString(lueSapMode),
                ManagesBatchInSap = tglGeneralBatchManaged.IsOn,
                ManagesSerialInSap = tglGeneralSerialManaged.IsOn
            },
            Remarks = new ItemRemarksState
            {
                GeneralRemarks = NullIfWhiteSpace(memGeneralNotes.Text),
                GeneralOperationalAlert = NullIfWhiteSpace(memGeneralOperationalAlert.Text),
                PurchasingRemarks = NullIfWhiteSpace(memPurchaseNotes.Text),
                SalesRemarks = NullIfWhiteSpace(memSalesNotes.Text),
                InventoryRemarks = NullIfWhiteSpace(memInventoryNotes.Text),
                LogisticsQualityRemarks = NullIfWhiteSpace(memLogisticsQualityNotes.Text)
            }
        };

        AddPresentationStates(state.Units.Presentations);
        AddBarcodeStates(state.Units.Barcodes);
        AddWarehouseStates(state.Inventory.Warehouses);
        AddAttachmentStates(state.Attachments.Files);
        AddSapFieldStates(state.Sap.FieldMappings);
        AddOperationalAlertStates(state.Remarks.OperationalAlerts);

        return state;
    }

    private static ItemMasterData ToMasterData(ItemEditState state)
    {
        return new ItemMasterData
        {
            General = new ItemGeneralData
            {
                AlternateCode = state.General.AlternateCode,
                SupplierSku = state.General.SupplierSku,
                LongDescription = state.General.LongDescription,
                ProductType = state.General.ProductType,
                Origin = state.General.Origin,
                Line = state.General.Line,
                SubGroup = state.General.SubGroup,
                Model = state.General.Model,
                Reference = state.General.Reference,
                SalesActive = state.General.SalesActive,
                PurchaseActive = state.General.PurchaseActive,
                ManageInventory = state.General.ManageInventory,
                IsService = state.General.IsService,
                IsKit = state.General.IsKit,
                BatchManaged = state.General.BatchManaged,
                SerialManaged = state.General.SerialManaged,
                Perishable = state.General.Perishable,
                ExpirationManaged = state.General.ExpirationManaged,
                RequiresScale = state.General.RequiresScale,
                AllowDiscount = state.General.AllowDiscount,
                AffectsInventory = state.General.AffectsInventory
            },
            Units = new ItemUnitsData
            {
                InventoryUnitOfMeasureId = state.Units.InventoryUnitOfMeasureId,
                PurchaseUnitOfMeasureId = state.Units.PurchaseUnitOfMeasureId,
                SalesUnitOfMeasureId = state.Units.SalesUnitOfMeasureId,
                PurchaseFactor = state.Units.PurchaseFactor,
                SalesFactor = state.Units.SalesFactor,
                NetWeight = state.Units.NetWeight,
                GrossWeight = state.Units.GrossWeight,
                Volume = state.Units.Volume,
                WeightUnit = state.Units.WeightUnit,
                VolumeUnit = state.Units.VolumeUnit,
                QuantityRounding = state.Units.QuantityRounding,
                AllowFractions = state.Units.AllowFractions,
                Presentations = state.Units.Presentations.Select(x => new ItemPresentationData
                {
                    Presentation = x.Presentation,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitCode = x.UnitCode,
                    InventoryFactor = x.InventoryFactor,
                    Barcode = x.Barcode,
                    AppliesToPurchase = x.AppliesToPurchase,
                    AppliesToSale = x.AppliesToSale,
                    AppliesToInventory = x.AppliesToInventory,
                    IsMain = x.IsMain,
                    IsActive = x.IsActive
                }).ToList(),
                Barcodes = state.Units.Barcodes.Select(x => new ItemBarcodeData
                {
                    Barcode = x.Barcode,
                    Scope = x.Scope,
                    Presentation = x.Presentation,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitCode = x.UnitCode,
                    InventoryFactor = x.InventoryFactor,
                    IsMain = x.IsMain,
                    IsActive = x.IsActive
                }).ToList()
            },
            Inventory = new ItemInventoryData
            {
                ManageInventory = state.Inventory.ManageInventory,
                ValuationMethod = state.Inventory.ValuationMethod,
                NegativeStockPolicy = state.Inventory.NegativeStockPolicy,
                AutoReplenishment = state.Inventory.AutoReplenishment,
                ManageLocations = state.Inventory.ManageLocations,
                RequiresCycleCount = state.Inventory.RequiresCycleCount,
                CoverageDays = state.Inventory.CoverageDays,
                GlobalMinimumStock = state.Inventory.GlobalMinimumStock,
                GlobalMaximumStock = state.Inventory.GlobalMaximumStock,
                GlobalReorderPoint = state.Inventory.GlobalReorderPoint,
                LeadTimeDays = state.Inventory.LeadTimeDays,
                MainWarehouseId = state.Inventory.MainWarehouseId,
                SupplyMethod = state.Inventory.SupplyMethod,
                ReplenishmentMethod = state.Inventory.ReplenishmentMethod,
                AbcClassification = state.Inventory.AbcClassification,
                DefaultLocationCode = state.Inventory.DefaultLocationCode,
                BatchRequired = state.Inventory.BatchRequired,
                SerialRequired = state.Inventory.SerialRequired,
                AllowTransfers = state.Inventory.AllowTransfers,
                Storable = state.Inventory.Storable,
                OperationNote = state.Inventory.OperationNote,
                Warehouses = state.Inventory.Warehouses.Select(x => new ItemWarehouseData
                {
                    WarehouseId = x.WarehouseId,
                    WarehouseCode = x.WarehouseCode,
                    WarehouseName = x.WarehouseName,
                    CurrentStock = x.CurrentStock,
                    CommittedStock = x.CommittedStock,
                    OrderedStock = x.OrderedStock,
                    AvailableStock = x.AvailableStock,
                    MinimumStock = x.MinimumStock,
                    MaximumStock = x.MaximumStock,
                    ReorderPoint = x.ReorderPoint,
                    RequiredStock = x.RequiredStock,
                    DefaultLocationCode = x.DefaultLocationCode,
                    WarehouseCost = x.WarehouseCost,
                    IsDefaultWarehouse = x.IsDefaultWarehouse,
                    IsLocked = x.IsLocked,
                    IsActive = x.IsActive
                }).ToList()
            },
            Purchasing = new ItemPurchasingData
            {
                PurchaseEnabled = state.Purchasing.PurchaseEnabled,
                MainSupplierCode = state.Purchasing.MainSupplierCode,
                AlternateSupplierCode = state.Purchasing.AlternateSupplierCode,
                PurchaseUnitOfMeasureId = state.Purchasing.PurchaseUnitOfMeasureId,
                PurchaseMultiple = state.Purchasing.PurchaseMultiple,
                MinimumOrderQuantity = state.Purchasing.MinimumOrderQuantity,
                LeadTimeDays = state.Purchasing.LeadTimeDays,
                PreferredPurchaseCurrency = state.Purchasing.PreferredPurchaseCurrency,
                AllowBackorder = state.Purchasing.AllowBackorder,
                RequiresPurchaseApproval = state.Purchasing.RequiresPurchaseApproval,
                LastPurchaseCost = state.Purchasing.LastPurchaseCost,
                StandardPurchaseCost = state.Purchasing.StandardPurchaseCost,
                SupplierDiscountPercent = state.Purchasing.SupplierDiscountPercent,
                PurchaseTaxId = state.Purchasing.PurchaseTaxId,
                PurchaseRetention = state.Purchasing.PurchaseRetention,
                PurchaseExpenseAccountCode = state.Purchasing.PurchaseExpenseAccountCode,
                AssignedBuyer = state.Purchasing.AssignedBuyer,
                ReturnPolicy = state.Purchasing.ReturnPolicy,
                LastPurchaseDate = state.Purchasing.LastPurchaseDate
            },
            Sales = new ItemSalesData
            {
                SalesEnabled = state.Sales.SalesEnabled,
                SalesUnitOfMeasureId = state.Sales.SalesUnitOfMeasureId,
                BasePrice = state.Sales.BasePrice,
                MainPriceList = state.Sales.MainPriceList,
                AllowDiscount = state.Sales.AllowDiscount,
                MaximumDiscountPercent = state.Sales.MaximumDiscountPercent,
                MinimumMarginPercent = state.Sales.MinimumMarginPercent,
                MinimumSaleQuantity = state.Sales.MinimumSaleQuantity,
                SalesMultiple = state.Sales.SalesMultiple,
                CommissionPercent = state.Sales.CommissionPercent,
                SalesTaxId = state.Sales.SalesTaxId,
                ExciseTax = state.Sales.ExciseTax,
                SuggestedRetention = state.Sales.SuggestedRetention,
                TaxableProduct = state.Sales.TaxableProduct,
                AffectsPromotions = state.Sales.AffectsPromotions,
                AllowsReturns = state.Sales.AllowsReturns,
                BlockedForEcommerce = state.Sales.BlockedForEcommerce,
                PreferredChannel = state.Sales.PreferredChannel,
                CommercialPolicy = state.Sales.CommercialPolicy
            },
            Costs = new ItemCostsData
            {
                AverageCost = state.Costs.AverageCost,
                LastCost = state.Costs.LastCost,
                StandardCost = state.Costs.StandardCost,
                ReplacementCost = state.Costs.ReplacementCost,
                CostCurrency = state.Costs.CostCurrency,
                CostUpdatedAt = state.Costs.CostUpdatedAt,
                CostingMethod = state.Costs.CostingMethod,
                BasePrice = state.Costs.BasePrice,
                SuggestedPrice = state.Costs.SuggestedPrice,
                GrossMargin = state.Costs.GrossMargin,
                GrossMarginPercent = state.Costs.GrossMarginPercent,
                MinimumAllowedMarginPercent = state.Costs.MinimumAllowedMarginPercent,
                TwelveMonthProfitabilityPercent = state.Costs.TwelveMonthProfitabilityPercent,
                PriceUpdatedAt = state.Costs.PriceUpdatedAt,
                Components = state.Costs.Components.Select(x => new ItemCostComponentData
                {
                    Concept = x.Concept,
                    Value = x.Value,
                    Percent = x.Percent,
                    Note = x.Note
                }).ToList()
            },
            Accounting = new ItemAccountingData
            {
                InventoryAccountCode = state.Accounting.InventoryAccountCode,
                IncomeAccountCode = state.Accounting.IncomeAccountCode,
                CostOfSalesAccountCode = state.Accounting.CostOfSalesAccountCode,
                SalesReturnAccountCode = state.Accounting.SalesReturnAccountCode,
                PurchaseReturnAccountCode = state.Accounting.PurchaseReturnAccountCode,
                CostVarianceAccountCode = state.Accounting.CostVarianceAccountCode,
                InventoryAdjustmentAccountCode = state.Accounting.InventoryAdjustmentAccountCode,
                PurchaseExpenseAccountCode = state.Accounting.PurchaseExpenseAccountCode,
                DefaultBranchCode = state.Accounting.DefaultBranchCode,
                CostCenterCode = state.Accounting.CostCenterCode,
                ProjectCode = state.Accounting.ProjectCode,
                BusinessLineCode = state.Accounting.BusinessLineCode,
                DepartmentCode = state.Accounting.DepartmentCode,
                AllowDocumentOverride = state.Accounting.AllowDocumentOverride,
                RequiresDimensionInMovements = state.Accounting.RequiresDimensionInMovements,
                GeneratesInventoryEntry = state.Accounting.GeneratesInventoryEntry,
                UsesWarehouseAccount = state.Accounting.UsesWarehouseAccount,
                UsesGroupAccount = state.Accounting.UsesGroupAccount,
                AllowsCompensation = state.Accounting.AllowsCompensation,
                AccountingBlocked = state.Accounting.AccountingBlocked,
                ReconciliationDays = state.Accounting.ReconciliationDays,
                AccountingIntegrationMethod = state.Accounting.AccountingIntegrationMethod,
                AccountingNotes = state.Accounting.AccountingNotes
            },
            Taxes = new ItemTaxesData
            {
                FiscalItemType = state.Taxes.FiscalItemType,
                PurchaseVatId = state.Taxes.PurchaseVatId,
                SalesVatId = state.Taxes.SalesVatId,
                ExciseTax = state.Taxes.ExciseTax,
                TaxableService = state.Taxes.TaxableService,
                ExemptGood = state.Taxes.ExemptGood,
                SuggestedRetention = state.Taxes.SuggestedRetention,
                TaxSupport = state.Taxes.TaxSupport,
                FiscalCode = state.Taxes.FiscalCode,
                FiscalCountry = state.Taxes.FiscalCountry,
                AppliesToPurchases = state.Taxes.AppliesToPurchases,
                AppliesToSales = state.Taxes.AppliesToSales,
                AffectsRetention = state.Taxes.AffectsRetention,
                AppliesCreditNote = state.Taxes.AppliesCreditNote,
                AppliesExport = state.Taxes.AppliesExport,
                RequiresTariffCode = state.Taxes.RequiresTariffCode,
                TariffCode = state.Taxes.TariffCode,
                CustomsClassification = state.Taxes.CustomsClassification,
                TaxNote = state.Taxes.TaxNote
            },
            Traceability = new ItemTraceabilityData
            {
                BatchControl = state.Traceability.BatchControl,
                SerialControl = state.Traceability.SerialControl,
                RequiresExpiration = state.Traceability.RequiresExpiration,
                ExpirationRequired = state.Traceability.ExpirationRequired,
                ExpirationAlertDays = state.Traceability.ExpirationAlertDays,
                QuarantineDays = state.Traceability.QuarantineDays,
                GeneratesBatchAutomatically = state.Traceability.GeneratesBatchAutomatically,
                BatchPrefix = state.Traceability.BatchPrefix,
                SerialLength = state.Traceability.SerialLength,
                FefoFifoMethod = state.Traceability.FefoFifoMethod,
                AllowsMultipleLotsPerDocument = state.Traceability.AllowsMultipleLotsPerDocument,
                AllowsReceiptWithoutLot = state.Traceability.AllowsReceiptWithoutLot,
                AllowsExpiredLotSale = state.Traceability.AllowsExpiredLotSale,
                RequiresLotInTransfers = state.Traceability.RequiresLotInTransfers,
                RequiresSerialInDispatch = state.Traceability.RequiresSerialInDispatch,
                OperationNote = state.Traceability.OperationNote
            },
            Variants = new ItemVariantsData
            {
                ManagesVariants = state.Variants.ManagesVariants,
                VariantType = state.Variants.VariantType,
                AutoGenerateCode = state.Variants.AutoGenerateCode,
                CodeMask = state.Variants.CodeMask,
                BaseVariant = state.Variants.BaseVariant,
                AllowsSalesByVariant = state.Variants.AllowsSalesByVariant,
                AllowsPurchasesByVariant = state.Variants.AllowsPurchasesByVariant,
                AllowsStockByVariant = state.Variants.AllowsStockByVariant
            },
            Sap = new ItemSapData
            {
                IsSynchronized = state.Sap.IsSynchronized,
                SapCode = state.Sap.SapCode,
                SapItemCode = state.Sap.SapItemCode,
                LastSynchronizationAt = state.Sap.LastSynchronizationAt,
                SynchronizationStatus = state.Sap.SynchronizationStatus,
                SapCompany = state.Sap.SapCompany,
                TargetDatabase = state.Sap.TargetDatabase,
                LastError = state.Sap.LastError,
                SynchronizeItem = state.Sap.SynchronizeItem,
                SapGroup = state.Sap.SapGroup,
                SapUnitGroup = state.Sap.SapUnitGroup,
                SapPlanningMethod = state.Sap.SapPlanningMethod,
                SapSupplyMethod = state.Sap.SapSupplyMethod,
                SapValuationMethod = state.Sap.SapValuationMethod,
                ManagesBatchInSap = state.Sap.ManagesBatchInSap,
                ManagesSerialInSap = state.Sap.ManagesSerialInSap,
                FieldMappings = state.Sap.FieldMappings.Select(x => new ItemSapFieldMappingData
                {
                    SystemField = x.SystemField,
                    SapField = x.SapField,
                    Description = x.Description,
                    Required = x.Required,
                    Enabled = x.Enabled
                }).ToList()
            },
            Attachments = new ItemAttachmentsData
            {
                Files = state.Attachments.Files.Select(x => new ItemAttachmentData
                {
                    DocumentType = x.DocumentType,
                    FileName = x.FileName,
                    Description = x.Description,
                    Category = x.Category,
                    Extension = x.Extension,
                    Size = x.Size,
                    UploadDate = x.UploadDate,
                    User = x.User,
                    IsMain = x.IsMain,
                    VisibleInSales = x.VisibleInSales,
                    VisibleInPurchases = x.VisibleInPurchases,
                    VisibleInPortal = x.VisibleInPortal,
                    Status = x.Status
                }).ToList()
            },
            Remarks = new ItemRemarksData
            {
                GeneralRemarks = state.Remarks.GeneralRemarks,
                GeneralOperationalAlert = state.Remarks.GeneralOperationalAlert,
                PurchasingRemarks = state.Remarks.PurchasingRemarks,
                SalesRemarks = state.Remarks.SalesRemarks,
                InventoryRemarks = state.Remarks.InventoryRemarks,
                LogisticsQualityRemarks = state.Remarks.LogisticsQualityRemarks,
                OperationalAlerts = state.Remarks.OperationalAlerts.Select(x => new ItemOperationalAlertData
                {
                    AlertType = x.AlertType,
                    Process = x.Process,
                    Message = x.Message,
                    ValidFrom = x.ValidFrom,
                    ValidTo = x.ValidTo,
                    IsBlocking = x.IsBlocking,
                    IsActive = x.IsActive
                }).ToList()
            }
        };
    }

    private void ApplyMasterData(ItemMasterData? masterData)
    {
        if (masterData is null)
        {
            return;
        }

        ApplyGeneralData(masterData.General);
        ApplyUnitsData(masterData.Units);
        ApplyInventoryData(masterData.Inventory);
        ApplyPurchasingData(masterData.Purchasing);
        ApplySalesData(masterData.Sales);
        ApplyCostsData(masterData.Costs);
        ApplyAccountingData(masterData.Accounting);
        ApplyTaxesData(masterData.Taxes);
        ApplyTraceabilityData(masterData.Traceability);
        ApplySapData(masterData.Sap);
        ApplyAttachmentsData(masterData.Attachments);
        ApplyRemarksData(masterData.Remarks);
    }

    private void ApplyGeneralData(ItemGeneralData? data)
    {
        if (data is null)
        {
            return;
        }

        txtAlternateCode.Text = data.AlternateCode ?? string.Empty;
        slueSupplierSku.Text = data.SupplierSku ?? string.Empty;
        memLongDescription.Text = data.LongDescription ?? string.Empty;
        SetLookupValue(lueProductType, data.ProductType);
        SetLookupValue(lueOrigin, data.Origin);
        SetLookupValue(lueLine, data.Line);
        SetLookupValue(lueSubGroup, data.SubGroup);
        txtModel.Text = data.Model ?? string.Empty;
        txtReference.Text = data.Reference ?? string.Empty;
        tglSalesActive.IsOn = data.SalesActive;
        tglPurchaseActive.IsOn = data.PurchaseActive;
        tglAffectsInventory.IsOn = data.AffectsInventory;
        tglGeneralBatchManaged.IsOn = data.BatchManaged;
        tglGeneralSerialManaged.IsOn = data.SerialManaged;
        tglGeneralPerishable.IsOn = data.Perishable;
        tglGeneralExpirationManaged.IsOn = data.ExpirationManaged;
        tglGeneralRequiresScale.IsOn = data.RequiresScale;
        tglGeneralAllowDiscount.IsOn = data.AllowDiscount;
    }

    private void ApplyUnitsData(ItemUnitsData? data)
    {
        if (data is null)
        {
            return;
        }

        lueInventoryUnit.EditValue = data.InventoryUnitOfMeasureId;
        luePurchaseUnit.EditValue = data.PurchaseUnitOfMeasureId;
        lueSalesUnit.EditValue = data.SalesUnitOfMeasureId;
        lueBaseUnit.EditValue = data.InventoryUnitOfMeasureId;
        spnNetWeight.Value = data.NetWeight;
        spnGrossWeight.Value = data.GrossWeight;
        spnVolume.Value = data.Volume;
        SetLookupValue(lueWeightUnit, data.WeightUnit);
        SetLookupValue(lueVolumeUnit, data.VolumeUnit);

        itemPresentationsTable.Clear();
        foreach (var row in data.Presentations)
        {
            itemPresentationsTable.Rows.Add(
                row.Presentation,
                ResolveUnitCode(row.UnitOfMeasureId) ?? row.UnitCode,
                row.InventoryFactor,
                row.Barcode ?? string.Empty,
                row.AppliesToPurchase,
                row.AppliesToSale,
                row.AppliesToInventory,
                row.IsMain,
                row.IsActive);
        }

        presentationBarcodesTable.Clear();
        foreach (var row in data.Barcodes)
        {
            presentationBarcodesTable.Rows.Add(
                row.Barcode,
                row.Scope,
                row.Presentation,
                ResolveUnitCode(row.UnitOfMeasureId) ?? row.UnitCode,
                row.InventoryFactor,
                row.IsMain,
                row.IsActive);
        }
    }

    private void ApplyInventoryData(ItemInventoryData? data)
    {
        if (data is null)
        {
            return;
        }

        tglAffectsInventory.IsOn = data.ManageInventory;
        SetLookupValue(lueValuationMethod, data.ValuationMethod);
        SetLookupValue(lueNegativeStockPolicy, data.NegativeStockPolicy);
        tglAutoReplenishment.IsOn = data.AutoReplenishment;
        tglManageLocations.IsOn = data.ManageLocations;
        tglRequiresCycleCount.IsOn = data.RequiresCycleCount;
        spnCoverageDays.Value = data.CoverageDays;
        spnGlobalMinStock.Value = data.GlobalMinimumStock;
        spnGlobalMaxStock.Value = data.GlobalMaximumStock;
        spnGlobalReorderPoint.Value = data.GlobalReorderPoint;
        spnLeadTimeDays.Value = data.LeadTimeDays;
        slueMainWarehouse.EditValue = data.MainWarehouseId;
        SetLookupValue(lueSupplyMethod, data.SupplyMethod);
        SetLookupValue(lueReplenishmentMethod, data.ReplenishmentMethod);
        SetLookupValue(lueAbcClassification, data.AbcClassification);
        slueDefaultBinLocation.Text = data.DefaultLocationCode ?? string.Empty;
        tglGeneralBatchManaged.IsOn = data.BatchRequired || tglGeneralBatchManaged.IsOn;
        tglGeneralSerialManaged.IsOn = data.SerialRequired || tglGeneralSerialManaged.IsOn;
        tglBlockedForMovements.IsOn = !data.AllowTransfers;
        memInventoryOperationNote.Text = data.OperationNote ?? string.Empty;

        warehouseStockTable.Clear();
        foreach (var row in data.Warehouses)
        {
            warehouseStockTable.Rows.Add(
                row.WarehouseCode,
                row.WarehouseName,
                row.CurrentStock,
                row.CommittedStock,
                row.OrderedStock,
                row.AvailableStock,
                row.MinimumStock,
                row.MaximumStock,
                row.ReorderPoint,
                row.IsActive ? "Activo" : "Inactivo",
                row.WarehouseId,
                row.RequiredStock,
                row.DefaultLocationCode ?? string.Empty,
                row.WarehouseCost,
                row.IsDefaultWarehouse,
                row.IsLocked);
        }
    }

    private void ApplyPurchasingData(ItemPurchasingData? data)
    {
        if (data is null)
        {
            return;
        }

        tglPurchaseActive.IsOn = data.PurchaseEnabled;
        slueSupplierSku.Text = data.MainSupplierCode ?? slueSupplierSku.Text;
        luePurchaseUnit.EditValue = data.PurchaseUnitOfMeasureId ?? luePurchaseUnit.EditValue;
        spnSuggestedPurchaseQty.Value = data.MinimumOrderQuantity;
        spnLeadTimeDays.Value = data.LeadTimeDays;
        tglSupplierBackorderAllowed.IsOn = data.AllowBackorder;
        tglPurchaseApprovalRequired.IsOn = data.RequiresPurchaseApproval;
        spnLastCost.Value = data.LastPurchaseCost;
        spnStandardCost.Value = data.StandardPurchaseCost;
        luePurchaseVat.EditValue = data.PurchaseTaxId;
        sluePurchaseExpenseAccount.Text = data.PurchaseExpenseAccountCode ?? string.Empty;
        memPurchasePolicy.Text = data.ReturnPolicy ?? string.Empty;
    }

    private void ApplySalesData(ItemSalesData? data)
    {
        if (data is null)
        {
            return;
        }

        tglSalesActive.IsOn = data.SalesEnabled;
        lueSalesUnit.EditValue = data.SalesUnitOfMeasureId;
        spnBaseSalesPrice.Value = data.BasePrice;
        SetLookupValue(lueMainPriceList, data.MainPriceList);
        tglAllowSalesDiscount.IsOn = data.AllowDiscount;
        spnMaxDiscount.Value = data.MaximumDiscountPercent;
        spnMinimumMargin.Value = data.MinimumMarginPercent;
        spnMinimumSale.Value = data.MinimumSaleQuantity;
        spnSalesMultiple.Value = data.SalesMultiple;
        spnSalesCommission.Value = data.CommissionPercent;
        lueTaxesSalesVat.EditValue = data.SalesTaxId;
        SetLookupValue(lueExciseTax, data.ExciseTax);
        SetLookupValue(lueTaxesSuggestedWithholding, data.SuggestedRetention);
        tglTaxableGoods.IsOn = data.TaxableProduct;
        tglAffectsPromotions.IsOn = data.AffectsPromotions;
        tglBlockedEcommerce.IsOn = data.BlockedForEcommerce;
        memSalesNotes.Text = data.CommercialPolicy ?? string.Empty;
    }

    private void ApplyCostsData(ItemCostsData? data)
    {
        if (data is null)
        {
            return;
        }

        spnAverageCost.Value = data.AverageCost;
        spnLastCost.Value = data.LastCost;
        spnStandardCost.Value = data.StandardCost;
        spnReplacementCost.Value = data.ReplacementCost;
        SetLookupValue(lueCostCurrency, data.CostCurrency);
        dtCostUpdatedAt.EditValue = data.CostUpdatedAt;
        spnAnalysisBasePrice.Value = data.BasePrice;
        spnSuggestedPrice.Value = data.SuggestedPrice;
        spnMinimumMarginPercent.Value = data.MinimumAllowedMarginPercent;
        dtPriceUpdatedAt.EditValue = data.PriceUpdatedAt;
    }

    private void ApplyAccountingData(ItemAccountingData? data)
    {
        if (data is null)
        {
            return;
        }

        slueInventoryAccount.Text = data.InventoryAccountCode ?? string.Empty;
        slueRevenueAccount.Text = data.IncomeAccountCode ?? string.Empty;
        slueCostOfGoodsSoldAccount.Text = data.CostOfSalesAccountCode ?? string.Empty;
        slueSalesReturnAccount.Text = data.SalesReturnAccountCode ?? string.Empty;
        sluePurchaseReturnAccount.Text = data.PurchaseReturnAccountCode ?? string.Empty;
        slueCostVarianceAccount.Text = data.CostVarianceAccountCode ?? string.Empty;
        slueInventoryAdjustmentAccount.Text = data.InventoryAdjustmentAccountCode ?? string.Empty;
        sluePurchaseExpenseAccount.Text = data.PurchaseExpenseAccountCode ?? string.Empty;
        tglGenerateInventoryJournal.IsOn = data.GeneratesInventoryEntry;
        tglUseWarehouseAccount.IsOn = data.UsesWarehouseAccount;
        tglUseGroupAccount.IsOn = data.UsesGroupAccount;
        tglAllowCompensation.IsOn = data.AllowsCompensation;
        tglAccountingBlocked.IsOn = data.AccountingBlocked;
        spnReconciliationDays.Value = data.ReconciliationDays;
        SetLookupValue(lueAccountingIntegrationMethod, data.AccountingIntegrationMethod);
        memAccountingNotes.Text = data.AccountingNotes ?? string.Empty;
    }

    private void ApplyTaxesData(ItemTaxesData? data)
    {
        if (data is null)
        {
            return;
        }

        SetLookupValue(lueFiscalItemType, data.FiscalItemType);
        luePurchaseVat.EditValue = data.PurchaseVatId;
        lueTaxesSalesVat.EditValue = data.SalesVatId;
        SetLookupValue(lueExciseTax, data.ExciseTax);
        tglTaxableService.IsOn = data.TaxableService;
        tglTaxExemptGoods.IsOn = data.ExemptGood;
        SetLookupValue(lueTaxesSuggestedWithholding, data.SuggestedRetention);
        SetLookupValue(lueTaxSupport, data.TaxSupport);
        txtFiscalCode.Text = data.FiscalCode ?? string.Empty;
        SetLookupValue(lueFiscalCountry, data.FiscalCountry);
        txtTariffCode.Text = data.TariffCode ?? string.Empty;
    }

    private void ApplyTraceabilityData(ItemTraceabilityData? data)
    {
        if (data is null)
        {
            return;
        }

        tglGeneralBatchManaged.IsOn = data.BatchControl;
        tglGeneralSerialManaged.IsOn = data.SerialControl;
        tglRequiresExpiration.IsOn = data.RequiresExpiration;
        tglExpirationMandatory.IsOn = data.ExpirationRequired;
        spnExpirationAlertDays.Value = data.ExpirationAlertDays;
        spnQuarantineDays.Value = data.QuarantineDays;
        tglAutoGenerateBatch.IsOn = data.GeneratesBatchAutomatically;
        txtBatchPrefix.Text = data.BatchPrefix ?? string.Empty;
        spnSerialLength.Value = data.SerialLength;
        SetLookupValue(lueIssueMethod, data.FefoFifoMethod);
        tglAllowMultipleBatches.IsOn = data.AllowsMultipleLotsPerDocument;
        tglAllowExpiredBatchSale.IsOn = data.AllowsExpiredLotSale;
        memLotOperationalNotes.Text = data.OperationNote ?? string.Empty;
    }

    private void ApplySapData(ItemSapData? data)
    {
        if (data is null)
        {
            return;
        }

        SetLookupValue(lueSapSyncStatus, data.SynchronizationStatus);
        txtSapLastSync.Text = data.LastSynchronizationAt?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
        txtSapLastError.Text = data.LastError ?? string.Empty;
        SetLookupValue(lueSapEnabled, data.SynchronizeItem ? "Si" : "No");
        SetLookupValue(lueSapMode, data.SapGroup);
        lueSapCompany.Text = data.SapCompany ?? string.Empty;

        sapFieldsTable.Clear();
        foreach (var row in data.FieldMappings)
        {
            sapFieldsTable.Rows.Add(
                row.SystemField,
                row.SapField,
                row.Description ?? string.Empty,
                row.Required,
                row.Enabled);
        }
    }

    private void ApplyAttachmentsData(ItemAttachmentsData? data)
    {
        if (data is null)
        {
            return;
        }

        attachmentsTable.Clear();
        foreach (var row in data.Files)
        {
            attachmentsTable.Rows.Add(
                row.DocumentType,
                row.FileName,
                row.Description ?? string.Empty,
                row.Extension ?? string.Empty,
                row.Size ?? string.Empty,
                row.UploadDate ?? DateTime.Today,
                row.User ?? string.Empty,
                row.IsMain,
                row.VisibleInSales,
                row.VisibleInPurchases,
                row.Status,
                row.Category ?? string.Empty,
                row.VisibleInPortal);
        }
    }

    private void ApplyRemarksData(ItemRemarksData? data)
    {
        if (data is null)
        {
            return;
        }

        memGeneralNotes.Text = data.GeneralRemarks ?? string.Empty;
        memGeneralOperationalAlert.Text = data.GeneralOperationalAlert ?? string.Empty;
        memPurchaseNotes.Text = data.PurchasingRemarks ?? string.Empty;
        memSalesNotes.Text = data.SalesRemarks ?? string.Empty;
        memInventoryNotes.Text = data.InventoryRemarks ?? string.Empty;
        memLogisticsQualityNotes.Text = data.LogisticsQualityRemarks ?? string.Empty;

        operationalAlertsTable.Clear();
        foreach (var row in data.OperationalAlerts)
        {
            operationalAlertsTable.Rows.Add(
                row.AlertType,
                row.Process,
                row.Message,
                row.ValidFrom,
                row.ValidTo.HasValue ? row.ValidTo.Value : DBNull.Value,
                row.IsBlocking,
                row.IsActive);
        }
    }

    private IReadOnlyCollection<SaveItemBarcodeRequest> BuildBarcodeRequests()
    {
        var barcodes = new List<SaveItemBarcodeRequest>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (DataRow row in itemPresentationsTable.Rows)
        {
            var barcode = Convert.ToString(row["CodigoBarras"])?.Trim();
            if (string.IsNullOrWhiteSpace(barcode))
            {
                continue;
            }

            var unitCode = Convert.ToString(row["Unidad"]);
            var factor = ToDecimal(row["FactorInventario"], 1);

            seen.Add(barcode);
            barcodes.Add(new SaveItemBarcodeRequest(
                barcode,
                ResolveUnitId(unitCode),
                "Presentation",
                factor <= 0 ? 1 : factor,
                ToBool(row["Principal"]),
                ToBool(row["Activa"], true)));
        }

        foreach (DataRow row in presentationBarcodesTable.Rows)
        {
            var barcode = Convert.ToString(row["CodigoBarras"])?.Trim();
            if (string.IsNullOrWhiteSpace(barcode) || !seen.Add(barcode))
            {
                continue;
            }

            var unitCode = Convert.ToString(row["Unidad"]);
            var factor = ToDecimal(row["FactorInventario"], 1);
            var scope = Convert.ToString(row["Alcance"]);

            barcodes.Add(new SaveItemBarcodeRequest(
                barcode,
                ResolveUnitId(unitCode),
                string.IsNullOrWhiteSpace(scope) ? "Presentation" : scope.Trim(),
                factor <= 0 ? 1 : factor,
                ToBool(row["Principal"]),
                ToBool(row["Activa"], true)));
        }

        return barcodes;
    }

    private bool TryGetUnits(out IReadOnlyCollection<UnitOfMeasureLookupItem> unitLookups)
    {
        unitLookups = lookups?.UnitOfMeasures ?? Array.Empty<UnitOfMeasureLookupItem>();
        if (unitLookups.Count > 0)
        {
            return true;
        }

        ShowValidationMessage("No existen unidades disponibles para crear presentaciones.");
        return false;
    }

    private DataRow? GetFocusedPresentationDataRow()
    {
        gvItemPresentations.PostEditor();
        gvItemPresentations.UpdateCurrentRow();
        return gvItemPresentations.GetFocusedDataRow();
    }

    private DataRow? GetFocusedBarcodeDataRow()
    {
        gvPresentationBarcodes.PostEditor();
        gvPresentationBarcodes.UpdateCurrentRow();
        return gvPresentationBarcodes.GetFocusedDataRow();
    }

    private bool TryGetWarehouses(out IReadOnlyCollection<WarehouseLookupItem> warehouseLookups)
    {
        warehouseLookups = lookups?.Warehouses ?? Array.Empty<WarehouseLookupItem>();
        if (warehouseLookups.Count > 0)
        {
            return true;
        }

        ShowValidationMessage("No existen bodegas disponibles para configurar inventario.");
        return false;
    }

    private DataRow? GetFocusedWarehouseDataRow()
    {
        gvWarehouseStock.PostEditor();
        gvWarehouseStock.UpdateCurrentRow();
        return gvWarehouseStock.GetFocusedDataRow();
    }

    private DataRow? GetFocusedOperationalAlertDataRow()
    {
        gvOperationalAlerts.PostEditor();
        gvOperationalAlerts.UpdateCurrentRow();
        return gvOperationalAlerts.GetFocusedDataRow();
    }

    private DataRow? GetFocusedAttachmentDataRow()
    {
        gvAttachments.PostEditor();
        gvAttachments.UpdateCurrentRow();
        return gvAttachments.GetFocusedDataRow();
    }

    private DataRow? GetFocusedSapFieldDataRow()
    {
        grvSapFieldMapping.PostEditor();
        grvSapFieldMapping.UpdateCurrentRow();
        return grvSapFieldMapping.GetFocusedDataRow();
    }

    private void AddPresentationRow(ItemPresentationRow row)
    {
        itemPresentationsTable.Rows.Add(
            row.Presentation,
            row.UnitCode,
            row.Factor,
            row.Barcode ?? string.Empty,
            row.AppliesPurchase,
            row.AppliesSale,
            row.AppliesInventory,
            row.IsMain,
            row.IsActive);
    }

    private void ApplyPresentationRow(DataRow dataRow, ItemPresentationRow row)
    {
        dataRow["Presentacion"] = row.Presentation;
        dataRow["Unidad"] = row.UnitCode;
        dataRow["FactorInventario"] = row.Factor;
        dataRow["CodigoBarras"] = row.Barcode ?? string.Empty;
        dataRow["AplicaCompra"] = row.AppliesPurchase;
        dataRow["AplicaVenta"] = row.AppliesSale;
        dataRow["AplicaInventario"] = row.AppliesInventory;
        dataRow["Principal"] = row.IsMain;
        dataRow["Activa"] = row.IsActive;
    }

    private ItemPresentationRow CreatePresentationRow(DataRow dataRow)
    {
        var unitCode = Convert.ToString(dataRow["Unidad"]);
        var unit = lookups?.UnitOfMeasures.FirstOrDefault(x =>
            string.Equals(x.Code, unitCode, StringComparison.OrdinalIgnoreCase));

        return new ItemPresentationRow(
            Convert.ToString(dataRow["Presentacion"]) ?? string.Empty,
            unit?.Id ?? 0,
            unit?.Code ?? unitCode ?? string.Empty,
            unit?.DisplayText ?? unitCode ?? string.Empty,
            ToDecimal(dataRow["FactorInventario"], 1),
            NullIfWhiteSpace(Convert.ToString(dataRow["CodigoBarras"])),
            ToBool(dataRow["AplicaCompra"], true),
            ToBool(dataRow["AplicaVenta"], true),
            ToBool(dataRow["AplicaInventario"], true),
            ToBool(dataRow["Principal"]),
            ToBool(dataRow["Activa"], true));
    }

    private void ClearMainPresentation(DataRow? exceptRow = null)
    {
        foreach (DataRow row in itemPresentationsTable.Rows)
        {
            if (ReferenceEquals(row, exceptRow))
            {
                continue;
            }

            row["Principal"] = false;
        }
    }

    private void AddBarcodeRow(ItemBarcodeRow row)
    {
        presentationBarcodesTable.Rows.Add(
            row.Barcode,
            row.Scope,
            row.Presentation,
            row.UnitCode,
            row.Factor,
            row.IsMain,
            row.IsActive);
    }

    private void ApplyBarcodeRow(DataRow dataRow, ItemBarcodeRow row)
    {
        dataRow["CodigoBarras"] = row.Barcode;
        dataRow["Alcance"] = row.Scope;
        dataRow["Presentacion"] = row.Presentation;
        dataRow["Unidad"] = row.UnitCode;
        dataRow["FactorInventario"] = row.Factor;
        dataRow["Principal"] = row.IsMain;
        dataRow["Activa"] = row.IsActive;
    }

    private ItemBarcodeRow CreateBarcodeRow(DataRow dataRow)
    {
        var unitCode = Convert.ToString(dataRow["Unidad"]);
        var unit = lookups?.UnitOfMeasures.FirstOrDefault(x =>
            string.Equals(x.Code, unitCode, StringComparison.OrdinalIgnoreCase));

        return new ItemBarcodeRow(
            Convert.ToString(dataRow["CodigoBarras"]) ?? string.Empty,
            Convert.ToString(dataRow["Alcance"]) ?? "General",
            Convert.ToString(dataRow["Presentacion"]) ?? string.Empty,
            unit?.Id ?? 0,
            unit?.Code ?? unitCode ?? string.Empty,
            unit?.DisplayText ?? unitCode ?? string.Empty,
            ToDecimal(dataRow["FactorInventario"], 1),
            ToBool(dataRow["Principal"]),
            ToBool(dataRow["Activa"], true));
    }

    private void ClearMainBarcode(DataRow? exceptRow = null)
    {
        foreach (DataRow row in presentationBarcodesTable.Rows)
        {
            if (ReferenceEquals(row, exceptRow))
            {
                continue;
            }

            row["Principal"] = false;
        }
    }

    private void AddWarehouseRow(ItemWarehouseRow row)
    {
        warehouseStockTable.Rows.Add(
            row.WarehouseCode,
            row.WarehouseName,
            0m,
            0m,
            0m,
            0m,
            row.MinimumStock,
            row.MaximumStock,
            row.ReorderPoint,
            row.IsActive ? "Activo" : "Inactivo",
            row.WarehouseId,
            row.RequiredStock,
            row.DefaultLocationCode ?? string.Empty,
            row.WarehouseCost,
            row.IsDefaultWarehouse,
            row.IsLocked);

        if (row.IsDefaultWarehouse)
        {
            slueMainWarehouse.EditValue = row.WarehouseId;
        }
    }

    private void ApplyWarehouseRow(DataRow dataRow, ItemWarehouseRow row)
    {
        dataRow["Bodega"] = row.WarehouseCode;
        dataRow["NombreBodega"] = row.WarehouseName;
        dataRow["Minimo"] = row.MinimumStock;
        dataRow["Maximo"] = row.MaximumStock;
        dataRow["Reorden"] = row.ReorderPoint;
        dataRow["Estado"] = row.IsActive ? "Activo" : "Inactivo";
        dataRow["WarehouseId"] = row.WarehouseId;
        dataRow["StockRequerido"] = row.RequiredStock;
        dataRow["UbicacionDefecto"] = row.DefaultLocationCode ?? string.Empty;
        dataRow["CostoBodega"] = row.WarehouseCost;
        dataRow["Principal"] = row.IsDefaultWarehouse;
        dataRow["Bloqueada"] = row.IsLocked;

        if (row.IsDefaultWarehouse)
        {
            slueMainWarehouse.EditValue = row.WarehouseId;
        }
    }

    private ItemWarehouseRow CreateWarehouseRow(DataRow dataRow)
    {
        var warehouseId = ToInt(dataRow["WarehouseId"]);
        var warehouseCode = Convert.ToString(dataRow["Bodega"]);
        var warehouse = lookups?.Warehouses.FirstOrDefault(x => x.Id == warehouseId) ??
            lookups?.Warehouses.FirstOrDefault(x =>
                string.Equals(x.Code, warehouseCode, StringComparison.OrdinalIgnoreCase));

        return new ItemWarehouseRow(
            warehouse?.Id ?? warehouseId,
            warehouse?.Code ?? warehouseCode ?? string.Empty,
            warehouse?.Name ?? Convert.ToString(dataRow["NombreBodega"]) ?? string.Empty,
            ToDecimal(dataRow["Minimo"]),
            ToDecimal(dataRow["Maximo"]),
            ToDecimal(dataRow["StockRequerido"]),
            ToDecimal(dataRow["Reorden"]),
            NullIfWhiteSpace(Convert.ToString(dataRow["UbicacionDefecto"])),
            ToDecimal(dataRow["CostoBodega"], spnAverageCost.Value),
            ToBool(dataRow["Principal"]),
            ToBool(dataRow["Bloqueada"]),
            !string.Equals(Convert.ToString(dataRow["Estado"]), "Inactivo", StringComparison.OrdinalIgnoreCase));
    }

    private void AddOperationalAlertRow(ItemOperationalAlertRow row)
    {
        operationalAlertsTable.Rows.Add(
            row.AlertType,
            row.Process,
            row.Message,
            row.ValidFrom,
            row.ValidTo.HasValue ? row.ValidTo.Value : DBNull.Value,
            row.IsBlocking,
            row.IsActive);
    }

    private void ApplyOperationalAlertRow(DataRow dataRow, ItemOperationalAlertRow row)
    {
        dataRow["TipoAlerta"] = row.AlertType;
        dataRow["Proceso"] = row.Process;
        dataRow["Mensaje"] = row.Message;
        dataRow["Desde"] = row.ValidFrom;
        dataRow["Hasta"] = row.ValidTo.HasValue ? row.ValidTo.Value : DBNull.Value;
        dataRow["Bloqueante"] = row.IsBlocking;
        dataRow["Activa"] = row.IsActive;
    }

    private ItemOperationalAlertRow CreateOperationalAlertRow(DataRow dataRow)
    {
        return new ItemOperationalAlertRow(
            Convert.ToString(dataRow["TipoAlerta"]) ?? string.Empty,
            Convert.ToString(dataRow["Proceso"]) ?? string.Empty,
            Convert.ToString(dataRow["Mensaje"]) ?? string.Empty,
            ToDate(dataRow["Desde"]) ?? DateTime.Today,
            ToDate(dataRow["Hasta"]),
            ToBool(dataRow["Bloqueante"]),
            ToBool(dataRow["Activa"], true));
    }

    private void AddAttachmentRow(ItemAttachmentRow row)
    {
        attachmentsTable.Rows.Add(
            row.DocumentType,
            row.FileName,
            row.Description,
            row.Extension,
            row.Size,
            row.Date,
            row.User,
            row.IsMain,
            row.VisibleInSales,
            row.VisibleInPurchases,
            row.Status,
            row.Category,
            row.VisibleInPortal);
    }

    private void ApplyAttachmentRow(DataRow dataRow, ItemAttachmentRow row)
    {
        dataRow["TipoDocumento"] = row.DocumentType;
        dataRow["NombreArchivo"] = row.FileName;
        dataRow["Descripcion"] = row.Description;
        dataRow["Extension"] = row.Extension;
        dataRow["Tamano"] = row.Size;
        dataRow["Fecha"] = row.Date;
        dataRow["Usuario"] = row.User;
        dataRow["Principal"] = row.IsMain;
        dataRow["VisibleVentas"] = row.VisibleInSales;
        dataRow["VisibleCompras"] = row.VisibleInPurchases;
        dataRow["Estado"] = row.Status;
        dataRow["Categoria"] = row.Category;
        dataRow["VisiblePortal"] = row.VisibleInPortal;
    }

    private ItemAttachmentRow CreateAttachmentRow(DataRow dataRow)
    {
        return new ItemAttachmentRow(
            Convert.ToString(dataRow["TipoDocumento"]) ?? string.Empty,
            Convert.ToString(dataRow["NombreArchivo"]) ?? string.Empty,
            Convert.ToString(dataRow["Descripcion"]) ?? string.Empty,
            Convert.ToString(dataRow["Categoria"]) ?? "Comercial",
            Convert.ToString(dataRow["Extension"]) ?? string.Empty,
            Convert.ToString(dataRow["Tamano"]) ?? string.Empty,
            ToDate(dataRow["Fecha"]) ?? DateTime.Today,
            Convert.ToString(dataRow["Usuario"]) ?? string.Empty,
            ToBool(dataRow["Principal"]),
            ToBool(dataRow["VisibleVentas"]),
            ToBool(dataRow["VisibleCompras"]),
            ToBool(dataRow["VisiblePortal"]),
            Convert.ToString(dataRow["Estado"]) ?? "Activo");
    }

    private void ClearMainAttachment(DataRow? exceptRow = null)
    {
        foreach (DataRow row in attachmentsTable.Rows)
        {
            if (ReferenceEquals(row, exceptRow))
            {
                continue;
            }

            row["Principal"] = false;
        }
    }

    private void AddSapFieldRow(ItemSapFieldMappingRow row)
    {
        sapFieldsTable.Rows.Add(
            row.SystemField,
            row.SapField,
            row.Description,
            row.Required,
            row.Enabled);
    }

    private void ApplySapFieldRow(DataRow dataRow, ItemSapFieldMappingRow row)
    {
        dataRow["SystemField"] = row.SystemField;
        dataRow["SapField"] = row.SapField;
        dataRow["Description"] = row.Description;
        dataRow["Required"] = row.Required;
        dataRow["Enabled"] = row.Enabled;
    }

    private ItemSapFieldMappingRow CreateSapFieldRow(DataRow dataRow)
    {
        return new ItemSapFieldMappingRow(
            Convert.ToString(dataRow["SystemField"]) ?? string.Empty,
            Convert.ToString(dataRow["SapField"]) ?? string.Empty,
            Convert.ToString(dataRow["Description"]) ?? string.Empty,
            ToBool(dataRow["Required"], true),
            ToBool(dataRow["Enabled"], true));
    }

    private void ClearMainWarehouse(DataRow? exceptRow = null)
    {
        foreach (DataRow row in warehouseStockTable.Rows)
        {
            if (ReferenceEquals(row, exceptRow))
            {
                continue;
            }

            row["Principal"] = false;
        }
    }

    private IReadOnlyCollection<SaveItemWarehouseRequest> BuildWarehouseRequests()
    {
        var warehouses = new List<SaveItemWarehouseRequest>();

        foreach (DataRow row in warehouseStockTable.Rows)
        {
            var warehouseId = ToInt(row["WarehouseId"]);
            var warehouseCode = Convert.ToString(row["Bodega"]);
            var warehouse = lookups?.Warehouses.FirstOrDefault(x => x.Id == warehouseId) ??
                lookups?.Warehouses.FirstOrDefault(x =>
                    string.Equals(x.Code, warehouseCode, StringComparison.OrdinalIgnoreCase));

            if (warehouse is null)
            {
                continue;
            }

            warehouses.Add(new SaveItemWarehouseRequest(
                warehouse.Id,
                ToDecimal(row["Minimo"]),
                ToDecimal(row["Maximo"]),
                ToDecimal(row["StockRequerido"]),
                ToDecimal(row["Reorden"]),
                NullIfWhiteSpace(Convert.ToString(row["UbicacionDefecto"])),
                ToDecimal(row["CostoBodega"], spnAverageCost.Value),
                ToBool(row["Principal"], warehouses.Count == 0),
                ToBool(row["Bloqueada"]),
                !string.Equals(Convert.ToString(row["Estado"]), "Inactivo", StringComparison.OrdinalIgnoreCase)));
        }

        return warehouses;
    }

    private decimal GetMainPresentationFactor()
    {
        foreach (DataRow row in itemPresentationsTable.Rows)
        {
            if (ToBool(row["Principal"]))
            {
                return ToDecimal(row["FactorInventario"], 1);
            }
        }

        return 1;
    }

    private string ResolveManagedBy()
    {
        if (tglGeneralSerialManaged.IsOn)
        {
            return "Serial";
        }

        return tglGeneralBatchManaged.IsOn ? "Batch" : "None";
    }

    private static int? GetLookupInt(LookUpEdit lookup)
    {
        if (lookup.EditValue is null)
        {
            return null;
        }

        if (lookup.EditValue is int value)
        {
            return value;
        }

        return int.TryParse(Convert.ToString(lookup.EditValue), out var parsed) ? parsed : null;
    }

    private static string? GetLookupString(LookUpEdit lookup)
    {
        return NullIfWhiteSpace(Convert.ToString(lookup.EditValue));
    }

    private static int? GetSearchLookupInt(SearchLookUpEdit lookup)
    {
        if (lookup.EditValue is null)
        {
            return null;
        }

        if (lookup.EditValue is int value)
        {
            return value;
        }

        return int.TryParse(Convert.ToString(lookup.EditValue), out var parsed) ? parsed : null;
    }

    private int? ResolveUnitId(string? unitCode)
    {
        if (string.IsNullOrWhiteSpace(unitCode))
        {
            return null;
        }

        return lookups?.UnitOfMeasures.FirstOrDefault(x =>
            string.Equals(x.Code, unitCode.Trim(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(x.DisplayText, unitCode.Trim(), StringComparison.OrdinalIgnoreCase))?.Id;
    }

    private string? ResolveUnitCode(int? unitId)
    {
        if (unitId is null)
        {
            return null;
        }

        return lookups?.UnitOfMeasures.FirstOrDefault(x => x.Id == unitId.Value)?.Code;
    }

    private void AddPresentationStates(ICollection<ItemPresentationState> presentations)
    {
        foreach (DataRow row in itemPresentationsTable.Rows)
        {
            var unitCode = Convert.ToString(row["Unidad"]) ?? string.Empty;

            presentations.Add(new ItemPresentationState
            {
                Presentation = Convert.ToString(row["Presentacion"]) ?? string.Empty,
                UnitOfMeasureId = ResolveUnitId(unitCode),
                UnitCode = unitCode,
                InventoryFactor = ToDecimal(row["FactorInventario"], 1),
                Barcode = NullIfWhiteSpace(Convert.ToString(row["CodigoBarras"])),
                AppliesToPurchase = ToBool(row["AplicaCompra"]),
                AppliesToSale = ToBool(row["AplicaVenta"]),
                AppliesToInventory = ToBool(row["AplicaInventario"]),
                IsMain = ToBool(row["Principal"]),
                IsActive = ToBool(row["Activa"], true)
            });
        }
    }

    private void AddBarcodeStates(ICollection<ItemBarcodeState> barcodes)
    {
        foreach (DataRow row in presentationBarcodesTable.Rows)
        {
            var unitCode = Convert.ToString(row["Unidad"]) ?? string.Empty;

            barcodes.Add(new ItemBarcodeState
            {
                Barcode = Convert.ToString(row["CodigoBarras"]) ?? string.Empty,
                Scope = Convert.ToString(row["Alcance"]) ?? "General",
                Presentation = Convert.ToString(row["Presentacion"]) ?? string.Empty,
                UnitOfMeasureId = ResolveUnitId(unitCode),
                UnitCode = unitCode,
                InventoryFactor = ToDecimal(row["FactorInventario"], 1),
                IsMain = ToBool(row["Principal"]),
                IsActive = ToBool(row["Activa"], true)
            });
        }
    }

    private void AddWarehouseStates(ICollection<ItemWarehouseState> warehouses)
    {
        foreach (DataRow row in warehouseStockTable.Rows)
        {
            warehouses.Add(new ItemWarehouseState
            {
                WarehouseId = ToInt(row["WarehouseId"]),
                WarehouseCode = Convert.ToString(row["Bodega"]) ?? string.Empty,
                WarehouseName = Convert.ToString(row["NombreBodega"]) ?? string.Empty,
                CurrentStock = ToDecimal(row["StockActual"]),
                CommittedStock = ToDecimal(row["Comprometido"]),
                OrderedStock = ToDecimal(row["Pedido"]),
                AvailableStock = ToDecimal(row["Disponible"]),
                MinimumStock = ToDecimal(row["Minimo"]),
                MaximumStock = ToDecimal(row["Maximo"]),
                ReorderPoint = ToDecimal(row["Reorden"]),
                RequiredStock = ToDecimal(row["StockRequerido"]),
                DefaultLocationCode = NullIfWhiteSpace(Convert.ToString(row["UbicacionDefecto"])),
                WarehouseCost = ToDecimal(row["CostoBodega"]),
                IsDefaultWarehouse = ToBool(row["Principal"]),
                IsLocked = ToBool(row["Bloqueada"]),
                IsActive = !string.Equals(Convert.ToString(row["Estado"]), "Inactivo", StringComparison.OrdinalIgnoreCase)
            });
        }
    }

    private void AddAttachmentStates(ICollection<ItemAttachmentState> attachments)
    {
        foreach (DataRow row in attachmentsTable.Rows)
        {
            attachments.Add(new ItemAttachmentState
            {
                DocumentType = Convert.ToString(row["TipoDocumento"]) ?? string.Empty,
                FileName = Convert.ToString(row["NombreArchivo"]) ?? string.Empty,
                Description = NullIfWhiteSpace(Convert.ToString(row["Descripcion"])),
                Category = NullIfWhiteSpace(Convert.ToString(row["Categoria"])),
                Extension = NullIfWhiteSpace(Convert.ToString(row["Extension"])),
                Size = NullIfWhiteSpace(Convert.ToString(row["Tamano"])),
                UploadDate = ToDate(row["Fecha"]),
                User = NullIfWhiteSpace(Convert.ToString(row["Usuario"])),
                IsMain = ToBool(row["Principal"]),
                VisibleInSales = ToBool(row["VisibleVentas"]),
                VisibleInPurchases = ToBool(row["VisibleCompras"]),
                VisibleInPortal = ToBool(row["VisiblePortal"]),
                Status = Convert.ToString(row["Estado"]) ?? "Activo"
            });
        }
    }

    private void AddSapFieldStates(ICollection<ItemSapFieldMappingState> mappings)
    {
        foreach (DataRow row in sapFieldsTable.Rows)
        {
            mappings.Add(new ItemSapFieldMappingState
            {
                SystemField = Convert.ToString(row["SystemField"]) ?? string.Empty,
                SapField = Convert.ToString(row["SapField"]) ?? string.Empty,
                Description = NullIfWhiteSpace(Convert.ToString(row["Description"])),
                Required = ToBool(row["Required"]),
                Enabled = ToBool(row["Enabled"], true)
            });
        }
    }

    private void AddOperationalAlertStates(ICollection<ItemOperationalAlertState> alerts)
    {
        foreach (DataRow row in operationalAlertsTable.Rows)
        {
            alerts.Add(new ItemOperationalAlertState
            {
                AlertType = Convert.ToString(row["TipoAlerta"]) ?? string.Empty,
                Process = Convert.ToString(row["Proceso"]) ?? string.Empty,
                Message = Convert.ToString(row["Mensaje"]) ?? string.Empty,
                ValidFrom = ToDate(row["Desde"]) ?? DateTime.Today,
                ValidTo = ToDate(row["Hasta"]),
                IsBlocking = ToBool(row["Bloqueante"]),
                IsActive = ToBool(row["Activa"], true)
            });
        }
    }

    private void EnsurePresentationColumns()
    {
        EnsureColumn(itemPresentationsTable, "Presentacion", typeof(string));
        EnsureColumn(itemPresentationsTable, "Unidad", typeof(string));
        EnsureColumn(itemPresentationsTable, "FactorInventario", typeof(decimal));
        EnsureColumn(itemPresentationsTable, "CodigoBarras", typeof(string));
        EnsureColumn(itemPresentationsTable, "AplicaCompra", typeof(bool));
        EnsureColumn(itemPresentationsTable, "AplicaVenta", typeof(bool));
        EnsureColumn(itemPresentationsTable, "AplicaInventario", typeof(bool));
        EnsureColumn(itemPresentationsTable, "Principal", typeof(bool));
        EnsureColumn(itemPresentationsTable, "Activa", typeof(bool));
    }

    private void EnsurePresentationBarcodeColumns()
    {
        EnsureColumn(presentationBarcodesTable, "CodigoBarras", typeof(string));
        EnsureColumn(presentationBarcodesTable, "Alcance", typeof(string));
        EnsureColumn(presentationBarcodesTable, "Presentacion", typeof(string));
        EnsureColumn(presentationBarcodesTable, "Unidad", typeof(string));
        EnsureColumn(presentationBarcodesTable, "FactorInventario", typeof(decimal));
        EnsureColumn(presentationBarcodesTable, "Principal", typeof(bool));
        EnsureColumn(presentationBarcodesTable, "Activa", typeof(bool));
    }

    private void EnsureWarehouseColumns()
    {
        EnsureColumn(warehouseStockTable, "Bodega", typeof(string));
        EnsureColumn(warehouseStockTable, "NombreBodega", typeof(string));
        EnsureColumn(warehouseStockTable, "StockActual", typeof(decimal));
        EnsureColumn(warehouseStockTable, "Comprometido", typeof(decimal));
        EnsureColumn(warehouseStockTable, "Pedido", typeof(decimal));
        EnsureColumn(warehouseStockTable, "Disponible", typeof(decimal));
        EnsureColumn(warehouseStockTable, "Minimo", typeof(decimal));
        EnsureColumn(warehouseStockTable, "Maximo", typeof(decimal));
        EnsureColumn(warehouseStockTable, "Reorden", typeof(decimal));
        EnsureColumn(warehouseStockTable, "Estado", typeof(string));
        EnsureColumn(warehouseStockTable, "WarehouseId", typeof(int));
        EnsureColumn(warehouseStockTable, "StockRequerido", typeof(decimal));
        EnsureColumn(warehouseStockTable, "UbicacionDefecto", typeof(string));
        EnsureColumn(warehouseStockTable, "CostoBodega", typeof(decimal));
        EnsureColumn(warehouseStockTable, "Principal", typeof(bool));
        EnsureColumn(warehouseStockTable, "Bloqueada", typeof(bool));
    }

    private void EnsureOperationalAlertColumns()
    {
        EnsureColumn(operationalAlertsTable, "TipoAlerta", typeof(string));
        EnsureColumn(operationalAlertsTable, "Proceso", typeof(string));
        EnsureColumn(operationalAlertsTable, "Mensaje", typeof(string));
        EnsureColumn(operationalAlertsTable, "Desde", typeof(DateTime));
        EnsureColumn(operationalAlertsTable, "Hasta", typeof(DateTime));
        EnsureColumn(operationalAlertsTable, "Bloqueante", typeof(bool));
        EnsureColumn(operationalAlertsTable, "Activa", typeof(bool));
    }

    private void EnsureAttachmentColumns()
    {
        EnsureColumn(attachmentsTable, "TipoDocumento", typeof(string));
        EnsureColumn(attachmentsTable, "NombreArchivo", typeof(string));
        EnsureColumn(attachmentsTable, "Descripcion", typeof(string));
        EnsureColumn(attachmentsTable, "Extension", typeof(string));
        EnsureColumn(attachmentsTable, "Tamano", typeof(string));
        EnsureColumn(attachmentsTable, "Fecha", typeof(DateTime));
        EnsureColumn(attachmentsTable, "Usuario", typeof(string));
        EnsureColumn(attachmentsTable, "Principal", typeof(bool));
        EnsureColumn(attachmentsTable, "VisibleVentas", typeof(bool));
        EnsureColumn(attachmentsTable, "VisibleCompras", typeof(bool));
        EnsureColumn(attachmentsTable, "Estado", typeof(string));
        EnsureColumn(attachmentsTable, "Categoria", typeof(string));
        EnsureColumn(attachmentsTable, "VisiblePortal", typeof(bool));
    }

    private void EnsureSapFieldColumns()
    {
        EnsureColumn(sapFieldsTable, "SystemField", typeof(string));
        EnsureColumn(sapFieldsTable, "SapField", typeof(string));
        EnsureColumn(sapFieldsTable, "Description", typeof(string));
        EnsureColumn(sapFieldsTable, "Required", typeof(bool));
        EnsureColumn(sapFieldsTable, "Enabled", typeof(bool));
    }

    private static void EnsureColumn(DataTable table, string name, Type type)
    {
        if (!table.Columns.Contains(name))
        {
            table.Columns.Add(name, type);
        }
    }

    private void SetActiveBadge(bool isActive)
    {
        lblStatus.Text = isActive ? "Activo" : "Inactivo";
    }

    private static decimal ToDecimal(object? value, decimal defaultValue = 0)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue;
        }

        return decimal.TryParse(Convert.ToString(value), out var parsed) ? parsed : defaultValue;
    }

    private static bool ToBool(object? value, bool defaultValue = false)
    {
        if (value is bool boolValue)
        {
            return boolValue;
        }

        return bool.TryParse(Convert.ToString(value), out var parsed) ? parsed : defaultValue;
    }

    private static int ToInt(object? value, int defaultValue = 0)
    {
        if (value is int intValue)
        {
            return intValue;
        }

        return int.TryParse(Convert.ToString(value), out var parsed) ? parsed : defaultValue;
    }

    private static DateTime? ToDate(object? value)
    {
        if (value is null || value == DBNull.Value)
        {
            return null;
        }

        if (value is DateTime dateValue)
        {
            return dateValue;
        }

        return DateTime.TryParse(Convert.ToString(value), out var parsed) ? parsed : null;
    }

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static SaveItemRequest EmptyRequest()
    {
        return new SaveItemRequest(
            string.Empty,
            string.Empty,
            null,
            null,
            null,
            "Product",
            null,
            null,
            null,
            true,
            true,
            true,
            null,
            null,
            "MovingAverage",
            "None",
            "EveryTransaction",
            null,
            null,
            0,
            0,
            1,
            1,
            true,
            false,
            null,
            true,
            Array.Empty<SaveItemBarcodeRequest>(),
            Array.Empty<SaveItemWarehouseRequest>(),
            null);
    }

    private sealed record LookupOption(string Value, string Display);
}
