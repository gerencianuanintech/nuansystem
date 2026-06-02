using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.StorageZones;

public sealed class StorageZonesForm : GeneralInventoryCatalogListFormBase
{
    public StorageZonesForm() : base(GeneralInventoryCatalogDescriptors.StorageZones) { }

    public StorageZonesForm(GeneralInventoryCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
        : base(GeneralInventoryCatalogDescriptors.StorageZones, viewModel, session, columnSettingsClient) { }

    protected override Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false)
    {
        return item is null ? new StorageZoneEditForm() : new StorageZoneEditForm(item, copyMode);
    }
}
