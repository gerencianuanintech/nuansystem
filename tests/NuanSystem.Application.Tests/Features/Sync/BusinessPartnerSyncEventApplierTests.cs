using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class BusinessPartnerSyncEventApplierTests
{
    [Fact]
    public async Task BusinessPartnerApplier_Created_UpsertsByGlobalId()
    {
        var repository = Substitute.For<IBusinessPartnerSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncApplyResult(true, false, 100, "Creado."));
        var applier = new BusinessPartnerSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<BusinessPartnerSyncPayload>(value =>
                value.GlobalId == payload.GlobalId &&
                value.Code == payload.Code &&
                value.IdentificationTypeCode == payload.IdentificationTypeCode),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BusinessPartnerApplier_Updated_UpdatesExistingByGlobalId()
    {
        var repository = Substitute.For<IBusinessPartnerSyncApplyRepository>();
        var payload = CreatePayload(name: "Cliente actualizado");
        var context = CreateContext(payload, SyncOperation.Updated);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Updated, Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncApplyResult(true, false, 100, "Actualizado."));
        var applier = new BusinessPartnerSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<BusinessPartnerSyncPayload>(value =>
                value.GlobalId == payload.GlobalId &&
                value.Name == "Cliente actualizado"),
            SyncOperation.Updated,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BusinessPartnerApplier_Disabled_DisablesWithoutSapIdentity()
    {
        var repository = Substitute.For<IBusinessPartnerSyncApplyRepository>();
        var payload = CreatePayload(isActive: false);
        var context = CreateContext(payload, SyncOperation.Disabled);
        repository.DisableFromSyncAsync(2, context, payload, false, Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSyncApplyResult(true, false, 100, "Desactivado."));
        var applier = new BusinessPartnerSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).DisableFromSyncAsync(
            2,
            context,
            Arg.Is<BusinessPartnerSyncPayload>(value => value.GlobalId == payload.GlobalId && value.IsActive == false),
            markDeleted: false,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BusinessPartnerApplier_ReapplyingSameEventId_IsIdempotentAndDoesNotDuplicate()
    {
        var repository = new InMemoryBusinessPartnerSyncApplyRepository();
        var payload = CreatePayload();
        var eventId = Guid.NewGuid();
        var context = CreateContext(payload, SyncOperation.Created, eventId);
        var applier = new BusinessPartnerSyncEventApplier(repository);

        var firstResult = await applier.ApplyAsync(context, CancellationToken.None);
        var secondResult = await applier.ApplyAsync(context, CancellationToken.None);

        firstResult.Applied.Should().BeTrue();
        secondResult.Applied.Should().BeTrue();
        secondResult.Message.Should().Contain("ya aplicado");
        repository.BusinessPartnerCount.Should().Be(1);
        repository.SyncInboxCount.Should().Be(1);
        repository.BusinessPartnerWriteCount.Should().Be(1);
        repository.GetBusinessPartnerName(payload.GlobalId).Should().Be(payload.Name);
        repository.GetSyncInboxStatus(eventId).Should().Be(SyncEventStatus.Applied);
        repository.UsedLocalIdAsIdentity.Should().BeFalse();
        repository.TouchedSapMapping.Should().BeFalse();
    }

    [Fact]
    public async Task Dispatcher_IgnoresUnsupportedEntityWithoutThrowing()
    {
        var dispatcher = new SyncEventApplierDispatcher(
            new StaticOptionsMonitor<MasterBranchSyncWorkerOptions>(new MasterBranchSyncWorkerOptions
            {
                SkeletonMode = false,
                EnabledEntityAppliers = ["BusinessPartner"]
            }),
            []);

        var result = await dispatcher.ApplyAsync(
            new SyncEventApplyContext(
                Guid.NewGuid(),
                SourceCompanyId: 1,
                EntityName: "Items",
                EntityGlobalId: Guid.NewGuid(),
                Operation: SyncOperation.Created.ToString(),
                PayloadJson: "{}",
                TargetCompanyId: 2,
                TargetId: 10),
            CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_ENTITY_APPLIER_DISABLED");
    }

    [Fact]
    public async Task Dispatcher_DoesNotApply_WhenSkeletonModeIsEnabled()
    {
        var entityApplier = Substitute.For<ISyncEntityEventApplier>();
        var dispatcher = new SyncEventApplierDispatcher(
            new StaticOptionsMonitor<MasterBranchSyncWorkerOptions>(new MasterBranchSyncWorkerOptions
            {
                SkeletonMode = true,
                EnabledEntityAppliers = ["BusinessPartner"]
            }),
            new[] { entityApplier });

        var result = await dispatcher.ApplyAsync(CreateContext(CreatePayload(), SyncOperation.Created), CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Message.Should().Contain("SkeletonMode");
        await entityApplier.DidNotReceiveWithAnyArgs().ApplyAsync(default!, default);
    }

    [Fact]
    public async Task Dispatcher_AppliesOnlyWhenBusinessPartnerIsEnabled()
    {
        var entityApplier = Substitute.For<ISyncEntityEventApplier>();
        entityApplier.CanApply("BusinessPartner").Returns(true);
        entityApplier.ApplyAsync(Arg.Any<SyncEventApplyContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncEventApplyResult(true, "Aplicado."));
        var dispatcher = new SyncEventApplierDispatcher(
            new StaticOptionsMonitor<MasterBranchSyncWorkerOptions>(new MasterBranchSyncWorkerOptions
            {
                SkeletonMode = false,
                EnabledEntityAppliers = ["BusinessPartner"]
            }),
            new[] { entityApplier });

        var result = await dispatcher.ApplyAsync(CreateContext(CreatePayload(), SyncOperation.Created), CancellationToken.None);

        result.Applied.Should().BeTrue();
        await entityApplier.Received(1).ApplyAsync(Arg.Any<SyncEventApplyContext>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void BusinessPartnerSyncPath_DoesNotUseSapCardCode()
    {
        var applier = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.MasterBranchSyncWorker",
            "Services",
            "BusinessPartnerSyncEventApplier.cs");
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "Sync",
            "BusinessPartnerSyncApplyRepository.cs");

        applier.Should().NotContain("SapCardCode");
        repository.Should().NotContain("SapCardCode");
        repository.Should().Contain("WHERE GlobalId = @GlobalId");
        repository.Should().Contain("EventId = @EventId");
        repository.Should().Contain("Status = N'Applied'");
    }

    private static SyncEventApplyContext CreateContext(
        BusinessPartnerSyncPayload payload,
        SyncOperation operation,
        Guid? eventId = null)
    {
        return new SyncEventApplyContext(
            eventId ?? Guid.NewGuid(),
            SourceCompanyId: 1,
            EntityName: "BusinessPartner",
            EntityGlobalId: payload.GlobalId,
            Operation: operation.ToString(),
            PayloadJson: CreatePayloadJson(payload, operation),
            TargetCompanyId: 2,
            TargetId: 10);
    }

    private static BusinessPartnerSyncPayload CreatePayload(
        string name = "Cliente Uno",
        bool isActive = true)
    {
        return new BusinessPartnerSyncPayload(
            GlobalId: Guid.NewGuid(),
            Code: "CLI-001",
            Name: name,
            CommercialName: null,
            PartnerType: "Customer",
            IdentificationTypeCode: "RUC",
            IdentificationNumber: "0999999999001",
            Email: "cliente@nuan.local",
            Phone: "0999999999",
            IsActive: isActive,
            ExternalSystem: null,
            ExternalCode: null);
    }

    private static string CreatePayloadJson(BusinessPartnerSyncPayload payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "BusinessPartner",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = operation.ToString(),
            payload
        };

        return JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var scriptPath = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(scriptPath))
            {
                return File.ReadAllText(scriptPath);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontro el archivo {Path.Combine(pathParts)}.");
    }

    private sealed class StaticOptionsMonitor<T>(T currentValue) : IOptionsMonitor<T>
    {
        public T CurrentValue { get; } = currentValue;

        public T Get(string? name) => CurrentValue;

        public IDisposable? OnChange(Action<T, string?> listener) => null;
    }

    private sealed class InMemoryBusinessPartnerSyncApplyRepository : IBusinessPartnerSyncApplyRepository
    {
        private readonly Dictionary<Guid, BusinessPartnerSyncPayload> _businessPartnersByGlobalId = [];
        private readonly Dictionary<Guid, SyncEventStatus> _syncInboxByEventId = [];

        public int BusinessPartnerCount => _businessPartnersByGlobalId.Count;

        public int SyncInboxCount => _syncInboxByEventId.Count;

        public int BusinessPartnerWriteCount { get; private set; }

        public bool UsedLocalIdAsIdentity { get; private set; }

        public bool TouchedSapMapping { get; private set; }

        public Task<bool> ExistsByGlobalIdAsync(
            int branchCompanyId,
            Guid globalId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_businessPartnersByGlobalId.ContainsKey(globalId));
        }

        public Task<BusinessPartnerSyncApplyResult> UpsertFromSyncAsync(
            int branchCompanyId,
            SyncEventApplyContext context,
            BusinessPartnerSyncPayload payload,
            SyncOperation operation,
            CancellationToken cancellationToken = default)
        {
            if (_syncInboxByEventId.TryGetValue(context.EventId, out var status) &&
                status == SyncEventStatus.Applied)
            {
                return Task.FromResult(new BusinessPartnerSyncApplyResult(
                    true,
                    true,
                    BusinessPartnerId: null,
                    "Evento ya aplicado en SyncInbox."));
            }

            _syncInboxByEventId[context.EventId] = SyncEventStatus.Pending;
            _businessPartnersByGlobalId[payload.GlobalId] = payload;
            BusinessPartnerWriteCount++;
            _syncInboxByEventId[context.EventId] = SyncEventStatus.Applied;

            return Task.FromResult(new BusinessPartnerSyncApplyResult(
                true,
                false,
                BusinessPartnerId: 1,
                "BusinessPartner sincronizado por GlobalId."));
        }

        public Task<BusinessPartnerSyncApplyResult> DisableFromSyncAsync(
            int branchCompanyId,
            SyncEventApplyContext context,
            BusinessPartnerSyncPayload payload,
            bool markDeleted,
            CancellationToken cancellationToken = default)
        {
            return UpsertFromSyncAsync(branchCompanyId, context, payload with { IsActive = false }, SyncOperation.Disabled, cancellationToken);
        }

        public string? GetBusinessPartnerName(Guid globalId)
        {
            return _businessPartnersByGlobalId.TryGetValue(globalId, out var payload)
                ? payload.Name
                : null;
        }

        public SyncEventStatus? GetSyncInboxStatus(Guid eventId)
        {
            return _syncInboxByEventId.TryGetValue(eventId, out var status)
                ? status
                : null;
        }
    }
}
