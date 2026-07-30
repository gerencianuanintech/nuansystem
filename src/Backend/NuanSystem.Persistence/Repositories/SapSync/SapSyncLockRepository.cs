using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncLockRepository(ITenantConnectionFactory connectionFactory) : ISapSyncLockRepository
{
    internal const string AcquireProcedure = "dbo.SP_NA_POST_SAPSYNCLOCKADQUIRIR";
    internal const string RenewProcedure = "dbo.SP_NA_PATCH_SAPSYNCLOCKRENOVAR";
    internal const string ReleaseProcedure = "dbo.SP_NA_DELETE_SAPSYNCLOCKLIBERAR";
    internal const string ReleaseExpiredProcedure = "dbo.SP_NA_DELETE_SAPSYNCLOCKLIBERARVENCIDO";

    public async Task<SapSyncLockDto?> TryAcquireAsync(
        int companyId,
        string entityCode,
        SapSyncDirection direction,
        string workerInstance,
        string correlationId,
        Guid? executionUid,
        string ownerToken,
        DateTime lockExpiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SapSyncLockDto>(new CommandDefinition(
            AcquireProcedure,
            new
            {
                CompanyId = companyId,
                EntityCode = entityCode.Trim(),
                Direction = direction.ToString(),
                WorkerInstance = workerInstance.Trim(),
                CorrelationId = correlationId.Trim(),
                ExecutionUid = executionUid,
                OwnerToken = ownerToken.Trim(),
                LockExpiresAtUtc = lockExpiresAtUtc
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public async Task<bool> RenewAsync(
        long id,
        string ownerToken,
        DateTime lockExpiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            RenewProcedure,
            new { Id = id, OwnerToken = ownerToken.Trim(), LockExpiresAtUtc = lockExpiresAtUtc },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken)) > 0;
    }

    public async Task ReleaseAsync(
        long id,
        string ownerToken,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            ReleaseProcedure,
            new { Id = id, OwnerToken = ownerToken.Trim() },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public async Task<bool> ReleaseExpiredAsync(
        long id,
        string reason,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            ReleaseExpiredProcedure,
            new
            {
                Id = id,
                Reason = reason.Trim(),
                AuditUserId = auditUserId,
                AuditUserName = string.IsNullOrWhiteSpace(auditUserName) ? null : auditUserName.Trim()
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken)) > 0;
    }
}
