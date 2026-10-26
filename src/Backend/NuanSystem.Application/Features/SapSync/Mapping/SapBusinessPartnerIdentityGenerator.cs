namespace NuanSystem.Application.Features.SapSync.Mapping;

public static class SapBusinessPartnerIdentityGenerator
{
    public static string GenerateSupplierCode(string taxIdentification, bool isPassport, string? existingCode = null)
        => Generate("P", taxIdentification, isPassport, existingCode);

    public static string GenerateCustomerCode(string taxIdentification, bool isPassport, string? existingCode = null)
        => Generate("C", taxIdentification, isPassport, existingCode);

    private static string Generate(string role, string taxIdentification, bool isPassport, string? existingCode)
        => !string.IsNullOrWhiteSpace(existingCode) ? existingCode.Trim() : role + (isPassport ? "E" : "L") + taxIdentification.Trim();

    public static bool LooksLikePassport(string? value)
        => !string.IsNullOrWhiteSpace(value) && value.Any(char.IsLetter);
}
