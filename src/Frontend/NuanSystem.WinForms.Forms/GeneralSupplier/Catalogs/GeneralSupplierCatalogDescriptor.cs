using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.GeneralSupplier.Catalogs;

public sealed record GeneralSupplierCatalogDescriptor(
    string Route,
    string FormKey,
    string Title,
    string SingularTitle,
    string CodeLabel,
    string NameLabel,
    CrudOperationPermissions Permissions);

