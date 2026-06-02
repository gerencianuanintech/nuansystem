using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.VariantAttributes;

public sealed class VariantAttributeEditForm : GeneralInventoryCatalogEditFormBase
{
    public VariantAttributeEditForm() : base(GeneralInventoryCatalogDescriptors.VariantAttributes) { }

    public VariantAttributeEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.VariantAttributes, item, copyMode) { }
}
