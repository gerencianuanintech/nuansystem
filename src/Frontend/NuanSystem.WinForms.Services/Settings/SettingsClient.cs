using Uri = System.Uri;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Settings.Models;

namespace NuanSystem.WinForms.Services.Settings;

public sealed class SettingsClient(INuanApiClient apiClient) : ISettingsClient
{
    public async Task<IReadOnlyCollection<CompanyParameterItem>> GetParametersAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<CompanyParameterItem>>(
            "/api/settings/parameters",
            cancellationToken);
    }

    public Task<CompanyParameterItem> SaveParameterAsync(
        SaveCompanyParameterRequest request,
        CancellationToken cancellationToken = default)
    {
        var key = Uri.EscapeDataString(request.Key);
        return apiClient.PutAsync<SaveCompanyParameterRequest, CompanyParameterItem>(
            $"/api/settings/parameters/{key}",
            request,
            cancellationToken);
    }
}
