using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Queries;

public sealed class GetUnitMeasuresQueryHandler(IUnitMeasureRepository repository)
    : IQueryHandler<GetUnitMeasuresQuery, IReadOnlyCollection<UnitMeasureDto>>
{
    public async Task<Result<IReadOnlyCollection<UnitMeasureDto>>> Handle(GetUnitMeasuresQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<UnitMeasureDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetUnitMeasureLookupQueryHandler(IUnitMeasureRepository repository)
    : IQueryHandler<GetUnitMeasureLookupQuery, IReadOnlyCollection<UnitMeasureLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<UnitMeasureLookupDto>>> Handle(GetUnitMeasureLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<UnitMeasureLookupDto>>.Success(await repository.GetLookupAsync(cancellationToken));
}

public sealed class GetUnitMeasureByIdQueryHandler(IUnitMeasureRepository repository)
    : IQueryHandler<GetUnitMeasureByIdQuery, UnitMeasureDto>
{
    public async Task<Result<UnitMeasureDto>> Handle(GetUnitMeasureByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        return item is null
            ? Result<UnitMeasureDto>.Failure("Unidad de medida no encontrada.",
                [new ApiError("UnitMeasureNotFound", "No existe la unidad de medida indicada.", nameof(request.Id))])
            : Result<UnitMeasureDto>.Success(item);
    }
}

public sealed class GetUnitMeasureHistoryQueryHandler(IUnitMeasureRepository repository)
    : IQueryHandler<GetUnitMeasureHistoryQuery, IReadOnlyCollection<UnitMeasureAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<UnitMeasureAuditChangeDto>>> Handle(GetUnitMeasureHistoryQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<UnitMeasureAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id, cancellationToken));
}
