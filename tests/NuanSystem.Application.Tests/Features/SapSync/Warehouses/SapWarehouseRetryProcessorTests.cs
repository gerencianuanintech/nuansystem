using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Warehouses.Contracts;
using NuanSystem.Application.Features.SapSync.Warehouses.Services;

namespace NuanSystem.Application.Tests.Features.SapSync.Warehouses;

public sealed class SapWarehouseRetryProcessorTests
{
    [Fact]
    public async Task ValidWarehouseV1Snapshot_IsReprocessedFromApprovedPayload()
    {
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        var globalId = Guid.NewGuid();
        sender.Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<WarehouseDto>.Success(new WarehouseDto
            {
                Id = 21,
                GlobalId = globalId,
                Code = "WH-RETRY",
                Name = "Bodega Retry",
                SapCode = "WH-RETRY",
                IsActive = true
            }));
        var processor = new SapWarehouseExecutionRetryProcessor(
            new SapWarehouseRecordProcessor(repository, sender));
        var claim = Claim(Snapshot("WH-RETRY"));

        var result = await processor.ProcessAsync(claim);

        processor.ApprovedSnapshotType.Should().Be(SapSyncApprovedSnapshotTypes.WarehouseV1);
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        result.LocalEntityId.Should().Be(21);
        result.LocalGlobalId.Should().Be(globalId);
    }

    [Fact]
    public async Task SnapshotWhoseIdentityDiffersFromClaim_IsTerminalConflictWithoutWrite()
    {
        var sender = Substitute.For<ISender>();
        var processor = new SapWarehouseExecutionRetryProcessor(
            new SapWarehouseRecordProcessor(Substitute.For<IWarehouseRepository>(), sender));
        var claim = Claim(Snapshot("WH-OTHER")) with { SourceRecordKey = "WH-CLAIM" };

        var result = await processor.ProcessAsync(claim);

        result.Action.Should().Be(SapSyncExecutionDetailActions.Skip);
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapWarehouseResultCodes.SnapshotInvalid);
        await sender.DidNotReceive().Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateWarehouseCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MalformedSnapshot_IsTerminalConflictWithoutLeakingPayload()
    {
        const string json = "{invalid secret-password}";
        var processor = new SapWarehouseExecutionRetryProcessor(
            new SapWarehouseRecordProcessor(
                Substitute.For<IWarehouseRepository>(), Substitute.For<ISender>()));
        var claim = new SapSyncExecutionDetailClaim(
            1, Guid.NewGuid(), "WH-01", SapSyncExecutionDetailStatuses.Processing, 1, 3,
            SapSyncApprovedSnapshotTypes.WarehouseV1, json,
            SHA256.HashData(Encoding.UTF8.GetBytes(json)), new string('A', 64),
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1));

        var result = await processor.ProcessAsync(claim);

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapWarehouseResultCodes.SnapshotInvalid);
        result.SafeMessage.Should().NotContain(json);
        result.SafeMessage.Should().NotContain("password");
    }

    private static SapSyncExecutionDetailClaim Claim(SapWarehouseSnapshot snapshot)
    {
        var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return new(
            1, Guid.NewGuid(), snapshot.WarehouseCode,
            SapSyncExecutionDetailStatuses.Processing, 1, 3,
            SapSyncApprovedSnapshotTypes.WarehouseV1, json,
            SHA256.HashData(Encoding.UTF8.GetBytes(json)), new string('B', 64),
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1));
    }

    private static SapWarehouseSnapshot Snapshot(string code) =>
        new(code, "Bodega Retry", "Direccion", "Cuenca", "Azuay", "EC", true);
}
