using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class DeleteItemGroupCommandHandler(IItemGroupRepository itemGroupRepository)
    : ICommandHandler<DeleteItemGroupCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemGroupCommand request, CancellationToken cancellationToken)
    {
        var deleted = await itemGroupRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            CreateItemGroupCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Grupo de artículos eliminado correctamente.")
            : Result<bool>.Failure(
                "Grupo de artículos no encontrado.",
                [new ApiError("ItemGroupNotFound", "No existe el grupo de artículos indicado.", nameof(request.Id))]);
    }
}
