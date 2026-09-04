namespace NuanSystem.Application.Features.BusinessPartners.Sync;

public sealed class BusinessPartnerThreeWayMergeService
{
    private const string ProtectedFieldErrorCode = "BP_PROTECTED_FIELD";

    public BusinessPartnerMergeResult Merge(
        BusinessPartnerCanonicalSnapshot @base,
        BusinessPartnerCanonicalSnapshot proposed,
        BusinessPartnerCanonicalSnapshot current)
    {
        ArgumentNullException.ThrowIfNull(@base);
        ArgumentNullException.ThrowIfNull(proposed);
        ArgumentNullException.ThrowIfNull(current);

        var protectedFields = GetProtectedProposalChanges(@base, proposed);
        if (protectedFields.Count > 0)
        {
            return new BusinessPartnerMergeResult(
                BusinessPartnerMergeStatus.Rejected,
                null,
                protectedFields,
                ProtectedFieldErrorCode);
        }

        var conflicts = new List<string>();
        var name = MergeValue("Name", @base.Name, proposed.Name, current.Name, conflicts);
        var commercialName = MergeValue(
            "CommercialName",
            @base.CommercialName,
            proposed.CommercialName,
            current.CommercialName,
            conflicts);
        var email = MergeValue("Email", @base.Email, proposed.Email, current.Email, conflicts);
        var phone = MergeValue("Phone", @base.Phone, proposed.Phone, current.Phone, conflicts);
        var addresses = MergeChildren(
            "Addresses",
            @base.Addresses,
            proposed.Addresses,
            current.Addresses,
            address => address.GlobalId,
            MergeAddress,
            conflicts);
        var contacts = MergeChildren(
            "Contacts",
            @base.Contacts,
            proposed.Contacts,
            current.Contacts,
            contact => contact.GlobalId,
            MergeContact,
            conflicts);

        var orderedConflicts = conflicts
            .Distinct(StringComparer.Ordinal)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        if (orderedConflicts.Length > 0)
        {
            return new BusinessPartnerMergeResult(
                BusinessPartnerMergeStatus.Conflict,
                null,
                orderedConflicts);
        }

        return new BusinessPartnerMergeResult(
            BusinessPartnerMergeStatus.Accepted,
            current with
            {
                Name = name,
                CommercialName = commercialName,
                Email = email,
                Phone = phone,
                Addresses = addresses,
                Contacts = contacts
            },
            []);
    }

    private static IReadOnlyCollection<string> GetProtectedProposalChanges(
        BusinessPartnerCanonicalSnapshot @base,
        BusinessPartnerCanonicalSnapshot proposed)
    {
        var changed = new List<string>();
        AddIfChanged("GlobalId", @base.GlobalId, proposed.GlobalId, changed);
        AddIfChanged("Code", @base.Code, proposed.Code, changed);
        AddIfChanged("PartnerType", @base.PartnerType, proposed.PartnerType, changed);
        AddIfChanged(
            "IdentificationTypeCode",
            @base.IdentificationTypeCode,
            proposed.IdentificationTypeCode,
            changed);
        AddIfChanged(
            "IdentificationNumber",
            @base.IdentificationNumber,
            proposed.IdentificationNumber,
            changed);
        AddIfChanged(
            "NormalizedIdentificationNumber",
            @base.NormalizedIdentificationNumber,
            proposed.NormalizedIdentificationNumber,
            changed);
        AddIfChanged("SapCardCode", @base.SapCardCode, proposed.SapCardCode, changed);
        AddIfChanged("IsActive", @base.IsActive, proposed.IsActive, changed);
        return changed.OrderBy(path => path, StringComparer.Ordinal).ToArray();
    }

    private static T MergeValue<T>(
        string path,
        T baseValue,
        T proposedValue,
        T currentValue,
        ICollection<string> conflicts)
    {
        var proposalChanged = !EqualityComparer<T>.Default.Equals(baseValue, proposedValue);
        var centralChanged = !EqualityComparer<T>.Default.Equals(baseValue, currentValue);
        if (proposalChanged
            && centralChanged
            && !EqualityComparer<T>.Default.Equals(proposedValue, currentValue))
        {
            conflicts.Add(path);
        }

        return proposalChanged ? proposedValue : currentValue;
    }

    private static void AddIfChanged<T>(
        string path,
        T baseValue,
        T proposedValue,
        ICollection<string> changed)
    {
        if (!EqualityComparer<T>.Default.Equals(baseValue, proposedValue))
        {
            changed.Add(path);
        }
    }

    private static IReadOnlyCollection<T> MergeChildren<T>(
        string collectionPath,
        IReadOnlyCollection<T> baseItems,
        IReadOnlyCollection<T> proposedItems,
        IReadOnlyCollection<T> currentItems,
        Func<T, Guid> globalId,
        Func<string, T, T, T, ICollection<string>, T> mergeExisting,
        ICollection<string> conflicts)
        where T : notnull
    {
        var baseById = IndexChildren(collectionPath, baseItems, globalId);
        var proposedById = IndexChildren(collectionPath, proposedItems, globalId);
        var currentById = IndexChildren(collectionPath, currentItems, globalId);
        var ids = baseById.Keys
            .Concat(proposedById.Keys)
            .Concat(currentById.Keys)
            .Distinct()
            .OrderBy(id => id)
            .ToArray();
        var merged = new List<T>(ids.Length);

        foreach (var id in ids)
        {
            var path = $"{collectionPath}/{id:N}";
            var hasBase = baseById.TryGetValue(id, out var baseItem);
            var hasProposed = proposedById.TryGetValue(id, out var proposedItem);
            var hasCurrent = currentById.TryGetValue(id, out var currentItem);

            if (!hasBase)
            {
                if (hasProposed && hasCurrent)
                {
                    if (EqualityComparer<T>.Default.Equals(proposedItem!, currentItem!))
                    {
                        merged.Add(proposedItem!);
                    }
                    else
                    {
                        conflicts.Add(path);
                    }
                }
                else if (hasProposed)
                {
                    merged.Add(proposedItem!);
                }
                else if (hasCurrent)
                {
                    merged.Add(currentItem!);
                }

                continue;
            }

            if (!hasProposed && !hasCurrent)
            {
                continue;
            }

            if (!hasProposed)
            {
                if (!EqualityComparer<T>.Default.Equals(baseItem!, currentItem!))
                {
                    conflicts.Add(path);
                }

                continue;
            }

            if (!hasCurrent)
            {
                if (!EqualityComparer<T>.Default.Equals(baseItem!, proposedItem!))
                {
                    conflicts.Add(path);
                }

                continue;
            }

            merged.Add(mergeExisting(path, baseItem!, proposedItem!, currentItem!, conflicts));
        }

        return merged.ToArray();
    }

    private static Dictionary<Guid, T> IndexChildren<T>(
        string collectionPath,
        IEnumerable<T> items,
        Func<T, Guid> globalId)
        where T : notnull
    {
        var result = new Dictionary<Guid, T>();
        foreach (var item in items)
        {
            var id = globalId(item);
            if (id == Guid.Empty || !result.TryAdd(id, item))
            {
                throw new ArgumentException(
                    $"{collectionPath} requiere GlobalId no vacio y unico para conciliacion.",
                    collectionPath);
            }
        }

        return result;
    }

    private static BusinessPartnerAddressSnapshot MergeAddress(
        string path,
        BusinessPartnerAddressSnapshot @base,
        BusinessPartnerAddressSnapshot proposed,
        BusinessPartnerAddressSnapshot current,
        ICollection<string> conflicts) =>
        current with
        {
            AddressType = MergeValue($"{path}/AddressType", @base.AddressType, proposed.AddressType, current.AddressType, conflicts),
            Line1 = MergeValue($"{path}/Line1", @base.Line1, proposed.Line1, current.Line1, conflicts),
            Line2 = MergeValue($"{path}/Line2", @base.Line2, proposed.Line2, current.Line2, conflicts),
            CountryCode = MergeValue($"{path}/CountryCode", @base.CountryCode, proposed.CountryCode, current.CountryCode, conflicts),
            ProvinceCode = MergeValue($"{path}/ProvinceCode", @base.ProvinceCode, proposed.ProvinceCode, current.ProvinceCode, conflicts),
            CityCode = MergeValue($"{path}/CityCode", @base.CityCode, proposed.CityCode, current.CityCode, conflicts),
            PostalCode = MergeValue($"{path}/PostalCode", @base.PostalCode, proposed.PostalCode, current.PostalCode, conflicts),
            Latitude = MergeValue($"{path}/Latitude", @base.Latitude, proposed.Latitude, current.Latitude, conflicts),
            Longitude = MergeValue($"{path}/Longitude", @base.Longitude, proposed.Longitude, current.Longitude, conflicts),
            IsPrimary = MergeValue($"{path}/IsPrimary", @base.IsPrimary, proposed.IsPrimary, current.IsPrimary, conflicts),
            IsActive = MergeValue($"{path}/IsActive", @base.IsActive, proposed.IsActive, current.IsActive, conflicts)
        };

    private static BusinessPartnerContactSnapshot MergeContact(
        string path,
        BusinessPartnerContactSnapshot @base,
        BusinessPartnerContactSnapshot proposed,
        BusinessPartnerContactSnapshot current,
        ICollection<string> conflicts) =>
        current with
        {
            ContactTypeCode = MergeValue($"{path}/ContactTypeCode", @base.ContactTypeCode, proposed.ContactTypeCode, current.ContactTypeCode, conflicts),
            ContactChannelCode = MergeValue($"{path}/ContactChannelCode", @base.ContactChannelCode, proposed.ContactChannelCode, current.ContactChannelCode, conflicts),
            Name = MergeValue($"{path}/Name", @base.Name, proposed.Name, current.Name, conflicts),
            Position = MergeValue($"{path}/Position", @base.Position, proposed.Position, current.Position, conflicts),
            Department = MergeValue($"{path}/Department", @base.Department, proposed.Department, current.Department, conflicts),
            Phone = MergeValue($"{path}/Phone", @base.Phone, proposed.Phone, current.Phone, conflicts),
            Extension = MergeValue($"{path}/Extension", @base.Extension, proposed.Extension, current.Extension, conflicts),
            Mobile = MergeValue($"{path}/Mobile", @base.Mobile, proposed.Mobile, current.Mobile, conflicts),
            Email = MergeValue($"{path}/Email", @base.Email, proposed.Email, current.Email, conflicts),
            Language = MergeValue($"{path}/Language", @base.Language, proposed.Language, current.Language, conflicts),
            ReceivesNotifications = MergeValue($"{path}/ReceivesNotifications", @base.ReceivesNotifications, proposed.ReceivesNotifications, current.ReceivesNotifications, conflicts),
            IsPrimary = MergeValue($"{path}/IsPrimary", @base.IsPrimary, proposed.IsPrimary, current.IsPrimary, conflicts),
            IsActive = MergeValue($"{path}/IsActive", @base.IsActive, proposed.IsActive, current.IsActive, conflicts),
            Notes = MergeValue($"{path}/Notes", @base.Notes, proposed.Notes, current.Notes, conflicts)
        };
}
