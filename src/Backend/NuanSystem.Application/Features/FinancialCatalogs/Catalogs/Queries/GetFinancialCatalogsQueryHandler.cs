using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Queries;

public sealed class GetFinancialCatalogsQueryHandler(IFinancialCatalogRepository catalogRepository)
    : IQueryHandler<GetFinancialCatalogsQuery, IReadOnlyCollection<FinancialCatalogDto>>
{
    public async Task<Result<IReadOnlyCollection<FinancialCatalogDto>>> Handle(
        GetFinancialCatalogsQuery request,
        CancellationToken cancellationToken)
    {
        var catalogs = await catalogRepository.GetAllAsync(request.CatalogKey, cancellationToken);
        return Result<IReadOnlyCollection<FinancialCatalogDto>>.Success(catalogs);
    }
}
