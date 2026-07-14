using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;

namespace NuanSystem.Application.Features.Sync.Configuration.Commands;

public sealed record CreateSyncProfileCommand(
    SaveSyncProfileRequest Request,
    int? AuditUserId,
    string? AuditUserName) : ICommand<int>;

public sealed record UpdateSyncProfileCommand(
    int Id,
    SaveSyncProfileRequest Request,
    int? AuditUserId,
    string? AuditUserName) : ICommand<bool>;

public sealed record ActivateSyncProfileCommand(
    int Id,
    int? AuditUserId,
    string? AuditUserName) : ICommand<bool>;

public sealed record DeactivateSyncProfileCommand(
    int Id,
    int? AuditUserId,
    string? AuditUserName) : ICommand<bool>;

public sealed record DeleteSyncProfileCommand(
    int Id,
    int? AuditUserId,
    string? AuditUserName) : ICommand<bool>;

public sealed record ValidateSyncProfileCommand(
    SaveSyncProfileRequest Request,
    int? ProfileId,
    int? UserId) : ICommand<SyncProfileValidationResultDto>;

public sealed record ValidatePersistedSyncProfileCommand(
    int Id,
    int? UserId) : ICommand<SyncProfileValidationResultDto>;
