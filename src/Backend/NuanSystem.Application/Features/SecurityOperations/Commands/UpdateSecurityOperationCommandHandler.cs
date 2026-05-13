using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityOperations.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityOperations.Commands;

public sealed class UpdateSecurityOperationCommandHandler(ISecurityOperationRepository operationRepository)
    : ICommandHandler<UpdateSecurityOperationCommand, SecurityOperationDto>
{
    public async Task<Result<SecurityOperationDto>> Handle(
        UpdateSecurityOperationCommand request,
        CancellationToken cancellationToken)
    {
        if (await operationRepository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<SecurityOperationDto>.Failure(
                "Operacion no encontrada.",
                new[] { new ApiError("SecurityOperationNotFound", "La operacion no existe.", nameof(request.Id)) });
        }

        var code = request.Code.Trim().ToUpperInvariant();
        if (await operationRepository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return Result<SecurityOperationDto>.Failure(
                "Ya existe una operacion con el codigo indicado.",
                new[] { new ApiError("SecurityOperationCodeAlreadyExists", "El codigo de operacion ya existe.", nameof(request.Code)) });
        }

        var name = request.Name.Trim();
        if (await operationRepository.ExistsByNameAsync(name, request.Id, cancellationToken))
        {
            return Result<SecurityOperationDto>.Failure(
                "Ya existe una operacion con el nombre indicado.",
                new[] { new ApiError("SecurityOperationNameAlreadyExists", "El nombre de operacion ya existe.", nameof(request.Name)) });
        }

        await operationRepository.UpdateAsync(new UpdateSecurityOperationData(
            request.Id,
            code,
            name,
            request.Description?.Trim(),
            request.RibbonPageName.Trim(),
            request.RibbonGroupName.Trim(),
            request.ActionKey.Trim(),
            request.IconLarge?.Trim(),
            request.IconSmall?.Trim(),
            request.DisplayOrder,
            request.IsActive,
            request.AuditUserId,
            request.AuditUserName?.Trim()), cancellationToken);
        var operation = await operationRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("La operacion fue actualizada pero no pudo consultarse.");

        return Result<SecurityOperationDto>.Success(operation, "Operacion actualizada correctamente.");
    }
}
