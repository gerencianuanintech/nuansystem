using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.StorageConditions;

public sealed class StorageConditionEditForm : GeneralInventoryCatalogEditFormBase
{
    public StorageConditionEditForm() : base(GeneralInventoryCatalogDescriptors.StorageConditions) { }

    public StorageConditionEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.StorageConditions, item, copyMode) { }
}
