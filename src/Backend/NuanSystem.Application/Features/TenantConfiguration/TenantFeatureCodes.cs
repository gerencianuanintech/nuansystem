namespace NuanSystem.Application.Features.TenantConfiguration;

public static class TenantFeatureCodes
{
    public const string SapB1Integration = "SAP_B1_INTEGRATION";
    public const string SriDocuments = "SRI_DOCUMENTS";
    public const string MultiBranchSync = "MULTI_BRANCH_SYNC";
    public const string InventoryModule = "INVENTORY_MODULE";
    public const string PurchasesModule = "PURCHASES_MODULE";
    public const string SalesModule = "SALES_MODULE";
    public const string AccountingModule = "ACCOUNTING_MODULE";
    public const string OfflineBranchMode = "OFFLINE_BRANCH_MODE";

    public static readonly IReadOnlyCollection<string> All =
    [
        SapB1Integration,
        SriDocuments,
        MultiBranchSync,
        InventoryModule,
        PurchasesModule,
        SalesModule,
        AccountingModule,
        OfflineBranchMode
    ];
}

