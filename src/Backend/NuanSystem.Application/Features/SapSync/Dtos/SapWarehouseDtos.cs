namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapWarehouseRecord(
    string WarehouseCode,
    string WarehouseName,
    string? Street,
    string? City,
    string? Province,
    string? Country,
    bool IsActive);

public sealed record SapWarehousePreviewItemDto(
    string SapWarehouseCode,
    string SapWarehouseName,
    string? Address,
    string? City,
    string? Province,
    string? Country,
    bool IsActiveInSap,
    string Status,
    string StatusName,
    int? LocalWarehouseId,
    string? LocalCode,
    string? LocalName,
    string? DifferenceSummary);

public sealed record SapWarehouseBranchMappingDto(
    string SapWarehouseCode,
    string BranchCode);

public sealed record SapWarehouseImportResultDto(
    int TotalRead,
    int Created,
    int Updated,
    int Unchanged,
    int Skipped,
    int Failed,
    IReadOnlyCollection<SapWarehouseImportItemResultDto> Items);

public sealed record SapWarehouseImportItemResultDto(
    string SapWarehouseCode,
    string SapWarehouseName,
    string Status,
    string Message,
    int? LocalWarehouseId,
    string? ResultCode = null);
