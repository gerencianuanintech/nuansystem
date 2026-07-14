using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Security.Menus.Models;

namespace NuanSystem.WinForms.Services.Security.Menus;

public sealed class MenuClient(INuanApiClient apiClient) : IMenuClient
{
    public async Task<IReadOnlyCollection<MenuItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<MenuItem>>("/api/security/menus", cancellationToken);
    }

    public Task<MenuItem> CreateAsync(SaveMenuRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveMenuRequest, MenuItem>("/api/security/menus", request, cancellationToken);
    }

    public Task<MenuItem> UpdateAsync(int id, SaveMenuRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Id = id,
            request.ParentId,
            request.Code,
            request.Name,
            request.Description,
            request.MenuType,
            request.FormKey,
            request.IconLarge,
            request.IconSmall,
            request.DisplayOrder,
            request.IsVisible,
            request.IsActive
        };

        return apiClient.PutAsync<object, MenuItem>($"/api/security/menus/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/menus/{id}", cancellationToken);
    }
}
