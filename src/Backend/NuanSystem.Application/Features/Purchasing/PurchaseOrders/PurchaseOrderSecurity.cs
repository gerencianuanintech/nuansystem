namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders;

public static class PurchaseOrderSecurity
{
    public const string FormKeyList = "purchase-orders";
    public const string FormKeyEdit = "purchase-orders-edit";
    public const string DocumentType = "PURCHASE_ORDER";

    public const string ActionCreate = "create";
    public const string ActionUpdate = "update";
    public const string ActionDelete = "delete";
    public const string ActionApprove = "approve";
    public const string ActionReject = "reject";
    public const string ActionSyncSap = "syncsap";
}
