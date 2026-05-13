namespace NuanSystem.WinForms.Services.Audit.Models;

public sealed record SecurityChangeItem(
    long Id,
    string EntityName,
    string RecordId,
    string Action,
    string FieldName,
    string? OldValue,
    string? NewValue,
    int? UserId,
    string? UserName,
    string Source,
    DateTime CreatedAt);
