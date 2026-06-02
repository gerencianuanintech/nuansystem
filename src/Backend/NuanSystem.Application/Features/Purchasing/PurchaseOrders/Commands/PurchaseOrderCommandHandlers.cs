using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed class CreatePurchaseOrderCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var data = PurchaseOrderCalculator.BuildPersistData(
            null,
            request.ToRequest(),
            PurchaseOrderStatuses.Draft,
            request.AuditUserId,
            request.AuditUserName);

        var id = await repository.CreateAsync(data, cancellationToken);
        var order = await repository.GetByIdAsync(id, cancellationToken);

        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se pudo recuperar la orden de compra creada.")
            : Result<PurchaseOrderDto>.Success(order, "Orden de compra guardada correctamente.");
    }
}

public sealed class UpdatePurchaseOrderCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<UpdatePurchaseOrderCommand, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.");
        }

        var validation = PurchaseOrderWorkflowPolicy.EnsureCanEdit(current.Status);
        if (!validation.IsSuccess)
        {
            return Result<PurchaseOrderDto>.Failure(validation.Message);
        }

        // TODO: Implementar UpdateIfEditableAsync con guarda atomica y control de concurrencia optimista/RowVersion.
        var data = PurchaseOrderCalculator.BuildPersistData(
            request.Id,
            request.ToRequest(),
            current.Status,
            request.AuditUserId,
            request.AuditUserName,
            current.SapStatus);

        var updated = await repository.UpdateAsync(data, cancellationToken);
        if (!updated)
        {
            return Result<PurchaseOrderDto>.Failure("No se pudo actualizar la orden de compra.");
        }

        var order = await repository.GetByIdAsync(request.Id, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se pudo recuperar la orden de compra actualizada.")
            : Result<PurchaseOrderDto>.Success(order, "Orden de compra actualizada correctamente.");
    }
}

public sealed class DeletePurchaseOrderCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<DeletePurchaseOrderCommand, bool>
{
    public async Task<Result<bool>> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<bool>.Failure("No se encontro la orden de compra.");
        }

        var validation = PurchaseOrderWorkflowPolicy.EnsureCanDelete(current.Status);
        if (!validation.IsSuccess)
        {
            return Result<bool>.Failure(validation.Message);
        }

        // TODO: Implementar DeleteIfCurrentAsync para anulacion segura con guarda atomica.
        var deleted = await repository.DeleteAsync(request.Id, request.AuditUserId, request.AuditUserName, cancellationToken);
        return deleted
            ? Result<bool>.Success(true, "Orden de compra eliminada correctamente.")
            : Result<bool>.Failure("No se pudo eliminar la orden de compra.");
    }
}

public sealed class SendPurchaseOrderToApprovalCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<SendPurchaseOrderToApprovalCommand, PurchaseOrderDto>
{
    public Task<Result<PurchaseOrderDto>> Handle(SendPurchaseOrderToApprovalCommand request, CancellationToken cancellationToken)
    {
        return PurchaseOrderWorkflow.ChangeStatusAsync(
            repository,
            request.Id,
            PurchaseOrderStatuses.PendingApproval,
            "Orden enviada a aprobacion.",
            [PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected],
            PurchaseOrderWorkflowPolicy.EnsureCanSendToApproval,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

public sealed class ApprovePurchaseOrderCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<ApprovePurchaseOrderCommand, PurchaseOrderDto>
{
    public Task<Result<PurchaseOrderDto>> Handle(ApprovePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        return PurchaseOrderWorkflow.ChangeStatusAsync(
            repository,
            request.Id,
            PurchaseOrderStatuses.Approved,
            "Orden aprobada correctamente.",
            [PurchaseOrderStatuses.PendingApproval],
            PurchaseOrderWorkflowPolicy.EnsureCanApprove,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

public sealed class RejectPurchaseOrderCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<RejectPurchaseOrderCommand, PurchaseOrderDto>
{
    public Task<Result<PurchaseOrderDto>> Handle(RejectPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        return PurchaseOrderWorkflow.ChangeStatusAsync(
            repository,
            request.Id,
            PurchaseOrderStatuses.Rejected,
            "Orden rechazada.",
            [PurchaseOrderStatuses.PendingApproval],
            PurchaseOrderWorkflowPolicy.EnsureCanReject,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

public sealed class SyncPurchaseOrderSapCommandHandler(IPurchaseOrderRepository repository)
    : ICommandHandler<SyncPurchaseOrderSapCommand, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(SyncPurchaseOrderSapCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.");
        }

        var validation = PurchaseOrderWorkflowPolicy.EnsureCanRequestSapSync(current.Status);
        if (!validation.IsSuccess)
        {
            await repository.AddSapLogAsync(request.Id, "PurchaseOrderSync", "Skipped", validation.Message, request.AuditUserId, request.AuditUserName, cancellationToken);
            return Result<PurchaseOrderDto>.Failure(validation.Message);
        }

        if (current.SapStatus == PurchaseOrderSapStatuses.Synced)
        {
            return Result<PurchaseOrderDto>.Failure("La orden ya fue sincronizada con SAP.");
        }

        await repository.AddSapLogAsync(request.Id, "PurchaseOrderSync", "Pending", "Pendiente de envio a SAP Business One. ObjectType 22.", request.AuditUserId, request.AuditUserName, cancellationToken);
        var updated = await repository.UpdateStatusIfCurrentAsync(
            request.Id,
            PurchaseOrderStatuses.SapPending,
            [PurchaseOrderStatuses.Approved, PurchaseOrderStatuses.SapError],
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
        if (!updated)
        {
            return Result<PurchaseOrderDto>.Failure("No se pudo actualizar el estado de la orden.");
        }

        var order = await repository.GetByIdAsync(request.Id, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se pudo recuperar la orden de compra.")
            : Result<PurchaseOrderDto>.Success(order, "Orden marcada como pendiente de sincronizacion SAP.");
    }
}

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

internal static class PurchaseOrderWorkflow
{
    public static async Task<Result<PurchaseOrderDto>> ChangeStatusAsync(
        IPurchaseOrderRepository repository,
        int id,
        string status,
        string message,
        IReadOnlyCollection<string> expectedCurrentStatuses,
        Func<string, Result<bool>> validateCurrentStatus,
        int? userId,
        string? userName,
        CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(id, cancellationToken);
        if (current is null)
        {
            return Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.");
        }

        var validation = validateCurrentStatus(current.Status);
        if (!validation.IsSuccess)
        {
            return Result<PurchaseOrderDto>.Failure(validation.Message);
        }

        // TODO: Persistir aprobador, fecha, observacion y nivel de aprobacion.
        var updated = await repository.UpdateStatusIfCurrentAsync(id, status, expectedCurrentStatuses, userId, userName, cancellationToken);
        if (!updated)
        {
            return Result<PurchaseOrderDto>.Failure("No se pudo actualizar el estado de la orden.");
        }

        var order = await repository.GetByIdAsync(id, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se pudo recuperar la orden de compra.")
            : Result<PurchaseOrderDto>.Success(order, message);
    }
}

internal static class PurchaseOrderCollections
{
    public static async Task<Result<PurchaseOrderDto>> UpdateAsync(
        IPurchaseOrderRepository repository,
        int id,
        Func<PurchaseOrderDto, PurchaseOrderSaveRequest> buildRequest,
        string message,
        int? userId,
        string? userName,
        CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(id, cancellationToken);
        if (current is null)
        {
            return Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.");
        }

        var validation = PurchaseOrderWorkflowPolicy.EnsureCanModifyAttachmentsOrRelatedDocuments(current.Status);
        if (!validation.IsSuccess)
        {
            return Result<PurchaseOrderDto>.Failure(validation.Message);
        }

        var data = PurchaseOrderCalculator.BuildPersistData(
            id,
            buildRequest(current),
            current.Status,
            userId,
            userName,
            current.SapStatus);

        var updated = await repository.UpdateAsync(data, cancellationToken);
        if (!updated)
        {
            return Result<PurchaseOrderDto>.Failure("No se pudo actualizar la orden de compra.");
        }

        var order = await repository.GetByIdAsync(id, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se pudo recuperar la orden de compra.")
            : Result<PurchaseOrderDto>.Success(order, message);
    }
}

internal static class PurchaseOrderMapping
{
    public static PurchaseOrderSaveRequest ToSaveRequest(
        PurchaseOrderDto order,
        IReadOnlyCollection<PurchaseOrderRelatedDocumentSaveRequest>? relatedDocuments = null,
        IReadOnlyCollection<PurchaseOrderAttachmentSaveRequest>? attachments = null)
    {
        return new PurchaseOrderSaveRequest(
            order.BranchId,
            order.DocumentSeriesId,
            order.SeriesCode,
            order.DocumentNumber,
            order.SupplierId,
            order.SupplierCode,
            order.SupplierName,
            order.SupplierTaxId,
            order.ContactName,
            order.Phone,
            order.Email,
            order.DocumentDate,
            order.DeliveryDate,
            order.CurrencyCode,
            order.ExchangeRate,
            order.PaymentTermId,
            order.PriceListId,
            order.BuyerId,
            order.MainWarehouseId,
            order.ProjectId,
            order.CostCenterId,
            order.PurchaseTypeId,
            order.Comments,
            order.DiscountPercent,
            order.Lines.Select(ToSaveRequest).ToArray(),
            order.Addresses.Select(ToSaveRequest).ToArray(),
            relatedDocuments ?? order.RelatedDocuments.Select(ToSaveRequest).ToArray(),
            attachments ?? order.Attachments.Select(ToSaveRequest).ToArray());
    }

    public static PurchaseOrderRelatedDocumentSaveRequest ToSaveRequest(PurchaseOrderRelatedDocumentDto document)
    {
        return new PurchaseOrderRelatedDocumentSaveRequest(
            document.Id,
            document.RelatedDocumentType,
            document.RelatedDocumentId,
            document.Series,
            document.Number,
            document.Date,
            document.Status,
            document.Reference,
            document.Comment,
            document.Total);
    }

    public static PurchaseOrderAttachmentSaveRequest ToSaveRequest(PurchaseOrderAttachmentDto attachment)
    {
        return new PurchaseOrderAttachmentSaveRequest(
            attachment.Id,
            attachment.FileName,
            attachment.OriginalFileName,
            attachment.FileExtension,
            attachment.MimeType,
            attachment.FileSize,
            attachment.StoragePath,
            attachment.Status,
            attachment.Comment);
    }

    private static PurchaseOrderLineSaveRequest ToSaveRequest(PurchaseOrderLineDto line)
    {
        return new PurchaseOrderLineSaveRequest(
            line.Id,
            line.LineNumber,
            line.ItemId,
            line.ItemCode,
            line.ItemName,
            line.UnitId,
            line.UnitCode,
            line.Quantity,
            line.UnitPrice,
            line.DiscountPercent,
            line.TaxId,
            line.TaxCode,
            line.TaxRate,
            line.WarehouseId,
            line.WarehouseCode,
            line.DeliveryDate,
            line.CostCenterId,
            line.ProjectId);
    }

    private static PurchaseOrderAddressSaveRequest ToSaveRequest(PurchaseOrderAddressDto address)
    {
        return new PurchaseOrderAddressSaveRequest(
            address.Id,
            address.AddressType,
            address.SourceAddressId,
            address.AddressName,
            address.Street,
            address.Reference,
            address.City,
            address.State,
            address.ZipCode,
            address.Country,
            address.Phone,
            address.Email,
            address.IsModified);
    }
}
