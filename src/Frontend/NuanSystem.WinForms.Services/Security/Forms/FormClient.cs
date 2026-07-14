using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Security.Forms.Models;

namespace NuanSystem.WinForms.Services.Security.Forms;

public sealed class FormClient(INuanApiClient apiClient) : IFormClient
{
    public async Task<IReadOnlyCollection<FormItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<FormItem>>("/api/security/forms", cancellationToken);
    }

    public Task<FormItem> CreateAsync(SaveFormRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveFormRequest, FormItem>("/api/security/forms", request, cancellationToken);
    }

    public Task<FormItem> UpdateAsync(int id, SaveFormRequest request, CancellationToken cancellationToken = default)
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

        return apiClient.PutAsync<object, FormItem>($"/api/security/forms/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/forms/{id}", cancellationToken);
    }
}
