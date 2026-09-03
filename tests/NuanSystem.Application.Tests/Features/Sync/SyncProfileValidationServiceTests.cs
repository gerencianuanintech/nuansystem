using FluentAssertions;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Configuration.Services;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Services;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncProfileValidationServiceTests
{
    [Fact]
    public async Task ValidateAsync_AcceptsValidProfileWithOneBranchAndOneEntity()
    {
        var service = CreateService();

        var result = await service.ValidateAsync(ValidRequest(), null, userId: 1);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_AcceptsValidProfileWithSeveralBranchesAndEntitiesWithDependencies()
    {
        var service = CreateService();
        var request = ValidRequest() with
        {
            Branches =
            [
                new SaveSyncProfileBranchRequest { BranchCompanyId = 2, IsActive = true },
                new SaveSyncProfileBranchRequest { BranchCompanyId = 3, IsActive = true }
            ],
            Entities =
            [
                Entity("BusinessPartner", 10, [2, 3]),
                Entity("UnitOfMeasure", 12, [2, 3]),
                Entity("ItemGroups", 15, [2, 3]),
                Entity("ItemFamilies", 18, [2, 3]),
                Entity("Item", 20, [2, 3]),
                Entity("Warehouse", 30, [2, 3])
            ]
        };

        var result = await service.ValidateAsync(request, null, userId: 1);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_RejectsInactiveDependency()
    {
        var service = CreateService();
        var request = ValidRequest() with
        {
            Entities =
            [
                Entity("ItemGroups", 10, [2]) with { IsActive = false },
                Entity("Item", 20, [2])
            ]
        };

        var result = await service.ValidateAsync(request, null, userId: 1);

        result.Errors.Should().Contain(error => error.Code == "SyncEntityDependencyMissing");
    }

    [Fact]
    public async Task ValidateAsync_RejectsDependencyDisabledForSameBranch()
    {
        var service = CreateService();
        var request = ValidRequest() with
        {
            Branches =
            [
                new SaveSyncProfileBranchRequest { BranchCompanyId = 2, IsActive = true },
                new SaveSyncProfileBranchRequest { BranchCompanyId = 3, IsActive = true }
            ],
            Entities =
            [
                Entity("ItemGroups", 10, [2]),
                Entity("Item", 20, [2, 3])
            ]
        };

        var result = await service.ValidateAsync(request, null, userId: 1);

        result.Errors.Should().Contain(error =>
            error.Code == "SyncEntityDependencyBranchMissing" && error.Message.Contains("3"));
    }

    [Fact]
    public async Task ValidateAsync_WarnsWhenManualOrderWillBeAdjusted()
    {
        var service = CreateService();
        var request = ValidRequest() with
        {
            Entities =
            [
                Entity("Item", 10, [2]),
                Entity("ItemGroups", 20, [2]),
                Entity("ItemFamilies", 30, [2]),
                Entity("UnitOfMeasure", 40, [2])
            ]
        };

        var result = await service.ValidateAsync(request, null, userId: 1);

        result.IsValid.Should().BeTrue();
        result.Warnings.Should().Contain(warning => warning.Code == "SyncEntityDependencyOrderAdjusted");
    }

    [Theory]
    [InlineData("Manual")]
    [InlineData("Interval")]
    [InlineData("Daily")]
    public async Task ValidateAsync_AcceptsSupportedSchedules(string scheduleType)
    {
        var service = CreateService();
        var schedule = scheduleType switch
        {
            "Interval" => new SaveSyncScheduleRequest { ScheduleType = "Interval", IntervalMinutes = 30 },
            "Daily" => new SaveSyncScheduleRequest { ScheduleType = "Daily", ExecutionTime = new TimeSpan(2, 0, 0) },
            _ => new SaveSyncScheduleRequest { ScheduleType = "Manual" }
        };

        var result = await service.ValidateAsync(ValidRequest() with { Schedule = schedule }, null, userId: 1);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public async Task ValidateAsync_ReturnsExpectedErrorCodes(SaveSyncProfileRequest request, string expectedCode)
    {
        var service = CreateService(duplicatedCode: expectedCode == "SyncProfileCodeDuplicated");

        var result = await service.ValidateAsync(request, null, userId: 1);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.Code).Should().Contain(expectedCode);
    }

    [Fact]
    public async Task ValidatePersistedAsync_RejectsInvalidProfileActivation()
    {
        var repository = new FakeSyncProfileRepository
        {
            PersistedProfile = new SyncProfileDetailDto(
                10,
                1,
                "MST",
                "Matriz",
                "BROKEN",
                "Perfil roto",
                null,
                "MasterToBranch",
                "Incremental",
                "MasterWins",
                500,
                3,
                30,
                30,
                false,
                null,
                null,
                DateTime.UtcNow,
                null,
                null,
                null,
                [],
                [],
                [],
                null)
        };
        var service = new SyncProfileValidationService(repository, new FakeSyncRoutingRepository(), new FakeSyncEntityCatalogService());

        var result = await service.ValidatePersistedAsync(10, userId: 1);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.Code).Should().Contain("SyncBranchesRequired");
        result.Errors.Select(error => error.Code).Should().Contain("SyncEntitiesRequired");
    }

    [Fact]
    public async Task ValidateAsync_AllowsInactiveProfileWithOperativePaymentTerms()
    {
        var service = CreateService();

        var result = await service.ValidateAsync(
            ValidRequest() with { IsActive = false, Entities = [Entity("BusinessPartnerPaymentTerms", 50, [2])] },
            null,
            userId: 1);

        result.IsValid.Should().BeTrue();
        result.Warnings.Select(warning => warning.Code).Should().NotContain("SyncEntityDraftOnly");
    }

    [Fact]
    public async Task ValidateAsync_AllowsCustomMasterDefinitionOnlyAsInactiveProfileDraft()
    {
        var customDefinition = new SyncEntityDefinitionLookupDto(
            100,
            "CustomCatalog",
            "Catalogo personalizado",
            null,
            300,
            true,
            true,
            true,
            false,
            "Code",
            "UpdatedAt",
            false,
            true,
            false,
            false,
            []);
        var service = new SyncProfileValidationService(
            new FakeSyncProfileRepository(),
            new FakeSyncRoutingRepository(),
            new FakeSyncEntityCatalogService { AdditionalDefinitions = [customDefinition] });

        var result = await service.ValidateAsync(
            ValidRequest() with { IsActive = false, Entities = [Entity("CustomCatalog", 300, [2])] },
            null,
            userId: 1);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain(error => error.Code == "SyncEntityUnknown");
        result.Warnings.Should().Contain(warning => warning.Code == "SyncEntityDraftOnly");
    }

    [Fact]
    public async Task ValidateAsync_RejectsActiveRoutingConflict()
    {
        var service = new SyncProfileValidationService(
            new FakeSyncProfileRepository(),
            new FakeSyncRoutingRepository { Conflict = true },
            new FakeSyncEntityCatalogService());

        var result = await service.ValidateAsync(ValidRequest(), profileId: 10, userId: 1);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.Code).Should().Contain("SyncRoutingActiveConflict");
    }

    [Fact]
    public async Task ValidateAsync_AcceptsInactiveBranchToMasterProposalDraft()
    {
        var service = CreateService(policyEnabled: false);
        var request = BranchToMasterRequest() with { IsActive = false };

        var result = await service.ValidateAsync(request, null, userId: 1);

        result.Errors.Should().BeEmpty(string.Join(" | ", result.Errors.Select(error => $"{error.Code}:{error.Message}")));
        result.IsValid.Should().BeTrue();
        result.Warnings.Should().Contain(error => error.Code == "SyncEntityDefinitionInactive");
        result.Warnings.Should().Contain(error => error.Code == "SyncEntityDraftOnly");
    }

    [Fact]
    public async Task ValidateAsync_ActivationFailsClosedUntilProposalApplierExists()
    {
        var service = CreateService(policyEnabled: true);

        var result = await service.ValidateAsync(BranchToMasterRequest(), null, userId: 1);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == "SyncEntityDefinitionInactive");
        result.Errors.Should().Contain(error => error.Code == "SyncEntityNotOperative");
        result.Errors.Should().NotContain(error => error.Code == "SyncBusinessPartnerSapCodePolicyRequired");
    }

    [Theory]
    [InlineData("Full", "CentralReview", "BusinessPartnerProposal", "Manual", "SyncBranchToMasterIncrementalOnly")]
    [InlineData("Incremental", "MasterWins", "BusinessPartnerProposal", "Manual", "SyncConflictStrategyNotSupported")]
    [InlineData("Incremental", "CentralReview", "Warehouse", "Manual", "SyncBranchToMasterEntityNotSupported")]
    [InlineData("Incremental", "CentralReview", "BusinessPartnerProposal", "Interval", "SyncBranchToMasterManualScheduleOnly")]
    public async Task ValidateAsync_RejectsInvalidBranchToMasterShapes(
        string executionMode,
        string conflictStrategy,
        string entityCode,
        string scheduleType,
        string expectedCode)
    {
        var service = CreateService(policyEnabled: true);
        var schedule = scheduleType == "Interval"
            ? new SaveSyncScheduleRequest { ScheduleType = "Interval", IntervalMinutes = 30 }
            : new SaveSyncScheduleRequest { ScheduleType = "Manual" };
        var request = BranchToMasterRequest() with
        {
            ExecutionMode = executionMode,
            ConflictStrategy = conflictStrategy,
            Entities = [Entity(entityCode, 10, [2]) with { SyncMode = executionMode, AllowDeactivate = false }],
            Schedule = schedule
        };

        var result = await service.ValidateAsync(request, null, userId: 1);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == expectedCode);
    }

    [Fact]
    public async Task ValidateAsync_RejectsActiveProposalWhenCodePolicyIsDisabled()
    {
        var service = CreateService(policyEnabled: false);

        var result = await service.ValidateAsync(BranchToMasterRequest(), null, userId: 1);

        result.Errors.Should().Contain(error => error.Code == "SyncBusinessPartnerSapCodePolicyRequired");
    }

    [Fact]
    public async Task ValidateAsync_DoesNotQueryProposalPolicyForUnknownCompany()
    {
        var policyRepository = new FakeBusinessPartnerSapCodePolicyRepository(enabled: true);
        var service = CreateService(policyRepository: policyRepository);

        var result = await service.ValidateAsync(
            BranchToMasterRequest() with { CompanyId = 999 },
            null,
            userId: 1);

        result.Errors.Should().Contain(error => error.Code == "SyncMasterCompanyNotFound");
        policyRepository.GetCalls.Should().Be(0);
    }

    [Fact]
    public async Task ValidateAsync_RejectsBranchToMasterWithoutManualSchedule()
    {
        var service = CreateService(policyEnabled: true);

        var result = await service.ValidateAsync(
            BranchToMasterRequest() with { Schedule = null },
            null,
            userId: 1);

        result.Errors.Should().Contain(error => error.Code == "SyncBranchToMasterManualScheduleOnly");
    }

    public static TheoryData<SaveSyncProfileRequest, string> InvalidRequests()
    {
        var data = new TheoryData<SaveSyncProfileRequest, string>
        {
            { ValidRequest() with { Code = "" }, "SyncProfileCodeRequired" },
            { ValidRequest() with { Name = "" }, "SyncProfileNameRequired" },
            { ValidRequest() with { Code = "DUP" }, "SyncProfileCodeDuplicated" },
            { ValidRequest() with { CompanyId = 999 }, "SyncMasterCompanyNotFound" },
            { ValidRequest() with { Branches = [] }, "SyncBranchesRequired" },
            { ValidRequest() with { Branches = [new SaveSyncProfileBranchRequest { BranchCompanyId = 1, IsActive = true }] }, "SyncBranchEqualsMaster" },
            { ValidRequest() with { Branches = [new SaveSyncProfileBranchRequest { BranchCompanyId = 2, IsActive = true }, new SaveSyncProfileBranchRequest { BranchCompanyId = 2, IsActive = true }] }, "SyncBranchDuplicated" },
            { ValidRequest() with { Branches = [new SaveSyncProfileBranchRequest { BranchCompanyId = 999, IsActive = true }] }, "SyncBranchNotFound" },
            { ValidRequest() with { Entities = [] }, "SyncEntitiesRequired" },
            { ValidRequest() with { Entities = [Entity("Warehouse", 10, [2]), Entity("Warehouse", 20, [2])] }, "SyncEntityDuplicated" },
            { ValidRequest() with { Entities = [Entity("UnknownEntity", 10, [2])] }, "SyncEntityUnknown" },
            { ValidRequest() with { Entities = [Entity("Provinces", 20, [2])] }, "SyncEntityDependencyMissing" },
            { ValidRequest() with { Entities = [Entity("Warehouse", 10, [])] }, "SyncEntityWithoutEnabledBranch" },
            { ValidRequest() with { Entities = [Entity("Warehouse", 10, [999])] }, "SyncMatrixBranchNotInProfile" },
            { ValidRequest() with { BatchSize = 0 }, "SyncBatchSizeInvalid" },
            { ValidRequest() with { MaxRetries = 11 }, "SyncMaxRetriesInvalid" },
            { ValidRequest() with { TimeoutMinutes = 0 }, "SyncTimeoutInvalid" },
            { ValidRequest() with { Direction = "Both" }, "SyncDirectionNotSupported" },
            { ValidRequest() with { ConflictStrategy = "BranchWins" }, "SyncConflictStrategyNotSupported" },
            { ValidRequest() with { Schedule = new SaveSyncScheduleRequest { ScheduleType = "Interval" } }, "SyncScheduleIntervalRequired" },
            { ValidRequest() with { Schedule = new SaveSyncScheduleRequest { ScheduleType = "Daily" } }, "SyncScheduleDailyTimeRequired" },
            { ValidRequest() with { Schedule = new SaveSyncScheduleRequest { ScheduleType = "Manual", TimeZoneId = "Invalid/Zone" } }, "SyncScheduleTimeZoneInvalid" },
            { ValidRequest() with { Entities = [Entity("Warehouse", 10, [2]) with { KeyField = "Code;DROP" }] }, "SyncTechnicalFieldExecutable" }
        };

        return data;
    }

    private static SyncProfileValidationService CreateService(
        bool duplicatedCode = false,
        bool policyEnabled = true,
        FakeBusinessPartnerSapCodePolicyRepository? policyRepository = null)
    {
        return new SyncProfileValidationService(
            new FakeSyncProfileRepository { DuplicatedCode = duplicatedCode },
            new FakeSyncRoutingRepository(),
            new FakeSyncEntityCatalogService(),
            policyRepository ?? new FakeBusinessPartnerSapCodePolicyRepository(policyEnabled));
    }

    private static SaveSyncProfileRequest BranchToMasterRequest() => ValidRequest() with
    {
        Direction = "BranchToMaster",
        ExecutionMode = "Incremental",
        ConflictStrategy = "CentralReview",
        Entities = [Entity("BusinessPartnerProposal", 10, [2]) with { AllowDeactivate = false }],
        Schedule = new SaveSyncScheduleRequest { ScheduleType = "Manual" }
    };

    private static SaveSyncProfileRequest ValidRequest()
    {
        return new SaveSyncProfileRequest
        {
            Code = "CATALOGS",
            Name = "Catalogos",
            CompanyId = 1,
            Direction = "MasterToBranch",
            ExecutionMode = "Incremental",
            ConflictStrategy = "MasterWins",
            BatchSize = 500,
            MaxRetries = 3,
            RetryDelaySeconds = 30,
            TimeoutMinutes = 30,
            IsActive = true,
            Branches = [new SaveSyncProfileBranchRequest { BranchCompanyId = 2, IsActive = true }],
            Entities = [Entity("Warehouse", 10, [2])],
            Schedule = new SaveSyncScheduleRequest { ScheduleType = "Manual" }
        };
    }

    private static SaveSyncProfileEntityRequest Entity(string code, int order, IReadOnlyCollection<int> branchCompanyIds)
    {
        return new SaveSyncProfileEntityRequest
        {
            EntityCode = code,
            EntityName = code,
            ExecutionOrder = order,
            SyncMode = "Incremental",
            IsActive = true,
            Branches = branchCompanyIds
                .Select(branchId => new SaveSyncEntityBranchRequest { BranchCompanyId = branchId, IsEnabled = true })
                .ToArray()
        };
    }

    private sealed class FakeSyncProfileRepository : ISyncProfileRepository
    {
        public bool DuplicatedCode { get; init; }
        public SyncProfileDetailDto? PersistedProfile { get; init; }

        public Task<PagedResultDto<SyncProfileListItemDto>> SearchAsync(SyncProfileListFilter filter, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResultDto<SyncProfileListItemDto>([], 0, filter.PageNumber, filter.PageSize));

        public Task<IReadOnlyCollection<SyncProfileSummaryDto>> ListAsync(int? companyId, bool? isActive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<SyncProfileSummaryDto>>([]);

        public Task<SyncProfileDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(PersistedProfile);

        public Task<SyncProfileDetailDto?> GetByCodeAsync(int companyId, string code, CancellationToken cancellationToken = default)
            => Task.FromResult(DuplicatedCode
                ? new SyncProfileDetailDto(99, companyId, "MST", "Matriz", code, "Duplicado", null, "MasterToBranch", "Incremental", "MasterWins", 500, 3, 30, 30, true, null, null, DateTime.UtcNow, null, null, null, [], [], [], null)
                : null);

        public Task<int> CreateAsync(SyncProfileAggregate profile, CancellationToken cancellationToken = default)
            => Task.FromResult(1);

        public Task<bool> UpdateAsync(SyncProfileAggregate profile, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> SetActiveAsync(int id, bool isActive, int? updatedByUserId, string? updatedByUserName, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> HasOperationalHistoryAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<IReadOnlyCollection<SyncCompanyLookupRecord>> GetCompanyLookupsAsync(int? userId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<SyncCompanyLookupRecord>>(
            [
                new SyncCompanyLookupRecord(1, "MST", "Matriz", true, true, null, true),
                new SyncCompanyLookupRecord(2, "BR1", "Sucursal 1", true, false, 1, true),
                new SyncCompanyLookupRecord(3, "BR2", "Sucursal 2", true, false, 1, true)
            ]);

        public Task RecordAuditAsync(
            int? profileId,
            string action,
            string? fieldName,
            string? oldValue,
            string? newValue,
            int? userId,
            string? userName,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeSyncRoutingRepository : ISyncRoutingRepository
    {
        public bool Conflict { get; init; }

        public Task<IReadOnlyCollection<SyncRoutingTargetDto>> ResolveTargetsAsync(
            SyncRoutingContext context,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<SyncRoutingTargetDto>>([]);

        public Task<IReadOnlyCollection<SyncRoutingConflictDto>> FindActiveConflictsAsync(
            int? profileId,
            int companyId,
            IReadOnlyCollection<SyncRoutingConflictCheckItem> combinations,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<SyncRoutingConflictDto>>(Conflict
                ? [new SyncRoutingConflictDto(20, "OTHER", 2, combinations.First().EntityCode)]
                : []);
    }

    private sealed class FakeSyncEntityCatalogService : ISyncEntityCatalogService
    {
        public IReadOnlyCollection<SyncEntityDefinitionLookupDto> AdditionalDefinitions { get; init; } = [];

        public Task<IReadOnlyCollection<SyncEntityDefinitionLookupDto>> GetAsync(
            bool includeInactive,
            int? includeId = null,
            CancellationToken cancellationToken = default)
        {
            var definitions = SyncMasterBranchEntityCodes.InitialCatalog
                .Select((item, index) => new SyncEntityDefinitionLookupDto(
                    index + 1,
                    item.EntityCode,
                    item.DisplayName,
                    item.Notes,
                    item.DefaultExecutionOrder,
                    item.SupportsIncremental,
                    item.SupportsInsert,
                    item.SupportsUpdate,
                    item.SupportsDeactivate,
                    item.DefaultKeyField,
                    item.DefaultModifiedAtField,
                    true,
                    item.EntityCode is not SyncMasterBranchEntityCodes.BusinessPartnerProposal
                        and not SyncMasterBranchEntityCodes.BusinessPartnerProposalResult,
                    item.HasProducer,
                    item.HasApplier,
                    item.Dependencies ?? []))
                .Concat(AdditionalDefinitions)
                .ToArray();
            return Task.FromResult<IReadOnlyCollection<SyncEntityDefinitionLookupDto>>(definitions);
        }
    }

    private sealed class FakeBusinessPartnerSapCodePolicyRepository(bool enabled)
        : IBusinessPartnerSapCodePolicyRepository
    {
        public int GetCalls { get; private set; }

        public Task<BusinessPartnerSapCodePolicyRecord?> GetByCompanyIdAsync(
            int companyId,
            CancellationToken cancellationToken = default)
        {
            GetCalls++;
            return Task.FromResult<BusinessPartnerSapCodePolicyRecord?>(
                new BusinessPartnerSapCodePolicyRecord(companyId, enabled, "None", "PASSPORT", new byte[8]));
        }

        public Task<BusinessPartnerSapCodePolicyWriteResult> SaveAsync(
            SaveBusinessPartnerSapCodePolicyData policy,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
