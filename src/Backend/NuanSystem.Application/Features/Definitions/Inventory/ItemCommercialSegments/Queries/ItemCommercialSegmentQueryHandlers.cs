using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Queries;

public sealed class GetItemCommercialSegmentsQueryHandler(IItemCommercialSegmentRepository repository) : IQueryHandler<GetItemCommercialSegmentsQuery, IReadOnlyCollection<ItemCommercialSegmentDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemCommercialSegmentDto>>> Handle(GetItemCommercialSegmentsQuery request, CancellationToken ct) => Result<IReadOnlyCollection<ItemCommercialSegmentDto>>.Success(await repository.GetAllAsync(ct));
}
public sealed class GetItemCommercialSegmentLookupQueryHandler(IItemCommercialSegmentRepository repository) : IQueryHandler<GetItemCommercialSegmentLookupQuery, IReadOnlyCollection<ItemCommercialSegmentLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemCommercialSegmentLookupDto>>> Handle(GetItemCommercialSegmentLookupQuery request, CancellationToken ct) => Result<IReadOnlyCollection<ItemCommercialSegmentLookupDto>>.Success(await repository.GetLookupAsync(ct));
}
public sealed class GetItemCommercialSegmentByIdQueryHandler(IItemCommercialSegmentRepository repository) : IQueryHandler<GetItemCommercialSegmentByIdQuery, ItemCommercialSegmentDto>
{
    public async Task<Result<ItemCommercialSegmentDto>> Handle(GetItemCommercialSegmentByIdQuery request, CancellationToken ct) => (await repository.GetByIdAsync(request.Id, ct)) is { } item ? Result<ItemCommercialSegmentDto>.Success(item) : Result<ItemCommercialSegmentDto>.Failure("Registro no encontrado.", [new ApiError("ItemCommercialSegmentNotFound", "Registro no encontrado.", nameof(request.Id))]);
}
public sealed class GetItemCommercialSegmentHistoryQueryHandler(IItemCommercialSegmentRepository repository) : IQueryHandler<GetItemCommercialSegmentHistoryQuery, IReadOnlyCollection<ItemCommercialSegmentAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ItemCommercialSegmentAuditChangeDto>>> Handle(GetItemCommercialSegmentHistoryQuery request, CancellationToken ct) => Result<IReadOnlyCollection<ItemCommercialSegmentAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, ct));
}
