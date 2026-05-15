using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Queries;

public sealed class GetItemFamilyByIdQueryHandler(IItemFamilyRepository itemFamilyRepository)
    : IQueryHandler<GetItemFamilyByIdQuery, ItemFamilyDto>
{
    public async Task<Result<ItemFamilyDto>> Handle(
        GetItemFamilyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var itemFamily = await itemFamilyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (itemFamily is null)
        {
            return Result<ItemFamilyDto>.Failure(
                "Linea/familia no encontrada.",
                [new ApiError("ItemFamilyNotFound", "No existe la linea/familia indicada.", nameof(request.Id))]);
        }

        return Result<ItemFamilyDto>.Success(itemFamily);
    }
}
