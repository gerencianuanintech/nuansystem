namespace NuanSystem.WinForms.Services.Settings.Models;

public sealed record SaveCompanyParameterRequest(
    string Key,
    string? Value,
    string? Description);
