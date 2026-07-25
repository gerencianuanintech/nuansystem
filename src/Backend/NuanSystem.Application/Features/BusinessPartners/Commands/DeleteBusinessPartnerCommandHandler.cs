using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class DeleteBusinessPartnerCommandHandler(
    IBusinessPartnerRepository repository,
    ITransactionRunner transactionRunner,
    IBusinessPartnerLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<DeleteBusinessPartnerCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
                if (current is null)
                {
                    return Result<bool>.Failure(
                        "Tercero comercial no encontrado.",
                        [new ApiError("BusinessPartnerNotFound", "Tercero comercial no encontrado.", nameof(request.Id))]);
                }

                var deleted = await repository.DeleteAsync(
                    request.Id, request.AuditUserId, request.AuditUserName, connection, transaction, token);
                if (!deleted)
                {
                    return Result<bool>.Failure("No se pudo eliminar el tercero comercial.");
                }

                await localOutboxWriter.EnqueueAsync(
                    current, SyncOperation.Deleted, connection, transaction, token);
                return Result<bool>.Success(true, "Tercero comercial eliminado correctamente.");
            },
            cancellationToken);
    }
}
