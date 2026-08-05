using System.Text.Json;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.SapIntegration.Provinces;

internal static class SapProvinceMapper
{
    internal static SapProvinceRecord Map(JsonElement item) => new(
        ReadString(item, "Country"),
        ReadString(item, "Code"),
        ReadString(item, "Name"));

    private static string ReadString(JsonElement element, string name)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                && property.Value.ValueKind == JsonValueKind.String
                && !string.IsNullOrWhiteSpace(property.Value.GetString()))
            {
                return property.Value.GetString()!.Trim();
            }
        }

        return string.Empty;
    }
}
