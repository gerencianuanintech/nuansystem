using NuanSystem.Application.Features.Companies.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ICompanyAdminRepository : IRepository
{
    Task<IReadOnlyCollection<CompanyDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CompanyDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateCompanyData company, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default);

    Task AssignUserAsync(int userId, int companyId, CancellationToken cancellationToken = default);
}
