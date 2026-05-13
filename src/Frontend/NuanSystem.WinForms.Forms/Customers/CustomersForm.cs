using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Customers.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Customers;

namespace NuanSystem.WinForms.Forms.Customers;

public sealed partial class CustomersForm : BaseCrudListForm
{
    private readonly CustomersViewModel viewModel;
    private readonly ApiSession session;

    public CustomersForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
        WireEvents();
    }

    public CustomersForm(CustomersViewModel viewModel, ApiSession session)
    {
        this.viewModel = viewModel;
        this.session = session;
        InitializeComponent();
        WireEvents();
    }

    private void WireEvents()
    {
        ConfigureCrudButtons(btnActualizar, btnNuevo, btnEditar, btnEliminar);
        if (session is not null)
        {
            ConfigureCrudPermissions(session, CrudOperationPermissions.Customers);
        }
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await viewModel.LoadAsync();
            grcClientes.DataSource = viewModel.Customers.ToList();
            grvClientes.BestFitColumns();
        });
    }

    protected override async Task CreateAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        using var frmCliente = new CustomerEditForm();
        if (frmCliente.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(frmCliente.Request);
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (viewModel is null || SelectedItem() is not { } customer)
        {
            return;
        }

        using var frmCliente = new CustomerEditForm(customer);
        if (frmCliente.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(customer.Id, frmCliente.Request);
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (viewModel is null || SelectedItem() is not { } customer)
        {
            return;
        }

        using var frmCliente = new CustomerEditForm(customer, copyMode: true);
        if (frmCliente.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(frmCliente.Request);
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (viewModel is null || SelectedItem() is not { } customer)
        {
            return;
        }

        if (!Confirm($"Eliminar el cliente {customer.Code}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(customer.Id);
        await LoadDataAsync();
    }

    private CustomerItem? SelectedItem()
    {
        return grvClientes.GetFocusedRow() as CustomerItem;
    }
}
