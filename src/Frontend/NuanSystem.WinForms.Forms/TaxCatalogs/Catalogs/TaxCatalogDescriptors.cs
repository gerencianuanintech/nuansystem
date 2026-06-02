using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs;

namespace NuanSystem.WinForms.Forms.TaxCatalogs.Catalogs;

public static class TaxCatalogDescriptors
{
    public static TaxCatalogDescriptor TaxRegimes { get; } = new(
        TaxCatalogRoutes.TaxRegimes,
        "tax-regimes",
        "Regimenes tributarios",
        "regimen tributario",
        "Codigo del regimen",
        "Nombre del regimen",
        new CrudOperationPermissions(PermissionCodes.TaxRegimesRead, PermissionCodes.TaxRegimesManage, PermissionCodes.TaxRegimesManage, PermissionCodes.TaxRegimesManage));

    public static TaxCatalogDescriptor TaxpayerTypes { get; } = new(
        TaxCatalogRoutes.TaxpayerTypes,
        "taxpayer-types",
        "Tipos de contribuyente",
        "tipo de contribuyente",
        "Codigo del tipo",
        "Nombre del tipo",
        new CrudOperationPermissions(PermissionCodes.TaxpayerTypesRead, PermissionCodes.TaxpayerTypesManage, PermissionCodes.TaxpayerTypesManage, PermissionCodes.TaxpayerTypesManage));

    public static TaxCatalogDescriptor RetentionTypes { get; } = new(
        TaxCatalogRoutes.RetentionTypes,
        "retention-types",
        "Tipos de retencion",
        "tipo de retencion",
        "Codigo del tipo",
        "Nombre del tipo",
        new CrudOperationPermissions(PermissionCodes.RetentionTypesRead, PermissionCodes.RetentionTypesManage, PermissionCodes.RetentionTypesManage, PermissionCodes.RetentionTypesManage));

    public static TaxCatalogDescriptor TaxSupports { get; } = new(
        TaxCatalogRoutes.TaxSupports,
        "tax-supports",
        "Sustentos tributarios",
        "sustento tributario",
        "Codigo del sustento",
        "Nombre del sustento",
        new CrudOperationPermissions(PermissionCodes.TaxSupportsRead, PermissionCodes.TaxSupportsManage, PermissionCodes.TaxSupportsManage, PermissionCodes.TaxSupportsManage));

    public static TaxCatalogDescriptor RetentionConcepts { get; } = new(
        TaxCatalogRoutes.RetentionConcepts,
        "retention-concepts",
        "Conceptos de retencion",
        "concepto de retencion",
        "Codigo del concepto",
        "Nombre del concepto",
        new CrudOperationPermissions(PermissionCodes.RetentionConceptsRead, PermissionCodes.RetentionConceptsManage, PermissionCodes.RetentionConceptsManage, PermissionCodes.RetentionConceptsManage));
}
