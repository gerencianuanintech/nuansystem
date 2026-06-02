using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Commands;

public sealed class DeleteGeneralSupplierCatalogCommandHandler(
    IGeneralSupplierCatalogRepository catalogRepository)
    : ICommandHandler<DeleteGeneralSupplierCatalogCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteGeneralSupplierCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var catalogKey = CreateGeneralSupplierCatalogCommandHandler.NormalizeKey(request.CatalogKey);
        var deleted = await catalogRepository.DeleteAsync(
            catalogKey,
            request.Id,
            request.AuditUserId,
            CreateGeneralSupplierCatalogCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        if (!deleted)
        {
            return Result<bool>.Failure(
                "No se encontro el catalogo de proveedor.",
                [new ApiError("GENERAL_SUPPLIER_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
        }

        return Result<bool>.Success(true, "Catalogo de proveedor eliminado correctamente.");
    }
}

