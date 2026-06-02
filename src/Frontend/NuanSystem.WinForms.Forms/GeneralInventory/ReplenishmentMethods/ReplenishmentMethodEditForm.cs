using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ReplenishmentMethods;

public sealed class ReplenishmentMethodEditForm : GeneralInventoryCatalogEditFormBase
{
    public ReplenishmentMethodEditForm() : base(GeneralInventoryCatalogDescriptors.ReplenishmentMethods) { }

    public ReplenishmentMethodEditForm(GeneralInventoryCatalogItem item, bool copyMode = false)
        : base(GeneralInventoryCatalogDescriptors.ReplenishmentMethods, item, copyMode) { }
}
