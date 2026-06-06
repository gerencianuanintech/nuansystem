namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record SecurityFormAccessOperationDto(
    int FormId,
    string FormCode,
    string FormName,
    string FormKey,
    int OperationId,
    string OperationCode,
    string OperationName,
    string? OperationDescription,
    string? ActionKey,
    string? RibbonPageName,
    string? RibbonGroupName,
    string? IconLarge,
    string? IconSmall,
    int DisplayOrder,
    bool IsAllowed,
    int? UpdatedByUserId,
    string? UpdatedByUserName,
    DateTime? UpdatedAt,
    int? CreatedByUserId,
    string? CreatedByUserName,
    DateTime? CreatedAt);
