using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Commands;

public sealed class DeleteGeneralInventoryCatalogCommandHandler(
    IGeneralInventoryCatalogRepository catalogRepository)
    : ICommandHandler<DeleteGeneralInventoryCatalogCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteGeneralInventoryCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var catalogKey = CreateGeneralInventoryCatalogCommandHandler.NormalizeKey(request.CatalogKey);
        var deleted = await catalogRepository.DeleteAsync(
            catalogKey,
            request.Id,
            request.AuditUserId,
            CreateGeneralInventoryCatalogCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        if (!deleted)
        {
            return Result<bool>.Failure(
                "No se encontro el maestro de inventario.",
                [new ApiError("GENERAL_INVENTORY_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
        }

        return Result<bool>.Success(true, "Maestro de inventario eliminado correctamente.");
    }
}
