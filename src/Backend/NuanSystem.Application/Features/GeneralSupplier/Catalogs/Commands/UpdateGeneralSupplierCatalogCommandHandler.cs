using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Commands;

public sealed class UpdateGeneralSupplierCatalogCommandHandler(
    IGeneralSupplierCatalogRepository catalogRepository)
    : ICommandHandler<UpdateGeneralSupplierCatalogCommand, GeneralSupplierCatalogDto>
{
    public async Task<Result<GeneralSupplierCatalogDto>> Handle(
        UpdateGeneralSupplierCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var catalogKey = CreateGeneralSupplierCatalogCommandHandler.NormalizeKey(request.CatalogKey);
        var existing = await catalogRepository.GetByIdAsync(catalogKey, request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<GeneralSupplierCatalogDto>.Failure(
                "No se encontro el catalogo de proveedor.",
                [new ApiError("GENERAL_SUPPLIER_NOT_FOUND", "El registro no existe.", nameof(request.Id))]);
        }

        var code = CreateGeneralSupplierCatalogCommandHandler.NormalizeCode(request.Code);
        if (await catalogRepository.ExistsByCodeAsync(catalogKey, code, request.Id, cancellationToken))
        {
            return Result<GeneralSupplierCatalogDto>.Failure(
                "Ya existe un catalogo de proveedor con el codigo indicado.",
                [new ApiError("GENERAL_SUPPLIER_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var updated = await catalogRepository.UpdateAsync(
            catalogKey,
            new UpdateGeneralSupplierCatalogData(
                request.Id,
                code,
                request.Name.Trim(),
                CreateGeneralSupplierCatalogCommandHandler.NormalizeOptional(request.Description),
                request.IsActive,
                request.AuditUserId,
                CreateGeneralSupplierCatalogCommandHandler.NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (!updated)
        {
            return Result<GeneralSupplierCatalogDto>.Failure(
                "No se pudo actualizar el catalogo de proveedor.",
                [new ApiError("GENERAL_SUPPLIER_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
        }

        var catalog = await catalogRepository.GetByIdAsync(catalogKey, request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El catalogo de proveedor fue actualizado pero no pudo consultarse.");

        return Result<GeneralSupplierCatalogDto>.Success(catalog, "Catalogo de proveedor actualizado correctamente.");
    }
}

