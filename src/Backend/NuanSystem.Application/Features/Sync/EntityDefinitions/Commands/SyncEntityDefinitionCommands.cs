using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;

namespace NuanSystem.Application.Features.Sync.EntityDefinitions.Commands;

public sealed record CreateSyncEntityDefinitionCommand(
    string Code,
    string Name,
    string? Description,
    int DefaultExecutionOrder,
    bool SupportsIncremental,
    bool SupportsInsert,
    bool SupportsUpdate,
    bool SupportsDeactivate,
    string? DefaultKeyField,
    string? DefaultModifiedAtField,
    bool IsActive,
    IReadOnlyCollection<int> DependencyDefinitionIds,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SyncEntityDefinitionDetailDto>;

public sealed record UpdateSyncEntityDefinitionCommand(
    int Id,
    string Name,
    string? Description,
    int DefaultExecutionOrder,
    bool SupportsIncremental,
    bool SupportsInsert,
    bool SupportsUpdate,
    bool SupportsDeactivate,
    string? DefaultKeyField,
    string? DefaultModifiedAtField,
    bool IsActive,
    IReadOnlyCollection<int> DependencyDefinitionIds,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SyncEntityDefinitionDetailDto>;

public sealed record DeleteSyncEntityDefinitionCommand(
    int Id,
    int? AuditUserId,
    string? AuditUserName) : ICommand<bool>;
