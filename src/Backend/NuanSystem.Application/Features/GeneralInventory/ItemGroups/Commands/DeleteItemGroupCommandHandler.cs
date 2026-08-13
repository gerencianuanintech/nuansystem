using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class DeleteItemGroupCommandHandler(
    IItemGroupRepository itemGroupRepository,
    ITransactionRunner transactionRunner,
    IItemGroupLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<DeleteItemGroupCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemGroupCommand request, CancellationToken cancellationToken)
    {
        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var existing = await itemGroupRepository.GetByIdAsync(
                    request.Id, connection, transaction, token);
                if (existing is null)
                {
                    return Result<bool>.Failure(
                        "Grupo de articulos no encontrado.",
                        [new ApiError("ItemGroupNotFound", "No existe el grupo de articulos indicado.", nameof(request.Id))]);
                }

                var deleteResult = await itemGroupRepository.DeleteWithResultAsync(
                    request.Id,
                    request.AuditUserId,
                    CreateItemGroupCommandHandler.NormalizeOptional(request.AuditUserName),
                    connection,
                    transaction,
                    token);

                if (deleteResult == -2)
                    return Result<bool>.Failure("No se puede eliminar un grupo del sistema.", [new ApiError("ITEM_GROUP_SYSTEM_PROTECTED", "El grupo pertenece al sistema.", nameof(request.Id))]);
                if (deleteResult == -3)
                    return Result<bool>.Failure("No se puede eliminar el grupo porque esta en uso.", [new ApiError("ITEM_GROUP_IN_USE", "Existen familias o articulos asociados.", nameof(request.Id))]);
                if (deleteResult <= 0)
                {
                    return Result<bool>.Failure("No se pudo eliminar el grupo de articulos.");
                }

                await localOutboxWriter.EnqueueAsync(
                    existing, SyncOperation.Deleted, connection, transaction, token);
                return Result<bool>.Success(true, "Grupo de articulos eliminado correctamente.");
            },
            cancellationToken);
    }
}
