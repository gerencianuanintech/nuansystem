using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Queries;

public sealed record GetItemGroupLookupQuery : IQuery<IReadOnlyCollection<ItemGroupLookupDto>>;
public sealed class GetItemGroupLookupQueryHandler(IItemGroupRepository repository) : IQueryHandler<GetItemGroupLookupQuery, IReadOnlyCollection<ItemGroupLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemGroupLookupDto>>> Handle(GetItemGroupLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemGroupLookupDto>>.Success(await repository.GetLookupAsync(cancellationToken));
}
