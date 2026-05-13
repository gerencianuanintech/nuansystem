using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Queries;

public sealed class GetItemGroupByIdQueryHandler(IItemGroupRepository itemGroupRepository)
    : IQueryHandler<GetItemGroupByIdQuery, ItemGroupDto>
{
    public async Task<Result<ItemGroupDto>> Handle(
        GetItemGroupByIdQuery request,
        CancellationToken cancellationToken)
    {
        var itemGroup = await itemGroupRepository.GetByIdAsync(request.Id, cancellationToken);
        if (itemGroup is null)
        {
            return Result<ItemGroupDto>.Failure(
                "Grupo de artículos no encontrado.",
                [new ApiError("ItemGroupNotFound", "No existe el grupo de artículos indicado.", nameof(request.Id))]);
        }

        return Result<ItemGroupDto>.Success(itemGroup);
    }
}
