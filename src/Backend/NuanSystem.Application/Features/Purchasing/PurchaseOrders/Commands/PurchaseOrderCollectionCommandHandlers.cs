using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed class AddPurchaseOrderRelatedDocumentCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<AddPurchaseOrderRelatedDocumentCommand, PurchaseOrderDto>
{
    public Task<Result<PurchaseOrderDto>> Handle(AddPurchaseOrderRelatedDocumentCommand request, CancellationToken cancellationToken)
    {
        return PurchaseOrderCollections.UpdateAsync(
            repository,
            request.Id,
            current => PurchaseOrderMapping.ToSaveRequest(
                current,
                relatedDocuments: current.RelatedDocuments
                    .Select(PurchaseOrderMapping.ToSaveRequest)
                    .Append(request.RelatedDocument with { Id = null })
                    .ToArray()),
            "Documento relacionado agregado correctamente.",
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

public sealed class DeletePurchaseOrderRelatedDocumentCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<DeletePurchaseOrderRelatedDocumentCommand, PurchaseOrderDto>
{
    public Task<Result<PurchaseOrderDto>> Handle(DeletePurchaseOrderRelatedDocumentCommand request, CancellationToken cancellationToken)
    {
        return PurchaseOrderCollections.UpdateAsync(
            repository,
            request.Id,
            current => PurchaseOrderMapping.ToSaveRequest(
                current,
                relatedDocuments: current.RelatedDocuments
                    .Where(document => document.Id != request.RelatedId)
                    .Select(PurchaseOrderMapping.ToSaveRequest)
                    .ToArray()),
            "Documento relacionado desvinculado correctamente.",
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

public sealed class AddPurchaseOrderAttachmentCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<AddPurchaseOrderAttachmentCommand, PurchaseOrderDto>
{
    public Task<Result<PurchaseOrderDto>> Handle(AddPurchaseOrderAttachmentCommand request, CancellationToken cancellationToken)
    {
        return PurchaseOrderCollections.UpdateAsync(
            repository,
            request.Id,
            current => PurchaseOrderMapping.ToSaveRequest(
                current,
                attachments: current.Attachments
                    .Select(PurchaseOrderMapping.ToSaveRequest)
                    .Append(request.Attachment with { Id = null })
                    .ToArray()),
            "Anexo agregado correctamente.",
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

public sealed class DeletePurchaseOrderAttachmentCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<DeletePurchaseOrderAttachmentCommand, PurchaseOrderDto>
{
    public Task<Result<PurchaseOrderDto>> Handle(DeletePurchaseOrderAttachmentCommand request, CancellationToken cancellationToken)
    {
        return PurchaseOrderCollections.UpdateAsync(
            repository,
            request.Id,
            current => PurchaseOrderMapping.ToSaveRequest(
                current,
                attachments: current.Attachments
                    .Where(attachment => attachment.Id != request.AttachmentId)
                    .Select(PurchaseOrderMapping.ToSaveRequest)
                    .ToArray()),
            "Anexo eliminado correctamente.",
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}
