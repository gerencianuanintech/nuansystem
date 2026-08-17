using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Queries;

public sealed class GetReplenishmentMethodsQueryHandler(IReplenishmentMethodRepository repository) : IQueryHandler<GetReplenishmentMethodsQuery, IReadOnlyCollection<ReplenishmentMethodDto>>
{
    public async Task<Result<IReadOnlyCollection<ReplenishmentMethodDto>>> Handle(GetReplenishmentMethodsQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ReplenishmentMethodDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetReplenishmentMethodLookupQueryHandler(IReplenishmentMethodRepository repository) : IQueryHandler<GetReplenishmentMethodLookupQuery, IReadOnlyCollection<ReplenishmentMethodLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ReplenishmentMethodLookupDto>>> Handle(GetReplenishmentMethodLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ReplenishmentMethodLookupDto>>.Success(await repository.GetLookupAsync(request.IncludeCode, cancellationToken));
}

public sealed class GetReplenishmentMethodByIdQueryHandler(IReplenishmentMethodRepository repository) : IQueryHandler<GetReplenishmentMethodByIdQuery, ReplenishmentMethodDto>
{
    public async Task<Result<ReplenishmentMethodDto>> Handle(GetReplenishmentMethodByIdQuery request, CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(request.Id, cancellationToken) is { } item
            ? Result<ReplenishmentMethodDto>.Success(item)
            : Result<ReplenishmentMethodDto>.Failure("Método de reposición no encontrado.", [new ApiError("ReplenishmentMethodNotFound", "No existe el método de reposición indicado.", nameof(request.Id))]);
}

public sealed class GetReplenishmentMethodHistoryQueryHandler(IReplenishmentMethodRepository repository) : IQueryHandler<GetReplenishmentMethodHistoryQuery, IReadOnlyCollection<ReplenishmentMethodAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ReplenishmentMethodAuditChangeDto>>> Handle(GetReplenishmentMethodHistoryQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ReplenishmentMethodAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, cancellationToken));
}
