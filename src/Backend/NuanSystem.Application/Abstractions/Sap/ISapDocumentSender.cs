using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapDocumentSender
{
    Task<Result<SapSendResultDto>> SendAsync(
        long documentId,
        string? documentType,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
