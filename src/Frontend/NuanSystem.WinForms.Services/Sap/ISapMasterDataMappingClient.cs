using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Services.Sap;

public interface ISapMasterDataMappingClient
{
    Task<IReadOnlyCollection<SapFieldMappingItem>> GetFieldMappingsAsync(
        long profileId,
        long profileEntityId,
        string? direction = null,
        CancellationToken cancellationToken = default);

    Task<SapMappingWriteResult> ReplaceFieldMappingsAsync(
        long profileId,
        long profileEntityId,
        IReadOnlyCollection<SaveSapFieldMappingRequest> mappings,
        CancellationToken cancellationToken = default);
}
