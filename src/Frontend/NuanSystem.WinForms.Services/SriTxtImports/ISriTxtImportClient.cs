using NuanSystem.WinForms.Services.SriTxtImports.Models;

namespace NuanSystem.WinForms.Services.SriTxtImports;

public interface ISriTxtImportClient
{
    Task<SriTxtImportPage> SearchAsync(
        SriTxtImportFilter filter,
        CancellationToken cancellationToken = default);

    Task<SriTxtImportDetail> GetDetailAsync(
        long importId,
        CancellationToken cancellationToken = default);

    Task<SriTxtImportRowPage> GetRowsAsync(
        long importId,
        string validity,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<SriTxtImportDetail> EnqueueAsync(
        long importId,
        byte[] rowVersion,
        CancellationToken cancellationToken = default);
}
