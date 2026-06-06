using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed class SaveSecurityDocumentSeriesFieldAccessCommandHandler(ISecurityRoleFormFieldAccessRepository repository)
    : ICommandHandler<SaveSecurityDocumentSeriesFieldAccessCommand, bool>
{
    public async Task<Result<bool>> Handle(SaveSecurityDocumentSeriesFieldAccessCommand request, CancellationToken cancellationToken)
    {
        if (request.RoleId <= 0 || request.FormId <= 0 || request.SecurityDocumentSeriesId <= 0 || string.IsNullOrWhiteSpace(request.CompanyCode) || string.IsNullOrWhiteSpace(request.DocumentType))
        {
            return Result<bool>.Failure("Debe seleccionar rol, empresa, formulario, tipo de documento y serie.");
        }

        await repository.SaveDocumentSeriesFieldsAsync(
            request.RoleId,
            request.CompanyCode.Trim(),
            request.FormId,
            request.DocumentType.Trim(),
            request.SecurityDocumentSeriesId,
            request.Fields,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return Result<bool>.Success(true, "Accesos a campos por serie guardados correctamente.");
    }
}
