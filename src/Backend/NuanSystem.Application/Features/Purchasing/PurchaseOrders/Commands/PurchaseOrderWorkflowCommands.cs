using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed record DeletePurchaseOrderCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

public sealed record SendPurchaseOrderToApprovalCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PurchaseOrderDto>;

public sealed record ApprovePurchaseOrderCommand(
    int Id,
    string? Observation,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PurchaseOrderDto>;

public sealed record RejectPurchaseOrderCommand(
    int Id,
    string? Observation,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PurchaseOrderDto>;

public sealed record SyncPurchaseOrderSapCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PurchaseOrderDto>;

public sealed record AddPurchaseOrderRelatedDocumentCommand(
    int Id,
    PurchaseOrderRelatedDocumentSaveRequest RelatedDocument,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PurchaseOrderDto>;

public sealed record DeletePurchaseOrderRelatedDocumentCommand(
    int Id,
    int RelatedId,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PurchaseOrderDto>;

public sealed record AddPurchaseOrderAttachmentCommand(
    int Id,
    PurchaseOrderAttachmentSaveRequest Attachment,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PurchaseOrderDto>;

public sealed record DeletePurchaseOrderAttachmentCommand(
    int Id,
    int AttachmentId,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PurchaseOrderDto>;
