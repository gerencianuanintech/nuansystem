using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

internal static class BusinessPartnerSyncEventFactory
{
    public static SyncPublishRequest CreateProposal(
        int companyId,
        BusinessPartnerOutboxWriteRequest request,
        BusinessPartnerCanonicalSnapshot proposed,
        BusinessPartnerCanonicalSnapshot? @base)
    {
        var payload = new BusinessPartnerProposalPayloadV1(
            BusinessPartnerSyncSchemaVersions.Proposal,
            proposed.GlobalId,
            proposed.Code,
            proposed.PartnerType,
            proposed.IdentificationTypeCode,
            proposed.IdentificationNumber,
            proposed.NormalizedIdentificationNumber,
            @base is null ? 0 : request.Base!.CanonicalVersion,
            request.OriginUserId,
            Normalize(request.OriginUserName),
            @base,
            proposed,
            GetChangedFields(@base, proposed));

        return Create(companyId, SyncMasterBranchEntityCodes.BusinessPartnerProposal, request, payload);
    }

    public static SyncPublishRequest CreateCanonical(
        int companyId,
        BusinessPartnerOutboxWriteRequest request,
        BusinessPartnerCanonicalSnapshot current)
    {
        var payload = new BusinessPartnerCanonicalPayloadV2(
            BusinessPartnerSyncSchemaVersions.Canonical,
            request.Current.CanonicalVersion,
            OriginCompanyId: null,
            request.CausationEventId,
            current);

        return Create(companyId, SyncMasterBranchEntityCodes.BusinessPartner, request, payload);
    }

    private static SyncPublishRequest Create(
        int companyId,
        string entityName,
        BusinessPartnerOutboxWriteRequest request,
        object payload) => new(
            companyId,
            entityName,
            request.Current.GlobalId,
            request.Current.Code,
            request.Operation,
            payload,
            SourceSystem: null,
            SourceReference: request.Current.Id.ToString());

    private static IReadOnlyCollection<string> GetChangedFields(
        BusinessPartnerCanonicalSnapshot? @base,
        BusinessPartnerCanonicalSnapshot proposed)
    {
        if (@base is null)
        {
            return new[] { "Addresses", "CommercialName", "Contacts", "Email", "Name", "Phone" };
        }

        var changed = new List<string>();
        Add("GlobalId", @base.GlobalId, proposed.GlobalId);
        Add("Code", @base.Code, proposed.Code);
        Add("Name", @base.Name, proposed.Name);
        Add("CommercialName", @base.CommercialName, proposed.CommercialName);
        Add("PartnerType", @base.PartnerType, proposed.PartnerType);
        Add("IdentificationTypeCode", @base.IdentificationTypeCode, proposed.IdentificationTypeCode);
        Add("IdentificationNumber", @base.IdentificationNumber, proposed.IdentificationNumber);
        Add("NormalizedIdentificationNumber", @base.NormalizedIdentificationNumber, proposed.NormalizedIdentificationNumber);
        Add("Email", @base.Email, proposed.Email);
        Add("Phone", @base.Phone, proposed.Phone);
        Add("SapCardCode", @base.SapCardCode, proposed.SapCardCode);
        Add("IsActive", @base.IsActive, proposed.IsActive);
        AddChildren("Addresses", @base.Addresses, proposed.Addresses, address => address.GlobalId);
        AddChildren("Contacts", @base.Contacts, proposed.Contacts, contact => contact.GlobalId);
        return changed.Distinct(StringComparer.Ordinal).OrderBy(path => path, StringComparer.Ordinal).ToArray();

        void Add<T>(string path, T left, T right)
        {
            if (!EqualityComparer<T>.Default.Equals(left, right)) changed.Add(path);
        }

        void AddChildren<T>(string root, IReadOnlyCollection<T> left, IReadOnlyCollection<T> right, Func<T, Guid> id)
        {
            var leftById = left.ToDictionary(id);
            var rightById = right.ToDictionary(id);
            foreach (var childId in leftById.Keys.Concat(rightById.Keys).Distinct().OrderBy(value => value))
            {
                var path = $"{root}/{childId:N}";
                if (!leftById.TryGetValue(childId, out var oldItem)
                    || !rightById.TryGetValue(childId, out var newItem))
                {
                    changed.Add(path);
                    continue;
                }

                foreach (var property in typeof(T).GetProperties()
                             .Where(property => property.Name != "GlobalId")
                             .OrderBy(property => property.Name, StringComparer.Ordinal))
                {
                    if (!Equals(property.GetValue(oldItem), property.GetValue(newItem)))
                    {
                        changed.Add($"{path}/{property.Name}");
                    }
                }
            }
        }
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
