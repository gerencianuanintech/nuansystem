using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.SalesChannels;

public sealed class SalesChannelEditForm : GeneralInventoryCatalogEditFormBase
{
    public SalesChannelEditForm() : base(GeneralInventoryCatalogDescriptors.SalesChannels) { }

    public SalesChannelEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.SalesChannels, item, copyMode) { }
}
