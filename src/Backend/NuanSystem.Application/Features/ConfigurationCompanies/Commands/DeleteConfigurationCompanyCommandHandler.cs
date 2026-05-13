using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

public sealed class DeleteConfigurationCompanyCommandHandler(IConfigurationCompanyRepository companyRepository)
    : ICommandHandler<DeleteConfigurationCompanyCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteConfigurationCompanyCommand request, CancellationToken cancellationToken)
    {
        if (await companyRepository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<bool>.Failure(
                "Compania no encontrada.",
                [new ApiError("ConfigurationCompanyNotFound", "La compania no existe.", nameof(request.Id))]);
        }

        var deleted = await companyRepository.DeleteAsync(request.Id, request.AuditUserId, request.AuditUserName?.Trim(), cancellationToken);
        return deleted
            ? Result<bool>.Success(true, "Compania eliminada correctamente.")
            : Result<bool>.Failure(
                "No se pudo eliminar la compania.",
                [new ApiError("ConfigurationCompanyDeleteBlocked", "La compania tiene usuarios activos asignados o no puede eliminarse.", nameof(request.Id))]);
    }
}
