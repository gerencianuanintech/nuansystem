using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Warehouses;

public sealed class WarehousesForm : GeneralInventoryCatalogListFormBase
{
    public WarehousesForm() : base(GeneralInventoryCatalogDescriptors.Warehouses) { }

    public WarehousesForm(GeneralInventoryCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
        : base(GeneralInventoryCatalogDescriptors.Warehouses, viewModel, session, columnSettingsClient) { }

    protected override Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false)
    {
        return item is null ? new WarehouseEditForm() : new WarehouseEditForm(item, copyMode);
    }
}
