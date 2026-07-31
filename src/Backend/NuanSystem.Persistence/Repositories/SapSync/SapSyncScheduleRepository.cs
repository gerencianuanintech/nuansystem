using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncScheduleRepository(
    IMasterConnectionFactory connectionFactory) : ISapSyncScheduleRepository
{
    internal const string CandidatesProcedure =
        "dbo.SP_NA_GET_SAPSYNCSCHEDULECANDIDATOSPAGINAR";
    internal const string ReserveProcedure =
        "dbo.SP_NA_PATCH_SAPSYNCSCHEDULERESERVAR";

    public async Task<SapSyncScheduleCandidatePage> GetCandidatesAsync(
        SapSyncScheduleCursor cursor,
        int pageSize,
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            CandidatesProcedure,
            new
            {
                UtcNow = utcNow,
                PageSize = pageSize,
                AfterCompanyId = cursor.CompanyId,
                AfterProfileId = cursor.ProfileId,
                AfterExecutionOrder = cursor.ExecutionOrder,
                AfterEntityId = cursor.EntityId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var items = (await grid.ReadAsync<SapSyncScheduleCandidateRow>())
            .Select(SapSyncScheduleCandidateRowMapper.Map)
            .AsList();
        var enabledCompanyCount = await grid.ReadSingleAsync<int>();
        return new SapSyncScheduleCandidatePage(items, enabledCompanyCount);
    }

    public async Task<bool> TryReserveAsync(
        SapSyncScheduleReservation reservation,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            ReserveProcedure,
            new
            {
                reservation.ScheduleId,
                reservation.ExpectedRowVersion,
                reservation.UtcNow,
                reservation.ObservedNextExecutionAtUtc,
                reservation.ScheduledAtUtc,
                reservation.NextExecutionAtUtc
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken)) == 1;
    }
}
