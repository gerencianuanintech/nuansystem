using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IChartOfAccountRepository : IRepository
{
    Task<IReadOnlyCollection<ChartOfAccountDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ChartOfAccountLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);

    Task<ChartOfAccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateChartOfAccountData account, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(int companyId, string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(int companyId, string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateChartOfAccountData account, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}
