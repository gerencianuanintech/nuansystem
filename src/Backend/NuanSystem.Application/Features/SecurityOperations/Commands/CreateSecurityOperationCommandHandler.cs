using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityOperations.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityOperations.Commands;

public sealed class CreateSecurityOperationCommandHandler(ISecurityOperationRepository operationRepository)
    : ICommandHandler<CreateSecurityOperationCommand, SecurityOperationDto>
{
    public async Task<Result<SecurityOperationDto>> Handle(
        CreateSecurityOperationCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await operationRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<SecurityOperationDto>.Failure(
                "Ya existe una operacion con el codigo indicado.",
                new[] { new ApiError("SecurityOperationCodeAlreadyExists", "El codigo de operacion ya existe.", nameof(request.Code)) });
        }

        var name = request.Name.Trim();
        if (await operationRepository.ExistsByNameAsync(name, cancellationToken))
        {
            return Result<SecurityOperationDto>.Failure(
                "Ya existe una operacion con el nombre indicado.",
                new[] { new ApiError("SecurityOperationNameAlreadyExists", "El nombre de operacion ya existe.", nameof(request.Name)) });
        }

        var id = await operationRepository.CreateAsync(new CreateSecurityOperationData(
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
        var operation = await operationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("La operacion fue creada pero no pudo consultarse.");

        return Result<SecurityOperationDto>.Success(operation, "Operacion creada correctamente.");
    }
}
