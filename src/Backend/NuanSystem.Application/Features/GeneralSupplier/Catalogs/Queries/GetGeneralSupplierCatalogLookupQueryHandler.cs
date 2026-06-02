using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Queries;

public sealed class GetGeneralSupplierCatalogLookupQueryHandler(
    IGeneralSupplierCatalogRepository catalogRepository)
    : IQueryHandler<GetGeneralSupplierCatalogLookupQuery, IReadOnlyCollection<GeneralSupplierCatalogLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<GeneralSupplierCatalogLookupDto>>> Handle(
        GetGeneralSupplierCatalogLookupQuery request,
        CancellationToken cancellationToken)
    {
        var catalogs = await catalogRepository.GetLookupAsync(request.CatalogKey, cancellationToken);

        return Result<IReadOnlyCollection<GeneralSupplierCatalogLookupDto>>.Success(catalogs);
    }
}

