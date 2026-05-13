using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Queries;

public sealed class GetItemGroupsQueryHandler(IItemGroupRepository itemGroupRepository)
    : IQueryHandler<GetItemGroupsQuery, IReadOnlyCollection<ItemGroupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemGroupDto>>> Handle(
        GetItemGroupsQuery request,
        CancellationToken cancellationToken)
    {
        var itemGroups = await itemGroupRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<ItemGroupDto>>.Success(itemGroups);
    }
}
