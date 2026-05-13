namespace NuanSystem.WinForms.Services.Settings.Models;

public sealed record CompanyParameterItem(
    int Id,
    int CompanyId,
    string Key,
    string? Value,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
