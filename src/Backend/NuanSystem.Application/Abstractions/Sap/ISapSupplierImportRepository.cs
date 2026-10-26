using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapSupplierImportRepository
{
    Task<BusinessPartnerSapImportResultData> ImportSupplierFromSapAsync(
        BusinessPartnerSapImportData supplier,
        CancellationToken cancellationToken = default);
}
