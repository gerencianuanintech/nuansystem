using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityUsers.Dtos;

namespace NuanSystem.Application.Features.SecurityUsers.Commands;

public sealed record UpdateUserCommand(
    int Id,
    string UserName,
    string? Email,
    string? PhoneNumber,
    bool EmailConfirmed,
    bool PhoneNumberConfirmed,
    string? FirstName,
    string? LastName,
    string DisplayName,
    string? Password,
    int? RoleId,
    bool IsActive,
    bool IsLocked,
    bool CanUseWeb,
    bool CanUseMobile,
    bool MustChangePassword,
    DateTime? LockoutEndAt,
    bool TwoFactorEnabled,
    string? ProfileImageUrl,
    byte[]? ProfileImage,
    string? ProfileImageContentType,
    string? ProfileImageFileName,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<UserAdminDto>;

