using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class GetSecurityFormAccessOperationsQueryHandler(ISecurityRoleFormAccessRepository repository)
    : IQueryHandler<GetSecurityFormAccessOperationsQuery, IReadOnlyCollection<SecurityFormAccessOperationDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityFormAccessOperationDto>>> Handle(
        GetSecurityFormAccessOperationsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RoleId <= 0 || request.FormId <= 0)
        {
            return Result<IReadOnlyCollection<SecurityFormAccessOperationDto>>.Failure("Debe seleccionar un rol y un formulario.");
        }

        var operations = await repository.GetOperationsAsync(
            request.RoleId,
            request.FormId,
            request.OnlyActive,
            request.Search?.Trim(),
            cancellationToken);

        return Result<IReadOnlyCollection<SecurityFormAccessOperationDto>>.Success(operations);
    }
}
