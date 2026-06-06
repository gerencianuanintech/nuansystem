using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class GetSecurityDocumentSeriesFieldAccessQueryHandler(ISecurityRoleFormFieldAccessRepository repository)
    : IQueryHandler<GetSecurityDocumentSeriesFieldAccessQuery, IReadOnlyCollection<SecurityFormFieldAccessDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityFormFieldAccessDto>>> Handle(GetSecurityDocumentSeriesFieldAccessQuery request, CancellationToken cancellationToken)
    {
        if (request.RoleId <= 0 || request.FormId <= 0 || request.SecurityDocumentSeriesId <= 0 || string.IsNullOrWhiteSpace(request.CompanyCode) || string.IsNullOrWhiteSpace(request.DocumentType))
        {
            return Result<IReadOnlyCollection<SecurityFormFieldAccessDto>>.Failure("Debe seleccionar rol, empresa, formulario, tipo de documento y serie.");
        }

        var fields = await repository.GetDocumentSeriesFieldsAsync(
            request.RoleId,
            request.CompanyCode.Trim(),
            request.FormId,
            request.DocumentType.Trim(),
            request.SecurityDocumentSeriesId,
            request.OnlyActive,
            request.Search?.Trim(),
            cancellationToken);

        return Result<IReadOnlyCollection<SecurityFormFieldAccessDto>>.Success(fields);
    }
}
