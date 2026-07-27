using NuanSystem.Application.Features.SriTxtImports.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISriTxtImportRepository : IRepository
{
    Task<SriTxtImportPageDto> SearchAsync(
        SriTxtImportFilter filter,
        CancellationToken cancellationToken = default);

    Task<SriTxtImportDetailDto?> GetByIdAsync(
        long importId,
        CancellationToken cancellationToken = default);

    Task<SriTxtImportRowPageDto?> GetRowsAsync(
        long importId,
        SriTxtImportRowFilter filter,
        CancellationToken cancellationToken = default);

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
