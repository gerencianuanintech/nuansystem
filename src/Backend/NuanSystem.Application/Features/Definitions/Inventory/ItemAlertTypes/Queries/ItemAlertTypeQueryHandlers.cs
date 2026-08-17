using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Queries;

public sealed class GetItemAlertTypesQueryHandler(IItemAlertTypeRepository repository) : IQueryHandler<GetItemAlertTypesQuery, IReadOnlyCollection<ItemAlertTypeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemAlertTypeDto>>> Handle(GetItemAlertTypesQuery request, CancellationToken ct) => Result<IReadOnlyCollection<ItemAlertTypeDto>>.Success(await repository.GetAllAsync(ct));
}
public sealed class GetItemAlertTypeLookupQueryHandler(IItemAlertTypeRepository repository) : IQueryHandler<GetItemAlertTypeLookupQuery, IReadOnlyCollection<ItemAlertTypeLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemAlertTypeLookupDto>>> Handle(GetItemAlertTypeLookupQuery request, CancellationToken ct) => Result<IReadOnlyCollection<ItemAlertTypeLookupDto>>.Success(await repository.GetLookupAsync(ct));
}
public sealed class GetItemAlertTypeByIdQueryHandler(IItemAlertTypeRepository repository) : IQueryHandler<GetItemAlertTypeByIdQuery, ItemAlertTypeDto>
{
    public async Task<Result<ItemAlertTypeDto>> Handle(GetItemAlertTypeByIdQuery request, CancellationToken ct) => (await repository.GetByIdAsync(request.Id, ct)) is { } item ? Result<ItemAlertTypeDto>.Success(item) : Result<ItemAlertTypeDto>.Failure("Registro no encontrado.", [new ApiError("ItemAlertTypeNotFound", "Registro no encontrado.", nameof(request.Id))]);
}
public sealed class GetItemAlertTypeHistoryQueryHandler(IItemAlertTypeRepository repository) : IQueryHandler<GetItemAlertTypeHistoryQuery, IReadOnlyCollection<ItemAlertTypeAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemAlertTypeAuditChangeDto>>> Handle(GetItemAlertTypeHistoryQuery request, CancellationToken ct) => Result<IReadOnlyCollection<ItemAlertTypeAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, ct));
}

