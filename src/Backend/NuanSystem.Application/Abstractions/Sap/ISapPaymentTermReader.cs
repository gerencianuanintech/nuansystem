using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapPaymentTermReader
{
    Task<IReadOnlyCollection<SapPaymentTermRecord>> GetAllAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}
