using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs;

namespace NuanSystem.WinForms.Forms.FinancialCatalogs.Catalogs;

public static class FinancialCatalogDescriptors
{
    public static FinancialCatalogDescriptor Banks { get; } = new(
        FinancialCatalogRoutes.Banks,
        "banks",
        "Bancos",
        "banco",
        "Codigo del banco",
        "Nombre del banco",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsBanksRead,
            PermissionCodes.FinancialCatalogsBanksManage,
            PermissionCodes.FinancialCatalogsBanksManage,
            PermissionCodes.FinancialCatalogsBanksManage));

    public static FinancialCatalogDescriptor BankAccountTypes { get; } = new(
        FinancialCatalogRoutes.BankAccountTypes,
        "bank-account-types",
        "Tipos de cuenta bancaria",
        "tipo de cuenta bancaria",
        "Codigo del tipo",
        "Nombre del tipo",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsBankAccountTypesRead,
            PermissionCodes.FinancialCatalogsBankAccountTypesManage,
            PermissionCodes.FinancialCatalogsBankAccountTypesManage,
            PermissionCodes.FinancialCatalogsBankAccountTypesManage));

    public static FinancialCatalogDescriptor Currencies { get; } = new(
        FinancialCatalogRoutes.Currencies,
        "currencies",
        "Monedas",
        "moneda",
        "Codigo de moneda",
        "Nombre de moneda",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsCurrenciesRead,
            PermissionCodes.FinancialCatalogsCurrenciesManage,
            PermissionCodes.FinancialCatalogsCurrenciesManage,
            PermissionCodes.FinancialCatalogsCurrenciesManage));

    public static FinancialCatalogDescriptor PriceLists { get; } = new(
        FinancialCatalogRoutes.PriceLists,
        "price-lists",
        "Listas de precios",
        "lista de precios",
        "Codigo de lista",
        "Nombre de lista",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsPriceListsRead,
            PermissionCodes.FinancialCatalogsPriceListsManage,
            PermissionCodes.FinancialCatalogsPriceListsManage,
            PermissionCodes.FinancialCatalogsPriceListsManage));

    public static FinancialCatalogDescriptor PurchasingAgents { get; } = new(
        FinancialCatalogRoutes.PurchasingAgents,
        "purchasing-agents",
        "Compradores",
        "comprador",
        "Codigo del comprador",
        "Nombre del comprador",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsPurchasingAgentsRead,
            PermissionCodes.FinancialCatalogsPurchasingAgentsManage,
            PermissionCodes.FinancialCatalogsPurchasingAgentsManage,
            PermissionCodes.FinancialCatalogsPurchasingAgentsManage));

    public static FinancialCatalogDescriptor AccountingPaymentMethods { get; } = new(
        FinancialCatalogRoutes.AccountingPaymentMethods,
        "accounting-payment-methods",
        "Metodos de pago contable",
        "metodo de pago contable",
        "Codigo del metodo",
        "Nombre del metodo",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsAccountingPaymentMethodsRead,
            PermissionCodes.FinancialCatalogsAccountingPaymentMethodsManage,
            PermissionCodes.FinancialCatalogsAccountingPaymentMethodsManage,
            PermissionCodes.FinancialCatalogsAccountingPaymentMethodsManage));

    public static FinancialCatalogDescriptor PaymentPriorities { get; } = new(
        FinancialCatalogRoutes.PaymentPriorities,
        "payment-priorities",
        "Prioridades de pago",
        "prioridad de pago",
        "Codigo de prioridad",
        "Nombre de prioridad",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsPaymentPrioritiesRead,
            PermissionCodes.FinancialCatalogsPaymentPrioritiesManage,
            PermissionCodes.FinancialCatalogsPaymentPrioritiesManage,
            PermissionCodes.FinancialCatalogsPaymentPrioritiesManage));

    public static FinancialCatalogDescriptor ApprovalFlows { get; } = new(
        FinancialCatalogRoutes.ApprovalFlows,
        "approval-flows",
        "Flujos de aprobacion",
        "flujo de aprobacion",
        "Codigo del flujo",
        "Nombre del flujo",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsApprovalFlowsRead,
            PermissionCodes.FinancialCatalogsApprovalFlowsManage,
            PermissionCodes.FinancialCatalogsApprovalFlowsManage,
            PermissionCodes.FinancialCatalogsApprovalFlowsManage));

    public static FinancialCatalogDescriptor PaymentDocumentTypes { get; } = new(
        FinancialCatalogRoutes.PaymentDocumentTypes,
        "payment-document-types",
        "Tipos de documento de pago",
        "tipo de documento de pago",
        "Codigo del tipo",
        "Nombre del tipo",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsPaymentDocumentTypesRead,
            PermissionCodes.FinancialCatalogsPaymentDocumentTypesManage,
            PermissionCodes.FinancialCatalogsPaymentDocumentTypesManage,
            PermissionCodes.FinancialCatalogsPaymentDocumentTypesManage));

    public static FinancialCatalogDescriptor Branches { get; } = new(
        FinancialCatalogRoutes.Branches,
        "branches",
        "Sucursales",
        "sucursal",
        "Codigo de sucursal",
        "Nombre de sucursal",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsBranchesRead,
            PermissionCodes.FinancialCatalogsBranchesManage,
            PermissionCodes.FinancialCatalogsBranchesManage,
            PermissionCodes.FinancialCatalogsBranchesManage));

    public static FinancialCatalogDescriptor Departments { get; } = new(
        FinancialCatalogRoutes.Departments,
        "departments",
        "Departamentos",
        "departamento",
        "Codigo de departamento",
        "Nombre de departamento",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsDepartmentsRead,
            PermissionCodes.FinancialCatalogsDepartmentsManage,
            PermissionCodes.FinancialCatalogsDepartmentsManage,
            PermissionCodes.FinancialCatalogsDepartmentsManage));

    public static FinancialCatalogDescriptor BusinessLines { get; } = new(
        FinancialCatalogRoutes.BusinessLines,
        "business-lines",
        "Lineas de negocio",
        "linea de negocio",
        "Codigo de linea",
        "Nombre de linea",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsBusinessLinesRead,
            PermissionCodes.FinancialCatalogsBusinessLinesManage,
            PermissionCodes.FinancialCatalogsBusinessLinesManage,
            PermissionCodes.FinancialCatalogsBusinessLinesManage));

    public static FinancialCatalogDescriptor CostCenters { get; } = new(
        FinancialCatalogRoutes.CostCenters,
        "cost-centers",
        "Centros de costo",
        "centro de costo",
        "Codigo de centro",
        "Nombre de centro",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsCostCentersRead,
            PermissionCodes.FinancialCatalogsCostCentersManage,
            PermissionCodes.FinancialCatalogsCostCentersManage,
            PermissionCodes.FinancialCatalogsCostCentersManage));

    public static FinancialCatalogDescriptor Projects { get; } = new(
        FinancialCatalogRoutes.Projects,
        "projects",
        "Proyectos",
        "proyecto",
        "Codigo de proyecto",
        "Nombre de proyecto",
        new CrudOperationPermissions(
            PermissionCodes.FinancialCatalogsProjectsRead,
            PermissionCodes.FinancialCatalogsProjectsManage,
            PermissionCodes.FinancialCatalogsProjectsManage,
            PermissionCodes.FinancialCatalogsProjectsManage));
}
