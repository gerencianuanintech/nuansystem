using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;

public sealed class DeleteItemFamilyCommandHandler(
    IItemFamilyRepository itemFamilyRepository,
    ITransactionRunner transactionRunner,
    IItemFamilyLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<DeleteItemFamilyCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemFamilyCommand request, CancellationToken cancellationToken)
    {
        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var current = await itemFamilyRepository.GetByIdAsync(request.Id, connection, transaction, token);
                if (current is null)
                {
                    return Result<bool>.Failure(
                        "Linea/familia no encontrada.",
                        [new ApiError("ItemFamilyNotFound", "No existe la linea/familia indicada.", nameof(request.Id))]);
                }

                var deleted = await itemFamilyRepository.DeleteAsync(
                    request.Id,
                    request.AuditUserId,
                    CreateItemFamilyCommandHandler.NormalizeOptional(request.AuditUserName),
                    connection,
                    transaction,
                    token);

                if (!deleted)
                {
                    return Result<bool>.Failure("No se pudo eliminar la linea/familia.");
                }

                await localOutboxWriter.EnqueueAsync(
                    current, SyncOperation.Deleted, connection, transaction, token);
                return Result<bool>.Success(true, "Linea/familia eliminada correctamente.");
            },
            cancellationToken);
    }
}
