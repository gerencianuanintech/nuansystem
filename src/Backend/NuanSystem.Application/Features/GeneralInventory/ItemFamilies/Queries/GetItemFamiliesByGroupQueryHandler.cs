using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Queries;

public sealed class GetItemFamiliesByGroupQueryHandler(IItemFamilyRepository itemFamilyRepository)
    : IQueryHandler<GetItemFamiliesByGroupQuery, IReadOnlyCollection<ItemFamilyDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemFamilyDto>>> Handle(
        GetItemFamiliesByGroupQuery request,
        CancellationToken cancellationToken)
    {
        var itemFamilies = await itemFamilyRepository.GetByGroupAsync(request.ItemGroupId, cancellationToken);
        return Result<IReadOnlyCollection<ItemFamilyDto>>.Success(itemFamilies);
    }
}
