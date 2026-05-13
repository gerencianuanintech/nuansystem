namespace NuanSystem.WinForms.Services.SecurityOperations.Models;

public sealed record SaveSecurityOperationRequest(
    string Code,
    string Name,
    string? Description,
    string RibbonPageName,
    string RibbonGroupName,
    string ActionKey,
    string? IconLarge,
    string? IconSmall,
    int DisplayOrder,
    bool IsActive);
