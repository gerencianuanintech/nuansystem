using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.TaxCatalogs.Catalogs;

public sealed record TaxCatalogDescriptor(
    string Route,
    string FormKey,
    string Title,
    string SingularTitle,
    string CodeLabel,
    string NameLabel,
    CrudOperationPermissions Permissions);
