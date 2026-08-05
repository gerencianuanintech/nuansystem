namespace NuanSystem.Application.Features.Geography.Common.Dtos;

public sealed class ReverseGeocodeResultDto
{
    public string? Country { get; set; }
    public string? CountryCode { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? FormattedAddress { get; set; }
}

public sealed class StaticMapResultDto
{
    public string ContentType { get; set; } = "image/png";
    public string ImageBase64 { get; set; } = string.Empty;
}
