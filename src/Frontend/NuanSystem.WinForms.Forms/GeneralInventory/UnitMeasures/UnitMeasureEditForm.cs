using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.UnitMeasures;

public sealed class UnitMeasureEditForm : GeneralInventoryCatalogEditFormBase
{
    public UnitMeasureEditForm() : base(GeneralInventoryCatalogDescriptors.UnitMeasures) { }

    public UnitMeasureEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.UnitMeasures, item, copyMode) { }
}
