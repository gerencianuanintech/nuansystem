using System.Globalization;

namespace NuanSystem.Application.Features.SapSync.Mapping;

public sealed record SapFieldMappingEngineResult(
    bool IsValid,
    IReadOnlyDictionary<string, object?> Values,
    IReadOnlyCollection<SapMappingValidationMessage> Errors);

public static class SapFieldMappingEngine
{
    public static SapFieldMappingEngineResult Map(
        IReadOnlyDictionary<string, object?> source,
        IReadOnlyCollection<SapFieldMappingDefinition> mappings,
        SapMappingDirection direction)
    {
        var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var errors = new List<SapMappingValidationMessage>();

        foreach (var mapping in mappings.Where(item => item.Direction is SapMappingDirection.Both || item.Direction == direction))
        {
            if (!IsTransformationAllowed(mapping.TransformationKey))
            {
                errors.Add(new(SapMappingValidationCodes.TransformationUnknown, $"La transformación '{mapping.TransformationKey}' no está permitida.", mapping.DestinationField));
                continue;
            }

            source.TryGetValue(mapping.SourceField, out var rawValue);
            if (rawValue is null || string.IsNullOrWhiteSpace(Convert.ToString(rawValue, CultureInfo.InvariantCulture)))
            {
                if (mapping.IsRequired)
                    errors.Add(new(SapMappingValidationCodes.FieldRequired, "El campo origen obligatorio no tiene valor.", mapping.SourceField));
                values[mapping.DestinationField] = null;
                continue;
            }

            values[mapping.DestinationField] = Apply(mapping.TransformationKey, rawValue);
        }

        return new(errors.Count == 0, values, errors);
    }

    private static bool IsTransformationAllowed(string? key)
        => key is not null && key.Trim() is "Trim" or "LowerInvariant" or "UpperInvariant" or "TaxIdNormalize" or "SapBooleanYN" or "SapBoolean" or "CatalogLookup" or "GenerateInternalCode" or "PrefixByTaxpayerType";

    private static object? Apply(string key, object value)
    {
        var text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        return key.Trim() switch
        {
            "Trim" => text.Trim(),
            "LowerInvariant" => text.Trim().ToLowerInvariant(),
            "UpperInvariant" => text.Trim().ToUpperInvariant(),
            "TaxIdNormalize" => new string(text.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant(),
            "SapBooleanYN" => text.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase),
            "SapBoolean" => text.Trim() is "1" or "true" or "Y" or "S" or "SI",
            _ => value
        };
    }
}
