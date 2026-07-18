using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncEntityDefinitionRepository(IMasterConnectionFactory connectionFactory)
    : ISyncEntityDefinitionRepository
{
    private const string SearchProcedure = "dbo.SP_NA_GET_SYNCENTITYDEFINITIONPAGINAR";
    private const string ListProcedure = "dbo.SP_NA_GET_SYNCENTITYDEFINITIONLISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_SYNCENTITYDEFINITIONBUSCARPORID";
    private const string GetByCodeProcedure = "dbo.SP_NA_GET_SYNCENTITYDEFINITIONBUSCARPORCODIGO";
    private const string LookupProcedure = "dbo.SP_NA_GET_SYNCENTITYDEFINITIONLOOKUP";
    private const string HistoryProcedure = "dbo.SP_NA_GET_SYNCENTITYDEFINITIONHISTORIAL";
    private const string CreateProcedure = "dbo.SP_NA_POST_SYNCENTITYDEFINITIONCREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_SYNCENTITYDEFINITIONACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_SYNCENTITYDEFINITIONELIMINAR";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<PagedResultDto<SyncEntityDefinitionRecord>> SearchAsync(
        SyncEntityDefinitionListFilter filter,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(
                SearchProcedure,
                new
                {
                    Search = Clean(filter.Search),
                    filter.IsActive,
                    filter.PageNumber,
                    filter.PageSize
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        var items = (await grid.ReadAsync<SyncEntityDefinitionRecord>()).AsList();
        var totalCount = await grid.ReadSingleAsync<int>();
        return new PagedResultDto<SyncEntityDefinitionRecord>(
            items,
            totalCount,
            filter.PageNumber < 1 ? 1 : filter.PageNumber,
            filter.PageSize is < 1 or > 500 ? 50 : filter.PageSize);
    }

    public async Task<IReadOnlyCollection<SyncEntityDefinitionRecord>> ListAsync(
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncEntityDefinitionRecord>(
            new CommandDefinition(
                ListProcedure,
                new { IsActive = isActive },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
        return rows.AsList();
    }

    public Task<SyncEntityDefinitionDetailRecord?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return ReadDetailAsync(GetByIdProcedure, new { Id = id }, cancellationToken);
    }

    public Task<SyncEntityDefinitionDetailRecord?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return ReadDetailAsync(GetByCodeProcedure, new { Code = Clean(code) }, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SyncEntityDefinitionDetailRecord>> GetLookupAsync(
        int? includeId,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(
                LookupProcedure,
                new { IncludeId = includeId, IncludeInactive = includeInactive },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        var definitions = (await grid.ReadAsync<SyncEntityDefinitionRecord>()).AsList();
        var dependencies = (await grid.ReadAsync<SyncEntityDefinitionDependencyRecord>()).AsList();
        var dependenciesByDefinition = dependencies
            .GroupBy(dependency => dependency.EntityDefinitionId)
            .ToDictionary(group => group.Key, group => (IReadOnlyCollection<SyncEntityDefinitionDependencyRecord>)group.ToArray());

        return definitions
            .Select(definition => new SyncEntityDefinitionDetailRecord(
                definition,
                dependenciesByDefinition.GetValueOrDefault(definition.Id, Array.Empty<SyncEntityDefinitionDependencyRecord>())))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<SecurityChangeDto>> GetHistoryAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SecurityChangeDto>(
            new CommandDefinition(
                HistoryProcedure,
                new { Id = id },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
        return rows.AsList();
    }

    public async Task<SyncEntityDefinitionMutationResult> CreateAsync(
        CreateSyncEntityDefinitionData definition,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    CreateProcedure,
                    new
                    {
                        Code = Clean(definition.Code),
                        Name = Clean(definition.Name),
                        Description = Clean(definition.Description),
                        definition.DefaultExecutionOrder,
                        definition.SupportsIncremental,
                        definition.SupportsInsert,
                        definition.SupportsUpdate,
                        definition.SupportsDeactivate,
                        DefaultKeyField = Clean(definition.DefaultKeyField),
                        DefaultModifiedAtField = Clean(definition.DefaultModifiedAtField),
                        definition.IsActive,
                        definition.AuditUserId,
                        AuditUserName = Clean(definition.AuditUserName),
                        DependenciesJson = SerializeDependencies(definition.DependencyDefinitionIds)
                    },
                    cancellationToken: cancellationToken,
                    commandType: CommandType.StoredProcedure));
            return SyncEntityDefinitionMutationResult.Success(id);
        }
        catch (SqlException exception) when (TryMapSqlError(exception.Number, out var error))
        {
            return SyncEntityDefinitionMutationResult.Failure(error);
        }
    }

    public async Task<SyncEntityDefinitionMutationResult> UpdateAsync(
        UpdateSyncEntityDefinitionData definition,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.CreateConnection();
            var affected = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    UpdateProcedure,
                    new
                    {
                        definition.Id,
                        Name = Clean(definition.Name),
                        Description = Clean(definition.Description),
                        definition.DefaultExecutionOrder,
                        definition.SupportsIncremental,
                        definition.SupportsInsert,
                        definition.SupportsUpdate,
                        definition.SupportsDeactivate,
                        DefaultKeyField = Clean(definition.DefaultKeyField),
                        DefaultModifiedAtField = Clean(definition.DefaultModifiedAtField),
                        definition.IsActive,
                        definition.AuditUserId,
                        AuditUserName = Clean(definition.AuditUserName),
                        DependenciesJson = SerializeDependencies(definition.DependencyDefinitionIds)
                    },
                    cancellationToken: cancellationToken,
                    commandType: CommandType.StoredProcedure));
            return affected > 0
                ? SyncEntityDefinitionMutationResult.Success(definition.Id)
                : SyncEntityDefinitionMutationResult.Failure(SyncEntityDefinitionMutationError.NotFound);
        }
        catch (SqlException exception) when (TryMapSqlError(exception.Number, out var error))
        {
            return SyncEntityDefinitionMutationResult.Failure(error);
        }
    }

    public async Task<SyncEntityDefinitionMutationResult> DeleteAsync(
        int id,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = connectionFactory.CreateConnection();
            var affected = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    DeleteProcedure,
                    new { Id = id, AuditUserId = auditUserId, AuditUserName = Clean(auditUserName) },
                    cancellationToken: cancellationToken,
                    commandType: CommandType.StoredProcedure));
            return affected > 0
                ? SyncEntityDefinitionMutationResult.Success(id)
                : SyncEntityDefinitionMutationResult.Failure(SyncEntityDefinitionMutationError.NotFound);
        }
        catch (SqlException exception) when (TryMapSqlError(exception.Number, out var error))
        {
            return SyncEntityDefinitionMutationResult.Failure(error);
        }
    }

    private async Task<SyncEntityDefinitionDetailRecord?> ReadDetailAsync(
        string procedure,
        object parameters,
        CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(
                procedure,
                parameters,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        var definition = await grid.ReadSingleOrDefaultAsync<SyncEntityDefinitionRecord>();
        var dependencies = (await grid.ReadAsync<SyncEntityDefinitionDependencyRecord>()).AsList();
        return definition is null ? null : new SyncEntityDefinitionDetailRecord(definition, dependencies);
    }

    private static string SerializeDependencies(IReadOnlyCollection<int> dependencyIds)
    {
        return JsonSerializer.Serialize(
            dependencyIds.Select(id => new DependencyPayload(id)),
            JsonOptions);
    }

    private static bool TryMapSqlError(int number, out SyncEntityDefinitionMutationError error)
    {
        error = number switch
        {
            51102 or 51106 or 51107 => SyncEntityDefinitionMutationError.InvalidData,
            51103 => SyncEntityDefinitionMutationError.DuplicateCode,
            51104 => SyncEntityDefinitionMutationError.InvalidDependency,
            51105 => SyncEntityDefinitionMutationError.DependencyCycle,
            51108 => SyncEntityDefinitionMutationError.SystemDefinition,
            51109 => SyncEntityDefinitionMutationError.ReferencedByProfile,
            51110 => SyncEntityDefinitionMutationError.RequiredByDefinition,
            _ => SyncEntityDefinitionMutationError.None
        };
        return error != SyncEntityDefinitionMutationError.None;
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record DependencyPayload(int DependencyDefinitionId);
}
