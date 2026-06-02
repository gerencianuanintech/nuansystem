using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.WarehouseLocations;

public sealed class WarehouseLocationsForm : GeneralInventoryCatalogListFormBase
{
    public WarehouseLocationsForm() : base(GeneralInventoryCatalogDescriptors.WarehouseLocations) { }

    public WarehouseLocationsForm(GeneralInventoryCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
        : base(GeneralInventoryCatalogDescriptors.WarehouseLocations, viewModel, session, columnSettingsClient) { }

    protected override Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false)
    {
        return item is null ? new WarehouseLocationEditForm() : new WarehouseLocationEditForm(item, copyMode);
    }
}
