namespace NuanSystem.WinForms.Services.Audit.Models;

public sealed record CreateAuditErrorLogRequest(
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
    string? UserAgent);
