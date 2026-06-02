using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class DeleteBusinessPartnerCommandHandler(IBusinessPartnerRepository repository)
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
        return deleted
            ? Result<bool>.Success(true, "Tercero comercial eliminado correctamente.")
            : Result<bool>.Failure("No se pudo eliminar el tercero comercial.");
    }
}
