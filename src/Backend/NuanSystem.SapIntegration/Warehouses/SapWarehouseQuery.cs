using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.SapIntegration.Warehouses;

internal static class SapWarehouseQuery
{
    internal const string Full = "Warehouses?$orderby=WarehouseCode";

    internal static SapServiceLayerReadOptions ReadOptions { get; } = new(
        MaxPages: 100,
        Operation: "consultar las bodegas",
        EntityDisplayName: "las bodegas");
}
