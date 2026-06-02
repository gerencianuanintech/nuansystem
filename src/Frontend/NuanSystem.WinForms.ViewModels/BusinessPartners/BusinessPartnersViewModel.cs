using NuanSystem.WinForms.Services.BusinessPartners;
using NuanSystem.WinForms.Services.BusinessPartners.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.BusinessPartners;

public sealed class BusinessPartnersViewModel : CrudViewModel<BusinessPartnerItem, SaveBusinessPartnerRequest>
{
    private readonly IBusinessPartnerClient client;
    private readonly string partnerType;
    private readonly string formKey;
    private BusinessPartnerLookups? lookups;

    public BusinessPartnersViewModel(IBusinessPartnerClient client, string partnerType, string formKey)
    {
        this.client = client;
        this.partnerType = partnerType;
        this.formKey = formKey;
    }

    public IReadOnlyCollection<BusinessPartnerItem> Partners => Items;
    public BusinessPartnerLookups? Lookups => lookups;

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(token => client.GetAsync(partnerType, token), cancellationToken);
    }

    public async Task<BusinessPartnerItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await client.GetByIdAsync(id, formKey, cancellationToken);
    }

    public async Task<BusinessPartnerLookups> LoadLookupsAsync(CancellationToken cancellationToken = default)
    {
        lookups = await client.GetLookupsAsync(formKey, cancellationToken);
        return lookups;
    }

    public override Task CreateAsync(SaveBusinessPartnerRequest request, CancellationToken cancellationToken = default)
    {
        return client.CreateAsync(formKey, request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveBusinessPartnerRequest request, CancellationToken cancellationToken = default)
    {
        return client.UpdateAsync(formKey, id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync(formKey, id, cancellationToken);
    }
}
