using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Queries;

public sealed class GetPurchaseOrdersQueryHandler(IPurchaseOrderRepository repository)
    : IQueryHandler<GetPurchaseOrdersQuery, IReadOnlyCollection<PurchaseOrderSummaryDto>>
{
    public async Task<Result<IReadOnlyCollection<PurchaseOrderSummaryDto>>> Handle(
        GetPurchaseOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await repository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<PurchaseOrderSummaryDto>>.Success(orders);
    }
}

public sealed class GetPurchaseOrderByIdQueryHandler(IPurchaseOrderRepository repository)
    : IQueryHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(
        GetPurchaseOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(request.Id, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.")
            : Result<PurchaseOrderDto>.Success(order);
    }
}

public sealed class GetPurchaseOrderLookupsQueryHandler(
    IPurchaseOrderRepository repository,
    ISecurityDocumentSeriesAccessRepository seriesAccessRepository)
    : IQueryHandler<GetPurchaseOrderLookupsQuery, PurchaseOrderLookupsDto>
{
    public async Task<Result<PurchaseOrderLookupsDto>> Handle(
        GetPurchaseOrderLookupsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.CompanyCode))
        {
            return Result<PurchaseOrderLookupsDto>.Failure("Debe existir un usuario autenticado y una empresa activa.");
        }

        var lookups = await repository.GetLookupsAsync(cancellationToken);
        var authorizedSeriesIds = await seriesAccessRepository.GetAuthorizedSeriesIdsForUserAsync(
            request.UserId,
            request.CompanyCode.Trim(),
            request.SeriesFormKey.Trim(),
            request.DocumentType.Trim(),
            request.ActionKey.Trim(),
            cancellationToken);

        lookups = lookups with
        {
            DocumentSeries = lookups.DocumentSeries
                .Where(series => authorizedSeriesIds.Contains(series.Id))
                .ToArray()
        };

        return Result<PurchaseOrderLookupsDto>.Success(lookups);
    }
}

public sealed class GetPurchaseOrderFieldAccessQueryHandler(ISecurityRoleFormFieldAccessRepository fieldAccessRepository)
    : IQueryHandler<GetPurchaseOrderFieldAccessQuery, IReadOnlyCollection<PurchaseOrderFieldAccessDto>>
{
    public async Task<Result<IReadOnlyCollection<PurchaseOrderFieldAccessDto>>> Handle(
        GetPurchaseOrderFieldAccessQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.CompanyCode))
        {
            return Result<IReadOnlyCollection<PurchaseOrderFieldAccessDto>>.Failure("Debe existir un usuario autenticado y una empresa activa.");
        }

        if (request.SecurityDocumentSeriesId <= 0)
        {
            return Result<IReadOnlyCollection<PurchaseOrderFieldAccessDto>>.Failure("Debe seleccionar una serie de documento.");
        }

        var fields = await fieldAccessRepository.GetEffectiveDocumentSeriesFieldsForUserAsync(
            request.UserId,
            request.CompanyCode.Trim(),
            PurchaseOrderSecurity.FormKeyEdit,
            PurchaseOrderSecurity.DocumentType,
            request.SecurityDocumentSeriesId,
            cancellationToken);

        var result = fields
            .Where(field => field.IsActive)
            .Select(field => new PurchaseOrderFieldAccessDto(
                field.FieldKey,
                field.ControlType,
                field.IsVisible,
                field.IsEditable,
                field.IsRequired,
                field.IsReadOnly))
            .ToArray();

        return Result<IReadOnlyCollection<PurchaseOrderFieldAccessDto>>.Success(result);
    }
}
