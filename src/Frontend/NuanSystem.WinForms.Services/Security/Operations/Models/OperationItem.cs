namespace NuanSystem.WinForms.Services.Security.Operations.Models;

public sealed record OperationItem(
    int Id,
    string Code,
    string Name,
    string? Description,
    string RibbonPageName,
    string RibbonGroupName,
    string ActionKey,
    string? IconLarge,
    string? IconSmall,
    int DisplayOrder,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName,
    DateTime CreatedAt,
    int? UpdatedByUserId,
    string? UpdatedByUserName,
    DateTime? UpdatedAt,
    int? DeletedByUserId,
    string? DeletedByUserName,
    DateTime? DeletedAt);
