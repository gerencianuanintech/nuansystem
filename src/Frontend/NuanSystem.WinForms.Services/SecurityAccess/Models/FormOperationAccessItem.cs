namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record FormOperationAccessItem(
    int OperationId,
    string Code,
    string Name,
    string? Description,
    string? ActionKey,
    string? RibbonPageName,
    string? RibbonGroupName,
    string? IconLarge,
    string? IconSmall,
    int DisplayOrder,
    bool IsAllowed);
