using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

internal static class PurchaseOrderSecurityGuard
{
    public static async Task<Result<bool>> ValidateSeriesOperationAsync(
        ISecurityDocumentSeriesAccessRepository seriesAccessRepository,
        ICompanyContext companyContext,
        int? userId,
        int? securityDocumentSeriesId,
        string actionKey,
        CancellationToken cancellationToken)
    {
        if (userId is null or <= 0)
        {
            return Result<bool>.Failure("No se pudo identificar el usuario autenticado.");
        }

        if (companyContext.CurrentCompany is null)
        {
            return Result<bool>.Failure("Debe seleccionar una empresa.");
        }

        if (securityDocumentSeriesId is null or <= 0)
        {
            return Result<bool>.Failure("Debe seleccionar una serie de documento autorizada.");
        }

        var isAllowed = await seriesAccessRepository.ValidateUserOperationAsync(
            userId.Value,
            companyContext.CurrentCompany.CompanyCode,
            PurchaseOrderSecurity.FormKeyEdit,
            PurchaseOrderSecurity.DocumentType,
            securityDocumentSeriesId.Value,
            actionKey,
            cancellationToken);

        return isAllowed
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No tienes permiso para ejecutar esta operacion con la serie seleccionada.", FailureKind.Forbidden,
                [new ApiError("PURCHASE_ORDER_SERIES_FORBIDDEN", "No tienes permiso para operar con la serie seleccionada.", "DocumentSeriesId")]);
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
            return Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.", FailureKind.NotFound,
                [new ApiError("PURCHASE_ORDER_NOT_FOUND", "La orden de compra no existe.", nameof(id))]);
        }

        var validation = PurchaseOrderWorkflowPolicy.EnsureCanModifyAttachmentsOrRelatedDocuments(current.Status);
        if (!validation.IsSuccess)
        {
            return Result<PurchaseOrderDto>.Failure(validation.Message, FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", validation.Message)]);
        }

        if (string.IsNullOrWhiteSpace(current.SeriesCode) || string.IsNullOrWhiteSpace(current.DocumentNumber))
        {
            return PurchaseOrderNumberingFailure.Missing();
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
            return Result<PurchaseOrderDto>.Failure("No se pudo actualizar la orden de compra.", FailureKind.Conflict,
                [new ApiError("PURCHASE_ORDER_STATE_CONFLICT", "La orden no admite la actualizacion solicitada.")]);
        }

        var order = await repository.GetByIdAsync(id, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se pudo recuperar la orden de compra.", FailureKind.Unexpected)
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

internal static class PurchaseOrderNumberingFailure
{
    public static Result<PurchaseOrderDto> Missing() => Result<PurchaseOrderDto>.Failure(
        "La orden de compra no tiene una serie y numero validos. Revise su configuracion sin renumerar el documento.",
        FailureKind.Conflict,
        [new ApiError("PURCHASE_ORDER_NUMBERING_MISSING", "La serie y el numero persistidos son obligatorios.")]);
}
