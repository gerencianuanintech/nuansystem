using NuanSystem.WinForms.Services.BusinessPartners.Models;

namespace NuanSystem.WinForms.Services.BusinessPartners;

public interface IBusinessPartnerClient
{
    Task<IReadOnlyCollection<BusinessPartnerItem>> GetAsync(string partnerType, CancellationToken cancellationToken = default);
    Task<BusinessPartnerItem> GetByIdAsync(int id, string formKey, CancellationToken cancellationToken = default);
    Task<BusinessPartnerLookups> GetLookupsAsync(string formKey, CancellationToken cancellationToken = default);
    Task<BusinessPartnerItem> CreateAsync(string formKey, SaveBusinessPartnerRequest request, CancellationToken cancellationToken = default);
    Task<BusinessPartnerItem> UpdateAsync(string formKey, int id, SaveBusinessPartnerRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string formKey, int id, CancellationToken cancellationToken = default);
}
