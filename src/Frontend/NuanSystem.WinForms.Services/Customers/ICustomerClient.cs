using NuanSystem.WinForms.Services.Customers.Models;

namespace NuanSystem.WinForms.Services.Customers;

public interface ICustomerClient
{
    Task<IReadOnlyCollection<CustomerItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<CustomerItem> CreateAsync(SaveCustomerRequest request, CancellationToken cancellationToken = default);
    Task<CustomerItem> UpdateAsync(int id, SaveCustomerRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
