using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemSubgroups;

public sealed class ItemSubgroupEditForm : GeneralInventoryCatalogEditFormBase
{
    public ItemSubgroupEditForm() : base(GeneralInventoryCatalogDescriptors.ItemSubgroups) { }

    public ItemSubgroupEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.ItemSubgroups, item, copyMode) { }
}
