using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SriDocuments.Dtos;

namespace NuanSystem.Application.Features.SriDocuments.Commands;

public sealed record EnqueueSriDocumentCommand(string Environment, string AccessKey, string SourceType, string SourceReference, string? BranchCode, int Priority, Guid? TraceId, int? AuditUserId, string? AuditUserName) : ICommand<SriDocumentQueueDetailDto>;
public sealed record CancelSriDocumentCommand(long Id, byte[] RowVersion, string? Reason, int? AuditUserId, string? AuditUserName) : ICommand<SriDocumentQueueDetailDto>;
public sealed record ReprocessSriDocumentCommand(long Id, byte[] RowVersion, string Reason, int? AuditUserId, string? AuditUserName) : ICommand<SriDocumentQueueDetailDto>;
