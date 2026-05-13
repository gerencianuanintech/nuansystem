using NuanSystem.WinForms.Services.Documents;
using NuanSystem.WinForms.Services.Documents.Models;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sap.Models;
using NuanSystem.WinForms.ViewModels.Common;
using NuanSystem.WinForms.Services.Customers;
using NuanSystem.WinForms.Services.Customers.Models;
using NuanSystem.WinForms.Services.InventoryItems;
using NuanSystem.WinForms.Services.InventoryItems.Models;

namespace NuanSystem.WinForms.ViewModels.Documents;

public sealed class DocumentsViewModel : ViewModelBase
{
    private readonly IDocumentClient documentClient;
    private readonly ICustomerClient customerClient;
    private readonly IItemClient itemClient;
    private readonly ISapClient sapClient;
    private IReadOnlyCollection<DocumentSummaryItem> documents = Array.Empty<DocumentSummaryItem>();
    private IReadOnlyCollection<CustomerItem> customers = Array.Empty<CustomerItem>();
    private IReadOnlyCollection<ItemItem> items = Array.Empty<ItemItem>();
    private bool isBusy;

    public DocumentsViewModel(
        IDocumentClient documentClient,
        ICustomerClient customerClient,
        IItemClient itemClient,
        ISapClient sapClient)
    {
        this.documentClient = documentClient;
        this.customerClient = customerClient;
        this.itemClient = itemClient;
        this.sapClient = sapClient;
    }

    public IReadOnlyCollection<DocumentSummaryItem> Documents
    {
        get => documents;
        private set => SetProperty(ref documents, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public IReadOnlyCollection<CustomerItem> Customers
    {
        get => customers;
        private set => SetProperty(ref customers, value);
    }

    public IReadOnlyCollection<ItemItem> Items
    {
        get => items;
        private set => SetProperty(ref items, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            Documents = await documentClient.GetAsync(cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public Task<SapSendResult> SendToSapAsync(long documentId, CancellationToken cancellationToken = default)
    {
        return sapClient.SendDocumentAsync(documentId, cancellationToken);
    }

    public async Task LoadCatalogsAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            Customers = (await customerClient.GetAsync(cancellationToken))
                .Where(customer => customer.IsActive)
                .ToArray();
            Items = (await itemClient.GetAsync(cancellationToken))
                .Where(item => item.IsActive)
                .ToArray();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public Task<DocumentDetailItem> CreateAsync(CreateDocumentRequest request, CancellationToken cancellationToken = default)
    {
        return documentClient.CreateAsync(request, cancellationToken);
    }
}
