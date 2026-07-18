using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class DeleteItemGroupCommandHandler(
    IItemGroupRepository itemGroupRepository,
    ISyncEventPublisher syncEventPublisher,
    ICompanyContext companyContext)
    : ICommandHandler<DeleteItemGroupCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemGroupCommand request, CancellationToken cancellationToken)
    {
        var existing = await itemGroupRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<bool>.Failure(
                "Grupo de artículos no encontrado.",
                [new ApiError("ItemGroupNotFound", "No existe el grupo de artículos indicado.", nameof(request.Id))]);
        }

        var deleted = await itemGroupRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            CreateItemGroupCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        if (!deleted)
        {
            return Result<bool>.Failure(
                "Grupo de artículos no encontrado.",
                [new ApiError("ItemGroupNotFound", "No existe el grupo de artículos indicado.", nameof(request.Id))]);
        }

        var syncResult = await ItemGroupSyncPublisher.PublishAsync(
            syncEventPublisher,
            companyContext,
            existing,
            SyncOperation.Deleted,
            cancellationToken);

        return syncResult is { IsSuccess: false }
            ? Result<bool>.Failure(syncResult.Message, syncResult.Errors)
            : Result<bool>.Success(true, "Grupo de artículos eliminado correctamente.");
    }
}
