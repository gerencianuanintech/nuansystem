using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Queries;

public sealed class GetFinancialCatalogByIdQueryHandler(IFinancialCatalogRepository catalogRepository)
    : IQueryHandler<GetFinancialCatalogByIdQuery, FinancialCatalogDto>
{
    public async Task<Result<FinancialCatalogDto>> Handle(
        GetFinancialCatalogByIdQuery request,
        CancellationToken cancellationToken)
    {
        var catalog = await catalogRepository.GetByIdAsync(request.CatalogKey, request.Id, cancellationToken);
        return catalog is null
            ? Result<FinancialCatalogDto>.Failure(
                "No se encontro el catalogo financiero.",
                [new ApiError("FINANCIAL_CATALOG_NOT_FOUND", "El registro no existe.", nameof(request.Id))])
            : Result<FinancialCatalogDto>.Success(catalog);
    }
}
