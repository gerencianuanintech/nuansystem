namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record SaveSecurityFormFieldAccessData(
    int FieldId,
    bool IsVisible,
    bool IsEditable,
    bool IsRequired,
    bool IsReadOnly,
    bool IsActive);
