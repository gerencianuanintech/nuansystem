using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.Companies.Dtos;

public sealed record CompanyConnectionTestData(
    DatabaseEngine DatabaseEngine,
    string Server,
    int? Port,
    string DatabaseName,
    string DatabaseUser,
    string DatabasePassword);
