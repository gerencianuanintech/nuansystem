using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Queries;

public sealed class GetSalesChannelsQueryHandler(ISalesChannelRepository repository) : IQueryHandler<GetSalesChannelsQuery, IReadOnlyCollection<SalesChannelDto>>
{
    public async Task<Result<IReadOnlyCollection<SalesChannelDto>>> Handle(GetSalesChannelsQuery request, CancellationToken ct) => Result<IReadOnlyCollection<SalesChannelDto>>.Success(await repository.GetAllAsync(ct));
}
public sealed class GetSalesChannelLookupQueryHandler(ISalesChannelRepository repository) : IQueryHandler<GetSalesChannelLookupQuery, IReadOnlyCollection<SalesChannelLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<SalesChannelLookupDto>>> Handle(GetSalesChannelLookupQuery request, CancellationToken ct) => Result<IReadOnlyCollection<SalesChannelLookupDto>>.Success(await repository.GetLookupAsync(ct));
}
public sealed class GetSalesChannelByIdQueryHandler(ISalesChannelRepository repository) : IQueryHandler<GetSalesChannelByIdQuery, SalesChannelDto>
{
    public async Task<Result<SalesChannelDto>> Handle(GetSalesChannelByIdQuery request, CancellationToken ct) => (await repository.GetByIdAsync(request.Id, ct)) is { } item ? Result<SalesChannelDto>.Success(item) : Result<SalesChannelDto>.Failure("Registro no encontrado.", [new ApiError("SalesChannelNotFound", "Registro no encontrado.", nameof(request.Id))]);
}
public sealed class GetSalesChannelHistoryQueryHandler(ISalesChannelRepository repository) : IQueryHandler<GetSalesChannelHistoryQuery, IReadOnlyCollection<SalesChannelAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<SalesChannelAuditChangeDto>>> Handle(GetSalesChannelHistoryQuery request, CancellationToken ct) => Result<IReadOnlyCollection<SalesChannelAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, ct));
}


