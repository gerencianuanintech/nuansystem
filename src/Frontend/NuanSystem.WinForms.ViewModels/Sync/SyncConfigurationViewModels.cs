using System.ComponentModel;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.ViewModels.Sync;

public sealed class SyncProfilesViewModel(ISyncConfigurationClient client)
{
    public SyncProfileListFilter Filter { get; } = new();

    public IReadOnlyCollection<SyncProfileListItem> Profiles { get; private set; } = Array.Empty<SyncProfileListItem>();

    public int TotalCount { get; private set; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var page = await client.SearchProfilesAsync(Filter, cancellationToken);
        Profiles = page.Items;
        TotalCount = page.TotalCount;
    }

    public Task<SyncConfigurationCatalog> LoadCatalogAsync(CancellationToken cancellationToken = default)
    {
        return client.GetCatalogAsync(cancellationToken);
    }

    public Task<SyncProfileDetail> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.GetProfileAsync(id, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteProfileAsync(id, cancellationToken);
    }

    public Task<SyncProfileDetail> ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.ActivateProfileAsync(id, cancellationToken);
    }

    public Task<SyncProfileDetail> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.DeactivateProfileAsync(id, cancellationToken);
    }

    public Task<SyncProfileValidationResult> ValidatePersistedAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.ValidatePersistedProfileAsync(id, cancellationToken);
    }

    public Task<CreateSyncProfileExecutionResult> ExecuteAsync(int id, ExecuteSyncProfileRequest request, CancellationToken cancellationToken = default)
    {
        return client.ExecuteProfileAsync(id, request, cancellationToken);
    }
}

public sealed class SyncProfileEditViewModel(ISyncConfigurationClient client)
{
    public SyncConfigurationCatalog Catalog { get; private set; } = new();

    public SyncProfileEditorState State { get; private set; } = SyncProfileEditorState.CreateNew();

    public async Task InitializeAsync(int? id, CancellationToken cancellationToken = default)
    {
        Catalog = await client.GetCatalogAsync(cancellationToken);
        State = id.HasValue
            ? SyncProfileEditorState.FromDetail(await client.GetProfileAsync(id.Value, cancellationToken), Catalog)
            : SyncProfileEditorState.CreateNew(Catalog);
    }

    public Task<SyncProfileValidationResult> ValidateAsync(CancellationToken cancellationToken = default)
    {
        return client.ValidateProfileAsync(State.ToRequest(), cancellationToken);
    }

    public Task<SyncProfileDetail> SaveAsync(CancellationToken cancellationToken = default)
    {
        return State.Id > 0
            ? client.UpdateProfileAsync(State.Id, State.ToRequest(), cancellationToken)
            : client.CreateProfileAsync(State.ToRequest(), cancellationToken);
    }
}

public sealed class SyncExecutionsViewModel(ISyncConfigurationClient client)
{
    public SyncProfileExecutionFilter Filter { get; } = new();

    public IReadOnlyCollection<SyncProfileExecutionListItem> Executions { get; private set; } = Array.Empty<SyncProfileExecutionListItem>();

    public int TotalCount { get; private set; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var page = await client.SearchExecutionsAsync(Filter, cancellationToken);
        Executions = page.Items;
        TotalCount = page.TotalCount;
    }

    public Task<SyncProfileExecutionDetail> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.GetExecutionAsync(id, cancellationToken);
    }

    public Task<CancelSyncProfileExecutionResult> CancelAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.CancelExecutionAsync(id, cancellationToken);
    }

    public Task<RetrySyncProfileExecutionResult> RetryAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.RetryExecutionAsync(id, cancellationToken);
    }
}

public sealed class SyncProfileExecutionDetailViewModel(ISyncConfigurationClient client)
{
    public SyncProfileExecutionDetail? Detail { get; private set; }

    public async Task LoadAsync(int id, CancellationToken cancellationToken = default)
    {
        Detail = await client.GetExecutionAsync(id, cancellationToken);
    }

    public Task<CancelSyncProfileExecutionResult> CancelAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.CancelExecutionAsync(id, cancellationToken);
    }

    public Task<RetrySyncProfileExecutionResult> RetryAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.RetryExecutionAsync(id, cancellationToken);
    }
}

public sealed class SyncProfileEditorState
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CompanyId { get; set; }
    public string Direction { get; set; } = "MasterToBranch";
    public string ExecutionMode { get; set; } = "Incremental";
    public string ConflictStrategy { get; set; } = "MasterWins";
    public int BatchSize { get; set; } = 500;
    public int MaxRetries { get; set; } = 3;
    public int RetryDelaySeconds { get; set; } = 30;
    public int TimeoutMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    public BindingList<SyncProfileBranchEditorRow> Branches { get; } = [];
    public BindingList<SyncProfileEntityEditorRow> Entities { get; } = [];
    public BindingList<SyncEntityBranchEditorRow> EntityBranches { get; } = [];
    public SyncScheduleEditorState Schedule { get; } = new();

    public static SyncProfileEditorState CreateNew(SyncConfigurationCatalog? catalog = null)
    {
        var state = new SyncProfileEditorState
        {
            Direction = catalog?.Directions.FirstOrDefault()?.Code ?? "MasterToBranch",
            ExecutionMode = catalog?.ExecutionModes.FirstOrDefault()?.Code ?? "Incremental",
            ConflictStrategy = catalog?.ConflictStrategies.FirstOrDefault()?.Code ?? "MasterWins",
            CompanyId = catalog?.MasterCompanies.FirstOrDefault(company => company.IsActive)?.Id ?? 0
        };

        state.Schedule.ScheduleType = catalog?.ScheduleTypes.FirstOrDefault()?.Code ?? "Manual";
        state.Schedule.TimeZoneId = catalog?.DefaultTimeZoneId ?? "America/Guayaquil";
        return state;
    }

    public static SyncProfileEditorState FromDetail(SyncProfileDetail detail, SyncConfigurationCatalog catalog)
    {
        var state = CreateNew(catalog);
        state.Id = detail.Id;
        state.Code = detail.Code;
        state.Name = detail.Name;
        state.Description = detail.Description;
        state.CompanyId = detail.CompanyId;
        state.Direction = detail.Direction;
        state.ExecutionMode = detail.ExecutionMode;
        state.ConflictStrategy = detail.ConflictStrategy;
        state.BatchSize = detail.BatchSize;
        state.MaxRetries = detail.MaxRetries;
        state.RetryDelaySeconds = detail.RetryDelaySeconds;
        state.TimeoutMinutes = detail.TimeoutMinutes;
        state.IsActive = detail.IsActive;

        foreach (var branch in detail.Branches)
        {
            state.Branches.Add(new SyncProfileBranchEditorRow
            {
                BranchCompanyId = branch.BranchCompanyId,
                BranchCompanyCode = branch.BranchCompanyCode,
                BranchCompanyName = branch.BranchCompanyName,
                BatchSize = branch.BatchSize,
                MaxRetries = branch.MaxRetries,
                IsActive = branch.IsActive
            });
        }

        foreach (var entity in detail.Entities.OrderBy(entity => entity.ExecutionOrder))
        {
            state.Entities.Add(new SyncProfileEntityEditorRow
            {
                EntityCode = entity.EntityCode,
                EntityName = entity.EntityName,
                ExecutionOrder = entity.ExecutionOrder,
                SyncMode = entity.SyncMode,
                KeyField = entity.KeyField,
                ModifiedAtField = entity.ModifiedAtField,
                VersionField = entity.VersionField,
                ActiveField = entity.ActiveField,
                AllowInsert = entity.AllowInsert,
                AllowUpdate = entity.AllowUpdate,
                AllowDeactivate = entity.AllowDeactivate,
                ContinueOnError = entity.ContinueOnError,
                BatchSize = entity.BatchSize,
                IsActive = entity.IsActive
            });

            foreach (var branch in entity.Branches)
            {
                state.EntityBranches.Add(new SyncEntityBranchEditorRow
                {
                    EntityCode = entity.EntityCode,
                    BranchCompanyId = branch.BranchCompanyId,
                    IsEnabled = branch.IsEnabled,
                    BatchSize = branch.BatchSize
                });
            }
        }

        if (detail.Schedule is not null)
        {
            state.Schedule.ScheduleType = detail.Schedule.ScheduleType;
            state.Schedule.IntervalMinutes = detail.Schedule.IntervalMinutes;
            state.Schedule.ExecutionTime = detail.Schedule.ExecutionTime;
            state.Schedule.TimeZoneId = detail.Schedule.TimeZoneId;
            state.Schedule.PreventConcurrentExecutions = detail.Schedule.PreventConcurrentExecutions;
            state.Schedule.IsActive = detail.Schedule.IsActive;
        }

        return state;
    }

    public SaveSyncProfileRequest ToRequest()
    {
        var entityBranches = EntityBranches
            .GroupBy(branch => branch.EntityCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.OrdinalIgnoreCase);

        return new SaveSyncProfileRequest
        {
            Code = Code.Trim(),
            Name = Name.Trim(),
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
            CompanyId = CompanyId,
            Direction = Direction,
            ExecutionMode = ExecutionMode,
            ConflictStrategy = ConflictStrategy,
            BatchSize = BatchSize,
            MaxRetries = MaxRetries,
            RetryDelaySeconds = RetryDelaySeconds,
            TimeoutMinutes = TimeoutMinutes,
            IsActive = IsActive,
            Branches = Branches.Select(branch => new SaveSyncProfileBranchRequest
            {
                BranchCompanyId = branch.BranchCompanyId,
                BatchSize = branch.BatchSize,
                MaxRetries = branch.MaxRetries,
                IsActive = branch.IsActive
            }).ToArray(),
            Entities = Entities.Select(entity => new SaveSyncProfileEntityRequest
            {
                EntityCode = entity.EntityCode,
                EntityName = entity.EntityName,
                ExecutionOrder = entity.ExecutionOrder,
                SyncMode = entity.SyncMode,
                KeyField = NullIfWhiteSpace(entity.KeyField),
                ModifiedAtField = NullIfWhiteSpace(entity.ModifiedAtField),
                VersionField = NullIfWhiteSpace(entity.VersionField),
                ActiveField = NullIfWhiteSpace(entity.ActiveField),
                AllowInsert = entity.AllowInsert,
                AllowUpdate = entity.AllowUpdate,
                AllowDeactivate = entity.AllowDeactivate,
                ContinueOnError = entity.ContinueOnError,
                BatchSize = entity.BatchSize,
                IsActive = entity.IsActive,
                Branches = entityBranches.TryGetValue(entity.EntityCode, out var branches)
                    ? branches.Select(branch => new SaveSyncEntityBranchRequest
                    {
                        BranchCompanyId = branch.BranchCompanyId,
                        IsEnabled = branch.IsEnabled,
                        BatchSize = branch.BatchSize
                    }).ToArray()
                    : Array.Empty<SaveSyncEntityBranchRequest>()
            }).ToArray(),
            Schedule = Schedule.ToRequest()
        };
    }

    public void AddEntityFromCatalog(SyncEntityCatalogItem catalogItem)
    {
        if (Entities.Any(entity => string.Equals(entity.EntityCode, catalogItem.Code, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        Entities.Add(new SyncProfileEntityEditorRow
        {
            EntityCode = catalogItem.Code,
            EntityName = catalogItem.Name,
            ExecutionOrder = catalogItem.DefaultExecutionOrder,
            SyncMode = catalogItem.SupportsIncremental ? "Incremental" : "Full",
            KeyField = catalogItem.DefaultKeyField,
            ModifiedAtField = catalogItem.DefaultModifiedAtField,
            AllowInsert = catalogItem.SupportsInsert,
            AllowUpdate = catalogItem.SupportsUpdate,
            AllowDeactivate = catalogItem.SupportsDeactivate,
            IsActive = true
        });

        foreach (var branch in Branches)
        {
            EntityBranches.Add(new SyncEntityBranchEditorRow
            {
                EntityCode = catalogItem.Code,
                BranchCompanyId = branch.BranchCompanyId,
                IsEnabled = true
            });
        }
    }

    public void AddBranch(CompanyLookupItem branch)
    {
        if (Branches.Any(item => item.BranchCompanyId == branch.Id))
        {
            return;
        }

        Branches.Add(new SyncProfileBranchEditorRow
        {
            BranchCompanyId = branch.Id,
            BranchCompanyCode = branch.Code,
            BranchCompanyName = branch.Name,
            IsActive = true
        });

        foreach (var entity in Entities)
        {
            EntityBranches.Add(new SyncEntityBranchEditorRow
            {
                EntityCode = entity.EntityCode,
                BranchCompanyId = branch.Id,
                IsEnabled = true
            });
        }
    }

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

public sealed class SyncProfileBranchEditorRow
{
    public int BranchCompanyId { get; set; }
    public string BranchCompanyCode { get; set; } = string.Empty;
    public string BranchCompanyName { get; set; } = string.Empty;
    public int? BatchSize { get; set; }
    public int? MaxRetries { get; set; }
    public bool IsActive { get; set; } = true;
    public string BranchDisplay => $"{BranchCompanyCode} - {BranchCompanyName}";
}

public sealed class SyncProfileEntityEditorRow
{
    public string EntityCode { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int ExecutionOrder { get; set; }
    public string SyncMode { get; set; } = "Incremental";
    public string? KeyField { get; set; }
    public string? ModifiedAtField { get; set; }
    public string? VersionField { get; set; }
    public string? ActiveField { get; set; }
    public bool AllowInsert { get; set; } = true;
    public bool AllowUpdate { get; set; } = true;
    public bool AllowDeactivate { get; set; } = true;
    public bool ContinueOnError { get; set; }
    public int? BatchSize { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class SyncEntityBranchEditorRow
{
    public string EntityCode { get; set; } = string.Empty;
    public int BranchCompanyId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public int? BatchSize { get; set; }
}

public sealed class SyncScheduleEditorState
{
    public string ScheduleType { get; set; } = "Manual";
    public int? IntervalMinutes { get; set; }
    public TimeSpan? ExecutionTime { get; set; }
    public string TimeZoneId { get; set; } = "America/Guayaquil";
    public bool PreventConcurrentExecutions { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public SaveSyncScheduleRequest ToRequest()
    {
        var isInterval = string.Equals(ScheduleType, "Interval", StringComparison.OrdinalIgnoreCase);
        var isDaily = string.Equals(ScheduleType, "Daily", StringComparison.OrdinalIgnoreCase);

        return new SaveSyncScheduleRequest
        {
            ScheduleType = ScheduleType,
            IntervalMinutes = isInterval ? IntervalMinutes : null,
            ExecutionTime = isDaily ? ExecutionTime : null,
            TimeZoneId = TimeZoneId,
            PreventConcurrentExecutions = PreventConcurrentExecutions,
            IsActive = IsActive
        };
    }
}
