using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Queries;

public sealed class GetItemBrandsQueryHandler(IItemBrandRepository repository)
    : IQueryHandler<GetItemBrandsQuery, IReadOnlyCollection<ItemBrandDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemBrandDto>>> Handle(GetItemBrandsQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemBrandDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetItemBrandLookupQueryHandler(IItemBrandRepository repository)
    : IQueryHandler<GetItemBrandLookupQuery, IReadOnlyCollection<ItemBrandLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemBrandLookupDto>>> Handle(GetItemBrandLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemBrandLookupDto>>.Success(await repository.GetLookupAsync(cancellationToken));
}

public sealed class GetItemBrandByIdQueryHandler(IItemBrandRepository repository)
    : IQueryHandler<GetItemBrandByIdQuery, ItemBrandDto>
{
    public async Task<Result<ItemBrandDto>> Handle(GetItemBrandByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        return item is null
            ? Result<ItemBrandDto>.Failure("Marca de articulos no encontrada.",
                [new ApiError("ItemBrandNotFound", "No existe la marca de articulos indicada.", nameof(request.Id))])
            : Result<ItemBrandDto>.Success(item);
    }
}

public sealed class GetItemBrandHistoryQueryHandler(IItemBrandRepository repository)
    : IQueryHandler<GetItemBrandHistoryQuery, IReadOnlyCollection<ItemBrandAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemBrandAuditChangeDto>>> Handle(GetItemBrandHistoryQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemBrandAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, cancellationToken));
}
