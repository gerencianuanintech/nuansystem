using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class GetSecurityFormAccessFormsQueryHandler(ISecurityRoleFormAccessRepository repository)
    : IQueryHandler<GetSecurityFormAccessFormsQuery, IReadOnlyCollection<SecurityFormAccessFormDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityFormAccessFormDto>>> Handle(
        GetSecurityFormAccessFormsQuery request,
        CancellationToken cancellationToken)
    {
        var forms = await repository.GetFormsAsync(
            request.FormType,
            request.OnlyActive,
            request.Search?.Trim(),
            cancellationToken);

        return Result<IReadOnlyCollection<SecurityFormAccessFormDto>>.Success(forms);
    }
}
