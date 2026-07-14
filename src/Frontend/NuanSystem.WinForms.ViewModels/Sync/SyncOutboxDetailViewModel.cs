using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Sync;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Sync;

public sealed class SyncOutboxDetailViewModel : ViewModelBase
{
    private readonly ISyncMonitorClient client;
    private readonly bool hasRetryPermission;
    private readonly bool hasRetryDeadLetterPermission;
    private readonly bool hasReleaseLockPermission;
    private SyncOutboxDetail? detail;
    private IReadOnlyCollection<SyncOutboxTarget> targets = Array.Empty<SyncOutboxTarget>();
    private IReadOnlyCollection<SyncAuditItem> auditItems = Array.Empty<SyncAuditItem>();
    private bool isBusy;
    private bool isManualActionBusy;
    private string? manualActionError;
    private SyncManualActionResult? lastManualActionResult;

    public SyncOutboxDetailViewModel(ISyncMonitorClient client)
        : this(client, false, false, false)
    {
    }

    public SyncOutboxDetailViewModel(ISyncMonitorClient client, ApiSession session)
        : this(
            client,
            session.HasPermission(PermissionCodes.SyncOutboxRetry),
            session.HasPermission(PermissionCodes.SyncOutboxRetryDeadLetter),
            session.HasPermission(PermissionCodes.SyncOutboxReleaseLock))
    {
    }

    public SyncOutboxDetailViewModel(
        ISyncMonitorClient client,
        bool hasRetryPermission,
        bool hasRetryDeadLetterPermission,
        bool hasReleaseLockPermission)
    {
        this.client = client;
        this.hasRetryPermission = hasRetryPermission;
        this.hasRetryDeadLetterPermission = hasRetryDeadLetterPermission;
        this.hasReleaseLockPermission = hasReleaseLockPermission;
    }

    public SyncOutboxDetail? Detail
    {
        get => detail;
        private set => SetProperty(ref detail, value);
    }

    public IReadOnlyCollection<SyncOutboxTarget> Targets
    {
        get => targets;
        private set => SetProperty(ref targets, value);
    }

    public IReadOnlyCollection<SyncAuditItem> AuditItems
    {
        get => auditItems;
        private set => SetProperty(ref auditItems, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public bool IsManualActionBusy
    {
        get => isManualActionBusy;
        private set => SetProperty(ref isManualActionBusy, value);
    }

    public string? ManualActionError
    {
        get => manualActionError;
        private set => SetProperty(ref manualActionError, value);
    }

    public SyncManualActionResult? LastManualActionResult
    {
        get => lastManualActionResult;
        private set => SetProperty(ref lastManualActionResult, value);
    }

    public bool CanRetry => !IsBusy
        && !IsManualActionBusy
        && hasRetryPermission
        && Detail?.Status == SyncEventStatus.Error;

    public bool CanRetryDeadLetter => !IsBusy
        && !IsManualActionBusy
        && hasRetryDeadLetterPermission
        && Detail?.Status == SyncEventStatus.DeadLetter;

    public bool CanReleaseExpiredLock => !IsBusy
        && !IsManualActionBusy
        && hasReleaseLockPermission
        && Detail is { Status: SyncEventStatus.InProcess or SyncEventStatus.Error, LockExpiresAt: not null }
        && IsExpired(Detail.LockExpiresAt.Value);

    public bool HasRetryPermission => hasRetryPermission;

    public bool HasRetryDeadLetterPermission => hasRetryDeadLetterPermission;

    public bool HasReleaseLockPermission => hasReleaseLockPermission;

    public bool HasAnyManualActionPermission => HasRetryPermission || HasRetryDeadLetterPermission || HasReleaseLockPermission;

    public bool HasLock => Detail?.LockExpiresAt is not null;

    public bool IsLockExpired => Detail?.LockExpiresAt is { } lockExpiresAt && IsExpired(lockExpiresAt);

    public async Task LoadAsync(long id, bool includeAudit, CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        ManualActionError = null;
        try
        {
            Detail = await client.GetOutboxDetailAsync(id, cancellationToken);
            Targets = await client.GetOutboxTargetsAsync(id, cancellationToken);
            AuditItems = includeAudit
                ? await client.SearchAuditAsync(new SyncAuditFilter
                {
                    EventId = Detail.EventId,
                    EntityGlobalId = Detail.EntityGlobalId,
                    PageSize = 200
                }, cancellationToken)
                : Array.Empty<SyncAuditItem>();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<SyncManualActionResult> RetryAsync(bool includeAudit, CancellationToken cancellationToken = default)
    {
        if (!CanRetry || Detail is null)
        {
            throw new InvalidOperationException("No se puede reintentar este evento.");
        }

        return await RunManualActionAsync(
            () => client.RetryAsync(Detail.Id, cancellationToken),
            includeAudit,
            cancellationToken);
    }

    public async Task<SyncManualActionResult> RetryDeadLetterAsync(string reason, bool includeAudit, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            ManualActionError = "Debe ingresar un motivo para reintentar DeadLetter.";
            throw new InvalidOperationException(ManualActionError);
        }

        if (!CanRetryDeadLetter || Detail is null)
        {
            throw new InvalidOperationException("No se puede reintentar este DeadLetter.");
        }

        return await RunManualActionAsync(
            () => client.RetryDeadLetterAsync(Detail.Id, new RetryDeadLetterRequest(reason.Trim()), cancellationToken),
            includeAudit,
            cancellationToken);
    }

    public async Task<SyncManualActionResult> ReleaseExpiredLockAsync(bool includeAudit, CancellationToken cancellationToken = default)
    {
        if (Detail is null)
        {
            throw new InvalidOperationException("Seleccione un evento de sincronizacion.");
        }

        if (Detail.LockExpiresAt is null || !IsExpired(Detail.LockExpiresAt.Value))
        {
            throw new InvalidOperationException("El lock todavia esta vigente.");
        }

        if (!CanReleaseExpiredLock)
        {
            throw new InvalidOperationException("No se puede liberar el lock de este evento.");
        }

        return await RunManualActionAsync(
            () => client.ReleaseExpiredLockAsync(Detail.Id, new ReleaseExpiredLockRequest("Liberacion manual desde Monitor Sync WinForms."), cancellationToken),
            includeAudit,
            cancellationToken);
    }

    private async Task<SyncManualActionResult> RunManualActionAsync(
        Func<Task<SyncManualActionResult>> action,
        bool includeAudit,
        CancellationToken cancellationToken)
    {
        IsManualActionBusy = true;
        ManualActionError = null;
        try
        {
            var result = await action();
            LastManualActionResult = result;
            await LoadAsync(result.Id, includeAudit, cancellationToken);
            return result;
        }
        catch (Exception exception)
        {
            ManualActionError = exception.Message;
            throw;
        }
        finally
        {
            IsManualActionBusy = false;
        }
    }

    private static bool IsExpired(DateTime value)
    {
        var utcValue = value.Kind switch
        {
            DateTimeKind.Local => value.ToUniversalTime(),
            DateTimeKind.Utc => value,
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return utcValue < DateTime.UtcNow;
    }
}
