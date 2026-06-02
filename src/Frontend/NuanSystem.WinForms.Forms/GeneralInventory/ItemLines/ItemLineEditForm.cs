using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemLines;

public sealed class ItemLineEditForm : GeneralInventoryCatalogEditFormBase
{
    public ItemLineEditForm() : base(GeneralInventoryCatalogDescriptors.ItemLines) { }

    public ItemLineEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.ItemLines, item, copyMode) { }
}
