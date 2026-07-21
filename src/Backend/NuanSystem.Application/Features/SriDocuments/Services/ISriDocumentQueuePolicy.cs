using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.SriDocuments.Services;

public interface ISriDocumentQueuePolicy
{
    Task<Result<bool>> ValidateEnqueueAsync(string environment, CancellationToken cancellationToken = default);
}
