namespace NuanSystem.Shared.Constants;

public static class PermissionCodes
{
    public const string CompaniesManage = "COMPANIES.MANAGE";
    public const string UsersManage = "SECURITY.USERS.MANAGE";
    public const string RolesManage = "SECURITY.ROLES.MANAGE";
    public const string AuditRead = "SECURITY.AUDIT.READ";
    public const string CustomersRead = "CATALOG.CUSTOMERS.READ";
    public const string CustomersManage = "CATALOG.CUSTOMERS.MANAGE";
    public const string ItemsRead = "CATALOG.ITEMS.READ";
    public const string ItemsManage = "CATALOG.ITEMS.MANAGE";
    public const string AccountingRead = "ACCOUNTING.CHARTOFACCOUNTS.READ";
    public const string AccountingManage = "ACCOUNTING.CHARTOFACCOUNTS.MANAGE";
    public const string DocumentsRead = "SALES.DOCUMENTS.READ";
    public const string DocumentsManage = "SALES.DOCUMENTS.MANAGE";
    public const string SapRead = "SAP.SYNC.READ";
    public const string SapManage = "SAP.SYNC.MANAGE";
    public const string SettingsManage = "SETTINGS.PARAMETERS.MANAGE";

    public static readonly IReadOnlyCollection<string> All =
    [
        CompaniesManage,
        UsersManage,
        RolesManage,
        AuditRead,
        CustomersRead,
        CustomersManage,
        ItemsRead,
        ItemsManage,
        AccountingRead,
        AccountingManage,
        DocumentsRead,
        DocumentsManage,
        SapRead,
        SapManage,
        SettingsManage
    ];
}
