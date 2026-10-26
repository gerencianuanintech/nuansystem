using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SapSync.IdentityPolicy.Models;

namespace NuanSystem.WinForms.Services.SapSync.IdentityPolicy;

public sealed class SapIdentityPolicyClient(INuanApiClient apiClient) : ISapIdentityPolicyClient
{
    public Task<SapIdentityPolicyItem?> GetAsync(CancellationToken cancellationToken = default)
        => apiClient.GetAsync<SapIdentityPolicyItem?>("/api/sap/identity-policy", cancellationToken);

    public Task SaveAsync(SaveSapIdentityPolicyRequest request, CancellationToken cancellationToken = default)
        => apiClient.PutAsync<SaveSapIdentityPolicyRequest, object>("/api/sap/identity-policy", request, cancellationToken);
}
