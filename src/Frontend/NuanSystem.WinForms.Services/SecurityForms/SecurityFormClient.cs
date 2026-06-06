using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SecurityForms.Models;

namespace NuanSystem.WinForms.Services.SecurityForms;

public sealed class SecurityFormClient(INuanApiClient apiClient) : ISecurityFormClient
{
    public async Task<IReadOnlyCollection<SecurityFormItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<SecurityFormItem>>("/api/security/forms", cancellationToken);
    }

    public Task<SecurityFormItem> CreateAsync(SaveSecurityFormRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveSecurityFormRequest, SecurityFormItem>("/api/security/forms", request, cancellationToken);
    }

    public Task<SecurityFormItem> UpdateAsync(int id, SaveSecurityFormRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Id = id,
            request.Code,
            request.Name,
            request.Description,
            request.FormKey,
            request.FormType,
            request.HasListView,
            request.HasEditView,
            request.IsVisible,
            request.IsActive
        };

        return apiClient.PutAsync<object, SecurityFormItem>($"/api/security/forms/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/forms/{id}", cancellationToken);
    }
}
