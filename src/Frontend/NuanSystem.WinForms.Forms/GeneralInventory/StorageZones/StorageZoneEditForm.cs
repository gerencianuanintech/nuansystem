using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.StorageZones;

public sealed class StorageZoneEditForm : GeneralInventoryCatalogEditFormBase
{
    public StorageZoneEditForm() : base(GeneralInventoryCatalogDescriptors.StorageZones) { }

    public StorageZoneEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.StorageZones, item, copyMode) { }
}
