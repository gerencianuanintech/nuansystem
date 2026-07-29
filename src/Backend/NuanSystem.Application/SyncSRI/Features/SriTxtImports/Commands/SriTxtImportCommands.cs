using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SriTxtImports.Dtos;

namespace NuanSystem.Application.Features.SriTxtImports.Commands;

public sealed record UploadSriTxtImportCommand(
    string OriginalFileName,
    long FileSizeBytes,
    string? DeclaredContentType,
    Stream Content,
    Guid TraceId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SriTxtImportDetailDto>;

public sealed record EnqueueSriTxtImportCommand(
    long ImportId,
    byte[] RowVersion,
    Guid TraceId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SriTxtImportDetailDto>;
