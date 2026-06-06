namespace NuanSystem.Application.Features.OperationalCatalogs;

internal static class OperationalCatalogNormalizer
{
    internal static string NormalizeKey(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    internal static string NormalizeCode(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    internal static string? NormalizeKeyOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : NormalizeKey(value);
    }

    internal static string? NormalizeCodeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : NormalizeCode(value);
    }

    internal static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
