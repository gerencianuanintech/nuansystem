namespace NuanSystem.Application.Features.Definitions.Inventory.ItemTypes;

public static class ItemTypeBehaviorCodes
{
    public const string Product = "Product";
    public const string Service = "Service";
    public const string Supply = "Supply";
    public const string Asset = "Asset";
    public const string Kit = "Kit";

    public static readonly IReadOnlyCollection<string> All =
        [Product, Service, Supply, Asset, Kit];

    public static bool IsValid(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        All.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string value) =>
        All.Single(code => code.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
}
