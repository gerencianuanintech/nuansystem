using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

namespace NuanSystem.WinForms.Forms.GeneralInventory.UnitMeasures;

public sealed class UnitMeasuresForm : GeneralInventoryCatalogListFormBase
{
    public UnitMeasuresForm() : base(GeneralInventoryCatalogDescriptors.UnitMeasures) { }

    public UnitMeasuresForm(GeneralInventoryCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
        : base(GeneralInventoryCatalogDescriptors.UnitMeasures, viewModel, session, columnSettingsClient) { }

    protected override Form CreateEditForm(GeneralInventoryCatalogItem? item = null, bool copyMode = false)
    {
        return item is null ? new UnitMeasureEditForm() : new UnitMeasureEditForm(item, copyMode);
    }
}
