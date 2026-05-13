namespace NuanSystem.Application.Features.Settings.Dtos;

public sealed record UpsertCompanyParameterData(
    string Key,
    string? Value,
    string? Description);
