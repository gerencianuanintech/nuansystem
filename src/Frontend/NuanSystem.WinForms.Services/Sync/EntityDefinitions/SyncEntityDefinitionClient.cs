using System.Globalization;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Sync.EntityDefinitions.Models;
using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.Services.Sync.EntityDefinitions;

public sealed class SyncEntityDefinitionClient(INuanApiClient apiClient) : ISyncEntityDefinitionClient
{
    private const string BasePath = "/api/sync/configuration/entities";

    public Task<PagedResult<SyncEntityDefinitionListItem>> SearchAsync(
        SyncEntityDefinitionListFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(
            ("search", filter.Search),
            ("isActive", filter.IsActive),
            ("pageNumber", filter.PageNumber),
            ("pageSize", filter.PageSize));

        return apiClient.GetAsync<PagedResult<SyncEntityDefinitionListItem>>($"{BasePath}{query}", cancellationToken);
    }

    public Task<IReadOnlyCollection<SyncEntityDefinitionLookupItem>> GetLookupAsync(
        int? includeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(("includeId", includeId));
        return apiClient.GetAsync<IReadOnlyCollection<SyncEntityDefinitionLookupItem>>(
            $"{BasePath}/lookup{query}",
            cancellationToken);
    }

    public Task<SyncEntityDefinitionDetail> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SyncEntityDefinitionDetail>($"{BasePath}/{id}", cancellationToken);
    }

    public Task<IReadOnlyCollection<SecurityChangeItem>> GetHistoryAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<SecurityChangeItem>>(
            $"{BasePath}/{id}/history",
            cancellationToken);
    }

    public Task<SyncEntityDefinitionDetail> CreateAsync(
        CreateSyncEntityDefinitionRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<CreateSyncEntityDefinitionRequest, SyncEntityDefinitionDetail>(
            BasePath,
            request,
            cancellationToken);
    }

    public Task<SyncEntityDefinitionDetail> UpdateAsync(
        int id,
        UpdateSyncEntityDefinitionRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<UpdateSyncEntityDefinitionRequest, SyncEntityDefinitionDetail>(
            $"{BasePath}/{id}",
            request,
            cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"{BasePath}/{id}", cancellationToken);
    }

    private static string BuildQuery(params (string Name, object? Value)[] parameters)
    {
        var values = parameters
            .Where(parameter => parameter.Value is not null)
            .Select(parameter => (parameter.Name, Value: Format(parameter.Value!)))
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
            .Select(parameter => $"{Uri.EscapeDataString(parameter.Name)}={Uri.EscapeDataString(parameter.Value)}")
            .ToArray();

        return values.Length == 0 ? string.Empty : $"?{string.Join('&', values)}";
    }

    private static string Format(object value)
    {
        return value switch
        {
            bool boolean => boolean ? "true" : "false",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
        };
    }
}
