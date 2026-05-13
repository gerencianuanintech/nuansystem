using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SecurityFields.Models;

namespace NuanSystem.WinForms.Services.SecurityFields;

public sealed class SecurityFieldClient(INuanApiClient apiClient) : ISecurityFieldClient
{
    public async Task<IReadOnlyCollection<SecurityFieldItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<SecurityFieldItem>>("/api/security/fields", cancellationToken);
    }

    public Task<SecurityFieldItem> CreateAsync(SaveSecurityFieldRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveSecurityFieldRequest, SecurityFieldItem>("/api/security/fields", request, cancellationToken);
    }

    public Task<SecurityFieldItem> UpdateAsync(int id, SaveSecurityFieldRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Id = id,
            request.FormId,
            request.Code,
            request.Name,
            request.FieldKey,
            request.Description,
            request.ControlType,
            request.DataType,
            request.IsRequired,
            request.ValidationMessage,
            request.IsReadOnly,
            request.IsVisible,
            request.IsCustom,
            request.DisplayOrder,
            request.IsActive
        };

        return apiClient.PutAsync<object, SecurityFieldItem>($"/api/security/fields/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/fields/{id}", cancellationToken);
    }
}
