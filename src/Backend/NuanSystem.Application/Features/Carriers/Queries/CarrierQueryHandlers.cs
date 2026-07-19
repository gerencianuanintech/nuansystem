using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Carriers.Commands;
using NuanSystem.Application.Features.Carriers.Dtos;

namespace NuanSystem.Application.Features.Carriers.Queries;

public sealed class GetCarriersQueryHandler(ICarrierRepository repository) : IQueryHandler<GetCarriersQuery, IReadOnlyCollection<CarrierListItemDto>>
{
    public async Task<Result<IReadOnlyCollection<CarrierListItemDto>>> Handle(GetCarriersQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<CarrierListItemDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetCarrierLookupQueryHandler(ICarrierRepository repository) : IQueryHandler<GetCarrierLookupQuery, IReadOnlyCollection<CarrierLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<CarrierLookupDto>>> Handle(GetCarrierLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<CarrierLookupDto>>.Success(await repository.GetLookupAsync(cancellationToken));
}

public sealed class GetCarrierByIdQueryHandler(ICarrierRepository repository) : IQueryHandler<GetCarrierByIdQuery, CarrierDetailDto>
{
    public async Task<Result<CarrierDetailDto>> Handle(GetCarrierByIdQuery request, CancellationToken cancellationToken)
    {
        var carrier = await repository.GetByIdAsync(request.Id, cancellationToken);
        return carrier is null ? UpdateCarrierCommandHandler.NotFound(request.Id) : Result<CarrierDetailDto>.Success(carrier);
    }
}

public sealed class GetCarrierHistoryQueryHandler(ICarrierRepository repository) : IQueryHandler<GetCarrierHistoryQuery, IReadOnlyCollection<CarrierAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<CarrierAuditChangeDto>>> Handle(GetCarrierHistoryQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<CarrierAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, cancellationToken));
}
