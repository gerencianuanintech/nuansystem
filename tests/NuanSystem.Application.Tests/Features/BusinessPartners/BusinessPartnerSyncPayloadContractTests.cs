using System.Text.Json.Nodes;
using FluentAssertions;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSyncPayloadContractTests
{
    private static readonly Guid PartnerId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid LowerAddressId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid HigherAddressId = Guid.Parse("20000000-0000-0000-0000-000000000002");
    private static readonly Guid LowerContactId = Guid.Parse("30000000-0000-0000-0000-000000000001");
    private static readonly Guid HigherContactId = Guid.Parse("30000000-0000-0000-0000-000000000002");
    private static readonly Guid CausationId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    private readonly BusinessPartnerSnapshotFactory snapshotFactory = new();
    private readonly SyncEventPayloadFactory payloadFactory = new();

    [Fact]
    public void Create_MapsStableReferenceCodesAndSortsChildrenWithoutMutatingSource()
    {
        var addresses = new[]
        {
            Address(HigherAddressId, "S", "EC", "PIC", "UIO"),
            Address(LowerAddressId, "B", "EC", "AZU", "CUE")
        };
        var contacts = new[]
        {
            Contact(HigherContactId, "BUYER", "MOBILE"),
            Contact(LowerContactId, "OWNER", "EMAIL")
        };
        var source = Partner(addresses, contacts);

        var snapshot = snapshotFactory.Create(source);

        snapshot.IdentificationTypeCode.Should().Be("RUC");
        snapshot.Addresses.Select(address => address.GlobalId)
            .Should().Equal(LowerAddressId, HigherAddressId);
        snapshot.Addresses.First().Should().Match<BusinessPartnerAddressSnapshot>(address =>
            address.CountryCode == "EC"
            && address.ProvinceCode == "AZU"
            && address.CityCode == "CUE");
        snapshot.Contacts.Select(contact => contact.GlobalId)
            .Should().Equal(LowerContactId, HigherContactId);
        snapshot.Contacts.First().Should().Match<BusinessPartnerContactSnapshot>(contact =>
            contact.ContactTypeCode == "OWNER"
            && contact.ContactChannelCode == "EMAIL");
        addresses.Select(address => address.GlobalId)
            .Should().Equal(HigherAddressId, LowerAddressId);
        contacts.Select(contact => contact.GlobalId)
            .Should().Equal(HigherContactId, LowerContactId);
        ReferenceEquals(snapshot.Addresses, addresses).Should().BeFalse();
        ReferenceEquals(snapshot.Contacts, contacts).Should().BeFalse();
    }

    [Fact]
    public void Create_RejectsMissingStableIdentificationTypeCode()
    {
        var source = Partner([], []);
        source.IdentificationTypeCode = " ";

        var action = () => snapshotFactory.Create(source);

        action.Should().Throw<ArgumentException>()
            .WithParameterName("partner");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Create_RejectsEmptyChildGlobalId(bool address)
    {
        var source = address
            ? Partner([Address(Guid.Empty, "B", "EC", "AZU", "CUE")], [])
            : Partner([], [Contact(Guid.Empty, "OWNER", "EMAIL")]);

        var action = () => snapshotFactory.Create(source);

        action.Should().Throw<ArgumentException>()
            .WithParameterName("partner");
    }

    [Fact]
    public void ProductionSerialization_IsDeterministicAndOmitsLocalOrProtectedData()
    {
        var first = SensitivePartner(
            [
                Address(HigherAddressId, "S", "EC", "PIC", "UIO"),
                Address(LowerAddressId, "B", "EC", "AZU", "CUE")
            ],
            [
                Contact(HigherContactId, "BUYER", "MOBILE"),
                Contact(LowerContactId, "OWNER", "EMAIL")
            ]);
        var second = SensitivePartner(
            [
                Address(LowerAddressId, "B", "EC", "AZU", "CUE"),
                Address(HigherAddressId, "S", "EC", "PIC", "UIO")
            ],
            [
                Contact(LowerContactId, "OWNER", "EMAIL"),
                Contact(HigherContactId, "BUYER", "MOBILE")
            ]);

        var firstJson = SerializeCanonical(first);
        var secondJson = SerializeCanonical(second);

        firstJson.Should().Be(secondJson);
        var root = JsonNode.Parse(firstJson)!.AsObject();
        var payload = root["payload"]!.AsObject();
        payload["schemaVersion"]!.GetValue<int>().Should().Be(2);
        payload["partner"]!["globalId"]!.GetValue<Guid>().Should().Be(PartnerId);
        payload["partner"]!["sapCardCode"]!.GetValue<string>().Should().Be("CN0999999999001");

        var partner = payload["partner"]!.AsObject();
        partner.ContainsKey("id").Should().BeFalse();
        partner.ContainsKey("rowVersion").Should().BeFalse();
        var address = partner["addresses"]!.AsArray()[0]!.AsObject();
        address.ContainsKey("countryId").Should().BeFalse();
        address.ContainsKey("provinceId").Should().BeFalse();
        address.ContainsKey("cityId").Should().BeFalse();
        var contact = partner["contacts"]!.AsArray()[0]!.AsObject();
        contact.ContainsKey("contactTypeId").Should().BeFalse();
        contact.ContainsKey("contactChannelId").Should().BeFalse();

        foreach (var forbidden in new[]
                 {
                     "Password", "ConnectionString", "BankAccounts", "BANK-ACCOUNT-SECRET",
                     "CustomerAccount", "ACCOUNTING-SECRET", "RetentionSettings", "RETENTION-SECRET",
                     "RowVersion", "ROWVERSION-SECRET", "SapCompanyCode", "SAP-CONNECTION-SECRET"
                 })
        {
            firstJson.Contains(forbidden, StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        }
    }

    [Fact]
    public void VersionedContracts_SerializeProposalCanonicalAndResultSchemasThroughProductionFactory()
    {
        var snapshot = snapshotFactory.Create(Partner([], []));
        var proposal = new BusinessPartnerProposalPayloadV1(
            BusinessPartnerSyncSchemaVersions.Proposal,
            PartnerId,
            snapshot.Code,
            snapshot.PartnerType,
            snapshot.IdentificationTypeCode,
            snapshot.IdentificationNumber,
            snapshot.NormalizedIdentificationNumber,
            7,
            81,
            "branch-user",
            snapshot,
            snapshot with { Name = "Proposed" },
            ["Name"]);
        var canonical = new BusinessPartnerCanonicalPayloadV2(
            BusinessPartnerSyncSchemaVersions.Canonical,
            8,
            10,
            CausationId,
            snapshot);
        var result = new BusinessPartnerProposalResultPayloadV1(
            BusinessPartnerSyncSchemaVersions.ProposalResult,
            PartnerId,
            CausationId,
            "Accepted",
            null,
            8,
            snapshot);

        PayloadSchema(Serialize("BusinessPartnerProposal", proposal)).Should().Be(1);
        PayloadSchema(Serialize("BusinessPartner", canonical)).Should().Be(2);
        PayloadSchema(Serialize("BusinessPartnerProposalResult", result)).Should().Be(1);
    }

    private string SerializeCanonical(BusinessPartnerDto partner) =>
        Serialize(
            "BusinessPartner",
            new BusinessPartnerCanonicalPayloadV2(
                BusinessPartnerSyncSchemaVersions.Canonical,
                8,
                10,
                CausationId,
                snapshotFactory.Create(partner)));

    private string Serialize(string entityName, object payload) =>
        payloadFactory.CreatePayloadJson(
            new SyncPublishRequest(
                10,
                entityName,
                PartnerId,
                "BP-10000000000000000000000000000001",
                SyncOperation.Updated,
                payload,
                SourceSystem: null,
                SourceReference: null));

    private static int PayloadSchema(string json) =>
        JsonNode.Parse(json)!["payload"]!["schemaVersion"]!.GetValue<int>();

    private static BusinessPartnerDto Partner(
        IReadOnlyCollection<BusinessPartnerAddressDto> addresses,
        IReadOnlyCollection<BusinessPartnerContactDto> contacts) =>
        new()
        {
            Id = 17,
            GlobalId = PartnerId,
            Code = "BP-10000000000000000000000000000001",
            Name = "Partner",
            CommercialName = "Trade",
            PartnerType = "Customer",
            IdentificationTypeId = 5,
            IdentificationTypeCode = "RUC",
            IdentificationNumber = "09.999-999 99001",
            NormalizedIdentificationNumber = "0999999999001",
            Email = "partner@example.test",
            Phone = "111",
            SapCardCode = "CN0999999999001",
            IsActive = true,
            Addresses = addresses,
            Contacts = contacts
        };

    private static BusinessPartnerDto SensitivePartner(
        IReadOnlyCollection<BusinessPartnerAddressDto> addresses,
        IReadOnlyCollection<BusinessPartnerContactDto> contacts)
    {
        var partner = Partner(addresses, contacts);
        partner.RowVersion = "ROWVERSION-SECRET";
        partner.Remarks = "Password=secret;ConnectionString=secret";
        partner.ExternalCode = "EXTERNAL-SECRET";
        partner.CustomerAccountCode = "ACCOUNTING-SECRET";
        partner.CreditLimit = 999999m;
        partner.SapCompanyCode = "SAP-CONNECTION-SECRET";
        partner.BankAccounts =
        [
            new BusinessPartnerBankAccountDto(
                Id: 1,
                BusinessPartnerId: 17,
                BankId: 2,
                BankAccountTypeId: 3,
                BankName: "Bank",
                AccountType: "Checking",
                AccountNumber: "BANK-ACCOUNT-SECRET",
                HolderName: "Holder",
                HolderIdentification: "0999999999001",
                CurrencyCode: "USD",
                SwiftCode: null,
                AbaRoutingCode: null,
                Iban: null,
                BankCountry: "EC",
                BankCity: "CUE",
                Notes: null,
                IsPrimary: true,
                IsActive: true)
        ];
        partner.RetentionSettings =
        [
            new BusinessPartnerRetentionSettingDto(
                Id: 1,
                BusinessPartnerId: 17,
                RetentionTypeId: 2,
                RetentionConceptId: 3,
                TaxSupportId: 4,
                RetentionType: "RETENTION-SECRET",
                SriCode: "001",
                Percent: 1m,
                EntryAccountId: 5,
                TaxSupport: "01",
                AppliesIva: true,
                AppliesIncome: false,
                IsCurrent: true,
                Notes: null)
        ];
        return partner;
    }

    private static BusinessPartnerAddressDto Address(
        Guid globalId,
        string type,
        string countryCode,
        string provinceCode,
        string cityCode) =>
        new()
        {
            Id = 500,
            GlobalId = globalId,
            BusinessPartnerId = 17,
            CountryId = 10,
            ProvinceId = 20,
            CityId = 30,
            AddressType = type,
            Line1 = $"Street {type}",
            CountryCode = countryCode,
            ProvinceCode = provinceCode,
            CityCode = cityCode,
            PostalCode = "010101",
            IsPrimary = type == "B",
            IsActive = true
        };

    private static BusinessPartnerContactDto Contact(
        Guid globalId,
        string typeCode,
        string channelCode) =>
        new()
        {
            Id = 600,
            GlobalId = globalId,
            BusinessPartnerId = 17,
            ContactTypeId = 40,
            ContactTypeCode = typeCode,
            ContactChannelId = 50,
            ContactChannelCode = channelCode,
            Name = $"Contact {typeCode}",
            Email = $"{typeCode.ToLowerInvariant()}@example.test",
            ReceivesNotifications = true,
            IsPrimary = typeCode == "OWNER",
            IsActive = true
        };
}
