using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SecurityForms.Dtos;

namespace NuanSystem.Application.Features.SecurityForms.Queries;

public sealed class GetSecurityFormsQueryHandler(ISecurityFormRepository formRepository)
    : IQueryHandler<GetSecurityFormsQuery, IReadOnlyCollection<SecurityFormDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityFormDto>>> Handle(GetSecurityFormsQuery request, CancellationToken cancellationToken)
    {
        var forms = await formRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<SecurityFormDto>>.Success(forms);
    }
}
