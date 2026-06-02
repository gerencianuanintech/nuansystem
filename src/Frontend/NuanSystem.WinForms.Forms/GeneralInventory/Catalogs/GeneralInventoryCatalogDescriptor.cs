using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;

public sealed record GeneralInventoryCatalogDescriptor(
    string Route,
    string FormKey,
    string Title,
    string SingularTitle,
    string CodeLabel,
    string NameLabel,
    CrudOperationPermissions Permissions);
