using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;
using NuanSystem.Shared.Responses;

using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Queries;

public sealed class GetItemSubgroupsQueryHandler(IItemSubgroupRepository repository) : IQueryHandler<GetItemSubgroupsQuery, IReadOnlyCollection<ItemSubgroupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemSubgroupDto>>> Handle(GetItemSubgroupsQuery request, CancellationToken ct) => Result<IReadOnlyCollection<ItemSubgroupDto>>.Success(await repository.GetAllAsync(ct));
}
public sealed class GetItemSubgroupLookupQueryHandler(IItemSubgroupRepository repository) : IQueryHandler<GetItemSubgroupLookupQuery, IReadOnlyCollection<ItemSubgroupLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemSubgroupLookupDto>>> Handle(GetItemSubgroupLookupQuery request, CancellationToken ct) => Result<IReadOnlyCollection<ItemSubgroupLookupDto>>.Success(await repository.GetLookupAsync(request.ItemFamilyId, ct));
}
public sealed class GetItemSubgroupByIdQueryHandler(IItemSubgroupRepository repository) : IQueryHandler<GetItemSubgroupByIdQuery, ItemSubgroupDto>
{
    public async Task<Result<ItemSubgroupDto>> Handle(GetItemSubgroupByIdQuery request, CancellationToken ct) => (await repository.GetByIdAsync(request.Id, ct)) is { } item ? Result<ItemSubgroupDto>.Success(item) : Result<ItemSubgroupDto>.Failure("Subgrupo de artículos no encontrado.", [new ApiError("ItemSubgroupNotFound", "No existe el subgrupo de artículos indicado.", nameof(request.Id))]);
}
public sealed class GetItemSubgroupHistoryQueryHandler(IItemSubgroupRepository repository) : IQueryHandler<GetItemSubgroupHistoryQuery, IReadOnlyCollection<ItemSubgroupAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemSubgroupAuditChangeDto>>> Handle(GetItemSubgroupHistoryQuery request, CancellationToken ct) => Result<IReadOnlyCollection<ItemSubgroupAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, ct));
}
