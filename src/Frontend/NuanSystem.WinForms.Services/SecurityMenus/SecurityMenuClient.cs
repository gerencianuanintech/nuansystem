using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SecurityMenus.Models;

namespace NuanSystem.WinForms.Services.SecurityMenus;

public sealed class SecurityMenuClient(INuanApiClient apiClient) : ISecurityMenuClient
{
    public async Task<IReadOnlyCollection<SecurityMenuItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<SecurityMenuItem>>("/api/security/menus", cancellationToken);
    }

    public Task<SecurityMenuItem> CreateAsync(SaveSecurityMenuRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveSecurityMenuRequest, SecurityMenuItem>("/api/security/menus", request, cancellationToken);
    }

    public Task<SecurityMenuItem> UpdateAsync(int id, SaveSecurityMenuRequest request, CancellationToken cancellationToken = default)
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

        return apiClient.PutAsync<object, SecurityMenuItem>($"/api/security/menus/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/menus/{id}", cancellationToken);
    }
}
