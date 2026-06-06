using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityForms.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityForms.Commands;

public sealed class CreateSecurityFormCommandHandler(ISecurityFormRepository formRepository)
    : ICommandHandler<CreateSecurityFormCommand, SecurityFormDto>
{
    public async Task<Result<SecurityFormDto>> Handle(CreateSecurityFormCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await formRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<SecurityFormDto>.Failure(
                "Ya existe un formulario con el codigo indicado.",
                [new ApiError("SecurityFormCodeAlreadyExists", "El codigo de formulario ya existe.", nameof(request.Code))]);
        }

        var formKey = request.FormKey.Trim();
        if (await formRepository.ExistsByFormKeyAsync(formKey, cancellationToken))
        {
            return Result<SecurityFormDto>.Failure(
                "Ya existe un formulario con la clave indicada.",
                [new ApiError("SecurityFormKeyAlreadyExists", "La clave de formulario ya existe.", nameof(request.FormKey))]);
        }

        var id = await formRepository.CreateAsync(new CreateSecurityFormData(
            code,
            request.Name.Trim(),
            request.Description?.Trim(),
            formKey,
            request.FormType,
            request.HasListView,
            request.HasEditView,
            request.IsVisible,
            request.IsActive,
            request.AuditUserId,
            request.AuditUserName?.Trim()), cancellationToken);

        var form = await formRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El formulario fue creado pero no pudo consultarse.");

        return Result<SecurityFormDto>.Success(form, "Formulario creado correctamente.");
    }
}
