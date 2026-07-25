using FluentValidation;
using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class CreateBusinessPartnerCommandValidator : AbstractValidator<CreateBusinessPartnerCommand>
{
    public CreateBusinessPartnerCommandValidator()
    {
        Include(new BusinessPartnerCommandRules<CreateBusinessPartnerCommand>());
    }
}

public sealed class UpdateBusinessPartnerCommandValidator : AbstractValidator<UpdateBusinessPartnerCommand>
{
    public UpdateBusinessPartnerCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        Include(new BusinessPartnerCommandRules<UpdateBusinessPartnerCommand>());
    }
}

public sealed class DeleteBusinessPartnerCommandValidator : AbstractValidator<DeleteBusinessPartnerCommand>
{
    public DeleteBusinessPartnerCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}

internal sealed class BusinessPartnerCommandRules<T> : AbstractValidator<T>
{
    private static readonly string[] PartnerTypes = ["Customer", "Supplier", "Both"];
    private static readonly string[] AddressTypes = ["Main", "Billing", "Shipping", "Other"];
    private static readonly string[] SapStatuses = ["Pending", "Synced", "Error"];

    public BusinessPartnerCommandRules()
    {
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.Code))).NotEmpty().MaximumLength(50);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.Name))).NotEmpty().MaximumLength(200);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.CommercialName))).MaximumLength(200);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.PartnerType))).NotEmpty().Must(PartnerTypes.Contains);
        RuleFor(command => GetInt(command, nameof(CreateBusinessPartnerCommand.IdentificationTypeId))).GreaterThan(0);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.IdentificationNumber))).NotEmpty().MaximumLength(50);
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
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.SapCardCode))).MaximumLength(50);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.SapCardType))).MaximumLength(1);
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.SapSyncStatus))).Must(value => string.IsNullOrWhiteSpace(value) || SapStatuses.Contains(value));
        RuleFor(command => GetString(command, nameof(CreateBusinessPartnerCommand.SapCardType)))
            .Must((command, value) => !string.Equals(GetString(command, nameof(CreateBusinessPartnerCommand.PartnerType)), "Supplier", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(value)
                || string.Equals(value, "S", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SAP CardType debe ser S para proveedores.");

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
