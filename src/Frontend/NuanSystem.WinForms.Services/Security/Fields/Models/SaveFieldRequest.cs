namespace NuanSystem.WinForms.Services.Security.Fields.Models;

public sealed record SaveFieldRequest(
    int FormId,
    string Code,
    string Name,
    string FieldKey,
    string? Description,
    string ControlType,
    string DataType,
    bool IsRequired,
    string? ValidationMessage,
    bool IsReadOnly,
    bool IsVisible,
    bool IsCustom,
    int DisplayOrder,
    bool IsActive);
