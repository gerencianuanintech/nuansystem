using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.AttachmentDocumentTypes;

public sealed class AttachmentDocumentTypeEditForm : GeneralInventoryCatalogEditFormBase
{
    public AttachmentDocumentTypeEditForm() : base(GeneralInventoryCatalogDescriptors.AttachmentDocumentTypes) { }

    public AttachmentDocumentTypeEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.AttachmentDocumentTypes, item, copyMode) { }
}
