using NuanSystem.Application.Features.SapSync.Mapping;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapMasterDataMappingRepository
{
    Task<IReadOnlyCollection<SapQueryDefinitionDto>> GetQueryDefinitionsAsync(
        string? entityCode,
        string? transport,
        bool activeOnly,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SapFieldMappingDto>> GetFieldMappingsAsync(
        long profileEntityId,
        SapMappingDirection? direction,
        bool activeOnly,
        CancellationToken cancellationToken = default);

    Task<SapMappingWriteResult> CreateQueryDefinitionAsync(
        SapQueryDefinitionWriteModel model,
        CancellationToken cancellationToken = default);

    Task<SapMappingWriteResult> ReplaceFieldMappingsAsync(
        long profileEntityId,
        IReadOnlyCollection<SapFieldMappingWriteModel> mappings,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);

    Task<SapMappingWriteResult> DeactivateQueryDefinitionAsync(
        long queryDefinitionId,
        byte[] expectedRowVersion,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
