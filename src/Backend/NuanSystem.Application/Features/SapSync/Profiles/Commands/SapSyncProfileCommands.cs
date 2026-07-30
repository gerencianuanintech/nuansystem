using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.SapSync.Profiles.Commands;

public sealed record CreateSapSyncProfileCommand(
    SaveSapSyncProfileRequest Profile,
    int UserId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SapSyncProfileWriteDto>;

public sealed record UpdateSapSyncProfileCommand(
    long Id,
    UpdateSapSyncProfileRequest Request,
    int UserId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SapSyncProfileWriteDto>;

public sealed record DeleteSapSyncProfileCommand(
    long Id,
    byte[] RowVersion,
    int UserId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<bool>;

public sealed record ValidateSapSyncProfileCommand(
    long Id,
    int UserId) : ICommand<SapSyncProfileValidationResultDto>;

public sealed record ActivateSapSyncProfileCommand(
    long Id,
    byte[] RowVersion,
    int UserId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SapSyncProfileWriteDto>;

public sealed record DeactivateSapSyncProfileCommand(
    long Id,
    byte[] RowVersion,
    int UserId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SapSyncProfileWriteDto>;
