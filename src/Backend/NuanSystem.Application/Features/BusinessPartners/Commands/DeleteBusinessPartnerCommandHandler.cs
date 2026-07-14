using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class DeleteBusinessPartnerCommandHandler(
    IBusinessPartnerRepository repository,
    ISyncEventPublisher syncEventPublisher,
    ICompanyContext companyContext)
    : ICommandHandler<DeleteBusinessPartnerCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<bool>.Failure(
                "Tercero comercial no encontrado.",
                [new ApiError("BusinessPartnerNotFound", "Tercero comercial no encontrado.", nameof(request.Id))]);
        }

        var deleted = await repository.DeleteAsync(request.Id, request.AuditUserId, request.AuditUserName, cancellationToken);
        if (!deleted)
        {
            return Result<bool>.Failure("No se pudo eliminar el tercero comercial.");
        }

        var syncResult = await BusinessPartnerSyncPublisher.PublishAsync(
            syncEventPublisher,
            companyContext,
            current,
            SyncOperation.Deleted,
            cancellationToken);

        if (syncResult is { IsSuccess: false })
        {
            return Result<bool>.Failure(syncResult.Message, syncResult.Errors);
        }

        return Result<bool>.Success(true, "Tercero comercial eliminado correctamente.");
    }
}
