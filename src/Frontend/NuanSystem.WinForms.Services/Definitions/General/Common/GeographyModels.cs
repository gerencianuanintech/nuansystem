namespace NuanSystem.WinForms.Services.Definitions.General.Common;

public sealed class GeographyLookupItem
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

public sealed class ReverseGeocodeResult
{
    public string? Country { get; set; }

    public string? CountryCode { get; set; }

    public string? Province { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? FormattedAddress { get; set; }
}

public sealed class StaticMapResult
{
    public string ContentType { get; set; } = "image/png";

    public string ImageBase64 { get; set; } = string.Empty;
}
