using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISapPaymentTermImportRepository : IRepository
{
    Task<SapPaymentTermUpsertResult> UpsertAsync(
        SapPaymentTermUpsertData data,
        CancellationToken cancellationToken = default);
}
