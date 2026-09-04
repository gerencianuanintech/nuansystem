using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.BusinessPartners.SyncConflicts;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Repositories.Sync;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSyncConflictLiveResolutionTests
{
    private static readonly Guid PartnerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid AddressId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ContactId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task FullAddressDisplay_IncludesEveryCanonicalFieldAndDistinguishesNullAndBooleanValues()
    {
        var snapshot = Snapshot(
            "Central",
            addresses:
            [
                new BusinessPartnerAddressSnapshot(
                    AddressId, "Billing", "Main 1", null, "EC", null, "UIO", "", null,
                    -78.501m, false, true)
            ]);

        var display = await DisplayThroughQueryAsync(snapshot, $"Addresses/{AddressId:N}");

        display.Should().Be(
            $"GlobalId={AddressId:D} | AddressType=Billing | Line1=Main 1 | Line2=<null> | " +
            "CountryCode=EC | ProvinceCode=<null> | CityCode=UIO | PostalCode=<empty> | " +
            "Latitude=<null> | Longitude=-78.501 | IsPrimary=false | IsActive=true");
    }

    [Fact]
    public async Task FullContactDisplay_IncludesEveryCanonicalFieldAndDistinguishesNullAndBooleanValues()
    {
        var snapshot = Snapshot(
            "Central",
            contacts:
            [
                new BusinessPartnerContactSnapshot(
                    ContactId, "SALES", null, "Ada", null, "Purchasing", "099", "", null,
                    "ada@example.com", "es-EC", false, true, false, null)
            ]);

        var display = await DisplayThroughQueryAsync(snapshot, $"Contacts/{ContactId:N}");

        display.Should().Be(
            $"GlobalId={ContactId:D} | ContactTypeCode=SALES | ContactChannelCode=<null> | Name=Ada | " +
            "Position=<null> | Department=Purchasing | Phone=099 | Extension=<empty> | Mobile=<null> | " +
            "Email=ada@example.com | Language=es-EC | ReceivesNotifications=false | " +
            "IsPrimary=true | IsActive=false | Notes=<null>");
    }

    [Fact]
    public void AcceptBranchPlan_OverlaysOnlySelectedFieldsOnLockedLiveCanonical()
    {
        var currentAddress = new BusinessPartnerAddressSnapshot(
            AddressId, "Billing", "Live line", "live line 2", "EC", "P", "C", "1701",
            -0.10m, -78.40m, true, true);
        var proposedAddress = currentAddress with
        {
            Line1 = "Branch line",
            PostalCode = "9999",
            Latitude = -0.20m,
            IsPrimary = false
        };
        var currentContact = new BusinessPartnerContactSnapshot(
            ContactId, "SALES", "EMAIL", "Ada", "Buyer", "Purchasing", "live-phone", "101",
            "live-mobile", "live@example.com", "es", false, true, true, "live-note");
        var proposedContact = currentContact with
        {
            Phone = "branch-phone",
            Extension = "202",
            Email = "branch@example.com",
            ReceivesNotifications = true,
            Notes = "branch-note"
        };
        var conflict = Conflict(
            Snapshot("Historical central"),
            Snapshot("Branch name", "Branch commercial", [proposedAddress], [proposedContact]),
            ["Name", $"Addresses/{AddressId:N}/Latitude", $"Contacts/{ContactId:N}/Extension"]);
        var live = new BusinessPartnerSyncConflictLiveCanonicalState(
            101,
            9,
            [9, 8, 7, 6, 5, 4, 3, 2],
            Snapshot("Live central", "Live commercial", [currentAddress], [currentContact]));
        var planner = new BusinessPartnerSyncConflictResolutionPlanner();

        var plan = planner.CreatePlan(10, conflict, live, "AcceptBranch", "accepted");

        plan.Should().NotBeNull();
        plan!.ExpectedBusinessPartnerId.Should().Be(101);
        plan.ExpectedCanonicalVersion.Should().Be(9);
        plan.ExpectedBusinessPartnerRowVersion.Should().Equal(9, 8, 7, 6, 5, 4, 3, 2);
        plan.ResolvedSnapshot.Should().BeEquivalentTo(live.Snapshot with
        {
            Name = "Branch name",
            Addresses = [currentAddress with { Latitude = -0.20m }],
            Contacts = [currentContact with { Extension = "202" }]
        });
        plan.OutboundEvent.TargetCompanyId.Should().BeNull();
        plan.OutboundEvent.CausationEventId.Should().Be(conflict.ProposalEventId);
        plan.OutboundEvent.PublishRequest.EntityName.Should().Be("BusinessPartner");
        plan.OutboundEvent.PublishRequest.Payload.Should().BeEquivalentTo(
            new BusinessPartnerCanonicalPayloadV2(
                BusinessPartnerSyncSchemaVersions.Canonical,
                10,
                20,
                conflict.ProposalEventId,
                plan.ResolvedSnapshot!));
    }

    [Fact]
    public void KeepCentralPlan_UsesLockedLiveCanonicalWithoutMutationAndRoutesToExactOrigin()
    {
        var conflict = Conflict(Snapshot("Historical central"), Snapshot("Branch"), ["Name"]);
        var live = new BusinessPartnerSyncConflictLiveCanonicalState(
            101,
            12,
            [8, 7, 6, 5, 4, 3, 2, 1],
            Snapshot("Live central", "Live changed after conflict"));
        var planner = new BusinessPartnerSyncConflictResolutionPlanner();

        var plan = planner.CreatePlan(10, conflict, live, "KeepCentral", "central remains");

        plan.Should().NotBeNull();
        plan!.ResolvedSnapshot.Should().BeNull();
        plan.ExpectedCanonicalVersion.Should().Be(12);
        plan.OutboundEvent.TargetCompanyId.Should().Be(20);
        plan.OutboundEvent.CausationEventId.Should().Be(conflict.ProposalEventId);
        plan.OutboundEvent.PublishRequest.EntityName.Should().Be("BusinessPartnerProposalResult");
        plan.OutboundEvent.PublishRequest.Payload.Should().BeEquivalentTo(
            new BusinessPartnerProposalResultPayloadV1(
                BusinessPartnerSyncSchemaVersions.ProposalResult,
                PartnerId,
                conflict.ProposalEventId,
                20,
                "Rejected",
                "central remains",
                12,
                live.Snapshot));
    }

    [Fact]
    public void AcceptBranchPlan_SupportsFullAddressAndContactAddsAndRemovalsDeterministically()
    {
        var removedAddressId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var addedAddressId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var removedContactId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var addedContactId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var keptAddress = Address(AddressId, "kept");
        var removedAddress = Address(removedAddressId, "removed");
        var addedAddress = Address(addedAddressId, "added");
        var keptContact = Contact(ContactId, "kept");
        var removedContact = Contact(removedContactId, "removed");
        var addedContact = Contact(addedContactId, "added");
        var live = new BusinessPartnerSyncConflictLiveCanonicalState(
            101,
            7,
            [1, 1, 1, 1, 1, 1, 1, 1],
            Snapshot("Live", addresses: [removedAddress, keptAddress], contacts: [removedContact, keptContact]));
        var conflict = Conflict(
            Snapshot("Historical"),
            Snapshot("Branch", addresses: [keptAddress, addedAddress], contacts: [keptContact, addedContact]),
            [
                $"Addresses/{removedAddressId:N}",
                $"Addresses/{addedAddressId:N}",
                $"Contacts/{removedContactId:N}",
                $"Contacts/{addedContactId:N}"
            ]);
        var planner = new BusinessPartnerSyncConflictResolutionPlanner();

        var plan = planner.CreatePlan(10, conflict, live, "AcceptBranch", "accepted");

        plan.Should().NotBeNull();
        plan!.ResolvedSnapshot!.Name.Should().Be("Live");
        plan.ResolvedSnapshot.Addresses.Should().Equal(keptAddress, addedAddress);
        plan.ResolvedSnapshot.Contacts.Should().Equal(keptContact, addedContact);
    }

    [Fact]
    public void AcceptResolutionProjection_ReturnsTheResolvedCanonicalDerivedFromLiveState()
    {
        var conflict = Conflict(Snapshot("Historical"), Snapshot("Branch"), ["Name"]);
        var live = new BusinessPartnerSyncConflictLiveCanonicalState(
            101,
            9,
            [1, 2, 3, 4, 5, 6, 7, 8],
            Snapshot("Live", "Live commercial"));
        var plan = new BusinessPartnerSyncConflictResolutionPlanner()
            .CreatePlan(10, conflict, live, "AcceptBranch", "accepted")!;

        var projected = BusinessPartnerSyncConflictRepository.ProjectResolvedRecord(
            conflict with { Status = "Resolved", Resolution = "AcceptBranch" },
            live,
            plan);

        projected.CurrentCanonicalVersion.Should().Be(10);
        projected.Canonical.Name.Should().Be("Branch");
        projected.Canonical.CommercialName.Should().Be("Live commercial");
        projected.Name.Should().Be("Branch");
    }

    [Fact]
    public void KeepResolutionProjection_ReturnsTheUnmodifiedLiveCanonical()
    {
        var conflict = Conflict(Snapshot("Historical"), Snapshot("Branch"), ["Name"]);
        var live = new BusinessPartnerSyncConflictLiveCanonicalState(
            101,
            12,
            [8, 7, 6, 5, 4, 3, 2, 1],
            Snapshot("Live", "Live commercial"));
        var plan = new BusinessPartnerSyncConflictResolutionPlanner()
            .CreatePlan(10, conflict, live, "KeepCentral", "central remains")!;

        var projected = BusinessPartnerSyncConflictRepository.ProjectResolvedRecord(
            conflict with { Status = "Resolved", Resolution = "KeepCentral" },
            live,
            plan);

        projected.CurrentCanonicalVersion.Should().Be(12);
        projected.Canonical.Should().BeEquivalentTo(live.Snapshot);
        projected.Name.Should().Be("Live");
    }

    [Fact]
    public void AcceptBranchPlan_RejectsAProtectedOrUnknownConflictPath()
    {
        var conflict = Conflict(Snapshot("Historical"), Snapshot("Branch"), ["SapCardCode"]);
        var live = new BusinessPartnerSyncConflictLiveCanonicalState(
            101,
            12,
            [8, 7, 6, 5, 4, 3, 2, 1],
            Snapshot("Live"));

        var plan = new BusinessPartnerSyncConflictResolutionPlanner()
            .CreatePlan(10, conflict, live, "AcceptBranch", "accepted");

        plan.Should().BeNull();
    }

    private static BusinessPartnerCanonicalSnapshot Snapshot(
        string name,
        string commercialName = "Central commercial",
        IReadOnlyCollection<BusinessPartnerAddressSnapshot>? addresses = null,
        IReadOnlyCollection<BusinessPartnerContactSnapshot>? contacts = null) => new(
        PartnerId,
        "BP-AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
        name,
        commercialName,
        "Customer",
        "RUC",
        "0999999999001",
        "0999999999001",
        "root@example.com",
        "0990000000",
        "CN0999999999001",
        true,
        addresses ?? [],
        contacts ?? []);

    private static async Task<string?> DisplayThroughQueryAsync(
        BusinessPartnerCanonicalSnapshot snapshot,
        string path)
    {
        var companyContext = Substitute.For<ICompanyContext>();
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            10, "CENTRAL", "Central", DatabaseEngine.SqlServer, "protected",
            SapIntegrationMode.None, IsMaster: true));
        var repository = Substitute.For<IBusinessPartnerSyncConflictRepository>();
        repository.ListAsync(10, "Open", Arg.Any<CancellationToken>()).Returns(
        [
            new BusinessPartnerSyncConflictRecord(
                81,
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                101,
                PartnerId,
                20,
                4,
                5,
                snapshot,
                snapshot,
                snapshot,
                [path],
                "Open",
                null,
                null,
                3,
                "branch-user",
                new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc),
                null,
                null,
                null,
                [1, 2, 3, 4, 5, 6, 7, 8],
                snapshot.Code,
                snapshot.Name)
        ]);
        var handler = new GetBusinessPartnerSyncConflictsQueryHandler(companyContext, repository);

        var result = await handler.Handle(
            new GetBusinessPartnerSyncConflictsQuery("Open"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        return result.Value!.Single().Differences.Single().ProposedValue;
    }

    private static BusinessPartnerSyncConflictRecord Conflict(
        BusinessPartnerCanonicalSnapshot historicalCanonical,
        BusinessPartnerCanonicalSnapshot proposed,
        IReadOnlyCollection<string> conflictFields) => new(
        81,
        Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
        101,
        PartnerId,
        20,
        4,
        5,
        Snapshot("Base"),
        proposed,
        historicalCanonical,
        conflictFields,
        "Open",
        null,
        null,
        3,
        "branch-user",
        new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc),
        null,
        null,
        null,
        [1, 2, 3, 4, 5, 6, 7, 8],
        historicalCanonical.Code,
        historicalCanonical.Name);

    private static BusinessPartnerAddressSnapshot Address(Guid id, string value) => new(
        id, "Billing", value, null, "EC", null, null, null, null, null, false, true);

    private static BusinessPartnerContactSnapshot Contact(Guid id, string value) => new(
        id, "SALES", "EMAIL", value, null, null, null, null, null, null, null,
        false, false, true, null);
}
