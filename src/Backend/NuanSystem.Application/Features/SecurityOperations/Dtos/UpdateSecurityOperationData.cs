namespace NuanSystem.Application.Features.SecurityOperations.Dtos;

public sealed record UpdateSecurityOperationData(
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
    int? UpdatedByUserId,
    string? UpdatedByUserName);
