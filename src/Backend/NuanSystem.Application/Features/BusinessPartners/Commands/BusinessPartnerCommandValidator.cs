using FluentValidation;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Policies;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class CreateBusinessPartnerCommandValidator : AbstractValidator<CreateBusinessPartnerCommand>
{
    public CreateBusinessPartnerCommandValidator()
    {
        Include(new BusinessPartnerCommandRules<CreateBusinessPartnerCommand>(validateIdentity: true));
    }
}

public sealed class UpdateBusinessPartnerCommandValidator : AbstractValidator<UpdateBusinessPartnerCommand>
{
    public UpdateBusinessPartnerCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.ExpectedRowVersion)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(IsEightByteBase64)
            .WithMessage("ExpectedRowVersion debe ser un rowversion base64 valido.")
            .WithErrorCode("BP_ROW_VERSION_INVALID");
        Include(new BusinessPartnerCommandRules<UpdateBusinessPartnerCommand>(validateIdentity: false));
    }

    private static bool IsEightByteBase64(string value)
    {
        try
        {
            return Convert.FromBase64String(value).Length == 8;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

public sealed class DeleteBusinessPartnerCommandValidator : AbstractValidator<DeleteBusinessPartnerCommand>
{
    public DeleteBusinessPartnerCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.ExpectedRowVersion)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(IsEightByteBase64)
            .WithMessage("ExpectedRowVersion debe ser un rowversion base64 valido.")
            .WithErrorCode("BP_ROW_VERSION_INVALID");
    }

    private static bool IsEightByteBase64(string value)
    {
        try
        {
            return Convert.FromBase64String(value).Length == 8;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

internal sealed class BusinessPartnerCommandRules<T> : AbstractValidator<T>
{
    private static readonly string[] PartnerTypes = ["Customer", "Supplier"];
    private static readonly string[] AddressTypes = ["Main", "Billing", "Shipping", "Other"];

    public BusinessPartnerCommandRules(bool validateIdentity)
    {
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.Name))).NotEmpty().MaximumLength(200);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.CommercialName))).MaximumLength(200);
        When(_ => validateIdentity, () =>
        {
            RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.PartnerType)))
                .NotEmpty().Must(PartnerTypes.Contains)
                .WithErrorCode("BP_ROLE_INVALID");
            RuleFor(command => GetInt(command, nameof(CreateBusinessPartnerCommand.IdentificationTypeId))).GreaterThan(0);
            RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.IdentificationNumber)))
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .Must(value => BusinessPartnerIdentityPolicy.NormalizeIdentification(value ?? string.Empty).Length > 0)
                .WithMessage("La identificacion debe contener letras o numeros.")
                .WithErrorCode("BP_IDENTIFICATION_INVALID");
        });
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.Email))).EmailAddress().MaximumLength(256).When(command => !string.IsNullOrWhiteSpace(GetString(command, nameof(CreateBusinessPartnerCommand.Email))));
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.Phone))).MaximumLength(50);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.Website))).MaximumLength(200);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.Remarks))).MaximumLength(1000);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.TaxpayerType))).MaximumLength(60);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.FiscalRegime))).MaximumLength(80);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.CountryCode))).MaximumLength(3);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.Province))).MaximumLength(120);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.City))).MaximumLength(120);
        RuleFor(command => GetInt(command, nameof(CreateBusinessPartnerCommand.CreditDays))).GreaterThanOrEqualTo(0);
        RuleFor(command => GetDecimal(command, nameof(CreateBusinessPartnerCommand.CreditLimit))).GreaterThanOrEqualTo(0);
        RuleFor(command => GetInt(command, nameof(CreateBusinessPartnerCommand.DeliveryDays))).GreaterThanOrEqualTo(0);
        RuleFor(command => GetDecimal(command, nameof(CreateBusinessPartnerCommand.MinimumOrderAmount))).GreaterThanOrEqualTo(0);
        RuleFor(command => GetDecimal(command, nameof(CreateBusinessPartnerCommand.CommercialDiscountPercent))).InclusiveBetween(0, 100);
        RuleFor(command => GetDecimal(command, nameof(CreateBusinessPartnerCommand.MinimumOrderQuantity))).GreaterThanOrEqualTo(0);
        RuleFor(command => GetInt(command, nameof(CreateBusinessPartnerCommand.AverageDeliveryDays))).GreaterThanOrEqualTo(0);
        RuleFor(command => GetInt(command, nameof(CreateBusinessPartnerCommand.LeadTimeDays))).GreaterThanOrEqualTo(0);
        RuleFor(command => GetInt(command, nameof(CreateBusinessPartnerCommand.DeliveryToleranceDays))).GreaterThanOrEqualTo(0);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.PreferredCurrencyCode))).MaximumLength(3);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.PurchaseCurrencyCode))).MaximumLength(3);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.Incoterm))).MaximumLength(20);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.PurchaseSupplierType))).MaximumLength(80);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.PreferredWarehouseCode))).MaximumLength(50);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.CreditStatus))).MaximumLength(30);
        RuleFor(command => GetAddresses(command))
            .Must(items => items.Count(item => item.IsPrimary && item.IsActive) <= 1)
            .WithMessage("Solo puede existir una direccion principal activa.")
            .OverridePropertyName(nameof(CreateBusinessPartnerCommand.Addresses));

        RuleFor(command => GetContacts(command))
            .Must(items => items.Count(item => item.IsPrimary && item.IsActive) <= 1)
            .WithMessage("Solo puede existir un contacto principal activo.")
            .OverridePropertyName(nameof(CreateBusinessPartnerCommand.Contacts));

        RuleFor(command => GetBankAccounts(command))
            .Must(items => items.Count(item => item.IsPrimary && item.IsActive) <= 1)
            .WithMessage("Solo puede existir una cuenta bancaria principal activa.")
            .OverridePropertyName(nameof(CreateBusinessPartnerCommand.BankAccounts));

        RuleForEach(command => GetRetentionSettings(command))
            .ChildRules(setting =>
        {
            setting.RuleFor(item => item.Percent).InclusiveBetween(0m, 100m);
        }).OverridePropertyName(nameof(CreateBusinessPartnerCommand.RetentionSettings));

        RuleForEach(command => GetAddresses(command))
            .ChildRules(address =>
        {
            address.RuleFor(item => item.AddressType).NotEmpty().Must(AddressTypes.Contains);
            address.RuleFor(item => item.Line1).NotEmpty().MaximumLength(300);
            address.RuleFor(item => item.Line2).MaximumLength(300);
            address.RuleFor(item => item.CountryCode).MaximumLength(3);
            address.RuleFor(item => item.Province).MaximumLength(120);
            address.RuleFor(item => item.City).MaximumLength(120);
            address.RuleFor(item => item.PostalCode).MaximumLength(30);
        }).OverridePropertyName(nameof(CreateBusinessPartnerCommand.Addresses));

        RuleForEach(command => GetContacts(command))
            .ChildRules(contact =>
        {
            contact.RuleFor(item => item.Name).NotEmpty().MaximumLength(150);
            contact.RuleFor(item => item.Position).MaximumLength(120);
            contact.RuleFor(item => item.Phone).MaximumLength(50);
            contact.RuleFor(item => item.Mobile).MaximumLength(50);
            contact.RuleFor(item => item.Email).EmailAddress().MaximumLength(256).When(item => !string.IsNullOrWhiteSpace(item.Email));
        }).OverridePropertyName(nameof(CreateBusinessPartnerCommand.Contacts));
    }

    private static string? GetString(T command, string name) => command?.GetType().GetProperty(name)?.GetValue(command) as string;
    private static int GetInt(T command, string name) => command?.GetType().GetProperty(name)?.GetValue(command) as int? ?? 0;
    private static decimal GetDecimal(T command, string name) => command?.GetType().GetProperty(name)?.GetValue(command) as decimal? ?? 0;
    private static IReadOnlyCollection<SaveBusinessPartnerAddressData> GetAddresses(T command) => command?.GetType().GetProperty("Addresses")?.GetValue(command) as IReadOnlyCollection<SaveBusinessPartnerAddressData> ?? [];
    private static IReadOnlyCollection<SaveBusinessPartnerContactData> GetContacts(T command) => command?.GetType().GetProperty("Contacts")?.GetValue(command) as IReadOnlyCollection<SaveBusinessPartnerContactData> ?? [];
    private static IReadOnlyCollection<SaveBusinessPartnerBankAccountData> GetBankAccounts(T command) => command?.GetType().GetProperty("BankAccounts")?.GetValue(command) as IReadOnlyCollection<SaveBusinessPartnerBankAccountData> ?? [];
    private static IReadOnlyCollection<SaveBusinessPartnerRetentionSettingData> GetRetentionSettings(T command) => command?.GetType().GetProperty("RetentionSettings")?.GetValue(command) as IReadOnlyCollection<SaveBusinessPartnerRetentionSettingData> ?? [];
}
