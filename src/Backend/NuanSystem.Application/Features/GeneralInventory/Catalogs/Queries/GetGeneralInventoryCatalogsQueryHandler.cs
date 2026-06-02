using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Queries;

public sealed class GetGeneralInventoryCatalogsQueryHandler(
    IGeneralInventoryCatalogRepository catalogRepository)
    : IQueryHandler<GetGeneralInventoryCatalogsQuery, IReadOnlyCollection<GeneralInventoryCatalogDto>>
{
    public async Task<Result<IReadOnlyCollection<GeneralInventoryCatalogDto>>> Handle(
        GetGeneralInventoryCatalogsQuery request,
        CancellationToken cancellationToken)
    {
        var catalogs = await catalogRepository.GetAllAsync(request.CatalogKey, cancellationToken);

        return Result<IReadOnlyCollection<GeneralInventoryCatalogDto>>.Success(catalogs);
    }
}
