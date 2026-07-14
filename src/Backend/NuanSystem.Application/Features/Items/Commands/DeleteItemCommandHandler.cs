using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed class DeleteItemCommandHandler(
    IItemRepository itemRepository,
    ISyncEventPublisher syncEventPublisher,
    ICompanyContext companyContext)
    : ICommandHandler<DeleteItemCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var current = await itemRepository.GetByIdAsync(request.Id, cancellationToken);
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
            cancellationToken);

        if (!deleted)
        {
            return Result<bool>.Failure("No se pudo eliminar el articulo.");
        }

        var syncResult = await ItemSyncPublisher.PublishAsync(
            syncEventPublisher,
            companyContext,
            current,
            SyncOperation.Deleted,
            cancellationToken);

        if (syncResult is { IsSuccess: false })
        {
            return Result<bool>.Failure(syncResult.Message, syncResult.Errors);
        }

        return Result<bool>.Success(true, "Articulo eliminado correctamente.");
    }
}
