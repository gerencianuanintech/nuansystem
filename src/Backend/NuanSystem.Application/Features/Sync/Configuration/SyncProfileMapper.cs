using NuanSystem.Application.Features.Sync.Configuration.Dtos;

namespace NuanSystem.Application.Features.Sync.Configuration;

internal static class SyncProfileMapper
{
    public static SyncProfileApiDetailDto ToApiDetail(SyncProfileDetailDto profile)
    {
        var branchesById = profile.Branches.ToDictionary(branch => branch.Id);
        var branchLinksByEntity = profile.EntityBranches
            .GroupBy(link => link.EntityCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.OrdinalIgnoreCase);

        return new SyncProfileApiDetailDto
        {
            Id = profile.Id,
            Code = profile.Code,
            Name = profile.Name,
            Description = profile.Description,
            CompanyId = profile.CompanyId,
            CompanyName = profile.CompanyName,
            Direction = profile.Direction,
            ExecutionMode = profile.ExecutionMode,
            ConflictStrategy = profile.ConflictStrategy,
            BatchSize = profile.BatchSize,
            MaxRetries = profile.MaxRetries,
            RetryDelaySeconds = profile.RetryDelaySeconds,
            TimeoutMinutes = profile.TimeoutMinutes,
            IsActive = profile.IsActive,
            Branches = profile.Branches.Select(ToApiBranch).ToArray(),
            Entities = profile.Entities.Select(entity => ToApiEntity(entity, branchLinksByEntity, branchesById)).ToArray(),
            Schedule = profile.Schedule is null ? null : ToApiSchedule(profile.Schedule)
        };
    }

    public static SyncProfileAggregate ToAggregate(
        int id,
        SaveSyncProfileRequest request,
        int? auditUserId,
        string? auditUserName)
    {
        var branches = request.Branches
            .Select(branch => new SyncProfileBranchRecord(
                0,
                id,
                branch.BranchCompanyId,
                null,
                null,
                branch.BatchSize,
                branch.MaxRetries,
                branch.IsActive,
                null))
            .ToArray();

        var entities = request.Entities
            .Select(entity => new SyncProfileEntityRecord(
                0,
                id,
                entity.EntityCode.Trim(),
                string.IsNullOrWhiteSpace(entity.EntityName) ? entity.EntityCode.Trim() : entity.EntityName.Trim(),
                entity.ExecutionOrder,
                entity.SyncMode.Trim(),
                Clean(entity.KeyField),
                Clean(entity.ModifiedAtField),
                Clean(entity.VersionField),
                Clean(entity.ActiveField),
                entity.AllowInsert,
                entity.AllowUpdate,
                entity.AllowDeactivate,
                entity.ContinueOnError,
                entity.BatchSize,
                entity.IsActive))
            .ToArray();

        var entityBranches = request.Entities
            .SelectMany(entity => entity.Branches.Select(branch => new SyncProfileEntityBranchRecord(
                0,
                0,
                0,
                id,
                entity.EntityCode.Trim(),
                branch.BranchCompanyId,
                branch.IsEnabled,
                branch.BatchSize)))
            .ToArray();

        return new SyncProfileAggregate(
            id,
            request.CompanyId,
            request.Code.Trim().ToUpperInvariant(),
            request.Name.Trim(),
            Clean(request.Description),
            request.Direction.Trim(),
            request.ExecutionMode.Trim(),
            request.ConflictStrategy.Trim(),
            request.BatchSize,
            request.MaxRetries,
            request.RetryDelaySeconds,
            request.TimeoutMinutes,
            request.IsActive,
            auditUserId,
            Clean(auditUserName),
            branches,
            entities,
            entityBranches,
            request.Schedule is null
                ? null
                : new SyncScheduleRecord(
                    0,
                    id,
                    request.Schedule.ScheduleType.Trim(),
                    request.Schedule.IntervalMinutes,
                    request.Schedule.ExecutionTime,
                    string.IsNullOrWhiteSpace(request.Schedule.TimeZoneId) ? "America/Guayaquil" : request.Schedule.TimeZoneId.Trim(),
                    request.Schedule.PreventConcurrentExecutions,
                    request.Schedule.IsActive));
    }

    public static IReadOnlyCollection<SyncEntityCatalogItemDto> ToEntityCatalog()
    {
        return SyncMasterBranchEntityCodes.InitialCatalog
            .Select(item => new SyncEntityCatalogItemDto
            {
                Code = item.EntityCode,
                Name = item.DisplayName,
                Description = item.Notes,
                DefaultExecutionOrder = item.DefaultExecutionOrder,
                SupportsIncremental = item.SupportsIncremental,
                HasProducer = item.HasProducer,
                HasApplier = item.HasApplier,
                SupportsInsert = item.SupportsInsert,
                SupportsUpdate = item.SupportsUpdate,
                SupportsDeactivate = item.SupportsDeactivate,
                DefaultKeyField = item.DefaultKeyField,
                DefaultModifiedAtField = item.DefaultModifiedAtField,
                Dependencies = item.Dependencies ?? Array.Empty<string>()
            })
            .ToArray();
    }

    private static SyncProfileBranchDto ToApiBranch(SyncProfileBranchRecord branch)
    {
        return new SyncProfileBranchDto
        {
            Id = branch.Id,
            BranchCompanyId = branch.BranchCompanyId,
            BranchCompanyCode = branch.BranchCompanyCode ?? string.Empty,
            BranchCompanyName = branch.BranchCompanyName ?? string.Empty,
            BatchSize = branch.BatchSize,
            MaxRetries = branch.MaxRetries,
            IsActive = branch.IsActive,
            LastSynchronizationAt = branch.LastSynchronizationAt
        };
    }

    private static SyncProfileEntityDto ToApiEntity(
        SyncProfileEntityRecord entity,
        IReadOnlyDictionary<string, SyncProfileEntityBranchRecord[]> branchLinksByEntity,
        IReadOnlyDictionary<int, SyncProfileBranchRecord> branchesById)
    {
        var links = branchLinksByEntity.TryGetValue(entity.EntityCode, out var entityLinks)
            ? entityLinks
            : [];

        return new SyncProfileEntityDto
        {
            Id = entity.Id,
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
            IsActive = entity.IsActive,
            Branches = links.Select(link => new SyncEntityBranchDto
            {
                Id = link.Id,
                SyncProfileBranchId = link.SyncProfileBranchId,
                BranchCompanyId = branchesById.TryGetValue(link.SyncProfileBranchId, out var branch)
                    ? branch.BranchCompanyId
                    : link.BranchCompanyId,
                IsEnabled = link.IsEnabled,
                BatchSize = link.BatchSize
            }).ToArray()
        };
    }

    private static SyncScheduleDto ToApiSchedule(SyncScheduleRecord schedule)
    {
        return new SyncScheduleDto
        {
            Id = schedule.Id,
            ScheduleType = schedule.ScheduleType,
            IntervalMinutes = schedule.IntervalMinutes,
            ExecutionTime = schedule.ExecutionTime,
            TimeZoneId = schedule.TimeZoneId,
            PreventConcurrentExecutions = schedule.PreventConcurrentExecutions,
            IsActive = schedule.IsActive
        };
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
