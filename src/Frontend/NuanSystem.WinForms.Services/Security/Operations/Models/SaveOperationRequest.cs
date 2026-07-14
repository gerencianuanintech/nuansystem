namespace NuanSystem.WinForms.Services.Security.Operations.Models;

public sealed record SaveOperationRequest(
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
