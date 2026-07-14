namespace NuanSystem.WinForms.Services.Security.Forms.Models;

public sealed record SaveFormRequest(
    string Code,
    string Name,
    string? Description,
    string FormKey,
    int FormType,
    bool HasListView,
    bool HasEditView,
    bool IsVisible,
    bool IsActive);
