using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Warehouses;

public sealed class WarehouseEditForm : GeneralInventoryCatalogEditFormBase
{
    public WarehouseEditForm() : base(GeneralInventoryCatalogDescriptors.Warehouses) { }

    public WarehouseEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.Warehouses, item, copyMode) { }
}
