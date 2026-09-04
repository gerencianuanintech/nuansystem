using NuanSystem.WinForms.Services.BusinessPartners.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.BusinessPartners;

public sealed class BusinessPartnerClient(INuanApiClient apiClient) : IBusinessPartnerClient
{
    public async Task<IReadOnlyCollection<BusinessPartnerItem>> GetAsync(string partnerType, CancellationToken cancellationToken = default)
    {
        var path = ResolveCollectionPath(partnerType);
        return await apiClient.GetAsync<List<BusinessPartnerItem>>(path, cancellationToken);
    }

    public Task<BusinessPartnerItem> GetByIdAsync(int id, string formKey, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<BusinessPartnerItem>($"{ResolveFormPath(formKey)}/{id}", cancellationToken);
    }

    public Task<BusinessPartnerLookups> GetLookupsAsync(string formKey, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<BusinessPartnerLookups>($"{ResolveFormPath(formKey)}/lookups", cancellationToken);
    }

    public Task<BusinessPartnerItem> CreateAsync(string formKey, SaveBusinessPartnerRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveBusinessPartnerRequest, BusinessPartnerItem>(ResolveFormPath(formKey), request, cancellationToken);
    }

    public Task<BusinessPartnerItem> UpdateAsync(string formKey, int id, SaveBusinessPartnerRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveBusinessPartnerRequest, BusinessPartnerItem>($"{ResolveFormPath(formKey)}/{id}", request, cancellationToken);
    }

    public async Task DeleteAsync(string formKey, int id, string expectedRowVersion, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<DeleteBusinessPartnerRequest, object>(
            $"{ResolveFormPath(formKey)}/{id}",
            new(expectedRowVersion),
            cancellationToken);
    }

    private static string ResolveCollectionPath(string partnerType)
    {
        return partnerType.Equals("Supplier", StringComparison.OrdinalIgnoreCase)
            ? "/api/commercial/suppliers"
            : "/api/commercial/customers";
    }

    private static string ResolveFormPath(string formKey)
    {
        return formKey.Equals("suppliers", StringComparison.OrdinalIgnoreCase)
            ? "/api/commercial/suppliers"
            : "/api/commercial/customers";
    }
}

file sealed record DeleteBusinessPartnerRequest(string ExpectedRowVersion);
