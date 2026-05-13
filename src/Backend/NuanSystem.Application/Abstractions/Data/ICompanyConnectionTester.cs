using NuanSystem.Application.Features.Companies.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ICompanyConnectionTester
{
    Task<CompanyConnectionTestResult> TestAsync(
        CompanyConnectionTestData connection,
        CancellationToken cancellationToken = default);
}
