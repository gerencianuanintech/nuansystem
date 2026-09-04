using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.BusinessPartners.SyncConflicts;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSyncConflictUseCaseTests
{
    private readonly ICompanyContext companyContext = Substitute.For<ICompanyContext>();
    private readonly IBusinessPartnerSyncConflictRepository repository =
        Substitute.For<IBusinessPartnerSyncConflictRepository>();

    [Fact]
    public void ResolveValidator_RequiresClosedStrategyReasonAndEightByteRowVersion()
    {
        var validator = new ResolveBusinessPartnerSyncConflictCommandValidator();

        validator.Validate(Command("AcceptBranch", "", "AQID"))
            .Errors.Should().Contain(error => error.ErrorCode == "BP_SYNC_CONFLICT_REASON_REQUIRED");
        validator.Validate(Command("LastWriteWins", "motivo", "AQIDBAUGBwg="))
            .Errors.Should().Contain(error => error.ErrorCode == "BP_SYNC_CONFLICT_RESOLUTION_INVALID");
        validator.Validate(Command("KeepCentral", "motivo", "not-base64"))
            .Errors.Should().Contain(error => error.ErrorCode == "BP_SYNC_CONFLICT_ROW_VERSION_INVALID");
        validator.Validate(Command("KeepCentral", "motivo", "AQID"))
            .Errors.Should().Contain(error => error.ErrorCode == "BP_SYNC_CONFLICT_ROW_VERSION_INVALID");
    }

    [Fact]
    public async Task List_RequiresCentralCompanyAndDoesNotQueryRepositoryFromBranch()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(BranchCompany());
        var handler = new GetBusinessPartnerSyncConflictsQueryHandler(companyContext, repository);

        var result = await handler.Handle(
            new GetBusinessPartnerSyncConflictsQuery("Open"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_SYNC_CONFLICT_MASTER_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().ListAsync(default, default!, default);
    }

    [Fact]
    public async Task Resolve_RequiresCentralCompanyAndDoesNotLoadConflictFromBranch()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(BranchCompany());
        var handler = new ResolveBusinessPartnerSyncConflictCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            Command("KeepCentral", "approved", "AQIDBAUGBwg="),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_SYNC_CONFLICT_MASTER_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default, default);
    }

    [Theory]
    [InlineData("open")]
    [InlineData("Pending")]
    public async Task List_RejectsStatusOutsideClosedCatalog(string status)
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany());
        var handler = new GetBusinessPartnerSyncConflictsQueryHandler(companyContext, repository);

        var result = await handler.Handle(
            new GetBusinessPartnerSyncConflictsQuery(status),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_SYNC_CONFLICT_STATUS_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ListAsync(default, default!, default);
    }

    [Fact]
    public async Task List_UsesAuthoritativeCompanyAndReturnsOnlyPerFieldDifferences()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany());
        repository.ListAsync(10, "Open", Arg.Any<CancellationToken>())
            .Returns([Conflict()]);
        var handler = new GetBusinessPartnerSyncConflictsQueryHandler(companyContext, repository);

        var result = await handler.Handle(
            new GetBusinessPartnerSyncConflictsQuery("Open"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        var dto = result.Value!.Single();
        dto.Differences.Should().ContainSingle(difference =>
            difference.FieldPath == "Name"
            && difference.BaseValue == "Base name"
            && difference.ProposedValue == "Branch name"
            && difference.CentralValue == "Central name");
        dto.GetType().GetProperties().Select(property => property.Name)
            .Should().NotContain(name => name.Contains("Snapshot", StringComparison.OrdinalIgnoreCase)
                || name.Contains("Payload", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith("Json", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task AcceptBranch_AppliesOnlyRecordedPathsAndEmitsIncrementedCanonical()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany());
        var conflict = Conflict(conflictFields: ["Name", $"Contacts/{ContactId:N}/Phone"]);
        repository.GetByIdAsync(10, conflict.Id, Arg.Any<CancellationToken>()).Returns(conflict);
        BusinessPartnerSyncConflictResolutionData? captured = null;
        repository.ResolveAsync(
                Arg.Do<BusinessPartnerSyncConflictResolutionData>(data => captured = data),
                Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncConflictResolutionResult(
                BusinessPartnerSyncConflictResolutionOutcome.Resolved,
                conflict with { Status = "Resolved", Resolution = "AcceptBranch" }));
        var handler = new ResolveBusinessPartnerSyncConflictCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            Command("AcceptBranch", "  aprobado por negocio  ", "AQIDBAUGBwg="),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.CompanyId.Should().Be(10);
        captured.Reason.Should().Be("aprobado por negocio");
        captured.ResolvedSnapshot!.Name.Should().Be("Branch name");
        captured.ResolvedSnapshot.CommercialName.Should().Be("Central commercial");
        captured.ResolvedSnapshot.Contacts.Single().Phone.Should().Be("branch-phone");
        captured.ResolvedSnapshot.Contacts.Single().Email.Should().Be("central@example.com");
        captured.OutboundEvent.TargetCompanyId.Should().BeNull();
        captured.OutboundEvent.CausationEventId.Should().Be(conflict.ProposalEventId);
        captured.OutboundEvent.PublishRequest.EntityName.Should().Be(SyncMasterBranchEntityCodes.BusinessPartner);
        captured.OutboundEvent.PublishRequest.Payload.Should().BeEquivalentTo(
            new BusinessPartnerCanonicalPayloadV2(
                BusinessPartnerSyncSchemaVersions.Canonical,
                6,
                20,
                conflict.ProposalEventId,
                captured.ResolvedSnapshot));
    }

    [Fact]
    public async Task KeepCentral_LeavesCanonicalUntouchedAndEmitsRejectedResultToExactOrigin()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany());
        var conflict = Conflict();
        repository.GetByIdAsync(10, conflict.Id, Arg.Any<CancellationToken>()).Returns(conflict);
        BusinessPartnerSyncConflictResolutionData? captured = null;
        repository.ResolveAsync(
                Arg.Do<BusinessPartnerSyncConflictResolutionData>(data => captured = data),
                Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncConflictResolutionResult(
                BusinessPartnerSyncConflictResolutionOutcome.Resolved,
                conflict with { Status = "Resolved", Resolution = "KeepCentral" }));
        var handler = new ResolveBusinessPartnerSyncConflictCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            Command("KeepCentral", "central prevalece", "AQIDBAUGBwg="),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured!.ResolvedSnapshot.Should().BeNull();
        captured.OutboundEvent.TargetCompanyId.Should().Be(20);
        captured.OutboundEvent.CausationEventId.Should().Be(conflict.ProposalEventId);
        captured.OutboundEvent.PublishRequest.EntityName.Should().Be(
            SyncMasterBranchEntityCodes.BusinessPartnerProposalResult);
        captured.OutboundEvent.PublishRequest.Payload.Should().BeEquivalentTo(
            new BusinessPartnerProposalResultPayloadV1(
                BusinessPartnerSyncSchemaVersions.ProposalResult,
                PartnerId,
                conflict.ProposalEventId,
                20,
                "Rejected",
                "central prevalece",
                5,
                conflict.Canonical));
    }

    [Fact]
    public async Task Resolve_ReturnsAlreadyResolvedIdempotentlyWithoutSecondWrite()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany());
        var resolved = Conflict() with
        {
            Status = "Resolved",
            Resolution = "KeepCentral",
            ResolutionReason = "previous"
        };
        repository.GetByIdAsync(10, resolved.Id, Arg.Any<CancellationToken>()).Returns(resolved);
        var handler = new ResolveBusinessPartnerSyncConflictCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            Command("KeepCentral", "retry", "AQIDBAUGBwg="),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Resolution.Should().Be("KeepCentral");
        await repository.DidNotReceiveWithAnyArgs().ResolveAsync(default!, default);
    }

    [Fact]
    public async Task Resolve_MapsRepositoryConcurrencyToStableExpectedRowVersionError()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany());
        var conflict = Conflict();
        repository.GetByIdAsync(10, conflict.Id, Arg.Any<CancellationToken>()).Returns(conflict);
        repository.ResolveAsync(Arg.Any<BusinessPartnerSyncConflictResolutionData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncConflictResolutionResult(
                BusinessPartnerSyncConflictResolutionOutcome.ConcurrencyConflict,
                null));
        var handler = new ResolveBusinessPartnerSyncConflictCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            Command("AcceptBranch", "approved", "AQIDBAUGBwg="),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error =>
            error.Code == "BP_SYNC_CONFLICT_CONCURRENCY_CONFLICT"
            && error.Field == "ExpectedRowVersion");
    }

    [Fact]
    public async Task Resolve_MapsOutboundCollisionToDistinctStableError()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany());
        var conflict = Conflict();
        repository.GetByIdAsync(10, conflict.Id, Arg.Any<CancellationToken>()).Returns(conflict);
        repository.ResolveAsync(Arg.Any<BusinessPartnerSyncConflictResolutionData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncConflictResolutionResult(
                BusinessPartnerSyncConflictResolutionOutcome.OutboundEventCollision,
                null));
        var handler = new ResolveBusinessPartnerSyncConflictCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            Command("KeepCentral", "approved", "AQIDBAUGBwg="),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_SYNC_EVENT_ID_COLLISION");
    }

    [Fact]
    public async Task AcceptBranch_RejectsUnknownOrProtectedUnrecordedPathWithoutWriting()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany());
        var conflict = Conflict(conflictFields: ["SapCardCode"]);
        repository.GetByIdAsync(10, conflict.Id, Arg.Any<CancellationToken>()).Returns(conflict);
        var handler = new ResolveBusinessPartnerSyncConflictCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            Command("AcceptBranch", "approved", "AQIDBAUGBwg="),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_SYNC_CONFLICT_PATH_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ResolveAsync(default!, default);
    }

    private static ResolveBusinessPartnerSyncConflictCommand Command(
        string resolution,
        string reason,
        string rowVersion) => new(81, resolution, reason, rowVersion, 7, "admin");

    private static readonly Guid PartnerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid ContactId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private static BusinessPartnerSyncConflictRecord Conflict(
        IReadOnlyCollection<string>? conflictFields = null)
    {
        var @base = Snapshot("Base name", "Base commercial", "base-phone", "base@example.com");
        var proposed = Snapshot("Branch name", "Branch commercial", "branch-phone", "branch@example.com");
        var canonical = Snapshot("Central name", "Central commercial", "central-phone", "central@example.com");
        return new BusinessPartnerSyncConflictRecord(
            81,
            Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            101,
            PartnerId,
            20,
            4,
            5,
            @base,
            proposed,
            canonical,
            conflictFields ?? ["Name"],
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
            "BP-AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
            "Central name");
    }

    private static BusinessPartnerCanonicalSnapshot Snapshot(
        string name,
        string commercialName,
        string phone,
        string email) => new(
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
        [],
        [new BusinessPartnerContactSnapshot(
            ContactId, "PRIMARY", "EMAIL", "Ada", null, null, phone, null, null, email,
            null, false, true, true, null)]);

    private static CompanyConnectionInfo MasterCompany() => new(
        10, "CENTRAL", "Central", DatabaseEngine.SqlServer, "protected",
        SapIntegrationMode.None, IsMaster: true);

    private static CompanyConnectionInfo BranchCompany() => new(
        20, "BRANCH", "Branch", DatabaseEngine.SqlServer, "protected",
        SapIntegrationMode.None, IsMaster: false, ParentCompanyId: 10);
}
