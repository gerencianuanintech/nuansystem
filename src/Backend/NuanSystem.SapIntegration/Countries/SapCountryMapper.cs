using System.Text.Json;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.SapIntegration.Countries;

internal static class SapCountryMapper
{
    // Validate the ISO property names against the target SAP Service Layer $metadata
    // before runtime enablement. The aliases keep supported SAP B1 metadata casing compatible.
    internal static SapCountryRecord Map(JsonElement item) => new(
        ReadString(item, "Code"),
        ReadString(item, "Name"),
        ReadFirstOptionalString(item, "ISOAlpha2Code", "IsoAlpha2Code", "ISOAlpha2", "IsoAlpha2"),
        ReadFirstOptionalString(item, "ISOAlpha3Code", "IsoAlpha3Code", "ISOAlpha3", "IsoAlpha3"));

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
}
