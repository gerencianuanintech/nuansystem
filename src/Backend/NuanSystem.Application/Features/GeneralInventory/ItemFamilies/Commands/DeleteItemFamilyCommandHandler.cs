using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;

public sealed class DeleteItemFamilyCommandHandler(IItemFamilyRepository itemFamilyRepository)
    : ICommandHandler<DeleteItemFamilyCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemFamilyCommand request, CancellationToken cancellationToken)
    {
        var deleted = await itemFamilyRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            CreateItemFamilyCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Linea/familia eliminada correctamente.")
            : Result<bool>.Failure(
                "Linea/familia no encontrada.",
                [new ApiError("ItemFamilyNotFound", "No existe la linea/familia indicada.", nameof(request.Id))]);
    }
}
