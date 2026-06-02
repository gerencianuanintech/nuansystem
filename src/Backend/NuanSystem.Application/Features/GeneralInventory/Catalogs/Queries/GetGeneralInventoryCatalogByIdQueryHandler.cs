using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Queries;

public sealed class GetGeneralInventoryCatalogByIdQueryHandler(
    IGeneralInventoryCatalogRepository catalogRepository)
    : IQueryHandler<GetGeneralInventoryCatalogByIdQuery, GeneralInventoryCatalogDto>
{
    public async Task<Result<GeneralInventoryCatalogDto>> Handle(
        GetGeneralInventoryCatalogByIdQuery request,
        CancellationToken cancellationToken)
    {
        var catalog = await catalogRepository.GetByIdAsync(request.CatalogKey, request.Id, cancellationToken);
        if (catalog is null)
        {
            return Result<GeneralInventoryCatalogDto>.Failure(
                "No se encontro el maestro de inventario.",
                [new ApiError("GENERAL_INVENTORY_NOT_FOUND", "El registro no existe.", nameof(request.Id))]);
        }

        return Result<GeneralInventoryCatalogDto>.Success(catalog);
    }
}
