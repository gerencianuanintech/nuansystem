using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed class SaveSecurityDocumentSeriesAccessCommandHandler(ISecurityDocumentSeriesAccessRepository repository)
    : ICommandHandler<SaveSecurityDocumentSeriesAccessCommand, bool>
{
    public async Task<Result<bool>> Handle(SaveSecurityDocumentSeriesAccessCommand request, CancellationToken cancellationToken)
    {
        if (request.RoleId <= 0 || request.SecurityDocumentSeriesId <= 0 || string.IsNullOrWhiteSpace(request.CompanyCode) || string.IsNullOrWhiteSpace(request.FormKey) || string.IsNullOrWhiteSpace(request.DocumentType))
        {
            return Result<bool>.Failure("Debe seleccionar rol, empresa, formulario, tipo de documento y serie.");
        }

        await repository.SaveAsync(
            request.RoleId,
            request.CompanyCode.Trim(),
            request.FormKey.Trim(),
            request.DocumentType.Trim(),
            request.SecurityDocumentSeriesId,
            request.IsSelected,
            request.Operations,
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            cancellationToken);

        return Result<bool>.Success(true, "Acceso por serie guardado correctamente.");
    }
}
