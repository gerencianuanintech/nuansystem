using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemTypes;

public sealed class ItemTypeEditForm : GeneralInventoryCatalogEditFormBase
{
    public ItemTypeEditForm() : base(GeneralInventoryCatalogDescriptors.ItemTypes) { }

    public ItemTypeEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.ItemTypes, item, copyMode) { }
}
