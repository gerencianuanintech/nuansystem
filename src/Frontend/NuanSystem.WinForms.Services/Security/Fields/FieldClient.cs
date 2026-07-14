using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Security.Fields.Models;

namespace NuanSystem.WinForms.Services.Security.Fields;

public sealed class FieldClient(INuanApiClient apiClient) : IFieldClient
{
    public async Task<IReadOnlyCollection<FieldItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<FieldItem>>("/api/security/fields", cancellationToken);
    }

    public Task<FieldItem> CreateAsync(SaveFieldRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveFieldRequest, FieldItem>("/api/security/fields", request, cancellationToken);
    }

    public Task<FieldItem> UpdateAsync(int id, SaveFieldRequest request, CancellationToken cancellationToken = default)
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

        return apiClient.PutAsync<object, FieldItem>($"/api/security/fields/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/fields/{id}", cancellationToken);
    }
}
