using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityFields.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SecurityFields.Commands;

public sealed class CreateSecurityFieldCommandHandler(ISecurityFieldRepository fieldRepository, ISecurityFormRepository formRepository)
    : ICommandHandler<CreateSecurityFieldCommand, SecurityFieldDto>
{
    public async Task<Result<SecurityFieldDto>> Handle(CreateSecurityFieldCommand request, CancellationToken cancellationToken)
    {
        if (await formRepository.GetByIdAsync(request.FormId, cancellationToken) is null)
        {
            return Result<SecurityFieldDto>.Failure(
                "Formulario no encontrado.",
                [new ApiError("SecurityFormNotFound", "El formulario no existe.", nameof(request.FormId))]);
        }

        var code = request.Code.Trim().ToUpperInvariant();
        if (await fieldRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<SecurityFieldDto>.Failure(
                "Ya existe un campo con el codigo indicado.",
                [new ApiError("SecurityFieldCodeAlreadyExists", "El codigo de campo ya existe.", nameof(request.Code))]);
        }

        var fieldKey = request.FieldKey.Trim();
        if (await fieldRepository.ExistsByFieldKeyAsync(request.FormId, fieldKey, cancellationToken))
        {
            return Result<SecurityFieldDto>.Failure(
                "Ya existe un campo con la clave indicada para este formulario.",
                [new ApiError("SecurityFieldKeyAlreadyExists", "La clave de campo ya existe para el formulario.", nameof(request.FieldKey))]);
        }

        var id = await fieldRepository.CreateAsync(new CreateSecurityFieldData(
            request.FormId,
            code,
            request.Name.Trim(),
            fieldKey,
            request.Description?.Trim(),
            request.ControlType.Trim(),
            request.DataType.Trim().ToLowerInvariant(),
            request.IsRequired,
            request.ValidationMessage?.Trim(),
            request.IsReadOnly,
            request.IsVisible,
            request.IsCustom,
            request.DisplayOrder,
            request.IsActive,
            request.AuditUserId,
            request.AuditUserName?.Trim()), cancellationToken);

        var field = await fieldRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El campo fue creado pero no pudo consultarse.");

        return Result<SecurityFieldDto>.Success(field, "Campo creado correctamente.");
    }
}
