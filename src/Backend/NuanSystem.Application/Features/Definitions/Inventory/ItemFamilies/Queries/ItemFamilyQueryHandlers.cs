using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Queries;

public sealed class GetItemFamiliesQueryHandler(IItemFamilyRepository repository)
    : IQueryHandler<GetItemFamiliesQuery, IReadOnlyCollection<ItemFamilyDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemFamilyDto>>> Handle(GetItemFamiliesQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemFamilyDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetItemFamilyLookupQueryHandler(IItemFamilyRepository repository)
    : IQueryHandler<GetItemFamilyLookupQuery, IReadOnlyCollection<ItemFamilyLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemFamilyLookupDto>>> Handle(GetItemFamilyLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemFamilyLookupDto>>.Success(await repository.GetLookupAsync(request.ItemGroupId, cancellationToken));
}

public sealed class GetItemFamilyByIdQueryHandler(IItemFamilyRepository repository)
    : IQueryHandler<GetItemFamilyByIdQuery, ItemFamilyDto>
{
    public async Task<Result<ItemFamilyDto>> Handle(GetItemFamilyByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        return item is null
            ? Result<ItemFamilyDto>.Failure("Familia de articulos no encontrada.",
                [new ApiError("ItemFamilyNotFound", "No existe la familia de articulos indicada.", nameof(request.Id))])
            : Result<ItemFamilyDto>.Success(item);
    }
}

public sealed class GetItemFamilyHistoryQueryHandler(IItemFamilyRepository repository)
    : IQueryHandler<GetItemFamilyHistoryQuery, IReadOnlyCollection<ItemFamilyAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemFamilyAuditChangeDto>>> Handle(GetItemFamilyHistoryQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemFamilyAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, cancellationToken));
}
