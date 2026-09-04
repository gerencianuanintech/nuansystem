using System.Data;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Persistence.Repositories.Sync;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class BusinessPartnerBidirectionalFlowTests
{
    [Fact]
    public async Task BranchTransaction_CreatesExactlyOneDurableProposal()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();

        var globalId = await flow.CreateCustomerInBranchA();

        flow.BranchA.Single(globalId).MasterSyncStatus.Should().Be("PendingMaster");
        flow.LocalEvents.Should().ContainSingle(item =>
            item.CompanyId == flow.BranchA.CompanyId &&
            item.EntityName == "BusinessPartnerProposal" &&
            item.EntityGlobalId == globalId);
        flow.MasterEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task ReplayedProposalEvent_IsIdempotentAcrossCentralAndDistribution()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        var globalId = await flow.CreateCustomerInBranchA();
        await flow.DrainUntilIdle();
        var proposal = flow.LocalEvents.Single(item => item.EntityName == "BusinessPartnerProposal");
        var before = flow.Counts(globalId);

        await flow.ReplayProposalAsync(proposal.EventId);
        await flow.DrainUntilIdle();

        flow.Counts(globalId).Should().Be(before);
        flow.Central.Single(globalId).CanonicalVersion.Should().Be(1);
    }

    [Fact]
    public async Task CentralUnavailable_LeavesDurableRetryWithoutLosingBranchData()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        var globalId = await flow.CreateCustomerInBranchA();
        flow.CentralAvailable = false;

        await flow.ProcessOnceAsync();

        flow.BranchA.Single(globalId).MasterSyncStatus.Should().Be("PendingMaster");
        flow.LocalEvents.Single(item => item.EntityGlobalId == globalId).Status.Should().Be(SyncEventStatus.Error);
        flow.MasterEvents.Should().BeEmpty();

        flow.CentralAvailable = true;
        flow.AdvancePastRetry();
        await flow.DrainUntilIdle();
        flow.Central.Single(globalId).MasterSyncStatus.Should().Be("Accepted");
    }

    [Fact]
    public async Task AcceptedProposal_ReachesOriginAndEverySiblingBranch()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        var globalId = await flow.CreateCustomerInBranchA();

        await flow.DrainUntilIdle();

        var central = flow.Central.Single(globalId);
        var origin = flow.BranchA.Single(globalId);
        var sibling = flow.BranchB.Single(globalId);
        origin.Should().BeEquivalentTo(central, options => options.Excluding(item => item.Id).Excluding(item => item.RowVersion));
        sibling.Should().BeEquivalentTo(central, options => options.Excluding(item => item.Id).Excluding(item => item.RowVersion));
        flow.CanonicalTargets(globalId).Should().BeEquivalentTo([flow.BranchA.CompanyId, flow.BranchB.CompanyId]);
    }

    [Fact]
    public async Task ApplyingCanonicalReplica_DoesNotPublishAnotherProposal()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        var globalId = await flow.CreateCustomerInBranchA();

        await flow.DrainUntilIdle();

        flow.LocalEvents.Count(item =>
            item.EntityGlobalId == globalId && item.EntityName == "BusinessPartnerProposal").Should().Be(1);
        flow.LocalEvents.Count(item =>
            item.EntityGlobalId == globalId && item.EntityName == "BusinessPartner").Should().Be(1);
    }

    [Fact]
    public async Task SameRoleDuplicateIdentification_IsRejected()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        await flow.CreateCustomerInBranchA("0999999999001");
        await flow.DrainUntilIdle();

        var duplicateId = await flow.CreateCustomerInBranchA("09.999-999 99001");
        await flow.DrainUntilIdle();

        flow.Central.Contains(duplicateId).Should().BeFalse();
        flow.BranchA.Single(duplicateId).MasterSyncStatus.Should().Be("Rejected");
        flow.ResultTargets(duplicateId).Should().Equal(flow.BranchA.CompanyId);
    }

    [Fact]
    public async Task SameIdentificationInDifferentRole_IsAcceptedWithDifferentIdentity()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        var customerId = await flow.CreateCustomerInBranchA("0999999999001");
        await flow.DrainUntilIdle();

        var supplierId = await flow.CreateSupplierInBranchA("09.999-999 99001");
        await flow.DrainUntilIdle();

        supplierId.Should().NotBe(customerId);
        flow.Central.Single(customerId).SapCardCode.Should().Be("C0999999999001");
        flow.Central.Single(supplierId).SapCardCode.Should().Be("P0999999999001");
        flow.BranchB.Single(customerId).PartnerType.Should().Be("Customer");
        flow.BranchB.Single(supplierId).PartnerType.Should().Be("Supplier");
    }

    [Fact]
    public async Task ConcurrentSameFieldChange_CreatesVisibleConflict()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        var globalId = await flow.CreateCustomerInBranchA();
        await flow.DrainUntilIdle();

        await flow.ProposeBranchAUpdateAsync(globalId, partner => partner.Name = "Nombre sucursal");
        flow.ApplyConcurrentCentralChange(globalId, partner => partner.Name = "Nombre central");
        await flow.DrainUntilIdle();

        flow.Central.Single(globalId).Name.Should().Be("Nombre central");
        flow.BranchA.Single(globalId).MasterSyncStatus.Should().Be("Conflict");
        flow.Conflicts.Should().ContainSingle(item =>
            item.GlobalId == globalId && item.Fields.SequenceEqual(new[] { "Name" }));
    }

    [Fact]
    public async Task ConcurrentDisjointChanges_AreMergedDeterministically()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        var globalId = await flow.CreateCustomerInBranchA();
        await flow.DrainUntilIdle();

        await flow.ProposeBranchAUpdateAsync(globalId, partner => partner.Name = "Nombre sucursal");
        flow.ApplyConcurrentCentralChange(globalId, partner => partner.Phone = "2222222222");
        await flow.DrainUntilIdle();

        flow.Central.Single(globalId).Name.Should().Be("Nombre sucursal");
        flow.Central.Single(globalId).Phone.Should().Be("2222222222");
        flow.Central.Single(globalId).CanonicalVersion.Should().Be(3);
        flow.BranchA.Single(globalId).MasterSyncStatus.Should().Be("Accepted");
        flow.BranchB.Single(globalId).Phone.Should().Be("2222222222");
        flow.Conflicts.Should().BeEmpty();
    }

    [Fact]
    public async Task EventPayload_ExcludesSecretsAndManagedCommercialFields()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        var globalId = await flow.CreateCustomerInBranchA(
            configure: partner =>
            {
                partner.SapLastError = "secret-password-token";
                partner.PriceListCode = "PRICE-PRIVATE";
                partner.CreditLimit = 999m;
                partner.CustomerAccountCode = "ACCOUNT-PRIVATE";
            });

        await flow.DrainUntilIdle();

        var payloads = flow.AllPayloads(globalId);
        payloads.Should().NotBeEmpty();
        payloads.Should().OnlyContain(payload =>
            !payload.Contains("password", StringComparison.OrdinalIgnoreCase) &&
            !payload.Contains("token", StringComparison.OrdinalIgnoreCase) &&
            !payload.Contains("PRICE-PRIVATE", StringComparison.Ordinal) &&
            !payload.Contains("ACCOUNT-PRIVATE", StringComparison.Ordinal) &&
            !payload.Contains("creditLimit", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task DisabledWorkerRelayAndProfiles_DoNotMutateBackgroundState()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Disabled();
        var globalId = await flow.CreateCustomerInBranchA();
        var before = flow.BackgroundFingerprint();

        var processed = await flow.ProcessOnceAsync();

        processed.Should().Be(0);
        flow.BackgroundFingerprint().Should().Be(before);
        flow.BranchA.Single(globalId).MasterSyncStatus.Should().Be("PendingMaster");
    }

    [Fact]
    public async Task Flow_DoesNotTouchSapOrCommercialState()
    {
        var flow = BusinessPartnerBidirectionalFlowHarness.Enabled();
        var before = flow.ForbiddenSurfaceFingerprint();

        var globalId = await flow.CreateCustomerInBranchA();
        await flow.DrainUntilIdle();

        flow.Central.Single(globalId).SapCardCode.Should().NotBeNullOrWhiteSpace();
        flow.ForbiddenSurfaceFingerprint().Should().Be(before);
        flow.SapSyncOutboxWrites.Should().Be(0);
        flow.StockCostPriceOrDocumentWrites.Should().Be(0);
    }

    private static BusinessPartnerDto ToDto(
        BusinessPartnerCanonicalSnapshot snapshot,
        long canonicalVersion,
        string masterSyncStatus,
        DateTime createdAt,
        string? message = null) =>
        new()
        {
            GlobalId = snapshot.GlobalId,
            Code = snapshot.Code,
            Name = snapshot.Name,
            CommercialName = snapshot.CommercialName,
            PartnerType = snapshot.PartnerType,
            IdentificationTypeId = 1,
            IdentificationTypeCode = snapshot.IdentificationTypeCode,
            IdentificationNumber = snapshot.IdentificationNumber,
            NormalizedIdentificationNumber = snapshot.NormalizedIdentificationNumber,
            Email = snapshot.Email,
            Phone = snapshot.Phone,
            SapCardCode = snapshot.SapCardCode,
            IsActive = snapshot.IsActive,
            CanonicalVersion = canonicalVersion,
            MasterSyncStatus = masterSyncStatus,
            MasterSyncMessage = message,
            CreatedAt = createdAt,
            Addresses = snapshot.Addresses.Select(item => new BusinessPartnerAddressDto
            {
                GlobalId = item.GlobalId,
                AddressType = item.AddressType,
                Line1 = item.Line1,
                Line2 = item.Line2,
                CountryCode = item.CountryCode,
                ProvinceCode = item.ProvinceCode,
                CityCode = item.CityCode,
                PostalCode = item.PostalCode,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                IsPrimary = item.IsPrimary,
                IsActive = item.IsActive
            }).ToArray(),
            Contacts = snapshot.Contacts.Select(item => new BusinessPartnerContactDto
            {
                GlobalId = item.GlobalId,
                ContactTypeCode = item.ContactTypeCode,
                ContactChannelCode = item.ContactChannelCode,
                Name = item.Name,
                Position = item.Position,
                Department = item.Department,
                Phone = item.Phone,
                Extension = item.Extension,
                Mobile = item.Mobile,
                Email = item.Email,
                Language = item.Language,
                ReceivesNotifications = item.ReceivesNotifications,
                IsPrimary = item.IsPrimary,
                IsActive = item.IsActive,
                Notes = item.Notes
            }).ToArray()
        };

    private sealed class BusinessPartnerBidirectionalFlowHarness
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
        private readonly MutableClock clock = new();
        private readonly MutableCompanyContext companyContext;
        private readonly InMemoryLocalOutboxRepository localOutbox;
        private readonly InMemorySyncOutboxRepository masterOutbox;
        private readonly InMemoryProposalApplyRepository proposalRepository;
        private readonly InMemoryBusinessPartnerSyncApplyRepository branchRepository;
        private readonly SyncEventApplierDispatcher dispatcher;
        private readonly MasterBranchSyncWorkerProcessor processor;
        private readonly List<string> sapSyncOutbox = [];
        private readonly List<string> stockCostPriceOrDocumentWrites = [];
        private int nextGlobalId;
        private int nextProposalEvent;

        private BusinessPartnerBidirectionalFlowHarness(bool enabled)
        {
            Central = new LogicalTenantState(10, clock);
            BranchA = new LogicalTenantState(21, clock);
            BranchB = new LogicalTenantState(22, clock);
            companyContext = new MutableCompanyContext();
            var resolver = new InMemoryCompanyResolver(
                Company(10, "CENTRAL", isMaster: true, parentCompanyId: null),
                Company(21, "BRANCH-A", isMaster: false, parentCompanyId: 10),
                Company(22, "BRANCH-B", isMaster: false, parentCompanyId: 10));
            localOutbox = new InMemoryLocalOutboxRepository(clock);
            masterOutbox = new InMemorySyncOutboxRepository(clock);
            var payloadFactory = new SyncEventPayloadFactory();
            var routingRepository = new InMemoryRoutingRepository(
                () => CentralAvailable,
                profilesEnabled: enabled,
                centralCompanyId: Central.CompanyId,
                branchCompanyIds: [BranchA.CompanyId, BranchB.CompanyId]);
            var routingService = new SyncRoutingService(
                routingRepository,
                new SyncDistributionPolicyEvaluator());
            var promotionService = new LocalSyncOutboxPromotionService(routingService, masterOutbox);
            var options = new MasterBranchSyncWorkerOptions
            {
                Enabled = enabled,
                WorkerInstance = "business-partner-flow-tests",
                BatchSize = 100,
                LockMinutes = 1,
                ErrorDelaySeconds = 1,
                SkeletonMode = false,
                EnabledEntityAppliers =
                [
                    SyncMasterBranchEntityCodes.BusinessPartnerProposal,
                    SyncMasterBranchEntityCodes.BusinessPartner,
                    SyncMasterBranchEntityCodes.BusinessPartnerProposalResult
                ],
                LocalOutboxRelay = new LocalOutboxRelayOptions
                {
                    Enabled = enabled,
                    BatchSize = 100,
                    LeaseMinutes = 1,
                    RetryDelaySeconds = 1
                }
            };
            var optionsMonitor = Substitute.For<IOptionsMonitor<MasterBranchSyncWorkerOptions>>();
            optionsMonitor.CurrentValue.Returns(options);

            proposalRepository = new InMemoryProposalApplyRepository(
                Central,
                localOutbox,
                payloadFactory,
                clock,
                ConflictsInternal);
            branchRepository = new InMemoryBusinessPartnerSyncApplyRepository(
                new Dictionary<int, LogicalTenantState>
                {
                    [BranchA.CompanyId] = BranchA,
                    [BranchB.CompanyId] = BranchB
                });
            dispatcher = new SyncEventApplierDispatcher(
                optionsMonitor,
                new ISyncEntityEventApplier[]
                {
                    new BusinessPartnerProposalSyncEventApplier(proposalRepository, resolver),
                    new BusinessPartnerSyncEventApplier(branchRepository, resolver),
                    new BusinessPartnerProposalResultSyncEventApplier(branchRepository, resolver)
                });
            var relay = new LocalSyncOutboxRelay(
                optionsMonitor,
                localOutbox,
                promotionService,
                NullLogger<LocalSyncOutboxRelay>.Instance);
            processor = new MasterBranchSyncWorkerProcessor(
                optionsMonitor,
                masterOutbox,
                new InMemorySyncAuditRepository(),
                dispatcher,
                relay,
                NullLogger<MasterBranchSyncWorkerProcessor>.Instance);
        }

        public bool CentralAvailable { get; set; } = true;
        public LogicalTenantState Central { get; }
        public LogicalTenantState BranchA { get; }
        public LogicalTenantState BranchB { get; }
        public IReadOnlyCollection<LocalSyncOutboxDto> LocalEvents => localOutbox.Events;
        public IReadOnlyCollection<SyncOutboxDto> MasterEvents => masterOutbox.Events;
        private List<ConflictObservation> ConflictsInternal { get; } = [];
        public IReadOnlyCollection<ConflictObservation> Conflicts => ConflictsInternal;
        public int SapSyncOutboxWrites => sapSyncOutbox.Count;
        public int StockCostPriceOrDocumentWrites => stockCostPriceOrDocumentWrites.Count;

        public static BusinessPartnerBidirectionalFlowHarness Enabled() => new(true);
        public static BusinessPartnerBidirectionalFlowHarness Disabled() => new(false);
        public async Task<Guid> CreateCustomerInBranchA(
            string identification = "0999999999001",
            Action<BusinessPartnerDto>? configure = null) =>
            await CreateInBranchAAsync("Customer", identification, configure);

        public async Task<Guid> CreateSupplierInBranchA(string identification) =>
            await CreateInBranchAAsync("Supplier", identification, configure: null);

        public async Task ProposeBranchAUpdateAsync(Guid globalId, Action<BusinessPartnerDto> change)
        {
            var original = BranchA.Single(globalId);
            var @base = Clone(original);
            var proposed = Clone(original);
            change(proposed);
            proposed.MasterSyncStatus = "PendingMaster";
            proposed.MasterSyncMessage = null;
            BranchA.Upsert(proposed);
            await WriteLocalEventAsync(BranchA.CompanyId, proposed, @base, SyncOperation.Updated);
        }

        public void ApplyConcurrentCentralChange(Guid globalId, Action<BusinessPartnerDto> change)
        {
            var current = Clone(Central.Single(globalId));
            change(current);
            current.CanonicalVersion++;
            current.MasterSyncStatus = "Accepted";
            Central.Upsert(current);
        }

        public Task<int> ProcessOnceAsync() => processor.ProcessOnceAsync();

        public async Task DrainUntilIdle()
        {
            for (var iteration = 0; iteration < 20; iteration++)
            {
                if (await ProcessOnceAsync() == 0)
                    return;
            }

            throw new InvalidOperationException("The in-memory flow did not become idle after 20 passes.");
        }

        public async Task ReplayProposalAsync(Guid eventId)
        {
            var syncEvent = masterOutbox.Events.Single(item => item.EventId == eventId);
            var target = masterOutbox.TargetsFor(syncEvent.Id).Single();
            var result = await dispatcher.ApplyAsync(CreateApplyContext(syncEvent, target));
            result.Applied.Should().BeTrue(result.Message);
        }

        public void AdvancePastRetry() => clock.Advance(TimeSpan.FromMinutes(2));

        public FlowCounts Counts(Guid globalId) => new(
            Central.Count(globalId),
            BranchA.Count(globalId),
            BranchB.Count(globalId),
            LocalEvents.Count(item => item.EntityGlobalId == globalId),
            MasterEvents.Count(item => item.EntityGlobalId == globalId));

        public IReadOnlyCollection<int> CanonicalTargets(Guid globalId) =>
            Targets(globalId, SyncMasterBranchEntityCodes.BusinessPartner);

        public IReadOnlyCollection<int> ResultTargets(Guid globalId) =>
            Targets(globalId, SyncMasterBranchEntityCodes.BusinessPartnerProposalResult);

        public IReadOnlyCollection<string> AllPayloads(Guid globalId) =>
            LocalEvents.Where(item => item.EntityGlobalId == globalId).Select(item => item.PayloadJson)
                .Concat(MasterEvents.Where(item => item.EntityGlobalId == globalId).Select(item => item.PayloadJson))
                .ToArray();

        public string BackgroundFingerprint() =>
            string.Join("|",
                LocalEvents.OrderBy(item => item.Id).Select(item => $"L:{item.Id}:{item.Status}:{item.AttemptCount}:{item.NextRetryAt:O}"))
            + "#" + string.Join("|",
                MasterEvents.OrderBy(item => item.Id).Select(item => $"M:{item.Id}:{item.Status}:{item.AttemptCount}:{item.NextRetryAt:O}"))
            + $"#C:{Central.Fingerprint()}#A:{BranchA.Fingerprint()}#B:{BranchB.Fingerprint()}";

        public string ForbiddenSurfaceFingerprint() =>
            $"sap={string.Join('|', sapSyncOutbox)};commercial={string.Join('|', stockCostPriceOrDocumentWrites)}";

        private async Task<Guid> CreateInBranchAAsync(
            string partnerType,
            string identification,
            Action<BusinessPartnerDto>? configure)
        {
            var suffix = ++nextGlobalId;
            var globalId = Guid.Parse($"10000000-0000-0000-0000-{suffix:D12}");
            var normalized = BusinessPartnerIdentityPolicy.NormalizeIdentification(identification);
            var partner = new BusinessPartnerDto
            {
                GlobalId = globalId,
                Code = BusinessPartnerIdentityPolicy.CreateInternalCode(globalId),
                Name = $"{partnerType} {suffix}",
                PartnerType = partnerType,
                IdentificationTypeId = 1,
                IdentificationTypeCode = "RUC",
                IdentificationNumber = identification,
                NormalizedIdentificationNumber = normalized,
                CanonicalVersion = 0,
                MasterSyncStatus = "PendingMaster",
                IsActive = true,
                CreatedAt = clock.UtcNow
            };
            configure?.Invoke(partner);
            BranchA.Upsert(partner);
            await WriteLocalEventAsync(BranchA.CompanyId, partner, @base: null, SyncOperation.Created);
            return globalId;
        }

        private async Task WriteLocalEventAsync(
            int companyId,
            BusinessPartnerDto current,
            BusinessPartnerDto? @base,
            SyncOperation operation)
        {
            companyContext.SetCurrentCompany(Company(companyId, "BRANCH-A", isMaster: false, parentCompanyId: 10));
            var writer = new BusinessPartnerLocalOutboxWriter(
                companyContext,
                new SyncEventPayloadFactory(),
                localOutbox);
            var generatedEventId = await writer.EnqueueAsync(
                new BusinessPartnerOutboxWriteRequest(current, @base, operation, 7, "branch-user", null),
                Substitute.For<IDbConnection>(),
                Substitute.For<IDbTransaction>());
            generatedEventId.Should().NotBeNull();
            var deterministicEventId = BusinessPartnerProposalApplyRepository.CreateDeterministicEventId(
                current.GlobalId,
                $"BranchProposal:{++nextProposalEvent}");
            localOutbox.ReplaceEventId(generatedEventId!.Value, deterministicEventId);
        }

        private IReadOnlyCollection<int> Targets(Guid globalId, string entityName) =>
            MasterEvents
                .Where(item => item.EntityGlobalId == globalId && item.EntityName == entityName)
                .SelectMany(item => masterOutbox.TargetsFor(item.Id))
                .Select(item => item.BranchCompanyId)
                .ToArray();

        private static SyncEventApplyContext CreateApplyContext(
            SyncOutboxDto syncEvent,
            SyncOutboxTargetDto target) =>
            new(
                syncEvent.EventId,
                syncEvent.CompanyId,
                syncEvent.EntityName,
                syncEvent.EntityGlobalId,
                syncEvent.Operation.ToString(),
                syncEvent.PayloadJson,
                target.BranchCompanyId,
                target.Id);

        private static CompanyConnectionInfo Company(
            int companyId,
            string code,
            bool isMaster,
            int? parentCompanyId) =>
            new(
                companyId,
                code,
                code,
                DatabaseEngine.SqlServer,
                "Server=(in-memory);Database=(in-memory)",
                SapIntegrationMode.None,
                CompanyOperationMode.Standalone,
                isMaster,
                parentCompanyId,
                isMaster ? null : code,
                SyncEnabled: true);

        private static BusinessPartnerDto Clone(BusinessPartnerDto source) =>
            JsonSerializer.Deserialize<BusinessPartnerDto>(JsonSerializer.Serialize(source, JsonOptions), JsonOptions)!;
    }

    private sealed class LogicalTenantState(int companyId, MutableClock clock)
    {
        private readonly Dictionary<Guid, BusinessPartnerDto> partners = [];
        private int nextId;

        public int CompanyId { get; } = companyId;
        public BusinessPartnerDto Single(Guid globalId) => partners.TryGetValue(globalId, out var partner)
            ? partner
            : throw new InvalidOperationException($"BusinessPartner {globalId} does not exist in company {CompanyId}.");
        public bool Contains(Guid globalId) => partners.ContainsKey(globalId);
        public int Count(Guid globalId) => partners.ContainsKey(globalId) ? 1 : 0;
        public IReadOnlyCollection<BusinessPartnerDto> All => partners.Values;
        public string Fingerprint() => string.Join("|", partners.Values
            .OrderBy(item => item.GlobalId)
            .Select(item => $"{item.GlobalId:D}:{item.Code}:{item.CanonicalVersion}:{item.MasterSyncStatus}:{item.Name}:{item.Phone}"));

        public void Upsert(BusinessPartnerDto partner)
        {
            if (partner.Id == 0)
                partner.Id = ++nextId;
            if (partner.CreatedAt == default)
                partner.CreatedAt = clock.UtcNow;
            partners[partner.GlobalId] = partner;
        }
    }

    private sealed class MutableClock
    {
        public DateTime UtcNow { get; private set; } = new(2026, 9, 4, 12, 0, 0, DateTimeKind.Utc);
        public void Advance(TimeSpan duration) => UtcNow = UtcNow.Add(duration);
    }

    private sealed class MutableCompanyContext : ICompanyContext
    {
        public bool HasActiveCompany => CurrentCompany is not null;
        public CompanyConnectionInfo? CurrentCompany { get; private set; }
        public void SetCurrentCompany(CompanyConnectionInfo company) => CurrentCompany = company;
    }

    private sealed class InMemoryCompanyResolver(params CompanyConnectionInfo[] companies) : ICompanyResolver
    {
        private readonly IReadOnlyDictionary<int, CompanyConnectionInfo> byId =
            companies.ToDictionary(item => item.CompanyId);
        private readonly IReadOnlyDictionary<string, CompanyConnectionInfo> byCode =
            companies.ToDictionary(item => item.CompanyCode, StringComparer.OrdinalIgnoreCase);

        public Task<CompanyConnectionInfo?> ResolveByCodeAsync(
            string companyCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(byCode.GetValueOrDefault(companyCode));

        public Task<CompanyConnectionInfo?> ResolveByIdAsync(
            int companyId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(byId.GetValueOrDefault(companyId));

        public Task<CompanyConnectionInfo?> ResolveByCodeForUserAsync(
            string companyCode,
            int userId,
            CancellationToken cancellationToken = default) =>
            ResolveByCodeAsync(companyCode, cancellationToken);
    }

    private sealed class InMemoryLocalOutboxRepository(MutableClock clock) : ILocalSyncOutboxRepository
    {
        private readonly List<LocalSyncOutboxDto> events = [];
        private long nextId;

        public IReadOnlyCollection<LocalSyncOutboxDto> Events => events.ToArray();

        public Task<long> CreateAsync(
            CreateLocalSyncOutboxData data,
            IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var id = ++nextId;
            events.Add(new LocalSyncOutboxDto(
                id,
                data.EventId,
                data.CompanyId,
                data.EntityName,
                data.EntityGlobalId,
                data.EntityCode,
                data.Operation,
                data.PayloadJson,
                SyncEventStatus.Pending,
                0,
                data.MaxAttempts,
                null,
                null,
                null,
                null,
                clock.UtcNow,
                null,
                null,
                data.TargetCompanyId,
                data.CausationEventId));
            return Task.FromResult(id);
        }

        public Task<long> AppendAsync(CreateLocalSyncOutboxData data) =>
            CreateAsync(data, Substitute.For<IDbConnection>(), Substitute.For<IDbTransaction>());

        public void ReplaceEventId(Guid generatedEventId, Guid deterministicEventId)
        {
            var item = events.Single(value => value.EventId == generatedEventId);
            item.EventId = deterministicEventId;
        }

        public Task<IReadOnlyCollection<LocalSyncOutboxCompanyDto>> GetRelayCompaniesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<LocalSyncOutboxCompanyDto>>(
                events.Select(item => item.CompanyId)
                    .Distinct()
                    .OrderBy(id => id)
                    .Select(id => new LocalSyncOutboxCompanyDto(id, $"COMPANY-{id}"))
                    .ToArray());

        public Task<int> ReleaseExpiredLeasesAsync(
            int companyId,
            string workerInstance,
            IReadOnlyCollection<string> enabledEntityNames,
            CancellationToken cancellationToken = default)
        {
            var released = 0;
            foreach (var item in events.Where(item =>
                         item.CompanyId == companyId &&
                         item.Status == SyncEventStatus.InProcess &&
                         item.LockExpiresAt <= clock.UtcNow &&
                         IsEnabled(item.EntityName, enabledEntityNames)))
            {
                item.Status = SyncEventStatus.Error;
                item.LockedBy = null;
                item.LockedAt = null;
                item.LockExpiresAt = null;
                item.NextRetryAt = clock.UtcNow;
                released++;
            }
            return Task.FromResult(released);
        }

        public Task<IReadOnlyCollection<LocalSyncOutboxDto>> ClaimAsync(
            int companyId,
            string workerInstance,
            int batchSize,
            TimeSpan leaseDuration,
            IReadOnlyCollection<string> enabledEntityNames,
            CancellationToken cancellationToken = default)
        {
            var claimed = events
                .Where(item => item.CompanyId == companyId &&
                               item.Status is SyncEventStatus.Pending or SyncEventStatus.Error &&
                               (item.NextRetryAt is null || item.NextRetryAt <= clock.UtcNow) &&
                               IsEnabled(item.EntityName, enabledEntityNames))
                .OrderBy(item => item.Id)
                .Take(batchSize)
                .ToArray();
            foreach (var item in claimed)
            {
                item.Status = SyncEventStatus.InProcess;
                item.AttemptCount++;
                item.LockedBy = workerInstance;
                item.LockedAt = clock.UtcNow;
                item.LockExpiresAt = clock.UtcNow.Add(leaseDuration);
            }
            return Task.FromResult<IReadOnlyCollection<LocalSyncOutboxDto>>(claimed);
        }

        public Task MarkPromotedAsync(
            int companyId,
            long id,
            string workerInstance,
            CancellationToken cancellationToken = default)
        {
            var item = Owned(companyId, id, workerInstance);
            item.Status = SyncEventStatus.Applied;
            item.ProcessedAt = clock.UtcNow;
            ClearLease(item);
            return Task.CompletedTask;
        }

        public Task MarkRetryAsync(
            int companyId,
            long id,
            string workerInstance,
            string errorMessage,
            TimeSpan retryDelay,
            CancellationToken cancellationToken = default)
        {
            var item = Owned(companyId, id, workerInstance);
            item.Status = SyncEventStatus.Error;
            item.LastErrorMessage = errorMessage;
            item.NextRetryAt = clock.UtcNow.Add(retryDelay);
            ClearLease(item);
            return Task.CompletedTask;
        }

        public Task MarkConflictAsync(
            int companyId,
            long id,
            string workerInstance,
            string errorMessage,
            CancellationToken cancellationToken = default)
        {
            var item = Owned(companyId, id, workerInstance);
            item.Status = SyncEventStatus.DeadLetter;
            item.LastErrorMessage = errorMessage;
            item.ProcessedAt = clock.UtcNow;
            ClearLease(item);
            return Task.CompletedTask;
        }

        private LocalSyncOutboxDto Owned(int companyId, long id, string workerInstance)
        {
            var item = events.Single(value => value.CompanyId == companyId && value.Id == id);
            if (!string.Equals(item.LockedBy, workerInstance, StringComparison.Ordinal))
                throw new InvalidOperationException("Local outbox lease ownership mismatch.");
            return item;
        }

        private static bool IsEnabled(string name, IReadOnlyCollection<string> enabled) =>
            enabled.Contains(name, StringComparer.OrdinalIgnoreCase);

        private static void ClearLease(LocalSyncOutboxDto item)
        {
            item.LockedBy = null;
            item.LockedAt = null;
            item.LockExpiresAt = null;
        }
    }

    private sealed class InMemoryRoutingRepository(
        Func<bool> centralAvailable,
        bool profilesEnabled,
        int centralCompanyId,
        IReadOnlyCollection<int> branchCompanyIds) : ISyncRoutingRepository
    {
        public Task<IReadOnlyCollection<SyncRoutingTargetDto>> ResolveTargetsAsync(
            SyncRoutingContext context,
            CancellationToken cancellationToken = default)
        {
            if (!profilesEnabled)
                return Task.FromResult<IReadOnlyCollection<SyncRoutingTargetDto>>([]);

            int[] targetIds;
            if (context.EntityCode == SyncMasterBranchEntityCodes.BusinessPartnerProposal)
            {
                if (!centralAvailable())
                    throw new InvalidOperationException("Central database is unavailable.");
                targetIds = [centralCompanyId];
            }
            else if (context.EntityCode == SyncMasterBranchEntityCodes.BusinessPartnerProposalResult)
            {
                targetIds = context.TargetCompanyId is int target ? [target] : [];
            }
            else if (context.EntityCode == SyncMasterBranchEntityCodes.BusinessPartner)
            {
                targetIds = branchCompanyIds.OrderBy(id => id).ToArray();
            }
            else
            {
                targetIds = [];
            }

            return Task.FromResult<IReadOnlyCollection<SyncRoutingTargetDto>>(
                targetIds.Select((target, index) => new SyncRoutingTargetDto(
                    1,
                    1,
                    "BP-PILOT",
                    context.SourceCompanyId,
                    target,
                    context.EntityCode,
                    100,
                    3,
                    1,
                    1,
                    AllowInsert: true,
                    AllowUpdate: true,
                    AllowDeactivate: true,
                    ContinueOnError: false,
                    SyncProfileEntityBranchId: index + 1,
                    DistributionMode: "All"))
                    .ToArray());
        }

        public Task<IReadOnlyCollection<SyncRoutingConflictDto>> FindActiveConflictsAsync(
            int? profileId,
            int companyId,
            IReadOnlyCollection<SyncRoutingConflictCheckItem> combinations,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<SyncRoutingConflictDto>>([]);
    }

    private sealed class InMemorySyncOutboxRepository(MutableClock clock) :
        ISyncOutboxRepository,
        ISyncOutboxPromotionRepository
    {
        private readonly Dictionary<long, SyncOutboxDto> events = [];
        private readonly Dictionary<long, SyncOutboxTargetDto> targets = [];
        private long nextEventId;
        private long nextTargetId;

        public IReadOnlyCollection<SyncOutboxDto> Events => events.Values.OrderBy(item => item.Id).ToArray();
        public IReadOnlyCollection<SyncOutboxTargetDto> TargetsFor(long outboxId) =>
            targets.Values.Where(item => item.OutboxId == outboxId).OrderBy(item => item.Id).ToArray();

        public Task<SyncOutboxPromotionResult> PromoteAsync(
            SyncOutboxPromotionData data,
            CancellationToken cancellationToken = default)
        {
            var existing = events.Values.SingleOrDefault(item => item.EventId == data.Event.EventId);
            if (existing is not null)
            {
                var sameEnvelope = existing.CompanyId == data.Event.CompanyId &&
                                   existing.EntityName == data.Event.EntityName &&
                                   existing.EntityGlobalId == data.Event.EntityGlobalId &&
                                   existing.EntityCode == data.Event.EntityCode &&
                                   existing.Operation == data.Event.Operation &&
                                   existing.PayloadJson == data.Event.PayloadJson;
                return Task.FromResult(new SyncOutboxPromotionResult(
                    sameEnvelope ? SyncOutboxPromotionStatus.Existing : SyncOutboxPromotionStatus.Conflict,
                    existing.Id,
                    sameEnvelope ? "The event already exists." : "EventId envelope conflict."));
            }

            var id = ++nextEventId;
            events[id] = new SyncOutboxDto(
                id,
                data.Event.EventId,
                data.Event.CompanyId,
                data.Event.EntityName,
                data.Event.EntityGlobalId,
                data.Event.EntityCode,
                data.Event.Operation,
                data.Event.PayloadJson,
                "LocalOutbox",
                data.Event.Id.ToString(),
                SyncEventStatus.Pending,
                0,
                data.Event.MaxAttempts,
                null,
                null,
                null,
                null,
                clock.UtcNow,
                null,
                null);
            foreach (var route in data.Targets.OrderBy(item => item.BranchCompanyId))
            {
                var targetId = ++nextTargetId;
                targets[targetId] = new SyncOutboxTargetDto(
                    targetId,
                    id,
                    route.BranchCompanyId,
                    SyncEventStatus.Pending,
                    0,
                    route.MaxRetries,
                    null,
                    null,
                    null,
                    clock.UtcNow,
                    null);
            }
            return Task.FromResult(new SyncOutboxPromotionResult(
                SyncOutboxPromotionStatus.Created,
                id,
                "The event was promoted."));
        }

        public Task<long> CreateAsync(
            CreateSyncOutboxEventData data,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyCollection<SyncOutboxDto>> GetPendingAsync(
            int companyId,
            int take,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<SyncOutboxDto>>(
                Events.Where(item => item.CompanyId == companyId && item.Status == SyncEventStatus.Pending)
                    .Take(take).ToArray());

        public Task<IReadOnlyCollection<SyncOutboxDto>> ClaimPendingAsync(
            string lockedBy,
            int take,
            TimeSpan lockDuration,
            IReadOnlyCollection<string> enabledEntityNames,
            CancellationToken cancellationToken = default)
        {
            var ids = events.Values
                .Where(item => item.Status is SyncEventStatus.Pending or SyncEventStatus.Error &&
                               (item.NextRetryAt is null || item.NextRetryAt <= clock.UtcNow) &&
                               enabledEntityNames.Contains(item.EntityName, StringComparer.OrdinalIgnoreCase))
                .OrderBy(item => item.Id)
                .Take(take)
                .Select(item => item.Id)
                .ToArray();
            foreach (var id in ids)
            {
                var item = events[id];
                events[id] = item with
                {
                    Status = SyncEventStatus.InProcess,
                    AttemptCount = item.AttemptCount + 1,
                    LockedBy = lockedBy,
                    LockedAt = clock.UtcNow,
                    LockExpiresAt = clock.UtcNow.Add(lockDuration)
                };
            }
            return Task.FromResult<IReadOnlyCollection<SyncOutboxDto>>(ids.Select(id => events[id]).ToArray());
        }

        public Task<int> ReleaseExpiredLocksAsync(CancellationToken cancellationToken = default)
        {
            var ids = events.Values
                .Where(item => item.Status == SyncEventStatus.InProcess && item.LockExpiresAt <= clock.UtcNow)
                .Select(item => item.Id)
                .ToArray();
            foreach (var id in ids)
                UpdateEvent(id, SyncEventStatus.Error, "Expired in-memory lease.", clock.UtcNow);
            return Task.FromResult(ids.Length);
        }

        public Task<IReadOnlyCollection<SyncOutboxTargetDto>> GetTargetsAsync(
            int companyId,
            long outboxId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(TargetsFor(outboxId));

        public Task<long> CreateTargetAsync(
            CreateSyncOutboxTargetData data,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task UpdateStatusAsync(
            long id,
            SyncEventStatus status,
            string? lastErrorMessage = null,
            CancellationToken cancellationToken = default)
        {
            UpdateEvent(id, status, lastErrorMessage, null);
            return Task.CompletedTask;
        }

        public Task MarkAppliedAsync(long id, CancellationToken cancellationToken = default)
        {
            UpdateEvent(id, SyncEventStatus.Applied, null, null, processed: true);
            return Task.CompletedTask;
        }

        public Task MarkIgnoredAsync(
            long id,
            string? reason,
            CancellationToken cancellationToken = default)
        {
            UpdateEvent(id, SyncEventStatus.Ignored, reason, null, processed: true);
            return Task.CompletedTask;
        }

        public Task MarkErrorAsync(
            long id,
            string errorMessage,
            TimeSpan retryDelay,
            CancellationToken cancellationToken = default)
        {
            UpdateEvent(id, SyncEventStatus.Error, errorMessage, clock.UtcNow.Add(retryDelay));
            return Task.CompletedTask;
        }

        public Task MarkDeadLetterAsync(
            long id,
            string errorMessage,
            CancellationToken cancellationToken = default)
        {
            UpdateEvent(id, SyncEventStatus.DeadLetter, errorMessage, null, processed: true);
            return Task.CompletedTask;
        }

        public Task<bool> TryMarkTargetInProcessAsync(
            long targetId,
            CancellationToken cancellationToken = default)
        {
            if (!targets.TryGetValue(targetId, out var target) ||
                target.Status is SyncEventStatus.Applied or SyncEventStatus.Ignored or SyncEventStatus.DeadLetter ||
                target.NextRetryAt > clock.UtcNow)
                return Task.FromResult(false);

            targets[targetId] = target with
            {
                Status = SyncEventStatus.InProcess,
                AttemptCount = target.AttemptCount + 1,
                UpdatedAt = clock.UtcNow
            };
            return Task.FromResult(true);
        }

        public Task MarkTargetAppliedAsync(long targetId, CancellationToken cancellationToken = default)
        {
            UpdateTarget(targetId, SyncEventStatus.Applied, null, null, applied: true);
            return Task.CompletedTask;
        }

        public Task MarkTargetIgnoredAsync(
            long targetId,
            string? reason,
            CancellationToken cancellationToken = default)
        {
            UpdateTarget(targetId, SyncEventStatus.Ignored, reason, null);
            return Task.CompletedTask;
        }

        public Task MarkTargetErrorAsync(
            long targetId,
            string errorMessage,
            TimeSpan retryDelay,
            CancellationToken cancellationToken = default)
        {
            UpdateTarget(targetId, SyncEventStatus.Error, errorMessage, clock.UtcNow.Add(retryDelay));
            return Task.CompletedTask;
        }

        public Task MarkTargetDeadLetterAsync(
            long targetId,
            string errorMessage,
            CancellationToken cancellationToken = default)
        {
            UpdateTarget(targetId, SyncEventStatus.DeadLetter, errorMessage, null);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<SyncOutboxDto>> GetRecentAsync(int companyId, int take, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<SyncOutboxDto>>(Events.Where(item => item.CompanyId == companyId).Take(take).ToArray());
        public Task<SyncDashboardDto> GetDashboardAsync(int companyId, int take, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<SyncSummaryDto> GetSummaryAsync(int companyId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<SyncOutboxListItemDto>> SearchOutboxAsync(int companyId, SyncOutboxQueryFilter filter, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<SyncOutboxDetailDto?> GetOutboxDetailAsync(int companyId, long id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<SyncOutboxDto?> GetByIdAsync(int companyId, long id, CancellationToken cancellationToken = default) =>
            Task.FromResult(events.GetValueOrDefault(id));
        public Task<SyncOutboxActionResultDto?> RetryErrorAsync(int companyId, long id, string? reason, string? createdBy, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<SyncOutboxActionResultDto?> RetryDeadLetterAsync(int companyId, long id, string reason, bool resetAttemptCount, string? createdBy, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<SyncOutboxActionResultDto?> ReleaseExpiredLockAsync(int companyId, long id, string? reason, string? createdBy, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        private void UpdateEvent(
            long id,
            SyncEventStatus status,
            string? error,
            DateTime? nextRetry,
            bool processed = false)
        {
            var item = events[id];
            events[id] = item with
            {
                Status = status,
                LastErrorMessage = error,
                NextRetryAt = nextRetry,
                LockedBy = null,
                LockedAt = null,
                LockExpiresAt = null,
                ProcessedAt = processed ? clock.UtcNow : item.ProcessedAt
            };
        }

        private void UpdateTarget(
            long id,
            SyncEventStatus status,
            string? error,
            DateTime? nextRetry,
            bool applied = false)
        {
            var item = targets[id];
            targets[id] = item with
            {
                Status = status,
                LastErrorMessage = error,
                NextRetryAt = nextRetry,
                AppliedAt = applied ? clock.UtcNow : item.AppliedAt,
                UpdatedAt = clock.UtcNow
            };
        }
    }

    private sealed class InMemoryProposalApplyRepository(
        LogicalTenantState central,
        InMemoryLocalOutboxRepository localOutbox,
        ISyncEventPayloadFactory payloadFactory,
        MutableClock clock,
        List<ConflictObservation> conflicts) : IBusinessPartnerProposalApplyRepository
    {
        private readonly HashSet<Guid> inbox = [];
        private readonly BusinessPartnerSnapshotFactory snapshotFactory = new();

        public async Task<BusinessPartnerProposalApplyResult> ApplyAsync(
            int centralCompanyId,
            SyncEventApplyContext context,
            BusinessPartnerProposalPayloadV1 proposal,
            CancellationToken cancellationToken = default)
        {
            if (!inbox.Add(context.EventId))
            {
                var version = central.Contains(proposal.GlobalId)
                    ? central.Single(proposal.GlobalId).CanonicalVersion
                    : 0;
                return new BusinessPartnerProposalApplyResult(
                    BusinessPartnerProposalApplyOutcome.Duplicate,
                    version,
                    "The proposal event was already applied.");
            }

            var current = central.Contains(proposal.GlobalId)
                ? CreateCentralState(central.Single(proposal.GlobalId))
                : null;
            var duplicateIdentification = central.All.Any(item =>
                item.GlobalId != proposal.GlobalId &&
                item.PartnerType == proposal.PartnerType &&
                item.NormalizedIdentificationNumber == proposal.NormalizedIdentificationNumber);
            var references = BusinessPartnerProposalApplyRepository.ResolveStableReferences(
                proposal.Proposed,
                new BusinessPartnerProposalApplyRepository.IdentificationReferenceRow(1, 1),
                [],
                []);
            var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
                proposal,
                current,
                duplicateIdentification,
                new BusinessPartnerSapCodePolicyData(BusinessPartnerSapPrefixMode.RoleOnly, "PASSPORT"),
                references.IsComplete);

            switch (decision.Outcome)
            {
                case BusinessPartnerProposalApplyOutcome.Accepted:
                {
                    var parameters = BusinessPartnerProposalApplyRepository.CreateAcceptParameters(
                        payloadFactory,
                        centralCompanyId,
                        context,
                        proposal,
                        decision,
                        current,
                        references);
                    central.Upsert(ToDto(
                        decision.Canonical!,
                        decision.CanonicalVersion,
                        "Accepted",
                        current is null ? clock.UtcNow : central.Single(proposal.GlobalId).CreatedAt));
                    await localOutbox.AppendAsync(new CreateLocalSyncOutboxData(
                        parameters.CanonicalEventId,
                        centralCompanyId,
                        SyncMasterBranchEntityCodes.BusinessPartner,
                        proposal.GlobalId,
                        parameters.Code,
                        Enum.Parse<SyncOperation>(context.Operation),
                        parameters.CanonicalPayloadJson,
                        TargetCompanyId: null,
                        CausationEventId: context.EventId));
                    return new BusinessPartnerProposalApplyResult(
                        decision.Outcome,
                        decision.CanonicalVersion,
                        "Proposal accepted.");
                }

                case BusinessPartnerProposalApplyOutcome.Conflict:
                {
                    var parameters = BusinessPartnerProposalApplyRepository.CreateConflictParameters(
                        payloadFactory,
                        centralCompanyId,
                        context,
                        proposal,
                        decision,
                        current);
                    conflicts.Add(new ConflictObservation(proposal.GlobalId, decision.ConflictFields));
                    await localOutbox.AppendAsync(new CreateLocalSyncOutboxData(
                        parameters.ResultEventId,
                        centralCompanyId,
                        SyncMasterBranchEntityCodes.BusinessPartnerProposalResult,
                        proposal.GlobalId,
                        null,
                        SyncOperation.Updated,
                        parameters.ResultPayloadJson,
                        TargetCompanyId: context.SourceCompanyId,
                        CausationEventId: context.EventId));
                    return new BusinessPartnerProposalApplyResult(
                        decision.Outcome,
                        decision.CanonicalVersion,
                        decision.Message,
                        decision.ErrorCode);
                }

                case BusinessPartnerProposalApplyOutcome.Rejected:
                {
                    var parameters = BusinessPartnerProposalApplyRepository.CreateRejectParameters(
                        payloadFactory,
                        centralCompanyId,
                        context,
                        proposal,
                        decision);
                    await localOutbox.AppendAsync(new CreateLocalSyncOutboxData(
                        parameters.ResultEventId,
                        centralCompanyId,
                        SyncMasterBranchEntityCodes.BusinessPartnerProposalResult,
                        proposal.GlobalId,
                        null,
                        SyncOperation.Updated,
                        parameters.ResultPayloadJson,
                        TargetCompanyId: context.SourceCompanyId,
                        CausationEventId: context.EventId));
                    return new BusinessPartnerProposalApplyResult(
                        decision.Outcome,
                        decision.CanonicalVersion,
                        decision.Message,
                        decision.ErrorCode);
                }

                default:
                    inbox.Remove(context.EventId);
                    return new BusinessPartnerProposalApplyResult(
                        decision.Outcome,
                        decision.CanonicalVersion,
                        decision.Message,
                        decision.ErrorCode);
            }
        }

        private BusinessPartnerProposalCentralState CreateCentralState(BusinessPartnerDto partner) =>
            new(partner.Id, partner.CanonicalVersion, snapshotFactory.Create(partner));
    }

    private sealed class InMemoryBusinessPartnerSyncApplyRepository(
        IReadOnlyDictionary<int, LogicalTenantState> branches) : IBusinessPartnerSyncApplyRepository
    {
        private readonly HashSet<string> inbox = new(StringComparer.Ordinal);

        public Task<BusinessPartnerSyncApplyResult> ApplyCanonicalAsync(
            int branchCompanyId,
            SyncEventApplyContext context,
            BusinessPartnerCanonicalPayloadV2 payload,
            CancellationToken cancellationToken = default)
        {
            _ = BusinessPartnerSyncApplyRepository.CreateCanonicalPreflightParameters(context, payload);
            var key = $"{branchCompanyId}:{context.EventId:D}";
            if (!inbox.Add(key))
                return Task.FromResult(BusinessPartnerSyncApplyRepository.MapCanonicalResult(
                    new BusinessPartnerSyncApplyRepository.ApplyResultRow
                    {
                        ResultCode = 2,
                        BusinessPartnerId = branches[branchCompanyId].Contains(payload.Partner.GlobalId)
                            ? branches[branchCompanyId].Single(payload.Partner.GlobalId).Id
                            : null
                    }));

            var branch = branches[branchCompanyId];
            if (branch.Contains(payload.Partner.GlobalId) &&
                branch.Single(payload.Partner.GlobalId).CanonicalVersion > payload.CanonicalVersion)
            {
                return Task.FromResult(BusinessPartnerSyncApplyRepository.MapCanonicalResult(
                    new BusinessPartnerSyncApplyRepository.ApplyResultRow
                    {
                        ResultCode = 3,
                        BusinessPartnerId = branch.Single(payload.Partner.GlobalId).Id
                    }));
            }

            var references = new BusinessPartnerSyncApplyRepository.StableReferenceResolution(
                true, 1, "[]", "[]");
            var parameters = BusinessPartnerSyncApplyRepository.CreateCanonicalParameters(
                context, payload, references);
            var existing = branch.Contains(payload.Partner.GlobalId)
                ? branch.Single(payload.Partner.GlobalId)
                : null;
            var applied = ToDto(
                payload.Partner,
                parameters.CanonicalVersion,
                "Accepted",
                existing?.CreatedAt ?? new DateTime(2026, 9, 4, 12, 0, 0, DateTimeKind.Utc));
            if (existing is not null)
                applied.Id = existing.Id;
            branch.Upsert(applied);
            return Task.FromResult(BusinessPartnerSyncApplyRepository.MapCanonicalResult(
                new BusinessPartnerSyncApplyRepository.ApplyResultRow
                {
                    ResultCode = 1,
                    BusinessPartnerId = applied.Id
                }));
        }

        public Task<BusinessPartnerSyncApplyResult> ApplyProposalResultAsync(
            int branchCompanyId,
            SyncEventApplyContext context,
            BusinessPartnerProposalResultPayloadV1 payload,
            CancellationToken cancellationToken = default)
        {
            _ = BusinessPartnerSyncApplyRepository.CreateProposalResultPreflightParameters(context, payload);
            var key = $"{branchCompanyId}:{context.EventId:D}";
            if (!inbox.Add(key))
                return Task.FromResult(BusinessPartnerSyncApplyRepository.MapProposalResult(
                    new BusinessPartnerSyncApplyRepository.ApplyResultRow
                    {
                        ResultCode = 2,
                        BusinessPartnerId = branches[branchCompanyId].Contains(payload.GlobalId)
                            ? branches[branchCompanyId].Single(payload.GlobalId).Id
                            : null
                    }));

            var branch = branches[branchCompanyId];
            var references = payload.Status == "Rejected" && payload.Canonical is not null
                ? new BusinessPartnerSyncApplyRepository.StableReferenceResolution(true, 1, "[]", "[]")
                : null;
            var parameters = BusinessPartnerSyncApplyRepository.CreateProposalResultParameters(
                context, payload, references);
            if (branch.Contains(payload.GlobalId))
            {
                var current = branch.Single(payload.GlobalId);
                current.MasterSyncStatus = parameters.Status;
                current.MasterSyncMessage = parameters.Message;
                current.CanonicalVersion = parameters.CanonicalVersion;
            }
            return Task.FromResult(BusinessPartnerSyncApplyRepository.MapProposalResult(
                new BusinessPartnerSyncApplyRepository.ApplyResultRow
                {
                    ResultCode = 1,
                    BusinessPartnerId = branch.Contains(payload.GlobalId) ? branch.Single(payload.GlobalId).Id : null
                }));
        }
    }

    private sealed class InMemorySyncAuditRepository : ISyncAuditRepository
    {
        private long nextId;
        public Task<long> AddAsync(CreateSyncAuditData data, CancellationToken cancellationToken = default) =>
            Task.FromResult(++nextId);
        public Task<IReadOnlyCollection<SyncAuditDto>> GetRecentAsync(int companyId, int take, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<SyncAuditDto>>([]);
        public Task<IReadOnlyCollection<SyncAuditDto>> SearchAuditAsync(int companyId, SyncAuditQueryFilter filter, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<SyncAuditDto>>([]);
    }

    private sealed record FlowCounts(
        int CentralPartners,
        int BranchAPartners,
        int BranchBPartners,
        int LocalEvents,
        int MasterEvents);

    private sealed record ConflictObservation(Guid GlobalId, IReadOnlyCollection<string> Fields);
}
