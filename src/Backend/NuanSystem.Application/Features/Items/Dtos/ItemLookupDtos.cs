namespace NuanSystem.Application.Features.Items.Dtos;

public sealed record ItemGroupLookupDto(int Id, string Code, string Name);

public sealed record ItemFamilyLookupDto(int Id, int ItemGroupId, string Code, string Name);

public sealed record UnitOfMeasureLookupDto(int Id, string Code, string Name);

public sealed record TaxLookupDto(int Id, string Code, string Name, decimal Rate);

public sealed record WarehouseLookupDto(int Id, string Code, string Name);

public sealed record ItemLookupsDto(
    IReadOnlyCollection<ItemGroupLookupDto> ItemGroups,
    IReadOnlyCollection<ItemFamilyLookupDto> ItemFamilies,
    IReadOnlyCollection<UnitOfMeasureLookupDto> UnitOfMeasures,
    IReadOnlyCollection<TaxLookupDto> Taxes,
    IReadOnlyCollection<WarehouseLookupDto> Warehouses);
