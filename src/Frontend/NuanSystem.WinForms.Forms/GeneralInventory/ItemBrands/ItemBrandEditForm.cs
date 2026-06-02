using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemBrands;

public sealed class ItemBrandEditForm : GeneralInventoryCatalogEditFormBase
{
    public ItemBrandEditForm() : base(GeneralInventoryCatalogDescriptors.ItemBrands) { }

    public ItemBrandEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.ItemBrands, item, copyMode) { }
}
