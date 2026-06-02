using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Queries;

public sealed class GetGeneralInventoryCatalogLookupQueryHandler(
    IGeneralInventoryCatalogRepository catalogRepository)
    : IQueryHandler<GetGeneralInventoryCatalogLookupQuery, IReadOnlyCollection<GeneralInventoryCatalogLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<GeneralInventoryCatalogLookupDto>>> Handle(
        GetGeneralInventoryCatalogLookupQuery request,
        CancellationToken cancellationToken)
    {
        var catalogs = await catalogRepository.GetLookupAsync(request.CatalogKey, cancellationToken);

        return Result<IReadOnlyCollection<GeneralInventoryCatalogLookupDto>>.Success(catalogs);
    }
}
