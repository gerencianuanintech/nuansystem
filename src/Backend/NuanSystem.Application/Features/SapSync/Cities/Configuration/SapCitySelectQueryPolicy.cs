using System.Text.RegularExpressions;

namespace NuanSystem.Application.Features.SapSync.Cities.Configuration;

public static partial class SapCitySelectQueryPolicy
{
    public const int MaximumLength = 12000;

    private static readonly string[] RequiredAliases =
        ["CountryCode", "ProvinceCode", "CityCode", "CityName"];

    public static bool TryValidate(string? query, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(query))
        {
            error = "La consulta de ciudades no puede estar vacia.";
            return false;
        }

        var normalized = query.Trim();
        if (normalized.Length > MaximumLength)
        {
            error = $"La consulta de ciudades no puede superar {MaximumLength} caracteres.";
            return false;
        }

        if (!normalized.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            error = "La consulta de ciudades debe iniciar con SELECT.";
            return false;
        }

        if (normalized.Contains(';')
            || normalized.Contains("--", StringComparison.Ordinal)
            || normalized.Contains("/*", StringComparison.Ordinal)
            || normalized.Contains("*/", StringComparison.Ordinal))
        {
            error = "La consulta de ciudades debe contener una sola sentencia SELECT sin comentarios.";
            return false;
        }

        if (ForbiddenKeywordRegex().IsMatch(normalized))
        {
            error = "La consulta de ciudades contiene una operacion no permitida.";
            return false;
        }

        foreach (var alias in RequiredAliases)
        {
            if (!Regex.IsMatch(
                normalized,
                    $"\\bAS\\s+(?:\\\"{Regex.Escape(alias)}\\\"(?=\\s*(?:,|FROM|$))|{Regex.Escape(alias)}\\b)",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                error = $"La consulta debe declarar el alias {alias}.";
                return false;
            }
        }

        return true;
    }

    public static string Normalize(string query) => query.Trim();

    [GeneratedRegex(@"\b(?:INSERT|UPDATE|DELETE|MERGE|UPSERT|DROP|ALTER|CREATE|TRUNCATE|CALL|DO|EXEC|EXECUTE|GRANT|REVOKE)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ForbiddenKeywordRegex();
}
