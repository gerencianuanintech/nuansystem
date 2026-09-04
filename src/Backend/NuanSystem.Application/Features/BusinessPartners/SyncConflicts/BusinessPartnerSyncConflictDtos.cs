using System.Globalization;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.BusinessPartners.Sync;

namespace NuanSystem.Application.Features.BusinessPartners.SyncConflicts;

public sealed record BusinessPartnerSyncConflictDto(
    long Id,
    Guid ProposalEventId,
    int? BusinessPartnerId,
    Guid BusinessPartnerGlobalId,
    int OriginCompanyId,
    string? Code,
    string? Name,
    long BaseCanonicalVersion,
    long CurrentCanonicalVersion,
    IReadOnlyCollection<BusinessPartnerSyncConflictDifferenceDto> Differences,
    string Status,
    string? Resolution,
    string? ResolutionReason,
    int? CreatedByUserId,
    string? CreatedByUserName,
    DateTime CreatedAt,
    int? ResolvedByUserId,
    string? ResolvedByUserName,
    DateTime? ResolvedAt,
    string RowVersion);

public sealed record BusinessPartnerSyncConflictDifferenceDto(
    string FieldPath,
    string? BaseValue,
    string? ProposedValue,
    string? CentralValue);

internal static class BusinessPartnerSyncConflictMapper
{
    public static BusinessPartnerSyncConflictDto ToDto(BusinessPartnerSyncConflictRecord conflict) => new(
        conflict.Id,
        conflict.ProposalEventId,
        conflict.BusinessPartnerId,
        conflict.BusinessPartnerGlobalId,
        conflict.OriginCompanyId,
        conflict.Code,
        conflict.Name,
        conflict.BaseCanonicalVersion,
        conflict.CurrentCanonicalVersion,
        conflict.ConflictFields.Select(path => new BusinessPartnerSyncConflictDifferenceDto(
            path,
            BusinessPartnerSyncConflictPaths.DisplayValue(conflict.Base, path),
            BusinessPartnerSyncConflictPaths.DisplayValue(conflict.Proposed, path),
            BusinessPartnerSyncConflictPaths.DisplayValue(conflict.Canonical, path))).ToArray(),
        conflict.Status,
        conflict.Resolution,
        conflict.ResolutionReason,
        conflict.CreatedByUserId,
        conflict.CreatedByUserName,
        conflict.CreatedAt,
        conflict.ResolvedByUserId,
        conflict.ResolvedByUserName,
        conflict.ResolvedAt,
        Convert.ToBase64String(conflict.RowVersion));
}

internal static class BusinessPartnerSyncConflictPaths
{
    public static bool TryApply(
        BusinessPartnerCanonicalSnapshot current,
        BusinessPartnerCanonicalSnapshot proposed,
        IReadOnlyCollection<string> conflictFields,
        out BusinessPartnerCanonicalSnapshot resolved)
    {
        resolved = current;
        foreach (var path in conflictFields)
        {
            if (!TryApplyPath(resolved, proposed, path, out resolved))
            {
                resolved = current;
                return false;
            }
        }

        return true;
    }

    public static string? DisplayValue(BusinessPartnerCanonicalSnapshot? snapshot, string path)
    {
        if (snapshot is null)
        {
            return null;
        }

        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return parts[0] switch
            {
                "GlobalId" => snapshot.GlobalId.ToString("D"),
                "Code" => snapshot.Code,
                "Name" => snapshot.Name,
                "CommercialName" => snapshot.CommercialName,
                "PartnerType" => snapshot.PartnerType,
                "IdentificationTypeCode" => snapshot.IdentificationTypeCode,
                "IdentificationNumber" => snapshot.IdentificationNumber,
                "NormalizedIdentificationNumber" => snapshot.NormalizedIdentificationNumber,
                "Email" => snapshot.Email,
                "Phone" => snapshot.Phone,
                "SapCardCode" => snapshot.SapCardCode,
                "IsActive" => snapshot.IsActive.ToString(CultureInfo.InvariantCulture),
                _ => null
            };
        }

        if (parts.Length is 2 or 3 && Guid.TryParse(parts[1], out var childId))
        {
            return parts[0] switch
            {
                "Addresses" => DisplayAddress(snapshot.Addresses.SingleOrDefault(item => item.GlobalId == childId), parts),
                "Contacts" => DisplayContact(snapshot.Contacts.SingleOrDefault(item => item.GlobalId == childId), parts),
                _ => null
            };
        }

        return null;
    }

    private static bool TryApplyPath(
        BusinessPartnerCanonicalSnapshot current,
        BusinessPartnerCanonicalSnapshot proposed,
        string path,
        out BusinessPartnerCanonicalSnapshot resolved)
    {
        resolved = path switch
        {
            "GlobalId" when current.GlobalId == proposed.GlobalId => current,
            "Name" => current with { Name = proposed.Name },
            "CommercialName" => current with { CommercialName = proposed.CommercialName },
            "Email" => current with { Email = proposed.Email },
            "Phone" => current with { Phone = proposed.Phone },
            _ => current
        };

        if (!ReferenceEquals(resolved, current) || path == "GlobalId")
        {
            return true;
        }

        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length is not (2 or 3) || !Guid.TryParse(parts[1], out var childId))
        {
            return false;
        }

        if (parts[0] == "Addresses")
        {
            return TryApplyAddress(current, proposed, childId, parts, out resolved);
        }

        if (parts[0] == "Contacts")
        {
            return TryApplyContact(current, proposed, childId, parts, out resolved);
        }

        return false;
    }

    private static bool TryApplyAddress(
        BusinessPartnerCanonicalSnapshot current,
        BusinessPartnerCanonicalSnapshot proposed,
        Guid childId,
        string[] parts,
        out BusinessPartnerCanonicalSnapshot resolved)
    {
        resolved = current;
        var currentItem = current.Addresses.SingleOrDefault(item => item.GlobalId == childId);
        var proposedItem = proposed.Addresses.SingleOrDefault(item => item.GlobalId == childId);
        if (parts.Length == 2)
        {
            resolved = current with { Addresses = ReplaceChild(current.Addresses, proposedItem, childId) };
            return true;
        }

        if (currentItem is null || proposedItem is null)
        {
            return false;
        }

        var item = parts[2] switch
        {
            "AddressType" => currentItem with { AddressType = proposedItem.AddressType },
            "Line1" => currentItem with { Line1 = proposedItem.Line1 },
            "Line2" => currentItem with { Line2 = proposedItem.Line2 },
            "CountryCode" => currentItem with { CountryCode = proposedItem.CountryCode },
            "ProvinceCode" => currentItem with { ProvinceCode = proposedItem.ProvinceCode },
            "CityCode" => currentItem with { CityCode = proposedItem.CityCode },
            "PostalCode" => currentItem with { PostalCode = proposedItem.PostalCode },
            "Latitude" => currentItem with { Latitude = proposedItem.Latitude },
            "Longitude" => currentItem with { Longitude = proposedItem.Longitude },
            "IsPrimary" => currentItem with { IsPrimary = proposedItem.IsPrimary },
            "IsActive" => currentItem with { IsActive = proposedItem.IsActive },
            _ => null
        };
        if (item is null)
        {
            return false;
        }

        resolved = current with { Addresses = ReplaceChild(current.Addresses, item, childId) };
        return true;
    }

    private static bool TryApplyContact(
        BusinessPartnerCanonicalSnapshot current,
        BusinessPartnerCanonicalSnapshot proposed,
        Guid childId,
        string[] parts,
        out BusinessPartnerCanonicalSnapshot resolved)
    {
        resolved = current;
        var currentItem = current.Contacts.SingleOrDefault(item => item.GlobalId == childId);
        var proposedItem = proposed.Contacts.SingleOrDefault(item => item.GlobalId == childId);
        if (parts.Length == 2)
        {
            resolved = current with { Contacts = ReplaceChild(current.Contacts, proposedItem, childId) };
            return true;
        }

        if (currentItem is null || proposedItem is null)
        {
            return false;
        }

        var item = parts[2] switch
        {
            "ContactTypeCode" => currentItem with { ContactTypeCode = proposedItem.ContactTypeCode },
            "ContactChannelCode" => currentItem with { ContactChannelCode = proposedItem.ContactChannelCode },
            "Name" => currentItem with { Name = proposedItem.Name },
            "Position" => currentItem with { Position = proposedItem.Position },
            "Department" => currentItem with { Department = proposedItem.Department },
            "Phone" => currentItem with { Phone = proposedItem.Phone },
            "Extension" => currentItem with { Extension = proposedItem.Extension },
            "Mobile" => currentItem with { Mobile = proposedItem.Mobile },
            "Email" => currentItem with { Email = proposedItem.Email },
            "Language" => currentItem with { Language = proposedItem.Language },
            "ReceivesNotifications" => currentItem with { ReceivesNotifications = proposedItem.ReceivesNotifications },
            "IsPrimary" => currentItem with { IsPrimary = proposedItem.IsPrimary },
            "IsActive" => currentItem with { IsActive = proposedItem.IsActive },
            "Notes" => currentItem with { Notes = proposedItem.Notes },
            _ => null
        };
        if (item is null)
        {
            return false;
        }

        resolved = current with { Contacts = ReplaceChild(current.Contacts, item, childId) };
        return true;
    }

    private static IReadOnlyCollection<T> ReplaceChild<T>(
        IReadOnlyCollection<T> current,
        T? proposed,
        Guid childId)
        where T : class
    {
        var remaining = current.Where(item => ChildId(item) != childId).ToList();
        if (proposed is not null)
        {
            remaining.Add(proposed);
        }

        return remaining.OrderBy(ChildId).ToArray();
    }

    private static Guid ChildId<T>(T item) => item switch
    {
        BusinessPartnerAddressSnapshot address => address.GlobalId,
        BusinessPartnerContactSnapshot contact => contact.GlobalId,
        _ => throw new InvalidOperationException("Tipo de hijo BusinessPartner no soportado.")
    };

    private static string? DisplayAddress(BusinessPartnerAddressSnapshot? item, string[] parts)
    {
        if (item is null)
        {
            return null;
        }

        if (parts.Length == 2)
        {
            return string.Join(" | ", new[]
            {
                $"GlobalId={item.GlobalId:D}",
                $"AddressType={DisplayString(item.AddressType)}",
                $"Line1={DisplayString(item.Line1)}",
                $"Line2={DisplayString(item.Line2)}",
                $"CountryCode={DisplayString(item.CountryCode)}",
                $"ProvinceCode={DisplayString(item.ProvinceCode)}",
                $"CityCode={DisplayString(item.CityCode)}",
                $"PostalCode={DisplayString(item.PostalCode)}",
                $"Latitude={DisplayDecimal(item.Latitude)}",
                $"Longitude={DisplayDecimal(item.Longitude)}",
                $"IsPrimary={DisplayBoolean(item.IsPrimary)}",
                $"IsActive={DisplayBoolean(item.IsActive)}"
            });
        }

        return parts[2] switch
        {
            "AddressType" => item.AddressType,
            "Line1" => item.Line1,
            "Line2" => item.Line2,
            "CountryCode" => item.CountryCode,
            "ProvinceCode" => item.ProvinceCode,
            "CityCode" => item.CityCode,
            "PostalCode" => item.PostalCode,
            "Latitude" => item.Latitude?.ToString(CultureInfo.InvariantCulture),
            "Longitude" => item.Longitude?.ToString(CultureInfo.InvariantCulture),
            "IsPrimary" => item.IsPrimary.ToString(CultureInfo.InvariantCulture),
            "IsActive" => item.IsActive.ToString(CultureInfo.InvariantCulture),
            _ => null
        };
    }

    private static string? DisplayContact(BusinessPartnerContactSnapshot? item, string[] parts)
    {
        if (item is null)
        {
            return null;
        }

        if (parts.Length == 2)
        {
            return string.Join(" | ", new[]
            {
                $"GlobalId={item.GlobalId:D}",
                $"ContactTypeCode={DisplayString(item.ContactTypeCode)}",
                $"ContactChannelCode={DisplayString(item.ContactChannelCode)}",
                $"Name={DisplayString(item.Name)}",
                $"Position={DisplayString(item.Position)}",
                $"Department={DisplayString(item.Department)}",
                $"Phone={DisplayString(item.Phone)}",
                $"Extension={DisplayString(item.Extension)}",
                $"Mobile={DisplayString(item.Mobile)}",
                $"Email={DisplayString(item.Email)}",
                $"Language={DisplayString(item.Language)}",
                $"ReceivesNotifications={DisplayBoolean(item.ReceivesNotifications)}",
                $"IsPrimary={DisplayBoolean(item.IsPrimary)}",
                $"IsActive={DisplayBoolean(item.IsActive)}",
                $"Notes={DisplayString(item.Notes)}"
            });
        }

        return parts[2] switch
        {
            "ContactTypeCode" => item.ContactTypeCode,
            "ContactChannelCode" => item.ContactChannelCode,
            "Name" => item.Name,
            "Position" => item.Position,
            "Department" => item.Department,
            "Phone" => item.Phone,
            "Extension" => item.Extension,
            "Mobile" => item.Mobile,
            "Email" => item.Email,
            "Language" => item.Language,
            "ReceivesNotifications" => item.ReceivesNotifications.ToString(CultureInfo.InvariantCulture),
            "IsPrimary" => item.IsPrimary.ToString(CultureInfo.InvariantCulture),
            "IsActive" => item.IsActive.ToString(CultureInfo.InvariantCulture),
            "Notes" => item.Notes,
            _ => null
        };
    }

    private static string DisplayString(string? value) => value switch
    {
        null => "<null>",
        "" => "<empty>",
        _ => value
    };

    private static string DisplayDecimal(decimal? value) => value?.ToString(CultureInfo.InvariantCulture) ?? "<null>";

    private static string DisplayBoolean(bool value) => value ? "true" : "false";
}
