using System.Text.Json;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.SapIntegration.Warehouses;

internal static class SapWarehouseMapper
{
    internal static SapWarehouseRecord Map(JsonElement item)
    {
        var inactive = ReadBooleanFlag(item, "Inactive")
            || ReadBooleanFlag(item, "Locked");

        return new SapWarehouseRecord(
            ReadString(item, "WarehouseCode"),
            ReadString(item, "WarehouseName"),
            ReadOptionalString(item, "Street"),
            ReadOptionalString(item, "City"),
            ReadFirstOptionalString(item, "State", "County", "StateCode"),
            ReadOptionalString(item, "Country"),
            !inactive);
    }

    private static string ReadString(JsonElement element, string name)
        => ReadOptionalString(element, name) ?? string.Empty;

    private static string? ReadFirstOptionalString(JsonElement element, params string[] names)
        => names
            .Select(name => ReadOptionalString(element, name))
            .FirstOrDefault(value => value is not null);

    private static string? ReadOptionalString(JsonElement element, string name)
        => element.TryGetProperty(name, out var property)
           && property.ValueKind == JsonValueKind.String
           && !string.IsNullOrWhiteSpace(property.GetString())
            ? property.GetString()!.Trim()
            : null;

    private static bool ReadBooleanFlag(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property))
        {
            return false;
        }

        if (property.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            return property.GetBoolean();
        }

        var value = property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

        return value is not null
            && value.Trim().ToUpperInvariant() is ("Y" or "YES" or "TYES" or "TRUE" or "1");
    }
}
