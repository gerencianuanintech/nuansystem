namespace NuanSystem.Application.Features.Carriers;

public static class CarrierIdentificationTypeCodes
{
    public const string Ruc = "04";
    public const string Cedula = "05";
    public const string Pasaporte = "06";

    public static IReadOnlyCollection<string> All { get; } = [Ruc, Cedula, Pasaporte];

    public static bool IsValid(string? code) =>
        !string.IsNullOrWhiteSpace(code) && All.Contains(code.Trim(), StringComparer.Ordinal);
}
