using System.ComponentModel;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.ViewModels.Sap;

public sealed class SapSyncProfilesViewModel(ISapSyncManagementClient client)
{
    public SapSyncProfileListFilter Filter { get; } = new();
    public IReadOnlyCollection<SapSyncProfileListItem> Profiles { get; private set; } = [];
    public int TotalCount { get; private set; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var result = await client.SearchProfilesAsync(Filter, cancellationToken);
        Profiles = result.Items;
        TotalCount = result.TotalCount;
    }

    public Task<SapSyncProfileValidationResult> ValidateAsync(long id, CancellationToken cancellationToken = default) => client.ValidateProfileAsync(id, cancellationToken);
    public Task<SapSyncProfileDetail> ActivateAsync(SapSyncProfileListItem item, CancellationToken cancellationToken = default) => client.ActivateProfileAsync(item.Id, item.RowVersion, cancellationToken);
    public Task<SapSyncProfileDetail> DeactivateAsync(SapSyncProfileListItem item, CancellationToken cancellationToken = default) => client.DeactivateProfileAsync(item.Id, item.RowVersion, cancellationToken);
    public Task DeleteAsync(SapSyncProfileListItem item, CancellationToken cancellationToken = default) => client.DeleteProfileAsync(item.Id, item.RowVersion, cancellationToken);
}

public sealed class SapSyncProfileEditViewModel(ISapSyncManagementClient client)
{
    public SapSyncProfileCatalog Catalog { get; private set; } = new();
    public SapSyncProfileEditorState State { get; private set; } = new();

    public async Task InitializeAsync(long? id, CancellationToken cancellationToken = default)
    {
        Catalog = await client.GetCatalogAsync(cancellationToken);
        State = id.HasValue
            ? SapSyncProfileEditorState.From(await client.GetProfileAsync(id.Value, cancellationToken), Catalog)
            : SapSyncProfileEditorState.Create(Catalog);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        var request = State.ToRequest();
        var saved = State.Id.HasValue
            ? await client.UpdateProfileAsync(State.Id.Value, new(request, State.RowVersion ?? []), cancellationToken)
            : await client.CreateProfileAsync(request, cancellationToken);
        State = SapSyncProfileEditorState.From(saved, Catalog);
    }
}

public sealed class SapSyncExecutionsViewModel(ISapSyncManagementClient client)
{
    public SapSyncExecutionFilter Filter { get; } = new();
    public IReadOnlyCollection<SapSyncExecutionListItem> Executions { get; private set; } = [];
    public int TotalCount { get; private set; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var result = await client.SearchExecutionsAsync(Filter, cancellationToken);
        Executions = result.Items;
        TotalCount = result.TotalCount;
    }
}

public sealed class SapSyncExecutionDetailViewModel(ISapSyncManagementClient client)
{
    public SapSyncExecutionDetail? Execution { get; private set; }
    public IReadOnlyCollection<SapSyncExecutionDetailItem> Details { get; private set; } = [];
    public SapSyncExecutionDetailFilter? Filter { get; private set; }

    public async Task LoadAsync(Guid executionUid, CancellationToken cancellationToken = default)
    {
        Execution = await client.GetExecutionAsync(executionUid, cancellationToken);
        Filter ??= new SapSyncExecutionDetailFilter { ExecutionUid = executionUid };
        var details = await client.SearchExecutionDetailsAsync(Filter, cancellationToken);
        Details = details.Items;
    }

    public async Task RetryAsync(string reason, CancellationToken cancellationToken = default)
    {
        if (Execution is null) return;
        await client.RetryExecutionAsync(Execution.ExecutionUid, Execution.RowVersion, reason, cancellationToken);
    }

    public async Task CancelAsync(CancellationToken cancellationToken = default)
    {
        if (Execution is null) return;
        await client.CancelExecutionAsync(Execution.ExecutionUid, Execution.RowVersion, cancellationToken);
    }

    public async Task ReleaseExpiredLockAsync(SapSyncExecutionDetailItem detail, string reason, CancellationToken cancellationToken = default) =>
        await client.ReleaseExpiredLockAsync(detail.Id, detail.RowVersion, reason, cancellationToken);
}

public sealed class SapSyncProfileEditorState
{
    public long? Id { get; set; }
    public int CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public byte[]? RowVersion { get; set; }
    public BindingList<SapSyncProfileEntityEditorRow> Entities { get; } = [];

    public static SapSyncProfileEditorState Create(SapSyncProfileCatalog catalog)
    {
        var state = new SapSyncProfileEditorState
        {
            CompanyId = catalog.Companies.FirstOrDefault()?.Id ?? 0
        };
        foreach (var capability in catalog.Entities.Where(item => item.IsActive && item.IsImplemented))
        {
            state.Entities.Add(SapSyncProfileEntityEditorRow.FromCapability(capability, catalog, state.Entities.Count + 1));
        }
        return state;
    }

    public static SapSyncProfileEditorState From(SapSyncProfileDetail detail, SapSyncProfileCatalog catalog)
    {
        var state = new SapSyncProfileEditorState
        {
            Id = detail.Id,
            CompanyId = detail.CompanyId,
            Code = detail.Code,
            Name = detail.Name,
            Description = detail.Description,
            IsActive = detail.IsActive,
            RowVersion = detail.RowVersion
        };
        foreach (var entity in detail.Entities.OrderBy(item => item.ExecutionOrder))
        {
            state.Entities.Add(SapSyncProfileEntityEditorRow.From(entity));
        }
        foreach (var capability in catalog.Entities.Where(item => item.IsActive && item.IsImplemented && state.Entities.All(row => !string.Equals(row.EntityCode, item.EntityCode, StringComparison.OrdinalIgnoreCase))))
        {
            state.Entities.Add(SapSyncProfileEntityEditorRow.FromCapability(capability, catalog, state.Entities.Count + 1));
        }
        return state;
    }

    public SaveSapSyncProfileRequest ToRequest() => new(
        CompanyId,
        Code.Trim(),
        Name.Trim(),
        string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
        Entities.Where(item => item.IsActive || item.Id.HasValue).Select(item => item.ToRequest()).ToArray());
}

public sealed class SapSyncProfileEntityEditorRow
{
    public long? Id { get; set; }
    public string EntityCode { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string Direction { get; set; } = "SapToErp";
    public string SyncMode { get; set; } = "Full";
    public int BatchSize { get; set; } = 100;
    public int MaxAttempts { get; set; } = 3;
    public int ExecutionOrder { get; set; } = 1;
    public bool ContinueOnError { get; set; }
    public int ExecutionTimeoutMinutes { get; set; } = 30;
    public bool IsActive { get; set; }
    public long? ScheduleId { get; set; }
    public string ScheduleType { get; set; } = "Manual";
    public int? IntervalMinutes { get; set; }
    public TimeSpan? ExecutionTime { get; set; }
    public string TimeZoneId { get; set; } = "America/Guayaquil";
    public bool PreventConcurrentExecutions { get; set; } = true;
    public bool ScheduleIsActive { get; set; }
    public DateTime? NextExecutionAtUtc { get; set; }
    public DateTime? LastExecutionAtUtc { get; set; }
    public byte[]? RowVersion { get; set; }
    public byte[]? ScheduleRowVersion { get; set; }

    public static SapSyncProfileEntityEditorRow FromCapability(SapSyncEntityCapability capability, SapSyncProfileCatalog catalog, int order) => new()
    {
        EntityCode = capability.EntityCode,
        EntityName = capability.DisplayName,
        Direction = capability.SupportsSapToErp ? "SapToErp" : "ErpToSap",
        SyncMode = capability.SupportsFull ? "Full" : "Incremental",
        ExecutionOrder = order,
        TimeZoneId = catalog.DefaultTimeZoneId
    };

    public static SapSyncProfileEntityEditorRow From(SapSyncProfileEntity entity) => new()
    {
        Id = entity.Id,
        EntityCode = entity.EntityCode,
        EntityName = entity.EntityCode,
        Direction = entity.Direction,
        SyncMode = entity.SyncMode,
        BatchSize = entity.BatchSize,
        MaxAttempts = entity.MaxAttempts,
        ExecutionOrder = entity.ExecutionOrder,
        ContinueOnError = entity.ContinueOnError,
        ExecutionTimeoutMinutes = entity.ExecutionTimeoutMinutes,
        IsActive = entity.IsActive,
        ScheduleId = entity.Schedule.Id,
        ScheduleType = entity.Schedule.ScheduleType,
        IntervalMinutes = entity.Schedule.IntervalMinutes,
        ExecutionTime = entity.Schedule.ExecutionTime,
        TimeZoneId = entity.Schedule.TimeZoneId,
        PreventConcurrentExecutions = entity.Schedule.PreventConcurrentExecutions,
        ScheduleIsActive = entity.Schedule.IsActive,
        NextExecutionAtUtc = entity.Schedule.NextExecutionAtUtc,
        LastExecutionAtUtc = entity.Schedule.LastExecutionAtUtc,
        RowVersion = entity.RowVersion,
        ScheduleRowVersion = entity.Schedule.RowVersion
    };

    public SaveSapSyncProfileEntityRequest ToRequest() => new(
        Id, EntityCode, Direction, SyncMode, BatchSize, MaxAttempts, ExecutionOrder, ContinueOnError,
        ExecutionTimeoutMinutes, IsActive,
        new(ScheduleId, ScheduleType,
            string.Equals(ScheduleType, "Interval", StringComparison.OrdinalIgnoreCase) ? IntervalMinutes : null,
            string.Equals(ScheduleType, "Daily", StringComparison.OrdinalIgnoreCase) ? ExecutionTime : null,
            TimeZoneId, PreventConcurrentExecutions, ScheduleIsActive, ScheduleRowVersion), RowVersion);
}

public static class SapSyncExecutionPolicy
{
    public static bool CanCancel(string? status) => Is(status, "Pending", "Running");
    public static bool CanRetry(string? status) => Is(status, "Failed", "CompletedWithErrors", "Cancelled");
    public static bool IsActive(string? status) => Is(status, "Pending", "Running", "Cancelling", "RetryScheduled");
    public static bool CanRelease(string? status) => Is(status, "Processing");
    private static bool Is(string? status, params string[] values) => values.Any(value => string.Equals(status, value, StringComparison.OrdinalIgnoreCase));
}
