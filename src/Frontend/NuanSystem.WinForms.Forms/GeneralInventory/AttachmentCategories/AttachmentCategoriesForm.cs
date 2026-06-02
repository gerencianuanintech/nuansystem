using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.AttachmentCategories;

public sealed class AttachmentCategoriesForm : GeneralInventoryCatalogListFormBase
{
    public AttachmentCategoriesForm() : base(GeneralInventoryCatalogDescriptors.AttachmentCategories) { }

    public AttachmentCategoriesForm(GeneralInventoryCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
        : base(GeneralInventoryCatalogDescriptors.AttachmentCategories, viewModel, session, columnSettingsClient) { }

    protected override Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false)
    {
        return item is null ? new AttachmentCategoryEditForm() : new AttachmentCategoryEditForm(item, copyMode);
    }
}
