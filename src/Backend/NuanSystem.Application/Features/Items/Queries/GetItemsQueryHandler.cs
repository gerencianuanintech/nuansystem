using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Items.Dtos;

namespace NuanSystem.Application.Features.Items.Queries;

public sealed class GetItemsQueryHandler(IItemRepository itemRepository)
    : IQueryHandler<GetItemsQuery, IReadOnlyCollection<ItemDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemDto>>> Handle(
        GetItemsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await itemRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<ItemDto>>.Success(items);
    }
}
