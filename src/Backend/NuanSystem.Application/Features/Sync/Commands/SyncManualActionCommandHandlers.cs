using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Commands;

public sealed class RetrySyncOutboxCommandHandler(
    ICompanyContext companyContext,
    ISyncOutboxRepository repository)
    : ICommandHandler<RetrySyncOutboxCommand, SyncOutboxActionResultDto>
{
    public async Task<Result<SyncOutboxActionResultDto>> Handle(RetrySyncOutboxCommand request, CancellationToken cancellationToken)
    {
        var companyId = SyncActionCompanyContext.GetActiveCompanyId(companyContext);
        var current = await repository.GetOutboxDetailAsync(companyId, request.Id, cancellationToken);

        if (current is null)
        {
            return Result<SyncOutboxActionResultDto>.Failure("Evento de sincronizacion no encontrado.");
        }

        if (current.Status != SyncEventStatus.Error)
        {
            return Result<SyncOutboxActionResultDto>.Failure("Solo se pueden reintentar eventos en estado Error.");
        }

        var result = await repository.RetryErrorAsync(
            companyId,
            request.Id,
            SyncActionReason.Normalize(request.Reason),
            request.AuditUserName,
            cancellationToken);

        return result is null
            ? Result<SyncOutboxActionResultDto>.Failure("El evento cambio de estado antes de reintentar.")
            : Result<SyncOutboxActionResultDto>.Success(result, "Evento de sincronizacion marcado para reintento.");
    }
}

public sealed class RetryDeadLetterSyncOutboxCommandHandler(
    ICompanyContext companyContext,
    ISyncOutboxRepository repository)
    : ICommandHandler<RetryDeadLetterSyncOutboxCommand, SyncOutboxActionResultDto>
{
    public async Task<Result<SyncOutboxActionResultDto>> Handle(RetryDeadLetterSyncOutboxCommand request, CancellationToken cancellationToken)
    {
        var reason = SyncActionReason.Normalize(request.Reason);
        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result<SyncOutboxActionResultDto>.Failure("El motivo es obligatorio para reintentar un DeadLetter.");
        }

        var companyId = SyncActionCompanyContext.GetActiveCompanyId(companyContext);
        var current = await repository.GetOutboxDetailAsync(companyId, request.Id, cancellationToken);

        if (current is null)
        {
            return Result<SyncOutboxActionResultDto>.Failure("Evento de sincronizacion no encontrado.");
        }

        if (current.Status != SyncEventStatus.DeadLetter)
        {
            return Result<SyncOutboxActionResultDto>.Failure("Solo se pueden reintentar eventos en estado DeadLetter.");
        }

        var result = await repository.RetryDeadLetterAsync(
            companyId,
            request.Id,
            reason,
            request.ResetAttemptCount,
            request.AuditUserName,
            cancellationToken);

        return result is null
            ? Result<SyncOutboxActionResultDto>.Failure("El evento cambio de estado antes de reintentar DeadLetter.")
            : Result<SyncOutboxActionResultDto>.Success(result, "Evento DeadLetter marcado para reintento.");
    }
}

public sealed class ReleaseExpiredSyncLockCommandHandler(
    ICompanyContext companyContext,
    ISyncOutboxRepository repository)
    : ICommandHandler<ReleaseExpiredSyncLockCommand, SyncOutboxActionResultDto>
{
    public async Task<Result<SyncOutboxActionResultDto>> Handle(ReleaseExpiredSyncLockCommand request, CancellationToken cancellationToken)
    {
        var companyId = SyncActionCompanyContext.GetActiveCompanyId(companyContext);
        var current = await repository.GetOutboxDetailAsync(companyId, request.Id, cancellationToken);

        if (current is null)
        {
            return Result<SyncOutboxActionResultDto>.Failure("Evento de sincronizacion no encontrado.");
        }

        if (current.Status is not (SyncEventStatus.InProcess or SyncEventStatus.Error) || current.LockExpiresAt is null)
        {
            return Result<SyncOutboxActionResultDto>.Failure("Solo se pueden liberar locks de eventos InProcess o Error con lock tecnico.");
        }

        if (current.LockExpiresAt.Value >= DateTime.UtcNow)
        {
            return Result<SyncOutboxActionResultDto>.Failure("No se puede liberar un lock vigente.");
        }

        var result = await repository.ReleaseExpiredLockAsync(
            companyId,
            request.Id,
            SyncActionReason.Normalize(request.Reason),
            request.AuditUserName,
            cancellationToken);

        return result is null
            ? Result<SyncOutboxActionResultDto>.Failure("El lock no esta vencido o el evento cambio de estado antes de liberarlo.")
            : Result<SyncOutboxActionResultDto>.Success(result, "Lock vencido liberado correctamente.");
    }
}

file static class SyncActionCompanyContext
{
    public static int GetActiveCompanyId(ICompanyContext companyContext)
    {
        if (!companyContext.HasActiveCompany || companyContext.CurrentCompany is null)
        {
            throw new InvalidOperationException("No hay empresa activa para ejecutar la accion de sincronizacion.");
        }

        return companyContext.CurrentCompany.CompanyId;
    }
}

file static class SyncActionReason
{
    public static string? Normalize(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return null;
        }

        reason = reason.Trim();
        return reason.Length <= 500 ? reason : reason[..500];
    }
}
