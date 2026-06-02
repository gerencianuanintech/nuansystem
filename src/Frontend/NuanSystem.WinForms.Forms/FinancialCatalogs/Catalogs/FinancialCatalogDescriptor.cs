using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.FinancialCatalogs.Catalogs;

public sealed record FinancialCatalogDescriptor(
    string Route,
    string FormKey,
    string Title,
    string SingularTitle,
    string CodeLabel,
    string NameLabel,
    CrudOperationPermissions Permissions);
