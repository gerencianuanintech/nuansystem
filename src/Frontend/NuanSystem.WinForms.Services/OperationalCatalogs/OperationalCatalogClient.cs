using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.OperationalCatalogs.Models;

namespace NuanSystem.WinForms.Services.OperationalCatalogs;

public sealed class OperationalCatalogClient : IOperationalCatalogClient
{
    private const string BasePath = "/api/operational-catalogs";
    private readonly INuanApiClient apiClient;

    public OperationalCatalogClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<OperationalCatalogItem>> GetAsync(
        string catalogKey,
        string? search = null,
        string? parentCatalogKey = null,
        string? parentCode = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<OperationalCatalogItem>>(
            BuildPath($"{BasePath}/{Uri.EscapeDataString(catalogKey)}", search, parentCatalogKey, parentCode, "isActive", isActive),
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<OperationalCatalogLookupItem>> GetLookupAsync(
        string catalogKey,
        string? parentCatalogKey = null,
        string? parentCode = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<OperationalCatalogLookupItem>>(
            BuildPath($"{BasePath}/{Uri.EscapeDataString(catalogKey)}/lookup", null, parentCatalogKey, parentCode, "activeOnly", activeOnly),
            cancellationToken);
    }

    public Task<OperationalCatalogItem> GetByIdAsync(
        string catalogKey,
        int id,
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<OperationalCatalogItem>(
            $"{BasePath}/{Uri.EscapeDataString(catalogKey)}/{id}",
            cancellationToken);
    }

    public Task<OperationalCatalogItem> CreateAsync(
        string catalogKey,
        SaveOperationalCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveOperationalCatalogRequest, OperationalCatalogItem>(
            $"{BasePath}/{Uri.EscapeDataString(catalogKey)}",
            request,
            cancellationToken);
    }

    public Task<OperationalCatalogItem> UpdateAsync(
        string catalogKey,
        int id,
        SaveOperationalCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveOperationalCatalogRequest, OperationalCatalogItem>(
            $"{BasePath}/{Uri.EscapeDataString(catalogKey)}/{id}",
            request,
            cancellationToken);
    }

    public async Task DeleteAsync(string catalogKey, int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>(
            $"{BasePath}/{Uri.EscapeDataString(catalogKey)}/{id}",
            cancellationToken);
    }

    private static string BuildPath(
        string path,
        string? search,
        string? parentCatalogKey,
        string? parentCode,
        string activeParameterName,
        bool? activeValue)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        if (!string.IsNullOrWhiteSpace(parentCatalogKey))
        {
            query.Add($"parentCatalogKey={Uri.EscapeDataString(parentCatalogKey.Trim())}");
        }

        if (!string.IsNullOrWhiteSpace(parentCode))
        {
            query.Add($"parentCode={Uri.EscapeDataString(parentCode.Trim())}");
        }

        if (activeValue.HasValue)
        {
            query.Add($"{activeParameterName}={(activeValue.Value ? "true" : "false")}");
        }

        return query.Count == 0 ? path : $"{path}?{string.Join("&", query)}";
    }
}
