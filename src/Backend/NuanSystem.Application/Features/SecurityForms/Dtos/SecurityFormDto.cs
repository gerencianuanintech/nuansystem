namespace NuanSystem.Application.Features.SecurityForms.Dtos;

public sealed record SecurityFormDto(
    int Id,
    string Code,
    string Name,
    string? Description,
    string FormKey,
    int FormType,
    string FormTypeName,
    bool HasListView,
    bool HasEditView,
    bool IsVisible,
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
