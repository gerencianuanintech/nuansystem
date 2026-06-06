namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record SaveSecurityFormFieldAccessItemRequest(
    int FieldId,
    bool IsVisible,
    bool IsEditable,
    bool IsRequired,
    bool IsReadOnly,
    bool IsActive);
