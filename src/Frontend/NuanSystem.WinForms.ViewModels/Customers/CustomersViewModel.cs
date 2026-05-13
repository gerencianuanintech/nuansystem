using NuanSystem.WinForms.Services.Customers;
using NuanSystem.WinForms.Services.Customers.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Customers;

public sealed class CustomersViewModel : CrudViewModel<CustomerItem, SaveCustomerRequest>
{
    private readonly ICustomerClient customerClient;

    public CustomersViewModel(ICustomerClient customerClient)
    {
        this.customerClient = customerClient;
    }

    public IReadOnlyCollection<CustomerItem> Customers => Items;

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(customerClient.GetAsync, cancellationToken);
    }

    public override Task CreateAsync(SaveCustomerRequest request, CancellationToken cancellationToken = default)
    {
        return customerClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveCustomerRequest request, CancellationToken cancellationToken = default)
    {
        return customerClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return customerClient.DeleteAsync(id, cancellationToken);
    }
}
