namespace NuanSystem.Application.Features.SecurityOperations.Dtos;

public sealed record CreateSecurityOperationData(
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
    string? CreatedByUserName);
