using FluentAssertions;
using NuanSystem.Application.Features.BusinessPartners.Sync;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerThreeWayMergeTests
{
    private static readonly Guid PartnerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid AddressId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid ContactId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    private readonly BusinessPartnerThreeWayMergeService service = new();

    [Fact]
    public void Merge_AcceptsDisjointBranchAndCentralScalarChanges()
    {
        var @base = Snapshot(name: "Base", phone: "111");
        var proposed = Snapshot(name: "Sucursal", phone: "111");
        var current = Snapshot(name: "Base", phone: "222");

        var result = service.Merge(@base, proposed, current);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Accepted);
        result.Merged!.Name.Should().Be("Sucursal");
        result.Merged.Phone.Should().Be("222");
        result.ConflictFields.Should().BeEmpty();
    }

    [Fact]
    public void Merge_PreservesCentralScalarChangeWhenProposalLeavesBaseValue()
    {
        var @base = Snapshot(commercialName: "Base trade name");
        var proposed = Snapshot(commercialName: "Base trade name");
        var current = Snapshot(commercialName: "Central trade name");

        var result = service.Merge(@base, proposed, current);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Accepted);
        result.Merged!.CommercialName.Should().Be("Central trade name");
    }

    [Fact]
    public void Merge_AcceptsWhenBranchAndCentralChooseSameValue()
    {
        var @base = Snapshot(email: "base@example.test");
        var proposed = Snapshot(email: "shared@example.test");
        var current = Snapshot(email: "shared@example.test");

        var result = service.Merge(@base, proposed, current);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Accepted);
        result.Merged!.Email.Should().Be("shared@example.test");
        result.ConflictFields.Should().BeEmpty();
    }

    [Fact]
    public void Merge_ReportsSameScalarWithDifferentValues()
    {
        var @base = Snapshot(name: "Base");
        var proposed = Snapshot(name: "Sucursal");
        var current = Snapshot(name: "Central");

        var result = service.Merge(@base, proposed, current);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Conflict);
        result.Merged.Should().BeNull();
        result.ConflictFields.Should().ContainSingle().Which.Should().Be("Name");
    }

    [Fact]
    public void Merge_MergesDisjointChangesWithinAddressByGlobalId()
    {
        var baseAddress = Address(line1: "Base", postalCode: "010101");
        var @base = Snapshot(addresses: [baseAddress]);
        var proposed = Snapshot(addresses: [baseAddress with { Line1 = "Sucursal" }]);
        var current = Snapshot(addresses: [baseAddress with { PostalCode = "020202" }]);

        var result = service.Merge(@base, proposed, current);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Accepted);
        result.Merged!.Addresses.Should().ContainSingle().Which.Should().Be(
            baseAddress with { Line1 = "Sucursal", PostalCode = "020202" });
    }

    [Fact]
    public void Merge_ReportsSameContactFieldUsingStableChildPath()
    {
        var baseContact = Contact(email: "base@example.test");
        var @base = Snapshot(contacts: [baseContact]);
        var proposed = Snapshot(contacts: [baseContact with { Email = "branch@example.test" }]);
        var current = Snapshot(contacts: [baseContact with { Email = "central@example.test" }]);

        var result = service.Merge(@base, proposed, current);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Conflict);
        result.ConflictFields.Should().ContainSingle().Which.Should().Be(
            $"Contacts/{ContactId:N}/Email");
    }

    [Theory]
    [InlineData("GlobalId")]
    [InlineData("Code")]
    [InlineData("PartnerType")]
    [InlineData("IdentificationTypeCode")]
    [InlineData("IdentificationNumber")]
    [InlineData("NormalizedIdentificationNumber")]
    [InlineData("SapCardCode")]
    [InlineData("IsActive")]
    public void Merge_RejectsProtectedProposalFieldChanges(string field)
    {
        var @base = Snapshot();
        var proposed = ChangeProtectedField(@base, field);

        var result = service.Merge(@base, proposed, @base);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Rejected);
        result.ErrorCode.Should().Be("BP_PROTECTED_FIELD");
        result.Merged.Should().BeNull();
        result.ConflictFields.Should().ContainSingle().Which.Should().Be(field);
    }

    [Fact]
    public void Merge_ReportsChildPathWhenBranchRemovesChildChangedByCentral()
    {
        var baseAddress = Address(line1: "Base");
        var @base = Snapshot(addresses: [baseAddress]);
        var proposed = Snapshot(addresses: []);
        var current = Snapshot(addresses: [baseAddress with { Line1 = "Central" }]);

        var result = service.Merge(@base, proposed, current);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Conflict);
        result.ConflictFields.Should().ContainSingle().Which.Should().Be(
            $"Addresses/{AddressId:N}");
    }

    [Fact]
    public void Merge_PreservesDisjointChildAddsAndOrdersThemByGlobalId()
    {
        var lowerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var higherId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var @base = Snapshot(addresses: []);
        var proposed = Snapshot(addresses: [Address(higherId, line1: "Branch")]);
        var current = Snapshot(addresses: [Address(lowerId, line1: "Central")]);

        var result = service.Merge(@base, proposed, current);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Accepted);
        result.Merged!.Addresses.Select(address => address.GlobalId)
            .Should().Equal(lowerId, higherId);
    }

    [Fact]
    public void Merge_DoesNotMutateInputCollections()
    {
        var higherId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var lowerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var proposedAddresses = new List<BusinessPartnerAddressSnapshot>
        {
            Address(higherId),
            Address(lowerId)
        };
        var @base = Snapshot(addresses: []);
        var proposed = Snapshot(addresses: proposedAddresses);
        var current = Snapshot(addresses: []);

        var result = service.Merge(@base, proposed, current);

        result.Status.Should().Be(BusinessPartnerMergeStatus.Accepted);
        proposedAddresses.Select(address => address.GlobalId).Should().Equal(higherId, lowerId);
        result.Merged!.Addresses.Select(address => address.GlobalId).Should().Equal(lowerId, higherId);
        result.Merged.Addresses.Should().NotBeSameAs(proposedAddresses);
    }

    private static BusinessPartnerCanonicalSnapshot Snapshot(
        string name = "Base",
        string? commercialName = "Trade",
        string? email = "base@example.test",
        string? phone = "111",
        IReadOnlyCollection<BusinessPartnerAddressSnapshot>? addresses = null,
        IReadOnlyCollection<BusinessPartnerContactSnapshot>? contacts = null) =>
        new(
            PartnerId,
            "BP-AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
            name,
            commercialName,
            "Customer",
            "RUC",
            "0999999999001",
            "0999999999001",
            email,
            phone,
            "CN0999999999001",
            true,
            addresses ?? [],
            contacts ?? []);

    private static BusinessPartnerAddressSnapshot Address(
        Guid? globalId = null,
        string line1 = "Main street",
        string? postalCode = "010101") =>
        new(
            globalId ?? AddressId,
            "Billing",
            line1,
            null,
            "EC",
            "AZU",
            "CUE",
            postalCode,
            null,
            null,
            true,
            true);

    private static BusinessPartnerContactSnapshot Contact(string? email = "contact@example.test") =>
        new(
            ContactId,
            "OWNER",
            "EMAIL",
            "Owner",
            null,
            null,
            null,
            null,
            null,
            email,
            "es",
            true,
            true,
            true,
            null);

    private static BusinessPartnerCanonicalSnapshot ChangeProtectedField(
        BusinessPartnerCanonicalSnapshot snapshot,
        string field) =>
        field switch
        {
            "GlobalId" => snapshot with { GlobalId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd") },
            "Code" => snapshot with { Code = "BP-CHANGED" },
            "PartnerType" => snapshot with { PartnerType = "Supplier" },
            "IdentificationTypeCode" => snapshot with { IdentificationTypeCode = "PASSPORT" },
            "IdentificationNumber" => snapshot with { IdentificationNumber = "AB123" },
            "NormalizedIdentificationNumber" => snapshot with { NormalizedIdentificationNumber = "AB123" },
            "SapCardCode" => snapshot with { SapCardCode = "CEAB123" },
            "IsActive" => snapshot with { IsActive = false },
            _ => throw new ArgumentOutOfRangeException(nameof(field), field, null)
        };
}
