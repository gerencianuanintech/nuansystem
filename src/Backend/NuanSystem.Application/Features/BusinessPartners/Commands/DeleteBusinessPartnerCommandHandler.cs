using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class DeleteBusinessPartnerCommandHandler(
    IBusinessPartnerUnitOfWork unitOfWork)
    : ICommandHandler<DeleteBusinessPartnerCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        return await unitOfWork.ExecuteAsync(
            async (transaction, token) =>
            {
                var current = await transaction.GetByIdAsync(request.Id, token);
                if (current is null)
                {
                    return Result<bool>.Failure(
                        "Tercero comercial no encontrado.",
                        [new ApiError("BusinessPartnerNotFound", "Tercero comercial no encontrado.", nameof(request.Id))]);
                }

                var deleted = await transaction.DeleteAsync(
                    request.Id, request.AuditUserId, request.AuditUserName, token);
                if (!deleted)
                {
                    return Result<bool>.Failure("No se pudo eliminar el tercero comercial.");
                }

                await transaction.EnqueueAsync(
                    current, SyncOperation.Deleted, token);
                return Result<bool>.Success(true, "Tercero comercial eliminado correctamente.");
            },
            cancellationToken);
    }
}
