using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Queries;

public sealed class GetItemByIdQueryHandler(IItemRepository itemRepository)
    : IQueryHandler<GetItemByIdQuery, ItemDto>
{
    public async Task<Result<ItemDto>> Handle(
        GetItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
        {
            return Result<ItemDto>.Failure(
                "Articulo no encontrado.",
                new[] { new ApiError("ItemNotFound", "No existe el articulo indicado.", nameof(request.Id)) });
        }

        return Result<ItemDto>.Success(item);
    }
}
