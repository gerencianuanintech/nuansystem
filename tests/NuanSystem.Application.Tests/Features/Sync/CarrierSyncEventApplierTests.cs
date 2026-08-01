using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class CarrierSyncEventApplierTests
{
    [Fact]
    public void CanApply_UsesOnlyCanonicalCarrierCode()
    {
        var applier = new CarrierSyncEventApplier(Substitute.For<ICarrierSyncApplyRepository>());

        applier.CanApply("Carrier").Should().BeTrue();
        applier.CanApply("carrier").Should().BeTrue();
        applier.CanApply("BusinessPartner").Should().BeFalse();
    }

    [Fact]
    public async Task Created_AppliesCarrierByGlobalId()
    {
        var repository = Substitute.For<ICarrierSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.ApplyAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new CarrierSyncApplyResult(true, false, false, 10, "Creado."));
        var applier = new CarrierSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        result.Terminal.Should().BeFalse();
        await repository.Received(1).ApplyAsync(
            2,
            context,
            Arg.Is<CarrierSyncPayloadV1>(value =>
                value.GlobalId == payload.GlobalId &&
                value.Code == payload.Code &&
                value.IdentificationTypeCode == "04"),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deleted_PreservesDeleteOperationForAtomicRepository()
    {
        var repository = Substitute.For<ICarrierSyncApplyRepository>();
        var payload = CreatePayload(isActive: false, isDeleted: true);
        var context = CreateContext(payload, SyncOperation.Deleted);
        repository.ApplyAsync(2, context, payload, SyncOperation.Deleted, Arg.Any<CancellationToken>())
            .Returns(new CarrierSyncApplyResult(true, false, false, 10, "Eliminado."));
        var applier = new CarrierSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).ApplyAsync(
            2,
            context,
            Arg.Is<CarrierSyncPayloadV1>(value => value.IsDeleted && !value.IsActive),
            SyncOperation.Deleted,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FullDisabled_WithTombstonePayload_PreservesLogicalDeletion()
    {
        var repository = Substitute.For<ICarrierSyncApplyRepository>();
        var payload = CreatePayload(isActive: false, isDeleted: true);
        var context = CreateContext(payload, SyncOperation.Disabled);
        repository.ApplyAsync(2, context, payload, SyncOperation.Disabled, Arg.Any<CancellationToken>())
            .Returns(new CarrierSyncApplyResult(true, false, false, 10, "Tombstone aplicado."));
        var applier = new CarrierSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).ApplyAsync(
            2,
            context,
            Arg.Is<CarrierSyncPayloadV1>(value => value.IsDeleted && !value.IsActive),
            SyncOperation.Disabled,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CodeCollision_ReturnsTerminalConflictWithoutAutomaticAdoption()
    {
        var repository = Substitute.For<ICarrierSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.ApplyAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new CarrierSyncApplyResult(
                false,
                false,
                true,
                null,
                "Conflicto terminal.",
                "SYNC_CARRIER_CODE_CONFLICT"));
        var applier = new CarrierSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_CARRIER_CODE_CONFLICT");
    }

    [Fact]
    public async Task GlobalIdMismatch_IsTerminalAndDoesNotCallRepository()
    {
        var repository = Substitute.For<ICarrierSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Updated) with { EntityGlobalId = Guid.NewGuid() };
        var applier = new CarrierSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task IdentificationTypeOutsideClosedCatalog_IsTerminal()
    {
        var repository = Substitute.For<ICarrierSyncApplyRepository>();
        var payload = CreatePayload() with { IdentificationTypeCode = "99" };
        var context = CreateContext(payload, SyncOperation.Created);
        var applier = new CarrierSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_CARRIER_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public async Task InvalidJson_IsTerminalAndDoesNotCallRepository()
    {
        var repository = Substitute.For<ICarrierSyncApplyRepository>();
        var context = new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "Carrier",
            Guid.NewGuid(),
            SyncOperation.Created.ToString(),
            "{invalid",
            2,
            10);
        var applier = new CarrierSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PAYLOAD_INVALID");
        await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default, default!, default!, default);
    }

    [Fact]
    public void Persistence_UsesAtomicStoredProcedureAndDoesNotAdoptByCode()
    {
        var source = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "CarrierSyncApplyRepository.cs");

        source.Should().Contain("SP_NA_POST_CARRIER_SYNC_APPLY_EVENT");
        source.Should().Contain("SYNC_CARRIER_CODE_CONFLICT");
        source.Should().Contain("context.EventId");
        source.Should().Contain("context.PayloadJson");
        source.ToLowerInvariant().Should().NotContain("adopt");
    }

    private static CarrierSyncPayloadV1 CreatePayload(bool isActive = true, bool isDeleted = false)
    {
        return new(
            Guid.NewGuid(),
            "TR-001",
            "Transportista Uno",
            "04",
            "0999999999001",
            "Piloto",
            isActive,
            isDeleted,
            new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc),
            null);
    }

    private static SyncEventApplyContext CreateContext(CarrierSyncPayloadV1 payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "Carrier",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = operation.ToString(),
            payload
        };

        return new(
            Guid.NewGuid(),
            1,
            "Carrier",
            payload.GlobalId,
            operation.ToString(),
            JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            2,
            10);
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontro {Path.Combine(pathParts)}.");
    }
}
