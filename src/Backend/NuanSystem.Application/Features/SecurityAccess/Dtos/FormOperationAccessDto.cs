namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record FormOperationAccessDto(
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
