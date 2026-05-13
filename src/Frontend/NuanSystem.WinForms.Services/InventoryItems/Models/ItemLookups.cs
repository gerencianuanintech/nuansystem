namespace NuanSystem.WinForms.Services.InventoryItems.Models;

public sealed record ItemGroupLookupItem(int Id, string Code, string Name)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record UnitOfMeasureLookupItem(int Id, string Code, string Name)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record TaxLookupItem(int Id, string Code, string Name, decimal Rate)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record WarehouseLookupItem(int Id, string Code, string Name)
{
    public string DisplayText => $"{Code} - {Name}";
}

public sealed record ItemLookups(
    IReadOnlyCollection<ItemGroupLookupItem> ItemGroups,
    IReadOnlyCollection<UnitOfMeasureLookupItem> UnitOfMeasures,
    IReadOnlyCollection<TaxLookupItem> Taxes,
    IReadOnlyCollection<WarehouseLookupItem> Warehouses);
