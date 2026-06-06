using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class ValidateCurrentSecurityFormAccessQueryHandler(ISecurityRoleFormAccessRepository repository)
    : IQueryHandler<ValidateCurrentSecurityFormAccessQuery, bool>
{
    public async Task<Result<bool>> Handle(
        ValidateCurrentSecurityFormAccessQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.FormKey) || string.IsNullOrWhiteSpace(request.ActionKey))
        {
            return Result<bool>.Failure("No fue posible validar el acceso solicitado.");
        }

        var isAllowed = await repository.ValidateUserOperationAsync(
            request.UserId,
            request.FormKey.Trim(),
            request.ActionKey.Trim(),
            cancellationToken);

        return Result<bool>.Success(isAllowed);
    }
}
