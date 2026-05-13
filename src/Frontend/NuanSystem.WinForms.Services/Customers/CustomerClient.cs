using NuanSystem.WinForms.Services.Customers.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Customers;

public sealed class CustomerClient : ICustomerClient
{
    private readonly INuanApiClient apiClient;

    public CustomerClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<CustomerItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<CustomerItem>>("/api/customers", cancellationToken);
    }

    public Task<CustomerItem> CreateAsync(SaveCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            request.Code,
            request.Name,
            request.TaxIdentification,
            request.Email,
            request.Phone,
            request.AddressLine
        };

        return apiClient.PostAsync<object, CustomerItem>("/api/customers", payload, cancellationToken);
    }

    public Task<CustomerItem> UpdateAsync(int id, SaveCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Id = id,
            request.Code,
            request.Name,
            request.TaxIdentification,
            request.Email,
            request.Phone,
            request.AddressLine,
            request.IsActive
        };

        return apiClient.PutAsync<object, CustomerItem>($"/api/customers/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/customers/{id}", cancellationToken);
    }
}
