using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.AttachmentCategories;

public sealed class AttachmentCategoryEditForm : GeneralInventoryCatalogEditFormBase
{
    public AttachmentCategoryEditForm() : base(GeneralInventoryCatalogDescriptors.AttachmentCategories) { }

    public AttachmentCategoryEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.AttachmentCategories, item, copyMode) { }
}
