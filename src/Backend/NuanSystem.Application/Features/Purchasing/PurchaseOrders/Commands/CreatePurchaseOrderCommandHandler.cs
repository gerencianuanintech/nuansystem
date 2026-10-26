using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;

public sealed class CreatePurchaseOrderCommandHandler(
    IPurchaseOrderRepository repository,
    ISecurityDocumentSeriesRepository seriesRepository,
    ISecurityDocumentNumberingService numberingService,
    ISecurityDocumentSeriesAccessRepository seriesAccessRepository,
    ICompanyContext companyContext)
    : ICommandHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var seriesValidation = await PurchaseOrderSecurityGuard.ValidateSeriesOperationAsync(
            seriesAccessRepository,
            companyContext,
            request.AuditUserId,
            request.DocumentSeriesId,
            PurchaseOrderSecurity.ActionCreate,
            cancellationToken);
        if (!seriesValidation.IsSuccess)
        {
            return Result<PurchaseOrderDto>.Failure(seriesValidation.Message,
                seriesValidation.FailureKind ?? FailureKind.Validation, seriesValidation.Errors);
        }

        var series = await seriesRepository.GetByIdAsync(request.DocumentSeriesId!.Value, cancellationToken);
        if (series is null || !series.IsActive || series.DocumentType != PurchaseOrderSecurity.DocumentType || string.IsNullOrWhiteSpace(series.Code))
        {
            return Result<PurchaseOrderDto>.Failure("La serie seleccionada no existe, no esta activa o no corresponde a Orden de Compra.");
        }

        var reservation = await numberingService.ReserveNumberAsync(
            series.Id,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
        if (!reservation.Success || string.IsNullOrWhiteSpace(reservation.FormattedNumber))
        {
            return Result<PurchaseOrderDto>.Failure(reservation.Message);
        }

        var saveRequest = request.ToRequest() with
        {
            DocumentSeriesId = series.Id,
            SeriesCode = series.Code,
            DocumentNumber = reservation.FormattedNumber
        };

        var data = PurchaseOrderCalculator.BuildPersistData(
            null,
            saveRequest,
            PurchaseOrderStatuses.Draft,
            request.AuditUserId,
            request.AuditUserName);

        var id = await repository.CreateAsync(data, cancellationToken);
        var order = await repository.GetByIdAsync(id, cancellationToken);

        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se pudo recuperar la orden de compra creada.", FailureKind.Unexpected)
            : Result<PurchaseOrderDto>.Success(order, "Orden de compra guardada correctamente.");
    }
}
