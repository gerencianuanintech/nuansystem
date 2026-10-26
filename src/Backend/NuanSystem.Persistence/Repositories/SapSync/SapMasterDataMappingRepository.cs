using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Mapping;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapMasterDataMappingRepository(IMasterConnectionFactory connectionFactory)
    : ISapMasterDataMappingRepository
{
    internal const string QueryDefinitionsProcedure = "dbo.SP_NA_GET_SAPQUERYDEFINITIONS_LISTAR";
    internal const string FieldMappingsProcedure = "dbo.SP_NA_GET_SAPFIELDMAPPINGS_LISTAR";
    internal const string CreateQueryDefinitionProcedure = "dbo.SP_NA_POST_SAPQUERYDEFINITIONCREAR";
    internal const string ReplaceFieldMappingsProcedure = "dbo.SP_NA_PUT_SAPFIELDMAPPINGSREEMPLAZAR";
    internal const string DeactivateQueryDefinitionProcedure = "dbo.SP_NA_PATCH_SAPQUERYDEFINITIONDESACTIVAR";

    public async Task<IReadOnlyCollection<SapQueryDefinitionDto>> GetQueryDefinitionsAsync(
        string? entityCode,
        string? transport,
        bool activeOnly,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SapQueryDefinitionDto>(new CommandDefinition(
            QueryDefinitionsProcedure,
            new { EntityCode = Clean(entityCode), Transport = Clean(transport), ActiveOnly = activeOnly },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyCollection<SapFieldMappingDto>> GetFieldMappingsAsync(
        long profileEntityId,
        SapMappingDirection? direction,
        bool activeOnly,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SapFieldMappingDto>(new CommandDefinition(
            FieldMappingsProcedure,
            new
            {
                SapSyncProfileEntityId = profileEntityId,
                Direction = direction?.ToString(),
                ActiveOnly = activeOnly
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public Task<SapMappingWriteResult> CreateQueryDefinitionAsync(
        SapQueryDefinitionWriteModel model,
        CancellationToken cancellationToken = default)
        => ExecuteWriteAsync(
            CreateQueryDefinitionProcedure,
            new
            {
                model.EntityCode,
                model.Transport,
                model.QueryName,
                model.QueryText,
                model.ParameterSchemaJson,
                model.WatermarkField,
                model.Version,
                model.IsApproved,
                model.IsActive,
                model.AuditUserId,
                AuditUserName = Clean(model.AuditUserName)
            },
            cancellationToken);

    public Task<SapMappingWriteResult> ReplaceFieldMappingsAsync(
        long profileEntityId,
        IReadOnlyCollection<SapFieldMappingWriteModel> mappings,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
        => ExecuteWriteAsync(
            ReplaceFieldMappingsProcedure,
            new
            {
                SapSyncProfileEntityId = profileEntityId,
                MappingsJson = JsonSerializer.Serialize(mappings),
                AuditUserId = auditUserId,
                AuditUserName = Clean(auditUserName)
            },
            cancellationToken);

    public Task<SapMappingWriteResult> DeactivateQueryDefinitionAsync(
        long queryDefinitionId,
        byte[] expectedRowVersion,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
        => ExecuteWriteAsync(
            DeactivateQueryDefinitionProcedure,
            new
            {
                Id = queryDefinitionId,
                ExpectedRowVersion = expectedRowVersion,
                AuditUserId = auditUserId,
                AuditUserName = Clean(auditUserName)
            },
            cancellationToken);

    private async Task<SapMappingWriteResult> ExecuteWriteAsync(
        string procedure,
        object parameters,
        CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<SapMappingWriteResult>(new CommandDefinition(
            procedure,
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    private static string? Clean(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
