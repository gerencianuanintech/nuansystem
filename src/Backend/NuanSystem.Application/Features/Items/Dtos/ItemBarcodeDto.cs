namespace NuanSystem.Application.Features.Items.Dtos;

public sealed record ItemBarcodeDto(
    int Id,
    int ItemId,
    string Barcode,
    int? UnitOfMeasureId,
    string BarcodeType,
    decimal ConversionFactor,
    bool IsMain,
    bool IsActive);
