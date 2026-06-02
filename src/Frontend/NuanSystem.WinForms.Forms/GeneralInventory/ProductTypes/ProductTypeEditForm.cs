using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ProductTypes;

public sealed class ProductTypeEditForm : GeneralInventoryCatalogEditFormBase
{
    public ProductTypeEditForm() : base(GeneralInventoryCatalogDescriptors.ProductTypes) { }

    public ProductTypeEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.ProductTypes, item, copyMode) { }
}
