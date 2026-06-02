using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Queries;

public sealed class GetGeneralSupplierCatalogsQueryHandler(
    IGeneralSupplierCatalogRepository catalogRepository)
    : IQueryHandler<GetGeneralSupplierCatalogsQuery, IReadOnlyCollection<GeneralSupplierCatalogDto>>
{
    public async Task<Result<IReadOnlyCollection<GeneralSupplierCatalogDto>>> Handle(
        GetGeneralSupplierCatalogsQuery request,
        CancellationToken cancellationToken)
    {
        var catalogs = await catalogRepository.GetAllAsync(request.CatalogKey, cancellationToken);

        return Result<IReadOnlyCollection<GeneralSupplierCatalogDto>>.Success(catalogs);
    }
}

