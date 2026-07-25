using System.Data;
using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IBusinessPartnerRepository : IRepository
{
    Task<IReadOnlyCollection<BusinessPartnerDto>> GetAllAsync(string? partnerType, CancellationToken cancellationToken = default);

    Task<BusinessPartnerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BusinessPartnerDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<BusinessPartnerLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateBusinessPartnerData partner, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateBusinessPartnerData partner, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int? excludingId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdentificationAsync(int identificationTypeId, string identificationNumber, int? excludingId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdentificationAsync(int identificationTypeId, string identificationNumber, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateBusinessPartnerData partner, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateBusinessPartnerData partner, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<BusinessPartnerSapImportResultData> ImportSupplierFromSapAsync(
        BusinessPartnerSapImportData supplier,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
