namespace NuanSystem.Application.Features.Audit.Dtos;

public sealed record AuditLogDto(
    long Id,
    int? UserId,
    string? UserName,
    string? CompanyCode,
    string HttpMethod,
    string Path,
    string? QueryString,
    int StatusCode,
    string? IpAddress,
    string? UserAgent,
    DateTime CreatedAt);
