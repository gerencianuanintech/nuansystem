using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Items.Dtos;

namespace NuanSystem.Application.Features.Items.Queries;

public sealed class GetItemLookupsQueryHandler(IItemRepository itemRepository)
    : IQueryHandler<GetItemLookupsQuery, ItemLookupsDto>
{
    public async Task<Result<ItemLookupsDto>> Handle(GetItemLookupsQuery request, CancellationToken cancellationToken)
    {
        var lookups = await itemRepository.GetLookupsAsync(cancellationToken);
        return Result<ItemLookupsDto>.Success(lookups);
    }
}
