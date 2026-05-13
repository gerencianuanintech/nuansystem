namespace NuanSystem.Application.Features.Companies.Dtos;

public sealed record CompanyConnectionTestResult(
    bool Success,
    string Message,
    string? ServerVersion);
