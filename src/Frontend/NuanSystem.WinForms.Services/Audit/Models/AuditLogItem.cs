namespace NuanSystem.WinForms.Services.Audit.Models;

public sealed record AuditLogItem(
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
