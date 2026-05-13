using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Services.Sap;

public interface ISapClient
{
    Task<IReadOnlyCollection<SapSyncLogItem>> GetSyncLogsAsync(CancellationToken cancellationToken = default);
    Task<SapSendResult> SendDocumentAsync(long documentId, CancellationToken cancellationToken = default);
}
