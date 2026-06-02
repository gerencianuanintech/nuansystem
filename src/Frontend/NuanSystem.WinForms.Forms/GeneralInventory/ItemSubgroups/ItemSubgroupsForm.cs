using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemSubgroups;

public sealed class ItemSubgroupsForm : GeneralInventoryCatalogListFormBase
{
    public ItemSubgroupsForm() : base(GeneralInventoryCatalogDescriptors.ItemSubgroups) { }

    public ItemSubgroupsForm(GeneralInventoryCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
        : base(GeneralInventoryCatalogDescriptors.ItemSubgroups, viewModel, session, columnSettingsClient) { }

    protected override Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false)
    {
        return item is null ? new ItemSubgroupEditForm() : new ItemSubgroupEditForm(item, copyMode);
    }
}
