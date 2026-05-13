namespace NuanSystem.Application.Features.SecurityForms.Dtos;

public sealed record UpdateSecurityFormData(
    int Id,
    string Code,
    string Name,
    string? Description,
    string FormKey,
    int FormType,
    bool IsVisible,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);
