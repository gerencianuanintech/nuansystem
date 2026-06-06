using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class GetSecurityFormFieldAccessQueryHandler(ISecurityRoleFormFieldAccessRepository repository)
    : IQueryHandler<GetSecurityFormFieldAccessQuery, IReadOnlyCollection<SecurityFormFieldAccessDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityFormFieldAccessDto>>> Handle(GetSecurityFormFieldAccessQuery request, CancellationToken cancellationToken)
    {
        if (request.RoleId <= 0 || request.FormId <= 0)
        {
            return Result<IReadOnlyCollection<SecurityFormFieldAccessDto>>.Failure("Debe seleccionar rol y formulario.");
        }

        var fields = await repository.GetFieldsAsync(
            request.RoleId,
            request.FormId,
            request.OnlyActive,
            request.Search?.Trim(),
            cancellationToken);

        return Result<IReadOnlyCollection<SecurityFormFieldAccessDto>>.Success(fields);
    }
}
