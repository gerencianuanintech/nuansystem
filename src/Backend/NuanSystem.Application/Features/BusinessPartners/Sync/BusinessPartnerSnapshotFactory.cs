using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Application.Features.BusinessPartners.Sync;

public sealed class BusinessPartnerSnapshotFactory
{
    public BusinessPartnerCanonicalSnapshot Create(BusinessPartnerDto partner)
    {
        ArgumentNullException.ThrowIfNull(partner);

        if (partner.GlobalId == Guid.Empty)
        {
            throw new ArgumentException("BusinessPartner requiere GlobalId para crear un snapshot.", nameof(partner));
        }

        if (string.IsNullOrWhiteSpace(partner.IdentificationTypeCode))
        {
            throw new ArgumentException(
                "BusinessPartner requiere IdentificationTypeCode estable para crear un snapshot.",
                nameof(partner));
        }

        EnsureStableChildIds(partner.Addresses.Select(address => address.GlobalId), "direccion", partner);
        EnsureStableChildIds(partner.Contacts.Select(contact => contact.GlobalId), "contacto", partner);

        var addresses = partner.Addresses
            .OrderBy(address => address.GlobalId)
            .Select(address => new BusinessPartnerAddressSnapshot(
                address.GlobalId,
                address.AddressType,
                address.Line1,
                address.Line2,
                address.CountryCode,
                address.ProvinceCode,
                address.CityCode,
                address.PostalCode,
                address.Latitude,
                address.Longitude,
                address.IsPrimary,
                address.IsActive))
            .ToArray();

        var contacts = partner.Contacts
            .OrderBy(contact => contact.GlobalId)
            .Select(contact => new BusinessPartnerContactSnapshot(
                contact.GlobalId,
                contact.ContactTypeCode,
                contact.ContactChannelCode,
                contact.Name,
                contact.Position,
                contact.Department,
                contact.Phone,
                contact.Extension,
                contact.Mobile,
                contact.Email,
                contact.Language,
                contact.ReceivesNotifications,
                contact.IsPrimary,
                contact.IsActive,
                contact.Notes))
            .ToArray();

        return new BusinessPartnerCanonicalSnapshot(
            partner.GlobalId,
            partner.Code,
            partner.Name,
            partner.CommercialName,
            partner.PartnerType,
            partner.IdentificationTypeCode,
            partner.IdentificationNumber,
            partner.NormalizedIdentificationNumber,
            partner.Email,
            partner.Phone,
            partner.SapCardCode,
            partner.IsActive,
            addresses,
            contacts);
    }

    private static void EnsureStableChildIds(
        IEnumerable<Guid> globalIds,
        string childName,
        BusinessPartnerDto partner)
    {
        var ids = globalIds.ToArray();
        if (ids.Any(id => id == Guid.Empty) || ids.Distinct().Count() != ids.Length)
        {
            throw new ArgumentException(
                $"Cada {childName} requiere un GlobalId estable y unico para crear un snapshot.",
                nameof(partner));
        }
    }
}
