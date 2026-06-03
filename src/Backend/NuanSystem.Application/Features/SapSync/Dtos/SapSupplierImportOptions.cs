namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSupplierImportOptions(
    int? AuditUserId,
    string? AuditUserName,
    bool WritePublicSapLog,
    bool WriteInbox,
    bool UseIncrementalWatermark,
    string WorkerInstance,
    string CorrelationId);
