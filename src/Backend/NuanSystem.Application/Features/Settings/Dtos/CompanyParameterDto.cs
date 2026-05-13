namespace NuanSystem.Application.Features.Settings.Dtos;

public sealed record CompanyParameterDto(
    int Id,
    int CompanyId,
    string Key,
    string? Value,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
