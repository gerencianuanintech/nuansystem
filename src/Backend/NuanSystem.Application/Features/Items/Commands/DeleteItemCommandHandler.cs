using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed class DeleteItemCommandHandler(
    IItemRepository itemRepository,
    ITransactionRunner transactionRunner,
    IItemLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<DeleteItemCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var current = await itemRepository.GetByIdAsync(request.Id, connection, transaction, token);
                if (current is null)
                {
                    return Result<bool>.Failure(
                        "Articulo no encontrado.",
                        [new ApiError("ItemNotFound", "No existe el articulo indicado.", nameof(request.Id))]);
                }

                var deleted = await itemRepository.DeleteAsync(
                    request.Id,
                    request.AuditUserId,
                    request.AuditUserName?.Trim(),
                    connection,
                    transaction,
                    token);

                if (!deleted)
                {
                    return Result<bool>.Failure("No se pudo eliminar el articulo.");
                }

                await localOutboxWriter.EnqueueAsync(
                    current, SyncOperation.Deleted, connection, transaction, token);
                return Result<bool>.Success(true, "Articulo eliminado correctamente.");
            },
            cancellationToken);
    }
}
