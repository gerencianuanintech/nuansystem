namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSupplierRecord(
    string CardCode,
    string CardName,
    string? TaxIdentification,
    string CardType,
    int? GroupCode,
    string? Phone,
    string? Email,
    string? Currency,
    bool IsActive,
    DateTime? CreatedAt,
    DateTime? UpdatedAt);
