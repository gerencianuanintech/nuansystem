using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityForms.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityForms.Commands;

public sealed class UpdateSecurityFormCommandHandler(ISecurityFormRepository formRepository)
    : ICommandHandler<UpdateSecurityFormCommand, SecurityFormDto>
{
    public async Task<Result<SecurityFormDto>> Handle(UpdateSecurityFormCommand request, CancellationToken cancellationToken)
    {
        if (await formRepository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<SecurityFormDto>.Failure(
                "Formulario no encontrado.",
                [new ApiError("SecurityFormNotFound", "El formulario no existe.", nameof(request.Id))]);
        }

        var code = request.Code.Trim().ToUpperInvariant();
        if (await formRepository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return Result<SecurityFormDto>.Failure(
                "Ya existe un formulario con el codigo indicado.",
                [new ApiError("SecurityFormCodeAlreadyExists", "El codigo de formulario ya existe.", nameof(request.Code))]);
        }

        var formKey = request.FormKey.Trim();
        if (await formRepository.ExistsByFormKeyAsync(formKey, request.Id, cancellationToken))
        {
            return Result<SecurityFormDto>.Failure(
                "Ya existe un formulario con la clave indicada.",
                [new ApiError("SecurityFormKeyAlreadyExists", "La clave de formulario ya existe.", nameof(request.FormKey))]);
        }

        await formRepository.UpdateAsync(new UpdateSecurityFormData(
            request.Id,
            code,
            request.Name.Trim(),
            request.Description?.Trim(),
            formKey,
            request.FormType,
            request.IsVisible,
            request.IsActive,
            request.AuditUserId,
            request.AuditUserName?.Trim()), cancellationToken);

        var form = await formRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El formulario fue actualizado pero no pudo consultarse.");

        return Result<SecurityFormDto>.Success(form, "Formulario actualizado correctamente.");
    }
}
