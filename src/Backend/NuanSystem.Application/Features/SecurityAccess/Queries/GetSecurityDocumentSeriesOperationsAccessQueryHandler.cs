using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class GetSecurityDocumentSeriesOperationsAccessQueryHandler(ISecurityDocumentSeriesAccessRepository repository)
    : IQueryHandler<GetSecurityDocumentSeriesOperationsAccessQuery, IReadOnlyCollection<SecurityDocumentSeriesOperationAccessDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityDocumentSeriesOperationAccessDto>>> Handle(
        GetSecurityDocumentSeriesOperationsAccessQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RoleId <= 0 || request.SecurityDocumentSeriesId <= 0 || string.IsNullOrWhiteSpace(request.CompanyCode) || string.IsNullOrWhiteSpace(request.FormKey) || string.IsNullOrWhiteSpace(request.DocumentType))
        {
            return Result<IReadOnlyCollection<SecurityDocumentSeriesOperationAccessDto>>.Failure("Debe seleccionar rol, empresa, formulario, tipo de documento y serie.");
        }

        var operations = await repository.GetOperationsAsync(
            request.RoleId,
            request.CompanyCode.Trim(),
            request.FormKey.Trim(),
            request.DocumentType.Trim(),
            request.SecurityDocumentSeriesId,
            request.OnlyActive,
            request.Search?.Trim(),
            cancellationToken);

        return Result<IReadOnlyCollection<SecurityDocumentSeriesOperationAccessDto>>.Success(operations);
    }
}
