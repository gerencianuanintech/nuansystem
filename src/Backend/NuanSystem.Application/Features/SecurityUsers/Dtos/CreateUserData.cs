namespace NuanSystem.Application.Features.SecurityUsers.Dtos;

public sealed record CreateUserData(
    string UserName,
    string? Email,
    string? PhoneNumber,
    bool EmailConfirmed,
    bool PhoneNumberConfirmed,
    string? FirstName,
    string? LastName,
    string DisplayName,
    string PasswordHash,
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
    int? CreatedByUserId,
    string? CreatedByUserName);

