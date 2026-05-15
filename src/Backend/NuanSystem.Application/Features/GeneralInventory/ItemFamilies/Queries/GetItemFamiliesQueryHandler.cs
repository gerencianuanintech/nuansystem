using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Queries;

public sealed class GetItemFamiliesQueryHandler(IItemFamilyRepository itemFamilyRepository)
    : IQueryHandler<GetItemFamiliesQuery, IReadOnlyCollection<ItemFamilyDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemFamilyDto>>> Handle(
        GetItemFamiliesQuery request,
        CancellationToken cancellationToken)
    {
        var itemFamilies = await itemFamilyRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<ItemFamilyDto>>.Success(itemFamilies);
    }
}
