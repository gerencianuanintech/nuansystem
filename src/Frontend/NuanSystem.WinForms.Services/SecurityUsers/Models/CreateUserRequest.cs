namespace NuanSystem.WinForms.Services.SecurityUsers.Models;

public sealed record CreateUserRequest(
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
    string? ProfileImageFileName);
