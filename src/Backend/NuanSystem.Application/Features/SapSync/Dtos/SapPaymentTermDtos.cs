namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapPaymentTermRecord(
    int GroupNumber,
    string Name,
    int AdditionalDays,
    int AdditionalMonths,
    int NumberOfInstallments);

public sealed record SapPaymentTermUpsertData(
    Guid ProposedGlobalId,
    string Code,
    string Name,
    int Days,
    bool IsCredit,
    string ExternalSystem,
    string ExternalCode,
    int? AuditUserId,
    string? AuditUserName);

public sealed record SapPaymentTermUpsertResult(
    string Status,
    int Id,
    Guid GlobalId,
    string Code,
    string Name,
    int Days,
    bool IsCredit,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Message);

public sealed record SapPaymentTermImportItemResultDto(
    int GroupNumber,
    string Name,
    string Status,
    string Message,
    int? LocalId = null);

public sealed record SapPaymentTermImportResultDto(
    int TotalRead,
    int Created,
    int Updated,
    int Unchanged,
    int Conflicted,
    int Failed,
    IReadOnlyCollection<SapPaymentTermImportItemResultDto> Items);
