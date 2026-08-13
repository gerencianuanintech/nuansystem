using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Queries;

public sealed record GetItemGroupHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ItemGroupAuditChangeDto>>;
public sealed class GetItemGroupHistoryQueryHandler(IItemGroupRepository repository) : IQueryHandler<GetItemGroupHistoryQuery, IReadOnlyCollection<ItemGroupAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemGroupAuditChangeDto>>> Handle(GetItemGroupHistoryQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemGroupAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, cancellationToken));
}
