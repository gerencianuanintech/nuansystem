namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record SecurityFormAccessFormItem(
    int Id,
    string Code,
    string Name,
    string? Description,
    string FormKey,
    int FormType,
    string FormTypeName,
    bool IsVisible,
    bool IsActive);
