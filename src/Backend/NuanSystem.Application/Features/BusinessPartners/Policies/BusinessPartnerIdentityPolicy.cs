namespace NuanSystem.Application.Features.BusinessPartners.Policies;

public static class BusinessPartnerIdentityPolicy
{
    public static string NormalizeIdentification(string value) => string.Concat(
        value.Trim().ToUpperInvariant()
            .Where(character => !char.IsWhiteSpace(character) && character is not '.' and not '-'));

    public static string CreateInternalCode(Guid globalId) =>
        $"BP-{globalId:N}".ToUpperInvariant();
}
