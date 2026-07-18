using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;

namespace NuanSystem.Application.Features.Sync.EntityDefinitions;

internal static class SyncEntityDefinitionMapper
{
    public static SyncEntityDefinitionListItemDto ToListItemDto(SyncEntityDefinitionRecord definition)
    {
        var technical = ResolveTechnicalCapabilities(definition.Code);
        return new SyncEntityDefinitionListItemDto
        {
            Id = definition.Id,
            Code = definition.Code,
            Name = definition.Name,
            Description = definition.Description,
            DefaultExecutionOrder = definition.DefaultExecutionOrder,
            SupportsIncremental = definition.SupportsIncremental,
            SupportsInsert = definition.SupportsInsert,
            SupportsUpdate = definition.SupportsUpdate,
            SupportsDeactivate = definition.SupportsDeactivate,
            DefaultKeyField = definition.DefaultKeyField,
            DefaultModifiedAtField = definition.DefaultModifiedAtField,
            IsSystem = definition.IsSystem,
            IsActive = definition.IsActive,
            DependencyCount = definition.DependencyCount,
            IsInUse = definition.IsInUse,
            HasProducer = technical.HasProducer,
            HasApplier = technical.HasApplier,
            CreatedByUserId = definition.CreatedByUserId,
            CreatedByUserName = definition.CreatedByUserName,
            CreatedAt = definition.CreatedAt,
            UpdatedByUserId = definition.UpdatedByUserId,
            UpdatedByUserName = definition.UpdatedByUserName,
            UpdatedAt = definition.UpdatedAt
        };
    }

    public static SyncEntityDefinitionDetailDto ToDetailDto(SyncEntityDefinitionDetailRecord detail)
    {
        var definition = detail.Definition;
        var technical = ResolveTechnicalCapabilities(definition.Code);
        return new SyncEntityDefinitionDetailDto
        {
            Id = definition.Id,
            Code = definition.Code,
            Name = definition.Name,
            Description = definition.Description,
            DefaultExecutionOrder = definition.DefaultExecutionOrder,
            SupportsIncremental = definition.SupportsIncremental,
            SupportsInsert = definition.SupportsInsert,
            SupportsUpdate = definition.SupportsUpdate,
            SupportsDeactivate = definition.SupportsDeactivate,
            DefaultKeyField = definition.DefaultKeyField,
            DefaultModifiedAtField = definition.DefaultModifiedAtField,
            IsSystem = definition.IsSystem,
            IsActive = definition.IsActive,
            HasProducer = technical.HasProducer,
            HasApplier = technical.HasApplier,
            CreatedByUserId = definition.CreatedByUserId,
            CreatedByUserName = definition.CreatedByUserName,
            CreatedAt = definition.CreatedAt,
            UpdatedByUserId = definition.UpdatedByUserId,
            UpdatedByUserName = definition.UpdatedByUserName,
            UpdatedAt = definition.UpdatedAt,
            Dependencies = detail.Dependencies
                .Select(dependency => new SyncEntityDefinitionDependencyDto(
                    dependency.Id,
                    dependency.DependencyDefinitionId,
                    dependency.DependencyCode,
                    dependency.DependencyName))
                .ToArray()
        };
    }

    public static SyncEntityDefinitionLookupDto ToLookupDto(SyncEntityDefinitionDetailRecord detail)
    {
        var definition = detail.Definition;
        var technical = ResolveTechnicalCapabilities(definition.Code);
        return new SyncEntityDefinitionLookupDto(
            definition.Id,
            definition.Code,
            definition.Name,
            definition.Description,
            definition.DefaultExecutionOrder,
            definition.SupportsIncremental,
            definition.SupportsInsert,
            definition.SupportsUpdate,
            definition.SupportsDeactivate,
            definition.DefaultKeyField,
            definition.DefaultModifiedAtField,
            definition.IsSystem,
            definition.IsActive,
            technical.HasProducer,
            technical.HasApplier,
            detail.Dependencies.Select(dependency => dependency.DependencyCode).ToArray());
    }

    public static SyncEntityCatalogItemDto ToProfileCatalogItem(SyncEntityDefinitionLookupDto definition)
    {
        return new SyncEntityCatalogItemDto
        {
            Id = definition.Id,
            Code = definition.Code,
            Name = definition.Name,
            Description = definition.Description ?? string.Empty,
            DefaultExecutionOrder = definition.DefaultExecutionOrder,
            SupportsIncremental = definition.SupportsIncremental,
            HasProducer = definition.HasProducer,
            HasApplier = definition.HasApplier,
            SupportsInsert = definition.SupportsInsert,
            SupportsUpdate = definition.SupportsUpdate,
            SupportsDeactivate = definition.SupportsDeactivate,
            DefaultKeyField = definition.DefaultKeyField,
            DefaultModifiedAtField = definition.DefaultModifiedAtField,
            IsSystem = definition.IsSystem,
            IsActive = definition.IsActive,
            Dependencies = definition.Dependencies
        };
    }

    private static (bool HasProducer, bool HasApplier) ResolveTechnicalCapabilities(string code)
    {
        var technical = SyncMasterBranchEntityCodes.Find(code);
        return technical is null
            ? (false, false)
            : (technical.HasProducer, technical.HasApplier);
    }
}
