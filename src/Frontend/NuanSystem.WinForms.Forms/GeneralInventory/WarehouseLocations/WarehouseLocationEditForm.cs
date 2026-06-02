using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.WarehouseLocations;

public sealed class WarehouseLocationEditForm : GeneralInventoryCatalogEditFormBase
{
    public WarehouseLocationEditForm() : base(GeneralInventoryCatalogDescriptors.WarehouseLocations) { }

    public WarehouseLocationEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.WarehouseLocations, item, copyMode) { }
}
