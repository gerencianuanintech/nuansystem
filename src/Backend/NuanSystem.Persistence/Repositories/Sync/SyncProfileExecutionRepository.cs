using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncProfileExecutionRepository(IMasterConnectionFactory connectionFactory) : ISyncProfileExecutionRepository
{
    public async Task<int> CreateAsync(CreateSyncProfileExecutionData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SP_NA_CREATE_SYNCPROFILEEXECUTION",
            data,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public async Task<bool> StartAsync(int executionId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affected = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SP_NA_START_SYNCPROFILEEXECUTION",
            new { ExecutionId = executionId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<bool> CompleteAsync(CompleteSyncProfileExecutionData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affected = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SP_NA_COMPLETE_SYNCPROFILEEXECUTION",
            data,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<bool> CancelAsync(int executionId, string? cancelledBy, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affected = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SP_NA_CANCEL_SYNCPROFILEEXECUTION",
            new { ExecutionId = executionId, CancelledBy = cancelledBy },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<SyncProfileExecutionDetailDto?> GetByIdAsync(int executionId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            "SP_NA_GET_SYNCPROFILEEXECUTION_BYID",
            new { ExecutionId = executionId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var header = await grid.ReadSingleOrDefaultAsync<SyncProfileExecutionHeaderRow>();
        if (header is null)
        {
            return null;
        }

        var details = (await grid.ReadAsync<SyncProfileExecutionEntityDetailRow>())
            .Select(row => row.ToDto())
            .AsList();
        return header.ToDto(details);
    }

    public async Task<PagedResultDto<SyncProfileExecutionListItemDto>> SearchAsync(
        SyncProfileExecutionFilter filter,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var parameters = new
        {
            filter.ProfileId,
            filter.Status,
            filter.ExecutionType,
            DateFrom = filter.DateFrom?.UtcDateTime,
            DateTo = filter.DateTo?.UtcDateTime,
            filter.PageNumber,
            filter.PageSize
        };

        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            "SP_NA_SEARCH_SYNCPROFILEEXECUTIONS",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var items = (await grid.ReadAsync<SyncProfileExecutionListItemRow>())
            .Select(row => row.ToDto())
            .AsList();
        var total = await grid.ReadSingleAsync<int>();
        return new PagedResultDto<SyncProfileExecutionListItemDto>(items, total, filter.PageNumber, filter.PageSize);
    }

    public async Task<int?> GetActiveExecutionAsync(int syncProfileId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int?>(new CommandDefinition(
            "SP_NA_GET_ACTIVE_SYNCPROFILEEXECUTION",
            new { SyncProfileId = syncProfileId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<SyncProfileExecutionDetailDto>> GetPendingAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = (await connection.QueryAsync<SyncProfileExecutionHeaderRow>(new CommandDefinition(
            "SP_NA_GET_PENDING_SYNCPROFILEEXECUTIONS",
            new { Take = take },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken))).AsList();

        return rows.Select(row => row.ToDto(Array.Empty<SyncProfileExecutionEntityDetailDto>())).ToArray();
    }

    public async Task<IReadOnlyCollection<DueSyncProfileDto>> GetDueProfilesAsync(
        DateTimeOffset utcNow,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<DueSyncProfileRow>(new CommandDefinition(
            "SP_NA_GET_DUE_SYNCPROFILES",
            new { UtcNow = utcNow.UtcDateTime },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return rows.Select(row => row.ToDto()).AsList();
    }

    public async Task<bool> MarkScheduledAsync(
        int syncProfileId,
        DateTimeOffset nextExecutionAt,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affected = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SP_NA_MARK_SYNCPROFILE_SCHEDULED",
            new { SyncProfileId = syncProfileId, NextExecutionAt = nextExecutionAt.UtcDateTime },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<int> UpsertDetailAsync(SyncProfileExecutionDetailUpdate data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SP_NA_UPSERT_SYNCPROFILEEXECUTIONDETAIL",
            data,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    private sealed record SyncProfileExecutionListItemRow(
        int Id,
        int SyncProfileId,
        string ProfileCode,
        string ProfileName,
        int CompanyId,
        string CompanyName,
        string ExecutionType,
        string Status,
        string CorrelationId,
        string? RequestedBy,
        DateTime RequestedAt,
        DateTime? StartedAt,
        DateTime? FinishedAt,
        int TotalEntities,
        int TotalRecordsRead,
        int TotalEventsPublished,
        int TotalSkipped,
        int TotalErrors,
        string? Message)
    {
        public SyncProfileExecutionListItemDto ToDto()
        {
            return new SyncProfileExecutionListItemDto(
                Id,
                SyncProfileId,
                ProfileCode,
                ProfileName,
                CompanyId,
                CompanyName,
                ExecutionType,
                Status,
                CorrelationId,
                RequestedBy,
                AsUtc(RequestedAt),
                AsUtc(StartedAt),
                AsUtc(FinishedAt),
                TotalEntities,
                TotalRecordsRead,
                TotalEventsPublished,
                TotalSkipped,
                TotalErrors,
                Message);
        }
    }

    private sealed record SyncProfileExecutionHeaderRow(
        int Id,
        int SyncProfileId,
        string ProfileCode,
        string ProfileName,
        int CompanyId,
        string CompanyName,
        string ExecutionType,
        string Status,
        string CorrelationId,
        string? RequestedBy,
        DateTime RequestedAt,
        DateTime? StartedAt,
        DateTime? FinishedAt,
        DateTime? CancelledAt,
        string? CancelledBy,
        string? EntityCodesJson,
        string? FromKey,
        int? MaxRecords,
        int TotalEntities,
        int TotalRecordsRead,
        int TotalEventsPublished,
        int TotalSkipped,
        int TotalErrors,
        string? Message)
    {
        public SyncProfileExecutionDetailDto ToDto(IReadOnlyCollection<SyncProfileExecutionEntityDetailDto> details)
        {
            return new SyncProfileExecutionDetailDto(
                Id,
                SyncProfileId,
                ProfileCode,
                ProfileName,
                CompanyId,
                CompanyName,
                ExecutionType,
                Status,
                CorrelationId,
                RequestedBy,
                AsUtc(RequestedAt),
                AsUtc(StartedAt),
                AsUtc(FinishedAt),
                AsUtc(CancelledAt),
                CancelledBy,
                EntityCodesJson,
                FromKey,
                MaxRecords,
                TotalEntities,
                TotalRecordsRead,
                TotalEventsPublished,
                TotalSkipped,
                TotalErrors,
                Message,
                details);
        }
    }

    private sealed record SyncProfileExecutionEntityDetailRow(
        int Id,
        int SyncProfileExecutionId,
        int SyncProfileEntityId,
        string EntityCode,
        string Status,
        DateTime? StartedAt,
        DateTime? FinishedAt,
        int TotalRecordsRead,
        int TotalEventsPublished,
        int TotalSkipped,
        int TotalErrors,
        string? LastProcessedKey,
        string? Message)
    {
        public SyncProfileExecutionEntityDetailDto ToDto()
        {
            return new SyncProfileExecutionEntityDetailDto(
                Id,
                SyncProfileExecutionId,
                SyncProfileEntityId,
                EntityCode,
                Status,
                AsUtc(StartedAt),
                AsUtc(FinishedAt),
                TotalRecordsRead,
                TotalEventsPublished,
                TotalSkipped,
                TotalErrors,
                LastProcessedKey,
                Message);
        }
    }

    private sealed record DueSyncProfileRow(
        int SyncProfileId,
        string ProfileCode,
        string ProfileName,
        int CompanyId,
        string ScheduleType,
        int? IntervalMinutes,
        TimeSpan? ExecutionTime,
        string TimeZoneId,
        DateTime? LastSuccessfulScheduledExecutionAt,
        DateTime ConfiguredAt,
        DateTime? NextExecutionAt)
    {
        public DueSyncProfileDto ToDto()
        {
            return new DueSyncProfileDto(
                SyncProfileId,
                ProfileCode,
                ProfileName,
                CompanyId,
                ScheduleType,
                IntervalMinutes,
                ExecutionTime,
                TimeZoneId,
                AsUtc(LastSuccessfulScheduledExecutionAt),
                AsUtc(ConfiguredAt),
                AsUtc(NextExecutionAt));
        }
    }

    private static DateTimeOffset AsUtc(DateTime value)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }

    private static DateTimeOffset? AsUtc(DateTime? value)
    {
        return value.HasValue ? AsUtc(value.Value) : null;
    }
}
