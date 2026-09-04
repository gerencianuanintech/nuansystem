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

    public SyncProfileListItem? ProfileSummary { get; private set; }

    public DateTimeOffset? LastSuccessfulScheduledExecutionAt { get; private set; }

    public BusinessPartnerSapCodePolicy? BusinessPartnerSapCodePolicy { get; private set; }

    public bool RequiresBusinessPartnerSapCodePolicy =>
        State.Entities.Any(entity => string.Equals(
            entity.EntityCode,
            "BusinessPartnerProposal",
            StringComparison.OrdinalIgnoreCase));

    public async Task InitializeAsync(int? id, CancellationToken cancellationToken = default)
    {
        Catalog = await client.GetCatalogAsync(cancellationToken);
        State = id.HasValue
            ? SyncProfileEditorState.FromDetail(await client.GetProfileAsync(id.Value, cancellationToken), Catalog)
            : SyncProfileEditorState.CreateNew(Catalog);
    }

    public async Task<BusinessPartnerSapCodePolicy?> LoadBusinessPartnerSapCodePolicyAsync(
        CancellationToken cancellationToken = default)
    {
        if (!RequiresBusinessPartnerSapCodePolicy)
        {
            BusinessPartnerSapCodePolicy = null;
            return null;
        }

        BusinessPartnerSapCodePolicy =
            await client.GetBusinessPartnerSapCodePolicyAsync(cancellationToken);
        return BusinessPartnerSapCodePolicy;
    }

    public async Task<BusinessPartnerSapCodePolicy> SaveBusinessPartnerSapCodePolicyAsync(
        SaveBusinessPartnerSapCodePolicyRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!RequiresBusinessPartnerSapCodePolicy)
        {
            throw new InvalidOperationException(
                "La política de códigos SAP sólo corresponde a perfiles con propuestas de socios.");
        }

        BusinessPartnerSapCodePolicy =
            await client.UpdateBusinessPartnerSapCodePolicyAsync(request, cancellationToken);
        return BusinessPartnerSapCodePolicy;
    }

    public async Task RefreshCatalogAsync(CancellationToken cancellationToken = default)
    {
        Catalog = await client.GetCatalogAsync(cancellationToken);

        foreach (var branch in State.Branches)
        {
            var catalogBranch = Catalog.BranchCompanies.FirstOrDefault(item => item.Id == branch.BranchCompanyId);
            if (catalogBranch is null)
            {
                continue;
            }

            branch.BranchCompanyCode = catalogBranch.Code;
            branch.BranchCompanyName = catalogBranch.Name;
            branch.BranchCode = catalogBranch.BranchCode;
            branch.DatabaseName = catalogBranch.DatabaseName;
        }
    }

    public async Task RefreshProfileSummaryAsync(
        bool includeExecutionSummary,
        CancellationToken cancellationToken = default)
    {
        if (State.Id <= 0)
        {
            ProfileSummary = null;
            LastSuccessfulScheduledExecutionAt = null;
            return;
        }

        var page = await client.SearchProfilesAsync(new SyncProfileListFilter
        {
            Search = State.Code,
            PageNumber = 1,
            PageSize = 50
        }, cancellationToken);
        ProfileSummary = page.Items.FirstOrDefault(profile => profile.Id == State.Id);

        if (!includeExecutionSummary)
        {
            LastSuccessfulScheduledExecutionAt = null;
            return;
        }

        var executions = await client.SearchExecutionsAsync(new SyncProfileExecutionFilter
        {
            ProfileId = State.Id,
            Status = "Completed",
            ExecutionType = "Scheduled",
            PageNumber = 1,
            PageSize = 1
        }, cancellationToken);
        LastSuccessfulScheduledExecutionAt = executions.Items.FirstOrDefault()?.FinishedAt;
    }

    public Task<SyncProfileValidationResult> ValidateAsync(CancellationToken cancellationToken = default)
    {
        State.ApplyDependencyOrder(Catalog.Entities);
        return client.ValidateProfileAsync(State.ToRequest(), cancellationToken);
    }

    public async Task<SyncProfileDetail> SaveAsync(CancellationToken cancellationToken = default)
    {
        State.ApplyDependencyOrder(Catalog.Entities);
        var saved = State.Id > 0
            ? await client.UpdateProfileAsync(State.Id, State.ToRequest(), cancellationToken)
            : await client.CreateProfileAsync(State.ToRequest(), cancellationToken);
        State = SyncProfileEditorState.FromDetail(saved, Catalog);
        ProfileSummary = null;
        LastSuccessfulScheduledExecutionAt = null;
        return saved;
    }
}

public sealed record SyncProfileDirectionOption(string Code, string Label)
{
    public override string ToString() => Label;
}

public static class SyncProfileDirectionPolicy
{
    public static IReadOnlyCollection<SyncProfileDirectionOption> Build(
        IReadOnlyCollection<LookupItem> directions)
    {
        ArgumentNullException.ThrowIfNull(directions);
        var available = directions
            .Select(direction => direction.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var result = new List<SyncProfileDirectionOption>(2);
        if (available.Contains("MasterToBranch"))
        {
            result.Add(new("MasterToBranch", "Central origen → sucursales destino"));
        }

        if (available.Contains("BranchToMaster"))
        {
            result.Add(new("BranchToMaster", "Sucursales origen → central destino"));
        }

        return result;
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

public static class SyncExecutionStatusPolicy
{
    public static bool IsActive(string? status)
    {
        return IsAny(status, "Pending", "Running", "Cancelling");
    }

    public static bool CanCancel(string? status)
    {
        return IsAny(status, "Pending", "Running");
    }

    public static bool CanRetry(string? status)
    {
        return IsAny(status, "Cancelled", "CompletedWithErrors", "Failed");
    }

    public static string StatusText(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return "Desconocido";
        }

        return status.Trim().ToUpperInvariant() switch
        {
            "PENDING" => "Pendiente",
            "RUNNING" => "En proceso",
            "CANCELLING" => "Cancelando",
            "CANCELLED" => "Cancelada",
            "COMPLETED" => "Completada",
            "COMPLETEDWITHERRORS" => "Completada con errores",
            "FAILED" => "Fallida",
            _ => status.Trim()
        };
    }

    public static string ExecutionTypeText(string? executionType)
    {
        if (string.IsNullOrWhiteSpace(executionType))
        {
            return "Manual";
        }

        return executionType.Trim().ToUpperInvariant() switch
        {
            "SCHEDULED" => "Programada",
            "RETRY" => "Reintento",
            "MANUAL" => "Manual",
            _ => executionType.Trim()
        };
    }

    private static bool IsAny(string? value, params string[] candidates)
    {
        return candidates.Any(candidate => string.Equals(value, candidate, StringComparison.OrdinalIgnoreCase));
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
            var catalogBranch = catalog.BranchCompanies.FirstOrDefault(item => item.Id == branch.BranchCompanyId);
            state.Branches.Add(new SyncProfileBranchEditorRow
            {
                BranchCompanyId = branch.BranchCompanyId,
                BranchCompanyCode = branch.BranchCompanyCode,
                BranchCompanyName = branch.BranchCompanyName,
                BranchCode = catalogBranch?.BranchCode,
                DatabaseName = catalogBranch?.DatabaseName,
                BatchSize = branch.BatchSize,
                MaxRetries = branch.MaxRetries,
                IsActive = branch.IsActive,
                LastSynchronizationAt = branch.LastSynchronizationAt
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
                    MatrixId = branch.Id,
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
        var activeBranchIds = Branches
            .Where(branch => branch.IsActive)
            .Select(branch => branch.BranchCompanyId)
            .ToHashSet();
        var entityBranches = EntityBranches
            .Where(branch => activeBranchIds.Contains(branch.BranchCompanyId))
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
        AddEntity(new SyncProfileEntityEditorRow
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
    }

    public bool AddEntity(SyncProfileEntityEditorRow source)
    {
        if (Entities.Any(entity => string.Equals(entity.EntityCode, source.EntityCode, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var entity = CloneEntity(source);
        Entities.Add(entity);
        RepositionEntity(entity, source.ExecutionOrder);

        foreach (var branch in Branches.Where(branch => branch.IsActive))
        {
            EntityBranches.Add(new SyncEntityBranchEditorRow
            {
                EntityCode = entity.EntityCode,
                BranchCompanyId = branch.BranchCompanyId,
                IsEnabled = true
            });
        }

        return true;
    }

    public bool UpdateEntity(SyncProfileEntityEditorRow source)
    {
        var target = Entities.FirstOrDefault(entity => string.Equals(
            entity.EntityCode,
            source.EntityCode,
            StringComparison.OrdinalIgnoreCase));
        if (target is null)
        {
            return false;
        }

        CopyEntity(source, target);
        RepositionEntity(target, source.ExecutionOrder);
        return true;
    }

    public bool RemoveEntity(string entityCode)
    {
        var entity = Entities.FirstOrDefault(item => string.Equals(
            item.EntityCode,
            entityCode,
            StringComparison.OrdinalIgnoreCase));
        if (entity is null)
        {
            return false;
        }

        Entities.Remove(entity);
        foreach (var relation in EntityBranches
                     .Where(item => string.Equals(item.EntityCode, entityCode, StringComparison.OrdinalIgnoreCase))
                     .ToArray())
        {
            EntityBranches.Remove(relation);
        }

        NormalizeEntityOrder();
        return true;
    }

    public bool MoveEntity(string entityCode, int offset)
    {
        var ordered = Entities.OrderBy(entity => entity.ExecutionOrder).ToList();
        var currentIndex = ordered.FindIndex(entity => string.Equals(
            entity.EntityCode,
            entityCode,
            StringComparison.OrdinalIgnoreCase));
        var targetIndex = currentIndex + offset;
        if (currentIndex < 0 || targetIndex < 0 || targetIndex >= ordered.Count)
        {
            return false;
        }

        var entity = ordered[currentIndex];
        ordered.RemoveAt(currentIndex);
        ordered.Insert(targetIndex, entity);
        AssignEntityOrder(ordered);
        return true;
    }

    public void ApplyDependencyOrder(IReadOnlyCollection<SyncEntityCatalogItem> catalog)
    {
        var entitiesByCode = Entities.ToDictionary(entity => entity.EntityCode, StringComparer.OrdinalIgnoreCase);
        var catalogByCode = catalog.ToDictionary(entity => entity.Code, StringComparer.OrdinalIgnoreCase);
        var visiting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ordered = new List<SyncProfileEntityEditorRow>(Entities.Count);

        void Visit(string code)
        {
            if (visited.Contains(code) || !entitiesByCode.TryGetValue(code, out var entity))
            {
                return;
            }

            if (!visiting.Add(code))
            {
                return;
            }

            if (catalogByCode.TryGetValue(code, out var definition))
            {
                foreach (var dependency in definition.Dependencies
                             .Where(entitiesByCode.ContainsKey)
                             .OrderBy(item => entitiesByCode[item].ExecutionOrder)
                             .ThenBy(item => item, StringComparer.OrdinalIgnoreCase))
                {
                    Visit(dependency);
                }
            }

            visiting.Remove(code);
            visited.Add(code);
            ordered.Add(entity);
        }

        foreach (var entity in Entities.OrderBy(item => item.ExecutionOrder).ThenBy(item => item.EntityCode))
        {
            Visit(entity.EntityCode);
        }

        AssignEntityOrder(ordered);
    }

    public bool SetEntityActive(string entityCode, bool isActive)
    {
        var entity = Entities.FirstOrDefault(item => string.Equals(
            item.EntityCode,
            entityCode,
            StringComparison.OrdinalIgnoreCase));
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = isActive;
        return true;
    }

    public SyncEntityBranchEditorRow? GetDistribution(string entityCode, int branchCompanyId)
    {
        return EntityBranches.FirstOrDefault(item =>
            item.BranchCompanyId == branchCompanyId
            && string.Equals(item.EntityCode, entityCode, StringComparison.OrdinalIgnoreCase));
    }

    public bool SetDistribution(
        string entityCode,
        int branchCompanyId,
        bool isEnabled,
        int? batchSize,
        bool updateBatch)
    {
        if (!Entities.Any(entity => string.Equals(
                entity.EntityCode,
                entityCode,
                StringComparison.OrdinalIgnoreCase))
            || !Branches.Any(branch => branch.BranchCompanyId == branchCompanyId && branch.IsActive)
            || (updateBatch
                && batchSize.HasValue
                && (batchSize.Value < 1 || batchSize.Value > 10000)))
        {
            return false;
        }

        var relation = GetDistribution(entityCode, branchCompanyId);
        if (relation is null)
        {
            relation = new SyncEntityBranchEditorRow
            {
                EntityCode = entityCode,
                BranchCompanyId = branchCompanyId
            };
            EntityBranches.Add(relation);
        }

        relation.IsEnabled = isEnabled;
        if (updateBatch)
        {
            relation.BatchSize = batchSize;
        }

        return true;
    }

    public void SetAllDistributionsEnabled(bool isEnabled)
    {
        foreach (var entity in Entities)
        {
            foreach (var branch in Branches.Where(branch => branch.IsActive))
            {
                var current = GetDistribution(entity.EntityCode, branch.BranchCompanyId);
                SetDistribution(
                    entity.EntityCode,
                    branch.BranchCompanyId,
                    isEnabled,
                    current?.BatchSize,
                    updateBatch: false);
            }
        }
    }

    public int EffectiveBatchSize(string entityCode, int branchCompanyId)
    {
        return GetDistribution(entityCode, branchCompanyId)?.BatchSize
               ?? Entities.FirstOrDefault(entity => string.Equals(
                   entity.EntityCode,
                   entityCode,
                   StringComparison.OrdinalIgnoreCase))?.BatchSize
               ?? Branches.FirstOrDefault(branch => branch.BranchCompanyId == branchCompanyId)?.BatchSize
               ?? BatchSize;
    }

    public bool AddBranch(
        CompanyLookupItem branch,
        int? batchSize = null,
        int? maxRetries = null,
        bool isActive = true)
    {
        if (Branches.Any(item => item.BranchCompanyId == branch.Id))
        {
            return false;
        }

        Branches.Add(new SyncProfileBranchEditorRow
        {
            BranchCompanyId = branch.Id,
            BranchCompanyCode = branch.Code,
            BranchCompanyName = branch.Name,
            BranchCode = branch.BranchCode,
            DatabaseName = branch.DatabaseName,
            BatchSize = batchSize,
            MaxRetries = maxRetries,
            IsActive = isActive
        });

        foreach (var entity in Entities.Where(_ => isActive))
        {
            EntityBranches.Add(new SyncEntityBranchEditorRow
            {
                EntityCode = entity.EntityCode,
                BranchCompanyId = branch.Id,
                IsEnabled = true
            });
        }

        return true;
    }

    public bool UpdateBranch(int branchCompanyId, int? batchSize, int? maxRetries, bool isActive)
    {
        var branch = Branches.FirstOrDefault(item => item.BranchCompanyId == branchCompanyId);
        if (branch is null)
        {
            return false;
        }

        branch.BatchSize = batchSize;
        branch.MaxRetries = maxRetries;
        return SetBranchActive(branchCompanyId, isActive);
    }

    public bool SetBranchActive(int branchCompanyId, bool isActive)
    {
        var branch = Branches.FirstOrDefault(item => item.BranchCompanyId == branchCompanyId);
        if (branch is null)
        {
            return false;
        }

        if (branch.IsActive == isActive)
        {
            return true;
        }

        branch.IsActive = isActive;
        if (!isActive)
        {
            foreach (var relation in EntityBranches
                         .Where(item => item.BranchCompanyId == branchCompanyId)
                         .ToArray())
            {
                EntityBranches.Remove(relation);
            }

            return true;
        }

        foreach (var entity in Entities)
        {
            if (EntityBranches.Any(item =>
                    item.BranchCompanyId == branchCompanyId
                    && string.Equals(item.EntityCode, entity.EntityCode, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            EntityBranches.Add(new SyncEntityBranchEditorRow
            {
                EntityCode = entity.EntityCode,
                BranchCompanyId = branchCompanyId,
                IsEnabled = true
            });
        }

        return true;
    }

    public bool RemoveBranch(int branchCompanyId)
    {
        var branch = Branches.FirstOrDefault(item => item.BranchCompanyId == branchCompanyId);
        if (branch is null)
        {
            return false;
        }

        Branches.Remove(branch);
        foreach (var relation in EntityBranches
                     .Where(item => item.BranchCompanyId == branchCompanyId)
                     .ToArray())
        {
            EntityBranches.Remove(relation);
        }

        return true;
    }

    private void RepositionEntity(SyncProfileEntityEditorRow entity, int desiredOrder)
    {
        var ordered = Entities
            .Where(item => !ReferenceEquals(item, entity))
            .OrderBy(item => item.ExecutionOrder)
            .ToList();
        var targetIndex = Math.Clamp(desiredOrder - 1, 0, ordered.Count);
        ordered.Insert(targetIndex, entity);
        AssignEntityOrder(ordered);
    }

    private void NormalizeEntityOrder()
    {
        AssignEntityOrder(Entities.OrderBy(entity => entity.ExecutionOrder).ToArray());
    }

    private static void AssignEntityOrder(IReadOnlyList<SyncProfileEntityEditorRow> ordered)
    {
        for (var index = 0; index < ordered.Count; index++)
        {
            ordered[index].ExecutionOrder = index + 1;
        }
    }

    private static SyncProfileEntityEditorRow CloneEntity(SyncProfileEntityEditorRow source)
    {
        var target = new SyncProfileEntityEditorRow();
        CopyEntity(source, target);
        return target;
    }

    private static void CopyEntity(SyncProfileEntityEditorRow source, SyncProfileEntityEditorRow target)
    {
        target.EntityCode = source.EntityCode.Trim();
        target.EntityName = source.EntityName.Trim();
        target.ExecutionOrder = source.ExecutionOrder;
        target.SyncMode = source.SyncMode;
        target.KeyField = source.KeyField;
        target.ModifiedAtField = source.ModifiedAtField;
        target.VersionField = source.VersionField;
        target.ActiveField = source.ActiveField;
        target.AllowInsert = source.AllowInsert;
        target.AllowUpdate = source.AllowUpdate;
        target.AllowDeactivate = source.AllowDeactivate;
        target.ContinueOnError = source.ContinueOnError;
        target.BatchSize = source.BatchSize;
        target.IsActive = source.IsActive;
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
    public string? BranchCode { get; set; }
    public string? DatabaseName { get; set; }
    public int? BatchSize { get; set; }
    public int? MaxRetries { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastSynchronizationAt { get; set; }
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
    public int MatrixId { get; set; }
    public string EntityCode { get; set; } = string.Empty;
    public int BranchCompanyId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public int? BatchSize { get; set; }
}

public enum SyncProfileEditorSection
{
    General,
    Branches,
    Entities,
    Distribution,
    Schedule
}

public static class SyncValidationSectionResolver
{
    private static readonly HashSet<string> DistributionCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "SyncBranchWithoutEnabledEntity",
        "SyncBranchFewEntities",
        "SyncEntityWithoutEnabledBranch"
    };

    public static SyncProfileEditorSection Resolve(SyncValidationMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message.Code.Contains("Matrix", StringComparison.OrdinalIgnoreCase)
            || DistributionCodes.Contains(message.Code))
        {
            return SyncProfileEditorSection.Distribution;
        }

        if (message.Code.Contains("Schedule", StringComparison.OrdinalIgnoreCase)
            || IsAnyField(message.Field, "Schedule", "ScheduleType", "IntervalMinutes", "ExecutionTime", "TimeZoneId"))
        {
            return SyncProfileEditorSection.Schedule;
        }

        if (message.Code.Contains("Branch", StringComparison.OrdinalIgnoreCase)
            || IsAnyField(message.Field, "Branches", "BranchCompanyId"))
        {
            return SyncProfileEditorSection.Branches;
        }

        if (message.Code.Contains("Entity", StringComparison.OrdinalIgnoreCase)
            || message.Code.Contains("TechnicalField", StringComparison.OrdinalIgnoreCase)
            || IsAnyField(
                message.Field,
                "Entities",
                "EntityCode",
                "ExecutionOrder",
                "SyncMode",
                "KeyField",
                "ModifiedAtField",
                "VersionField",
                "ActiveField",
                "AllowInsert",
                "AllowUpdate",
                "AllowDeactivate",
                "ContinueOnError"))
        {
            return SyncProfileEditorSection.Entities;
        }

        return SyncProfileEditorSection.General;
    }

    public static string DisplayName(SyncProfileEditorSection section)
    {
        return section switch
        {
            SyncProfileEditorSection.Branches => "Sucursales",
            SyncProfileEditorSection.Entities => "Entidades",
            SyncProfileEditorSection.Distribution => "Distribución",
            SyncProfileEditorSection.Schedule => "Programación",
            _ => "General"
        };
    }

    private static bool IsAnyField(string? field, params string[] candidates)
    {
        return candidates.Any(candidate => string.Equals(field, candidate, StringComparison.OrdinalIgnoreCase));
    }
}

public sealed class SyncScheduleEditorState
{
    public string ScheduleType { get; set; } = "Manual";
    public int? IntervalMinutes { get; set; }
    public TimeSpan? ExecutionTime { get; set; }
    public string TimeZoneId { get; set; } = "America/Guayaquil";
    public bool PreventConcurrentExecutions { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public void Configure(
        string scheduleType,
        int? intervalMinutes,
        TimeSpan? executionTime,
        string? timeZoneId,
        bool preventConcurrentExecutions,
        bool isActive)
    {
        ScheduleType = scheduleType.Trim();
        IntervalMinutes = string.Equals(ScheduleType, "Interval", StringComparison.OrdinalIgnoreCase)
            ? intervalMinutes
            : null;
        ExecutionTime = string.Equals(ScheduleType, "Daily", StringComparison.OrdinalIgnoreCase)
            ? executionTime
            : null;
        TimeZoneId = string.IsNullOrWhiteSpace(timeZoneId)
            ? "America/Guayaquil"
            : timeZoneId.Trim();
        PreventConcurrentExecutions = preventConcurrentExecutions;
        IsActive = isActive;
    }

    public string EffectiveFrequencyText()
    {
        return ScheduleType switch
        {
            "Interval" when IntervalMinutes.HasValue => $"Cada {IntervalMinutes.Value} minutos",
            "Daily" when ExecutionTime.HasValue => $"Diaria a las {ExecutionTime.Value:hh\\:mm}",
            _ => "Ejecución manual"
        };
    }

    public SaveSyncScheduleRequest ToRequest()
    {
        var isInterval = string.Equals(ScheduleType, "Interval", StringComparison.OrdinalIgnoreCase);
        var isDaily = string.Equals(ScheduleType, "Daily", StringComparison.OrdinalIgnoreCase);

        return new SaveSyncScheduleRequest
        {
            ScheduleType = ScheduleType,
            IntervalMinutes = isInterval ? IntervalMinutes : null,
            ExecutionTime = isDaily ? ExecutionTime : null,
            TimeZoneId = string.IsNullOrWhiteSpace(TimeZoneId) ? "America/Guayaquil" : TimeZoneId.Trim(),
            PreventConcurrentExecutions = PreventConcurrentExecutions,
            IsActive = IsActive
        };
    }
}
