namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

internal static class WarehouseCommandHelpers
{
    public static string NormalizeCode(string code)
    {
        return code.Trim().ToUpperInvariant();
    }

    public static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
