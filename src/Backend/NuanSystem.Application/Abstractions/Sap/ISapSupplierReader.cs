using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapSupplierReader
{
    Task<IReadOnlyCollection<SapSupplierRecord>> GetSuppliersAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}
