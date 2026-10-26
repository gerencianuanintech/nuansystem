using NuanSystem.Application.Features.SapSync.Mapping;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapBusinessPartnerIdentityPolicyRepository
{
    Task<SapBusinessPartnerIdentityPolicy?> GetAsync(int companyId, CancellationToken cancellationToken = default);
    Task<long> SaveAsync(int companyId, SapBusinessPartnerIdentityPolicy policy, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default);
}
