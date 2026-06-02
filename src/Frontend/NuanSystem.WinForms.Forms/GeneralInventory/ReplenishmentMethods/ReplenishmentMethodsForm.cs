using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ReplenishmentMethods;

public sealed class ReplenishmentMethodsForm : GeneralInventoryCatalogListFormBase
{
    public ReplenishmentMethodsForm() : base(GeneralInventoryCatalogDescriptors.ReplenishmentMethods) { }

    public ReplenishmentMethodsForm(GeneralInventoryCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
        : base(GeneralInventoryCatalogDescriptors.ReplenishmentMethods, viewModel, session, columnSettingsClient) { }

    protected override Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false)
    {
        return item is null ? new ReplenishmentMethodEditForm() : new ReplenishmentMethodEditForm(item, copyMode);
    }
}
