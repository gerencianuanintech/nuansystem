using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Queries;

public sealed class GetItemLinesQueryHandler(IItemLineRepository repository)
    : IQueryHandler<GetItemLinesQuery, IReadOnlyCollection<ItemLineDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemLineDto>>> Handle(GetItemLinesQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemLineDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetItemLineLookupQueryHandler(IItemLineRepository repository)
    : IQueryHandler<GetItemLineLookupQuery, IReadOnlyCollection<ItemLineLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemLineLookupDto>>> Handle(GetItemLineLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemLineLookupDto>>.Success(await repository.GetLookupAsync(cancellationToken));
}

public sealed class GetItemLineByIdQueryHandler(IItemLineRepository repository)
    : IQueryHandler<GetItemLineByIdQuery, ItemLineDto>
{
    public async Task<Result<ItemLineDto>> Handle(GetItemLineByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        return item is null
            ? Result<ItemLineDto>.Failure("Linea de articulos no encontrada.",
                [new ApiError("ItemLineNotFound", "No existe la linea de articulos indicada.", nameof(request.Id))])
            : Result<ItemLineDto>.Success(item);
    }
}

public sealed class GetItemLineHistoryQueryHandler(IItemLineRepository repository)
    : IQueryHandler<GetItemLineHistoryQuery, IReadOnlyCollection<ItemLineAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemLineAuditChangeDto>>> Handle(
        GetItemLineHistoryQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ItemLineAuditChangeDto>>.Success(
            await repository.GetHistoryAsync(request.Id, cancellationToken));
}
