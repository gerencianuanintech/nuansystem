using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Commands;

public sealed class UpdateGeneralInventoryCatalogCommandHandler(
    IGeneralInventoryCatalogRepository catalogRepository)
    : ICommandHandler<UpdateGeneralInventoryCatalogCommand, GeneralInventoryCatalogDto>
{
    public async Task<Result<GeneralInventoryCatalogDto>> Handle(
        UpdateGeneralInventoryCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var catalogKey = CreateGeneralInventoryCatalogCommandHandler.NormalizeKey(request.CatalogKey);
        var existing = await catalogRepository.GetByIdAsync(catalogKey, request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<GeneralInventoryCatalogDto>.Failure(
                "No se encontro el maestro de inventario.",
                [new ApiError("GENERAL_INVENTORY_NOT_FOUND", "El registro no existe.", nameof(request.Id))]);
        }

        var code = CreateGeneralInventoryCatalogCommandHandler.NormalizeCode(request.Code);
        if (await catalogRepository.ExistsByCodeAsync(catalogKey, code, request.Id, cancellationToken))
        {
            return Result<GeneralInventoryCatalogDto>.Failure(
                "Ya existe un maestro de inventario con el codigo indicado.",
                [new ApiError("GENERAL_INVENTORY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var updated = await catalogRepository.UpdateAsync(
            catalogKey,
            new UpdateGeneralInventoryCatalogData(
                request.Id,
                code,
                request.Name.Trim(),
                CreateGeneralInventoryCatalogCommandHandler.NormalizeOptional(request.Description),
                request.IsActive,
                request.AuditUserId,
                CreateGeneralInventoryCatalogCommandHandler.NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (!updated)
        {
            return Result<GeneralInventoryCatalogDto>.Failure(
                "No se pudo actualizar el maestro de inventario.",
                [new ApiError("GENERAL_INVENTORY_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
        }

        var catalog = await catalogRepository.GetByIdAsync(catalogKey, request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El maestro de inventario fue actualizado pero no pudo consultarse.");

        return Result<GeneralInventoryCatalogDto>.Success(catalog, "Maestro de inventario actualizado correctamente.");
    }
}
