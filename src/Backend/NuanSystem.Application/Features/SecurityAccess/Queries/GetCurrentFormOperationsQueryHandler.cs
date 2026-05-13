using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class GetCurrentFormOperationsQueryHandler(ISecurityAccessRepository securityAccessRepository)
    : IQueryHandler<GetCurrentFormOperationsQuery, IReadOnlyCollection<FormOperationAccessDto>>
{
    public async Task<Result<IReadOnlyCollection<FormOperationAccessDto>>> Handle(GetCurrentFormOperationsQuery request, CancellationToken cancellationToken)
    {
        var operations = await securityAccessRepository.GetFormOperationsAsync(request.UserId, request.FormKey, cancellationToken);
        return Result<IReadOnlyCollection<FormOperationAccessDto>>.Success(operations);
    }
}
