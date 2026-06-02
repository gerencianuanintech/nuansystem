using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Queries;

public sealed class GetFinancialCatalogLookupQueryHandler(IFinancialCatalogRepository catalogRepository)
    : IQueryHandler<GetFinancialCatalogLookupQuery, IReadOnlyCollection<FinancialCatalogLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<FinancialCatalogLookupDto>>> Handle(
        GetFinancialCatalogLookupQuery request,
        CancellationToken cancellationToken)
    {
        var catalogs = await catalogRepository.GetLookupAsync(request.CatalogKey, cancellationToken);
        return Result<IReadOnlyCollection<FinancialCatalogLookupDto>>.Success(catalogs);
    }
}
