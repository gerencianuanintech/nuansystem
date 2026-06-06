using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityDocumentNumberingService
{
    Task<ReserveSecurityDocumentNumberResult> ReserveNumberAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default);
}
