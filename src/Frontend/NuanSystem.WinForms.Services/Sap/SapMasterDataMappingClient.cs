using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Services.Sap;

public sealed class SapMasterDataMappingClient(INuanApiClient apiClient) : ISapMasterDataMappingClient
{
    public Task<IReadOnlyCollection<SapFieldMappingItem>> GetFieldMappingsAsync(
        long profileId,
        long profileEntityId,
        string? direction = null,
        CancellationToken cancellationToken = default)
    {
        var query = string.IsNullOrWhiteSpace(direction) ? string.Empty : $"?direction={Uri.EscapeDataString(direction)}";
        return apiClient.GetAsync<IReadOnlyCollection<SapFieldMappingItem>>(
            $"/api/sap/profiles/{profileId}/entities/{profileEntityId}/field-mappings{query}",
            cancellationToken);
    }

    public Task<SapMappingWriteResult> ReplaceFieldMappingsAsync(
        long profileId,
        long profileEntityId,
        IReadOnlyCollection<SaveSapFieldMappingRequest> mappings,
        CancellationToken cancellationToken = default)
        => apiClient.PutAsync<ReplaceSapFieldMappingsRequest, SapMappingWriteResult>(
            $"/api/sap/profiles/{profileId}/entities/{profileEntityId}/field-mappings",
            new(mappings),
            cancellationToken);

    private sealed record ReplaceSapFieldMappingsRequest(
        IReadOnlyCollection<SaveSapFieldMappingRequest> Mappings);
}
