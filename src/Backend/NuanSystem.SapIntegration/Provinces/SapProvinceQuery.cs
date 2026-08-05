using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.SapIntegration.Provinces;

internal static class SapProvinceQuery
{
    internal const string Full = "States?$orderby=Country,Code";

    internal static SapServiceLayerReadOptions ReadOptions { get; } = new(
        MaxPages: 100,
        Operation: "consultar las provincias",
        EntityDisplayName: "las provincias");
}
