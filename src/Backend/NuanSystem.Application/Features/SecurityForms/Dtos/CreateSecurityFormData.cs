namespace NuanSystem.Application.Features.SecurityForms.Dtos;

public sealed record CreateSecurityFormData(
    string Code,
    string Name,
    string? Description,
    string FormKey,
    int FormType,
    bool HasListView,
    bool HasEditView,
    bool IsVisible,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);
