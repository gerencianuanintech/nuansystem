using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Profiles;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncExecutionRepository(ITenantConnectionFactory connectionFactory) : ISapSyncExecutionRepository
{
    internal const string CreateProcedure = "dbo.SP_NA_POST_SAPSYNCEXECUTIONCREAR";
    internal const string SearchProcedure = "dbo.SP_NA_GET_SAPSYNCEXECUTIONPAGINAR";
    internal const string GetByUidProcedure = "dbo.SP_NA_GET_SAPSYNCEXECUTIONBUSCARPORUID";
    internal const string SearchDetailsProcedure = "dbo.SP_NA_GET_SAPSYNCEXECUTIONDETALLEPAGINAR";
    internal const string UpsertDetailProcedure = "dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLEGUARDAR";
    internal const string TransitionProcedure = "dbo.SP_NA_PATCH_SAPSYNCEXECUTIONTRANSICIONAR";
    internal const string RequestCancellationProcedure = "dbo.SP_NA_PATCH_SAPSYNCEXECUTIONCANCELARSOLICITAR";
    internal const string ClaimDetailProcedure = "dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLECLAIM";
    internal const string RenewDetailProcedure = "dbo.SP_NA_PATCH_SAPSYNCEXECUTIONDETALLERENOVAR";
    internal const string ReleaseDetailProcedure = "dbo.SP_NA_PATCH_SAPSYNCEXECUTIONDETALLELIBERAR";
    internal const string ManualRetryProcedure = "dbo.SP_NA_POST_SAPSYNCEXECUTIONREINTENTOMANUAL";
    internal const string CompleteDetailProcedure = "dbo.SP_NA_PATCH_SAPSYNCEXECUTIONDETALLECOMPLETAR";
    internal const string RecoverExpiredProcedure = "dbo.SP_NA_POST_SAPSYNCEXECUTIONDETALLERECUPERARVENCIDOS";
    internal const string ReleaseExpiredProcedure = "dbo.SP_NA_PATCH_SAPSYNCEXECUTIONDETALLELIBERARVENCIDO";

    public Task<SapSyncExecutionWriteResult> CreateAsync(
        SapSyncExecutionCreateData data,
        CancellationToken cancellationToken = default) =>
        ExecuteWriteAsync(CreateProcedure, data, cancellationToken);

    public async Task<SapSyncPagedResult<SapSyncExecutionListItemDto>> SearchAsync(
        SapSyncExecutionFilter filter,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            SearchProcedure,
            filter,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        var items = (await grid.ReadAsync<SapSyncExecutionListItemDto>()).AsList();
        var total = await grid.ReadSingleAsync<int>();
        return new SapSyncPagedResult<SapSyncExecutionListItemDto>(
            items,
            total,
            Math.Max(filter.PageNumber, 1),
            filter.PageSize is < 1 or > 500 ? 50 : filter.PageSize);
    }

    public async Task<SapSyncExecutionDto?> GetByExecutionUidAsync(
        Guid executionUid,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SapSyncExecutionDto>(new CommandDefinition(
            GetByUidProcedure,
            new { ExecutionUid = executionUid },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public async Task<SapSyncPagedResult<SapSyncExecutionDetailListItemDto>> SearchDetailsAsync(
        SapSyncExecutionDetailFilter filter,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            SearchDetailsProcedure,
            filter,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        var items = (await grid.ReadAsync<SapSyncExecutionDetailListItemDto>()).AsList();
        var total = await grid.ReadSingleAsync<int>();
        return new SapSyncPagedResult<SapSyncExecutionDetailListItemDto>(
            items,
            total,
            Math.Max(filter.PageNumber, 1),
            filter.PageSize is < 1 or > 500 ? 100 : filter.PageSize);
    }

    public Task<SapSyncExecutionWriteResult> UpsertDetailAsync(
        SapSyncExecutionDetailData detail,
        CancellationToken cancellationToken = default) =>
        ExecuteWriteAsync(UpsertDetailProcedure, detail, cancellationToken);

    public Task<SapSyncExecutionWriteResult> TransitionAsync(
        SapSyncExecutionStateData state,
        CancellationToken cancellationToken = default) =>
        ExecuteWriteAsync(TransitionProcedure, state, cancellationToken);

    public Task<SapSyncExecutionWriteResult> RequestCancellationAsync(
        Guid executionUid,
        int? requestedByUserId,
        string? requestedByUserName,
        byte[] expectedRowVersion,
        CancellationToken cancellationToken = default) =>
        ExecuteWriteAsync(
            RequestCancellationProcedure,
            new
            {
                ExecutionUid = executionUid,
                RequestedByUserId = requestedByUserId,
                RequestedByUserName = Clean(requestedByUserName),
                ExpectedRowVersion = expectedRowVersion
            },
            cancellationToken);

    public async Task<SapSyncExecutionDetailClaim?> TryClaimDueDetailAsync(
        string workerInstance,
        string ownerToken,
        DateTime lockExpiresAtUtc,
        IReadOnlyCollection<string> approvedSnapshotTypes,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SapSyncExecutionDetailClaim>(new CommandDefinition(
            ClaimDetailProcedure,
            new
            {
                WorkerInstance = Clean(workerInstance),
                OwnerToken = Clean(ownerToken),
                LockExpiresAtUtc = lockExpiresAtUtc,
                ApprovedSnapshotTypesCsv = string.Join(',', approvedSnapshotTypes)
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public async Task<SapSyncExecutionRetryResult> CreateManualRetryAsync(
        SapSyncExecutionRetryRequest request,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<SapSyncExecutionRetryResult>(new CommandDefinition(
            ManualRetryProcedure,
            new
            {
                request.ParentExecutionUid,
                request.ClientRequestId,
                Reason = Clean(request.Reason),
                request.RequestedByUserId,
                RequestedByUserName = Clean(request.RequestedByUserName),
                request.ExpectedRowVersion
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public Task<SapSyncExecutionWriteResult> CompleteClaimedDetailAsync(
        SapSyncExecutionDetailCompletion completion,
        CancellationToken cancellationToken = default) =>
        ExecuteWriteAsync(CompleteDetailProcedure, completion, cancellationToken);

    public async Task<int> RecoverExpiredDetailLocksAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            RecoverExpiredProcedure,
            new { UtcNow = utcNow },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public Task<SapSyncExecutionWriteResult> ReleaseExpiredDetailLockAsync(
        long detailId,
        string reason,
        int? requestedByUserId,
        string? requestedByUserName,
        byte[] expectedRowVersion,
        CancellationToken cancellationToken = default) =>
        ExecuteWriteAsync(ReleaseExpiredProcedure, new
        {
            DetailId = detailId,
            Reason = Clean(reason),
            RequestedByUserId = requestedByUserId,
            RequestedByUserName = Clean(requestedByUserName),
            ExpectedRowVersion = expectedRowVersion
        }, cancellationToken);

    public Task<bool> RenewDetailLockAsync(
        long detailId,
        string ownerToken,
        DateTime lockExpiresAtUtc,
        CancellationToken cancellationToken = default) =>
        ExecuteBooleanAsync(
            RenewDetailProcedure,
            new { DetailId = detailId, OwnerToken = Clean(ownerToken), LockExpiresAtUtc = lockExpiresAtUtc },
            cancellationToken);

    public Task<bool> ReleaseDetailLockAsync(
        long detailId,
        string ownerToken,
        CancellationToken cancellationToken = default) =>
        ExecuteBooleanAsync(
            ReleaseDetailProcedure,
            new { DetailId = detailId, OwnerToken = Clean(ownerToken) },
            cancellationToken);

    private async Task<SapSyncExecutionWriteResult> ExecuteWriteAsync(
        string procedure,
        object parameters,
        CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<SapSyncExecutionWriteResult>(new CommandDefinition(
            procedure,
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    private async Task<bool> ExecuteBooleanAsync(
        string procedure,
        object parameters,
        CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            procedure,
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken)) > 0;
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
