using NuanSystem.WinForms.Services.SapSync.IdentityPolicy.Models;

namespace NuanSystem.WinForms.Services.SapSync.IdentityPolicy;

public interface ISapIdentityPolicyClient
{
    Task<SapIdentityPolicyItem?> GetAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(SaveSapIdentityPolicyRequest request, CancellationToken cancellationToken = default);
}
