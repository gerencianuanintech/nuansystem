namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record SecurityFormAccessFormDto(
    int Id,
    string Code,
    string Name,
    string? Description,
    string FormKey,
    int FormType,
    string FormTypeName,
    bool IsVisible,
    bool IsActive);
