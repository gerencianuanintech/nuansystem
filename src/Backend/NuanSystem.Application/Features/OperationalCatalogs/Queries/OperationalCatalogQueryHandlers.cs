using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.OperationalCatalogs.Dtos;
using static NuanSystem.Application.Features.OperationalCatalogs.OperationalCatalogNormalizer;

namespace NuanSystem.Application.Features.OperationalCatalogs.Queries;

public sealed class GetOperationalCatalogsQueryHandler(IOperationalCatalogRepository repository)
    : IQueryHandler<GetOperationalCatalogsQuery, IReadOnlyCollection<OperationalCatalogDto>>
{
    public async Task<Result<IReadOnlyCollection<OperationalCatalogDto>>> Handle(GetOperationalCatalogsQuery request, CancellationToken cancellationToken)
    {
        var items = await repository.GetAllAsync(
            new OperationalCatalogFilterData(
                NormalizeKey(request.CatalogKey),
                NormalizeOptional(request.Search),
                NormalizeKeyOptional(request.ParentCatalogKey),
                NormalizeCodeOptional(request.ParentCode),
                request.IsActive),
            cancellationToken);

        return Result<IReadOnlyCollection<OperationalCatalogDto>>.Success(items);
    }
}

public sealed class GetOperationalCatalogByIdQueryHandler(IOperationalCatalogRepository repository)
    : IQueryHandler<GetOperationalCatalogByIdQuery, OperationalCatalogDto>
{
    public async Task<Result<OperationalCatalogDto>> Handle(GetOperationalCatalogByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(NormalizeKey(request.CatalogKey), request.Id, cancellationToken);
        return item is null
            ? Result<OperationalCatalogDto>.Failure("El valor del catalogo operativo no existe.")
            : Result<OperationalCatalogDto>.Success(item);
    }
}

public sealed class GetOperationalCatalogLookupQueryHandler(IOperationalCatalogRepository repository)
    : IQueryHandler<GetOperationalCatalogLookupQuery, IReadOnlyCollection<OperationalCatalogLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<OperationalCatalogLookupDto>>> Handle(GetOperationalCatalogLookupQuery request, CancellationToken cancellationToken)
    {
        var items = await repository.GetLookupAsync(
            NormalizeKey(request.CatalogKey),
            NormalizeKeyOptional(request.ParentCatalogKey),
            NormalizeCodeOptional(request.ParentCode),
            request.ActiveOnly,
            cancellationToken);

        return Result<IReadOnlyCollection<OperationalCatalogLookupDto>>.Success(items);
    }
}
