using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.SapIntegration.Countries;

internal static class SapCountryQuery
{
    internal const string Full = "Countries?$orderby=Code";

    internal static SapServiceLayerReadOptions ReadOptions { get; } = new(
        MaxPages: 100,
        Operation: "consultar los paises",
        EntityDisplayName: "los paises");
}
