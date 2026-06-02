namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSupplierPreviewItemDto(
    string SapCardCode,
    string SapCardName,
    string? TaxIdentification,
    string? Email,
    string? Phone,
    string? Currency,
    bool IsActiveInSap,
    string Status,
    string StatusName,
    int? LocalBusinessPartnerId,
    string? LocalCode,
    string? LocalName,
    string? DifferenceSummary);
