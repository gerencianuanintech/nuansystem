using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.TaxCatalogs.Taxes.Queries;

public sealed class GetTaxesQueryHandler(ITaxRepository repository)
    : IQueryHandler<GetTaxesQuery, IReadOnlyCollection<TaxDto>>
{
    public async Task<Result<IReadOnlyCollection<TaxDto>>> Handle(GetTaxesQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<TaxDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetTaxLookupQueryHandler(ITaxRepository repository)
    : IQueryHandler<GetTaxLookupQuery, IReadOnlyCollection<TaxLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<TaxLookupDto>>> Handle(GetTaxLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<TaxLookupDto>>.Success(await repository.GetLookupAsync(cancellationToken));
}

public sealed class GetTaxByIdQueryHandler(ITaxRepository repository)
    : IQueryHandler<GetTaxByIdQuery, TaxDto>
{
    public async Task<Result<TaxDto>> Handle(GetTaxByIdQuery request, CancellationToken cancellationToken)
    {
        var tax = await repository.GetByIdAsync(request.Id, cancellationToken);
        return tax is null
            ? Result<TaxDto>.Failure("Impuesto no encontrado.",
                [new ApiError("TAX_NOT_FOUND", "El registro no existe.", nameof(request.Id))])
            : Result<TaxDto>.Success(tax);
    }
}
