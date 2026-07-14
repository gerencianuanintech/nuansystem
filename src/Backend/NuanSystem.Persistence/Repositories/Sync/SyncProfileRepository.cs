using System.Data;
using System.Globalization;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncProfileRepository(IMasterConnectionFactory connectionFactory) : ISyncProfileRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_SYNCPROFILELISTAR";
    private const string SearchProcedure = "dbo.SP_NA_GET_SYNCPROFILEPAGINAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_SYNCPROFILEBUSCARPORID";
    private const string GetByCodeProcedure = "dbo.SP_NA_GET_SYNCPROFILEBUSCARPORCODIGO";
    private const string CreateProcedure = "dbo.SP_NA_POST_SYNCPROFILECREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_SYNCPROFILEACTUALIZAR";
    private const string SetActiveProcedure = "dbo.SP_NA_PATCH_SYNCPROFILEACTIVAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_SYNCPROFILEELIMINAR";
    private const string HasHistoryProcedure = "dbo.SP_NA_GET_SYNCPROFILETIENEHISTORIAL";
    private const string CompanyLookupsProcedure = "dbo.SP_NA_GET_SYNCCONFIGURATIONCOMPANYLOOKUPS";
    private const string AuditProcedure = "dbo.SP_NA_POST_SYNCPROFILEAUDITREGISTRAR";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<PagedResultDto<SyncProfileListItemDto>> SearchAsync(
        SyncProfileListFilter filter,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(
                SearchProcedure,
                new
                {
                    filter.Search,
                    filter.CompanyId,
                    filter.IsActive,
                    filter.ExecutionMode,
                    filter.PageNumber,
                    filter.PageSize,
                    filter.UserId
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        var items = (await grid.ReadAsync<SyncProfileListItemDto>()).AsList();
        var totalCount = await grid.ReadSingleAsync<int>();

        return new PagedResultDto<SyncProfileListItemDto>(
            items,
            totalCount,
            filter.PageNumber < 1 ? 1 : filter.PageNumber,
            filter.PageSize is < 1 or > 500 ? 50 : filter.PageSize);
    }

    public async Task<IReadOnlyCollection<SyncProfileSummaryDto>> ListAsync(
        int? companyId,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncProfileSummaryDto>(
            new CommandDefinition(
                ListProcedure,
                new { CompanyId = companyId, IsActive = isActive },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return rows.AsList();
    }

    public Task<SyncProfileDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return ReadDetailAsync(GetByIdProcedure, new { Id = id }, cancellationToken);
    }

    public Task<SyncProfileDetailDto?> GetByCodeAsync(
        int companyId,
        string code,
        CancellationToken cancellationToken = default)
    {
        return ReadDetailAsync(
            GetByCodeProcedure,
            new { CompanyId = companyId, Code = NormalizeCode(code) },
            cancellationToken);
    }

    public async Task<int> CreateAsync(SyncProfileAggregate profile, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                CreateProcedure,
                BuildParameters(profile, includeId: false),
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateAsync(SyncProfileAggregate profile, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                UpdateProcedure,
                BuildParameters(profile, includeId: true),
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> SetActiveAsync(
        int id,
        bool isActive,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                SetActiveProcedure,
                new { Id = id, IsActive = isActive, UpdatedByUserId = updatedByUserId, UpdatedByUserName = updatedByUserName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                DeleteProcedure,
                new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> HasOperationalHistoryAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                HasHistoryProcedure,
                new { Id = id },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<IReadOnlyCollection<SyncCompanyLookupRecord>> GetCompanyLookupsAsync(
        int? userId,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncCompanyLookupRecord>(
            new CommandDefinition(
                CompanyLookupsProcedure,
                new { UserId = userId },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return rows.AsList();
    }

    public async Task RecordAuditAsync(
        int? profileId,
        string action,
        string? fieldName,
        string? oldValue,
        string? newValue,
        int? userId,
        string? userName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                AuditProcedure,
                new
                {
                    ProfileId = profileId,
                    Action = action,
                    FieldName = fieldName,
                    OldValue = oldValue,
                    NewValue = newValue,
                    UserId = userId,
                    UserName = Clean(userName)
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    private async Task<SyncProfileDetailDto?> ReadDetailAsync(
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

        var header = await grid.ReadSingleOrDefaultAsync<SyncProfileHeaderRecord>();
        if (header is null)
        {
            return null;
        }

        var branches = (await grid.ReadAsync<SyncProfileBranchRecord>()).AsList();
        var entities = (await grid.ReadAsync<SyncProfileEntityRecord>()).AsList();
        var entityBranches = (await grid.ReadAsync<SyncProfileEntityBranchRecord>()).AsList();
        var schedule = await grid.ReadSingleOrDefaultAsync<SyncScheduleRecord>();

        return new SyncProfileDetailDto(
            header.Id,
            header.CompanyId,
            header.CompanyCode,
            header.CompanyName,
            header.Code,
            header.Name,
            header.Description,
            header.Direction,
            header.ExecutionMode,
            header.ConflictStrategy,
            header.BatchSize,
            header.MaxRetries,
            header.RetryDelaySeconds,
            header.TimeoutMinutes,
            header.IsActive,
            header.CreatedByUserId,
            header.CreatedByUserName,
            header.CreatedAt,
            header.UpdatedByUserId,
            header.UpdatedByUserName,
            header.UpdatedAt,
            branches,
            entities,
            entityBranches,
            schedule);
    }

    private static object BuildParameters(SyncProfileAggregate profile, bool includeId)
    {
        var parameters = new DynamicParameters();
        if (includeId)
        {
            parameters.Add("Id", profile.Id);
        }

        parameters.Add("CompanyId", profile.CompanyId);
        parameters.Add("Code", NormalizeCode(profile.Code));
        parameters.Add("Name", Clean(profile.Name));
        parameters.Add("Description", Clean(profile.Description));
        parameters.Add("Direction", Clean(profile.Direction));
        parameters.Add("ExecutionMode", Clean(profile.ExecutionMode));
        parameters.Add("ConflictStrategy", Clean(profile.ConflictStrategy));
        parameters.Add("BatchSize", profile.BatchSize);
        parameters.Add("MaxRetries", profile.MaxRetries);
        parameters.Add("RetryDelaySeconds", profile.RetryDelaySeconds);
        parameters.Add("TimeoutMinutes", profile.TimeoutMinutes);
        parameters.Add("IsActive", profile.IsActive);
        parameters.Add("AuditUserId", profile.AuditUserId);
        parameters.Add("AuditUserName", Clean(profile.AuditUserName));
        parameters.Add("BranchesJson", SerializeBranches(profile.Branches));
        parameters.Add("EntitiesJson", SerializeEntities(profile.Entities));
        parameters.Add("EntityBranchesJson", SerializeEntityBranches(profile.EntityBranches));
        parameters.Add("ScheduleJson", SerializeSchedule(profile.Schedule));

        return parameters;
    }

    private static string SerializeBranches(IEnumerable<SyncProfileBranchRecord> branches)
    {
        var payload = branches.Select(branch => new
        {
            branch.BranchCompanyId,
            branch.BatchSize,
            branch.MaxRetries,
            branch.IsActive
        });

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static string SerializeEntities(IEnumerable<SyncProfileEntityRecord> entities)
    {
        var payload = entities.Select(entity => new
        {
            EntityCode = NormalizeCode(entity.EntityCode),
            EntityName = Clean(entity.EntityName),
            entity.ExecutionOrder,
            SyncMode = Clean(entity.SyncMode),
            KeyField = Clean(entity.KeyField),
            ModifiedAtField = Clean(entity.ModifiedAtField),
            VersionField = Clean(entity.VersionField),
            ActiveField = Clean(entity.ActiveField),
            entity.AllowInsert,
            entity.AllowUpdate,
            entity.AllowDeactivate,
            entity.ContinueOnError,
            entity.BatchSize,
            entity.IsActive
        });

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static string SerializeEntityBranches(IEnumerable<SyncProfileEntityBranchRecord> entityBranches)
    {
        var payload = entityBranches.Select(entityBranch => new
        {
            EntityCode = NormalizeCode(entityBranch.EntityCode),
            entityBranch.BranchCompanyId,
            entityBranch.IsEnabled,
            entityBranch.BatchSize
        });

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static string? SerializeSchedule(SyncScheduleRecord? schedule)
    {
        if (schedule is null)
        {
            return null;
        }

        var payload = new
        {
            ScheduleType = Clean(schedule.ScheduleType),
            schedule.IntervalMinutes,
            ExecutionTime = schedule.ExecutionTime?.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture),
            TimeZoneId = string.IsNullOrWhiteSpace(schedule.TimeZoneId) ? "America/Guayaquil" : schedule.TimeZoneId.Trim(),
            schedule.PreventConcurrentExecutions,
            schedule.IsActive
        };

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static string NormalizeCode(string value)
    {
        return value.Trim();
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private sealed record SyncProfileHeaderRecord(
        int Id,
        int CompanyId,
        string CompanyCode,
        string CompanyName,
        string Code,
        string Name,
        string? Description,
        string Direction,
        string ExecutionMode,
        string ConflictStrategy,
        int BatchSize,
        int MaxRetries,
        int RetryDelaySeconds,
        int TimeoutMinutes,
        bool IsActive,
        int? CreatedByUserId,
        string? CreatedByUserName,
        DateTime CreatedAt,
        int? UpdatedByUserId,
        string? UpdatedByUserName,
        DateTime? UpdatedAt);
}
