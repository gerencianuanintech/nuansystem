using System.Data;
using System.Globalization;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Profiles;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncProfileRepository(IMasterConnectionFactory connectionFactory) : ISapSyncProfileRepository
{
    internal const string SearchProcedure = "dbo.SP_NA_GET_SAPSYNCPROFILEPAGINAR";
    internal const string GetByIdProcedure = "dbo.SP_NA_GET_SAPSYNCPROFILEBUSCARPORID";
    internal const string CapabilitiesProcedure = "dbo.SP_NA_GET_SAPSYNCHANDLERCAPABILITYLISTAR";
    internal const string CreateProcedure = "dbo.SP_NA_POST_SAPSYNCPROFILECREAR";
    internal const string UpdateProcedure = "dbo.SP_NA_PUT_SAPSYNCPROFILEACTUALIZAR";
    internal const string SetActiveProcedure = "dbo.SP_NA_PATCH_SAPSYNCPROFILEACTIVAR";
    internal const string DeleteProcedure = "dbo.SP_NA_DELETE_SAPSYNCPROFILEELIMINAR";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<SapSyncPagedResult<SapSyncProfileListItemDto>> SearchAsync(
        SapSyncProfileFilter filter,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            SearchProcedure,
            new
            {
                filter.CompanyId,
                Search = Clean(filter.Search),
                filter.IsActive,
                EntityCode = Clean(filter.EntityCode),
                filter.PageNumber,
                filter.PageSize
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var items = (await grid.ReadAsync<SapSyncProfileListItemDto>()).AsList();
        var total = await grid.ReadSingleAsync<int>();
        return new SapSyncPagedResult<SapSyncProfileListItemDto>(
            items,
            total,
            Math.Max(filter.PageNumber, 1),
            filter.PageSize is < 1 or > 500 ? 50 : filter.PageSize);
    }

    public async Task<SapSyncProfileDetailDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            GetByIdProcedure,
            new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var header = await grid.ReadSingleOrDefaultAsync<ProfileHeader>();
        if (header is null)
        {
            return null;
        }

        var entities = (await grid.ReadAsync<ProfileEntityRow>())
            .Select(static row => row.ToContract())
            .ToArray();

        return header.ToContract(entities);
    }

    public async Task<IReadOnlyCollection<SapSyncHandlerCapabilityDto>> GetHandlerCapabilitiesAsync(
        bool activeOnly,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SapSyncHandlerCapabilityDto>(new CommandDefinition(
            CapabilitiesProcedure,
            new { ActiveOnly = activeOnly },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public Task<SapSyncProfileWriteResult> CreateAsync(
        SapSyncProfileAggregate profile,
        CancellationToken cancellationToken = default)
    {
        return ExecuteWriteAsync(
            CreateProcedure,
            BuildParameters(profile, includeId: false),
            cancellationToken);
    }

    public Task<SapSyncProfileWriteResult> UpdateAsync(
        SapSyncProfileAggregate profile,
        CancellationToken cancellationToken = default)
    {
        return ExecuteWriteAsync(
            UpdateProcedure,
            BuildParameters(profile, includeId: true),
            cancellationToken);
    }

    public Task<SapSyncProfileWriteResult> SetActiveAsync(
        long id,
        bool isActive,
        byte[] expectedRowVersion,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        return ExecuteWriteAsync(
            SetActiveProcedure,
            new
            {
                Id = id,
                IsActive = isActive,
                ExpectedRowVersion = expectedRowVersion,
                AuditUserId = auditUserId,
                AuditUserName = Clean(auditUserName)
            },
            cancellationToken);
    }

    public Task<SapSyncProfileWriteResult> DeleteAsync(
        long id,
        byte[] expectedRowVersion,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        return ExecuteWriteAsync(
            DeleteProcedure,
            new
            {
                Id = id,
                ExpectedRowVersion = expectedRowVersion,
                AuditUserId = auditUserId,
                AuditUserName = Clean(auditUserName)
            },
            cancellationToken);
    }

    private async Task<SapSyncProfileWriteResult> ExecuteWriteAsync(
        string procedure,
        object parameters,
        CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<SapSyncProfileWriteResult>(new CommandDefinition(
            procedure,
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    private static DynamicParameters BuildParameters(SapSyncProfileAggregate profile, bool includeId)
    {
        var parameters = new DynamicParameters();
        if (includeId)
        {
            parameters.Add("Id", profile.Id);
            parameters.Add("ExpectedRowVersion", profile.RowVersion);
        }

        parameters.Add("CompanyId", profile.CompanyId);
        parameters.Add("Code", NormalizeCode(profile.Code));
        parameters.Add("Name", Clean(profile.Name));
        parameters.Add("Description", Clean(profile.Description));
        parameters.Add("IsActive", profile.IsActive);
        parameters.Add("EntitiesJson", JsonSerializer.Serialize(
            profile.Entities.Select(entity => new
            {
                entity.Id,
                EntityCode = NormalizeCode(entity.EntityCode),
                Direction = entity.Direction.ToString(),
                SyncMode = Clean(entity.SyncMode),
                entity.BatchSize,
                entity.MaxAttempts,
                entity.ExecutionOrder,
                entity.ContinueOnError,
                entity.ExecutionTimeoutMinutes,
                entity.IsActive,
                EntityRowVersion = ToHex(entity.RowVersion),
                Schedule = new
                {
                    entity.Schedule.Id,
                    ScheduleType = Clean(entity.Schedule.ScheduleType),
                    entity.Schedule.IntervalMinutes,
                    ExecutionTime = entity.Schedule.ExecutionTime?.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture),
                    TimeZoneId = string.IsNullOrWhiteSpace(entity.Schedule.TimeZoneId)
                        ? "America/Guayaquil"
                        : entity.Schedule.TimeZoneId.Trim(),
                    entity.Schedule.PreventConcurrentExecutions,
                    entity.Schedule.NextExecutionAtUtc,
                    entity.Schedule.LastScheduledAtUtc,
                    entity.Schedule.LastExecutionAtUtc,
                    entity.Schedule.LastSuccessfulExecutionAtUtc,
                    entity.Schedule.IsActive,
                    ScheduleRowVersion = ToHex(entity.Schedule.RowVersion)
                }
            }),
            JsonOptions));
        parameters.Add("AuditUserId", profile.AuditUserId);
        parameters.Add("AuditUserName", Clean(profile.AuditUserName));
        return parameters;
    }

    private static string NormalizeCode(string value) => value.Trim();
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? ToHex(byte[]? value) => value is null ? null : Convert.ToHexString(value);

    private sealed record ProfileHeader(
        long Id,
        int CompanyId,
        string CompanyCode,
        string CompanyName,
        string Code,
        string Name,
        string? Description,
        bool IsActive,
        int? CreatedByUserId,
        string? CreatedByUserName,
        DateTime CreatedAtUtc,
        int? UpdatedByUserId,
        string? UpdatedByUserName,
        DateTime? UpdatedAtUtc,
        byte[] RowVersion)
    {
        public SapSyncProfileDetailDto ToContract(IReadOnlyCollection<SapSyncProfileEntityData> entities) =>
            new(
                Id,
                CompanyId,
                CompanyCode,
                CompanyName,
                Code,
                Name,
                Description,
                IsActive,
                CreatedByUserId,
                CreatedByUserName,
                CreatedAtUtc,
                UpdatedByUserId,
                UpdatedByUserName,
                UpdatedAtUtc,
                RowVersion,
                entities);
    }

    private sealed record ProfileEntityRow(
        long Id,
        string EntityCode,
        Application.Features.SapSync.Enums.SapSyncDirection Direction,
        string SyncMode,
        int BatchSize,
        int MaxAttempts,
        int ExecutionOrder,
        bool ContinueOnError,
        int ExecutionTimeoutMinutes,
        bool IsActive,
        byte[] RowVersion,
        long ScheduleId,
        string ScheduleType,
        int? IntervalMinutes,
        TimeSpan? ExecutionTime,
        string TimeZoneId,
        bool PreventConcurrentExecutions,
        DateTime? NextExecutionAtUtc,
        DateTime? LastScheduledAtUtc,
        DateTime? LastExecutionAtUtc,
        DateTime? LastSuccessfulExecutionAtUtc,
        bool ScheduleIsActive,
        byte[] ScheduleRowVersion)
    {
        public SapSyncProfileEntityData ToContract() =>
            new(
                Id,
                EntityCode,
                Direction,
                SyncMode,
                BatchSize,
                MaxAttempts,
                ExecutionOrder,
                ContinueOnError,
                ExecutionTimeoutMinutes,
                IsActive,
                new SapSyncScheduleData(
                    ScheduleId,
                    ScheduleType,
                    IntervalMinutes,
                    ExecutionTime,
                    TimeZoneId,
                    PreventConcurrentExecutions,
                    NextExecutionAtUtc,
                    LastScheduledAtUtc,
                    LastExecutionAtUtc,
                    LastSuccessfulExecutionAtUtc,
                    ScheduleIsActive,
                    ScheduleRowVersion),
                RowVersion);
    }
}
