using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.AttachmentDocumentTypes;

public sealed class AttachmentDocumentTypesForm : GeneralInventoryCatalogListFormBase
{
    public AttachmentDocumentTypesForm() : base(GeneralInventoryCatalogDescriptors.AttachmentDocumentTypes) { }

    public AttachmentDocumentTypesForm(GeneralInventoryCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
        : base(GeneralInventoryCatalogDescriptors.AttachmentDocumentTypes, viewModel, session, columnSettingsClient) { }

    protected override Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false)
    {
        return item is null ? new AttachmentDocumentTypeEditForm() : new AttachmentDocumentTypeEditForm(item, copyMode);
    }
}
