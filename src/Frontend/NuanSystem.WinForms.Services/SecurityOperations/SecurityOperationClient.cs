using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SecurityOperations.Models;

namespace NuanSystem.WinForms.Services.SecurityOperations;

public sealed class SecurityOperationClient(INuanApiClient apiClient) : ISecurityOperationClient
{
    public async Task<IReadOnlyCollection<SecurityOperationItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<SecurityOperationItem>>("/api/security/operations", cancellationToken);
    }

    public Task<SecurityOperationItem> CreateAsync(SaveSecurityOperationRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveSecurityOperationRequest, SecurityOperationItem>("/api/security/operations", request, cancellationToken);
    }

    public Task<SecurityOperationItem> UpdateAsync(int id, SaveSecurityOperationRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Id = id,
            request.Code,
            request.Name,
            request.Description,
            request.RibbonPageName,
            request.RibbonGroupName,
            request.ActionKey,
            request.IconLarge,
            request.IconSmall,
            request.DisplayOrder,
            request.IsActive
        };

        return apiClient.PutAsync<object, SecurityOperationItem>($"/api/security/operations/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/operations/{id}", cancellationToken);
    }
}
