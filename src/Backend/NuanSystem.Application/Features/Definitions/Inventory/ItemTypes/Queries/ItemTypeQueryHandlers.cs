using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Queries;

public sealed class GetItemTypesQueryHandler(IItemTypeRepository repository)
    : IQueryHandler<GetItemTypesQuery, IReadOnlyCollection<ItemTypeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemTypeDto>>> Handle(GetItemTypesQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemTypeDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetItemTypeLookupQueryHandler(IItemTypeRepository repository)
    : IQueryHandler<GetItemTypeLookupQuery, IReadOnlyCollection<ItemTypeLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemTypeLookupDto>>> Handle(GetItemTypeLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemTypeLookupDto>>.Success(await repository.GetLookupAsync(cancellationToken));
}

public sealed class GetItemTypeByIdQueryHandler(IItemTypeRepository repository)
    : IQueryHandler<GetItemTypeByIdQuery, ItemTypeDto>
{
    public async Task<Result<ItemTypeDto>> Handle(GetItemTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var itemType = await repository.GetByIdAsync(request.Id, cancellationToken);
        return itemType is null
            ? UpdateItemTypeCommandHandler.NotFound(request.Id)
            : Result<ItemTypeDto>.Success(itemType);
    }
}

public sealed class GetItemTypeHistoryQueryHandler(IItemTypeRepository repository)
    : IQueryHandler<GetItemTypeHistoryQuery, IReadOnlyCollection<ItemTypeAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemTypeAuditChangeDto>>> Handle(
        GetItemTypeHistoryQuery request,
        CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemTypeAuditChangeDto>>.Success(
            await repository.GetHistoryAsync(request.Id, cancellationToken));
}
