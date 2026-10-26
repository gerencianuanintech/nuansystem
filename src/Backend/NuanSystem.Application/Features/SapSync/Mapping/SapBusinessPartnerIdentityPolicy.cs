namespace NuanSystem.Application.Features.SapSync.Mapping;

public enum SapBusinessPartnerRole { Customer, Supplier }

public sealed class SapBusinessPartnerIdentityPolicy
{
    public SapBusinessPartnerIdentityPolicy(string customerNationalPrefix, string customerForeignPrefix, string supplierNationalPrefix, string supplierForeignPrefix, string passportTaxpayerTypeCodes, string passportIdentificationTypeCode)
    {
        CustomerNationalPrefix = Required(customerNationalPrefix, nameof(customerNationalPrefix));
        CustomerForeignPrefix = Required(customerForeignPrefix, nameof(customerForeignPrefix));
        SupplierNationalPrefix = Required(supplierNationalPrefix, nameof(supplierNationalPrefix));
        SupplierForeignPrefix = Required(supplierForeignPrefix, nameof(supplierForeignPrefix));
        PassportTaxpayerTypeCodes = Required(passportTaxpayerTypeCodes, nameof(passportTaxpayerTypeCodes));
        PassportIdentificationTypeCode = Required(passportIdentificationTypeCode, nameof(passportIdentificationTypeCode));
    }

    public string CustomerNationalPrefix { get; }
    public string CustomerForeignPrefix { get; }
    public string SupplierNationalPrefix { get; }
    public string SupplierForeignPrefix { get; }
    public string PassportTaxpayerTypeCodes { get; }
    public string PassportIdentificationTypeCode { get; }

    public string GenerateCode(SapBusinessPartnerRole role, string identification, string taxpayerType, string? existingCode = null)
    {
        if (!string.IsNullOrWhiteSpace(existingCode)) return existingCode.Trim();
        if (string.IsNullOrWhiteSpace(identification)) throw new ArgumentException("La identificación es obligatoria.", nameof(identification));
        var foreign = PassportTaxpayerTypeCodes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Contains(taxpayerType.Trim(), StringComparer.OrdinalIgnoreCase);
        var prefix = role == SapBusinessPartnerRole.Supplier ? (foreign ? SupplierForeignPrefix : SupplierNationalPrefix) : (foreign ? CustomerForeignPrefix : CustomerNationalPrefix);
        return prefix + identification.Trim();
    }

    private static string Required(string value, string name) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("El valor es obligatorio.", name) : value.Trim();
}
