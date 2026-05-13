using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Companies.Commands;

public sealed class AssignUserCompanyCommandHandler(ICompanyAdminRepository companyRepository)
    : ICommandHandler<AssignUserCompanyCommand, bool>
{
    public async Task<Result<bool>> Handle(
        AssignUserCompanyCommand request,
        CancellationToken cancellationToken)
    {
        if (!await companyRepository.UserExistsAsync(request.UserId, cancellationToken))
        {
            return Result<bool>.Failure(
                "El usuario indicado no existe.",
                new[] { new ApiError("UserNotFound", "No existe el usuario indicado.", nameof(request.UserId)) });
        }

        await companyRepository.AssignUserAsync(request.UserId, request.CompanyId, cancellationToken);

        return Result<bool>.Success(true, "Empresa asignada al usuario correctamente.");
    }
}
