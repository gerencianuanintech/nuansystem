using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Abstractions.Tenancy;

public sealed record CompanyConnectionInfo(
    int CompanyId,
    string CompanyCode,
    string CommercialName,
    DatabaseEngine DatabaseEngine,
    string ConnectionString,
    SapIntegrationMode SapIntegrationMode);
