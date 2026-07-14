using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Security.Operations.Models;

namespace NuanSystem.WinForms.Services.Security.Operations;

public sealed class OperationClient(INuanApiClient apiClient) : IOperationClient
{
    public async Task<IReadOnlyCollection<OperationItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<OperationItem>>("/api/security/operations", cancellationToken);
    }

    public Task<OperationItem> CreateAsync(SaveOperationRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveOperationRequest, OperationItem>("/api/security/operations", request, cancellationToken);
    }

    public Task<OperationItem> UpdateAsync(int id, SaveOperationRequest request, CancellationToken cancellationToken = default)
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

        return apiClient.PutAsync<object, OperationItem>($"/api/security/operations/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/operations/{id}", cancellationToken);
    }
}
