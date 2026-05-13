namespace NuanSystem.Application.Features.Audit.Dtos;

public sealed record AuditErrorLogDto(
    long Id,
    string Source,
    int? UserId,
    string? UserName,
    string? CompanyCode,
    string? ModuleKey,
    string? FormName,
    string? ActionName,
    string? HttpMethod,
    string? Path,
    string? QueryString,
    int? StatusCode,
    string ErrorMessage,
    string? ExceptionType,
    string? StackTrace,
    string? TraceId,
    string? IpAddress,
    string? MachineName,
    string? UserAgent,
    DateTime CreatedAt);
