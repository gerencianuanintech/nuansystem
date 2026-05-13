using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed class DeleteItemCommandHandler(IItemRepository itemRepository)
    : ICommandHandler<DeleteItemCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var deleted = await itemRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Articulo eliminado correctamente.")
            : Result<bool>.Failure("Articulo no encontrado.", [new ApiError("ItemNotFound", "No existe el articulo indicado.", nameof(request.Id))]);
    }
}
