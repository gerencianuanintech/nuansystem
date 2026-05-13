using NuanSystem.Application.Features.Customers.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ICustomerRepository : IRepository
{
    Task<IReadOnlyCollection<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateCustomerData customer, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateCustomerData customer, CancellationToken cancellationToken = default);

    Task<bool> SetActiveStateAsync(int id, bool isActive, CancellationToken cancellationToken = default);
}
