namespace NuanSystem.WinForms.ViewModels.GeneralSupplier.Catalogs;

public sealed record GeneralSupplierCatalogDescriptor(
    string Route,
    string FormKey,
    string Title,
    string SingularTitle,
    string CodeLabel,
    string NameLabel,
    string ReadPermission,
    string CreatePermission,
    string UpdatePermission,
    string DeletePermission);
