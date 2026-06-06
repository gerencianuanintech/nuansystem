namespace NuanSystem.WinForms.Services.SecurityForms.Models;

public sealed record SaveSecurityFormRequest(
    string Code,
    string Name,
    string? Description,
    string FormKey,
    int FormType,
    bool HasListView,
    bool HasEditView,
    bool IsVisible,
    bool IsActive);
