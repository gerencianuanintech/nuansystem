using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Queries;

public sealed class GetProductTypesQueryHandler(IProductTypeRepository repository)
    : IQueryHandler<GetProductTypesQuery, IReadOnlyCollection<ProductTypeDto>>
{
    public async Task<Result<IReadOnlyCollection<ProductTypeDto>>> Handle(GetProductTypesQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ProductTypeDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetProductTypeLookupQueryHandler(IProductTypeRepository repository)
    : IQueryHandler<GetProductTypeLookupQuery, IReadOnlyCollection<ProductTypeLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ProductTypeLookupDto>>> Handle(GetProductTypeLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ProductTypeLookupDto>>.Success(await repository.GetLookupAsync(cancellationToken));
}

public sealed class GetProductTypeByIdQueryHandler(IProductTypeRepository repository)
    : IQueryHandler<GetProductTypeByIdQuery, ProductTypeDto>
{
    public async Task<Result<ProductTypeDto>> Handle(GetProductTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        return item is null
            ? Result<ProductTypeDto>.Failure("Tipo de producto no encontrado.",
                [new ApiError("ProductTypeNotFound", "No existe el tipo de producto indicado.", nameof(request.Id))])
            : Result<ProductTypeDto>.Success(item);
    }
}

public sealed class GetProductTypeHistoryQueryHandler(IProductTypeRepository repository)
    : IQueryHandler<GetProductTypeHistoryQuery, IReadOnlyCollection<ProductTypeAuditChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<ProductTypeAuditChangeDto>>> Handle(
        GetProductTypeHistoryQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ProductTypeAuditChangeDto>>.Success(
            await repository.GetHistoryAsync(request.Id, cancellationToken));
}
