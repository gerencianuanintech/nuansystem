using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.SapIntegration.Warehouses;

internal static class SapWarehouseQuery
{
    internal const string Full = "Warehouses?$orderby=WarehouseCode";

    internal static string Build(SapWarehouseFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var expressions = new List<string>(2);
        AddContainsExpression(expressions, filter.NameContains);
        AddExactExpression(expressions, filter.ExactName);

        if (expressions.Count == 0)
        {
            return Full;
        }

        var encodedFilter = Uri.EscapeDataString(string.Join(" or ", expressions));
        return $"Warehouses?$filter={encodedFilter}&$orderby=WarehouseCode";
    }

    internal static SapServiceLayerReadOptions ReadOptions { get; } = new(
        MaxPages: 100,
        Operation: "consultar las bodegas",
        EntityDisplayName: "las bodegas");

    private static void AddContainsExpression(List<string> expressions, string? value)
    {
        var normalized = Normalize(value);
        if (normalized is not null)
        {
            expressions.Add($"contains(toupper(WarehouseName),'{EscapeLiteral(normalized)}')");
        }
    }

    private static void AddExactExpression(List<string> expressions, string? value)
    {
        var normalized = Normalize(value);
        if (normalized is not null)
        {
            expressions.Add($"toupper(WarehouseName) eq '{EscapeLiteral(normalized)}'");
        }
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    private static string EscapeLiteral(string value) =>
        value.Replace("'", "''", StringComparison.Ordinal);
}
