using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityOperations.Dtos;

namespace NuanSystem.Application.Features.SecurityOperations.Queries;

public sealed class GetSecurityOperationsQueryHandler(ISecurityOperationRepository operationRepository)
    : IQueryHandler<GetSecurityOperationsQuery, IReadOnlyCollection<SecurityOperationDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityOperationDto>>> Handle(
        GetSecurityOperationsQuery request,
        CancellationToken cancellationToken)
    {
        var operations = await operationRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<SecurityOperationDto>>.Success(operations);
    }
}
