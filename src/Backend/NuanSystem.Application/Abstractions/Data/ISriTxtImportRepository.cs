using NuanSystem.Application.Features.SriTxtImports.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISriTxtImportRepository : IRepository
{
    Task<SriTxtImportPersistenceResult> RegisterValidatedAsync(
        RegisterValidatedSriTxtImportData data,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetStagedEnvironmentsAsync(
        long importId,
        CancellationToken cancellationToken = default);

    Task<SriTxtImportEnqueuePersistenceResult> EnqueueAsync(
        EnqueueSriTxtImportData data,
        CancellationToken cancellationToken = default);
}
