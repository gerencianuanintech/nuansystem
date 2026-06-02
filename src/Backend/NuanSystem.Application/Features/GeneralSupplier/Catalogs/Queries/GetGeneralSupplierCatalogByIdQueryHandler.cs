using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Queries;

public sealed class GetGeneralSupplierCatalogByIdQueryHandler(
    IGeneralSupplierCatalogRepository catalogRepository)
    : IQueryHandler<GetGeneralSupplierCatalogByIdQuery, GeneralSupplierCatalogDto>
{
    public async Task<Result<GeneralSupplierCatalogDto>> Handle(
        GetGeneralSupplierCatalogByIdQuery request,
        CancellationToken cancellationToken)
    {
        var catalog = await catalogRepository.GetByIdAsync(request.CatalogKey, request.Id, cancellationToken);
        if (catalog is null)
        {
            return Result<GeneralSupplierCatalogDto>.Failure(
                "No se encontro el catalogo de proveedor.",
                [new ApiError("GENERAL_SUPPLIER_NOT_FOUND", "El registro no existe.", nameof(request.Id))]);
        }

        return Result<GeneralSupplierCatalogDto>.Success(catalog);
    }
}

