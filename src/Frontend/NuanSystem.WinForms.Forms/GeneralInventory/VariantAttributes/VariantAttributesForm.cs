using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.VariantAttributes;

public sealed class VariantAttributesForm : GeneralInventoryCatalogListFormBase
{
    public VariantAttributesForm() : base(GeneralInventoryCatalogDescriptors.VariantAttributes) { }

    public VariantAttributesForm(GeneralInventoryCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
        : base(GeneralInventoryCatalogDescriptors.VariantAttributes, viewModel, session, columnSettingsClient) { }

    protected override Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false)
    {
        return item is null ? new VariantAttributeEditForm() : new VariantAttributeEditForm(item, copyMode);
    }
}
