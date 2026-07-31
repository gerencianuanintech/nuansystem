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

public sealed class SapWarehouseRecordProcessorTests
{
    [Fact]
    public async Task W1_NewActiveWarehouse_IsCreatedWithSapIdentity()
    {
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        sender.Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                var command = call.Arg<CreateWarehouseCommand>();
                return Result<WarehouseDto>.Success(Local(
                    id: 101,
                    globalId: Guid.NewGuid(),
                    code: command.Code,
                    sapCode: command.SapCode,
                    name: command.Name));
            });
        var processor = new SapWarehouseRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(Snapshot(" WH-01 ", " Bodega Centro "), 7, "tester");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        result.Action.Should().Be(SapSyncExecutionDetailActions.Create);
        result.ResultCode.Should().Be(SapWarehouseResultCodes.Created);
        result.LocalGlobalId.Should().NotBeNull().And.NotBe(Guid.Empty);
        await sender.Received(1).Send(
            Arg.Is<CreateWarehouseCommand>(command =>
                command.GlobalId == null
                && command.Code == "WH-01"
                && command.Name == "Bodega Centro"
                && command.ExternalSystem == "SAP_B1"
                && command.ExternalCode == "WH-01"
                && command.SapCode == "WH-01"
                && command.IsActive
                && command.AuditUserId == 7
                && command.AuditUserName == "tester"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task W2_LinkedWarehouse_UpdatePreservesGlobalIdCodeAndLocalFields()
    {
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        var globalId = Guid.NewGuid();
        var local = Local(10, globalId, "LOCAL-CODE", "WH-02", "Nombre anterior");
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([local]);
        sender.Send(Arg.Any<UpdateWarehouseCommand>(), Arg.Any<CancellationToken>())
            .Returns(call => Result<WarehouseDto>.Success(local));
        var processor = new SapWarehouseRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            Snapshot("WH-02", "Nombre SAP", street: "Calle SAP", city: "Cuenca"), 8, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Updated);
        result.LocalGlobalId.Should().Be(globalId);
        await sender.Received(1).Send(
            Arg.Is<UpdateWarehouseCommand>(command =>
                command.Id == local.Id
                && command.GlobalId == globalId
                && command.Code == "LOCAL-CODE"
                && command.Name == "Nombre SAP"
                && command.Address == "Calle SAP"
                && command.City == "Cuenca"
                && command.Description == local.Description
                && command.BranchCode == local.BranchCode
                && command.Phone == local.Phone
                && command.Email == local.Email
                && command.ManagerName == local.ManagerName
                && command.AllowsSales == local.AllowsSales
                && command.AllowsPurchases == local.AllowsPurchases
                && command.AllowsTransfers == local.AllowsTransfers
                && command.AllowsProduction == local.AllowsProduction
                && command.IsDefault == local.IsDefault
                && command.IsActive == local.IsActive),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task W3_LinkedIdenticalWarehouse_IsUnchangedWithoutWrite()
    {
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        var local = Local(10, Guid.NewGuid(), "WH-03", "WH-03", "Bodega WH-03");
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([local]);
        var processor = new SapWarehouseRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(Snapshot("WH-03", "Bodega WH-03"), null, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Unchanged);
        result.ResultCode.Should().Be(SapWarehouseResultCodes.Unchanged);
        await sender.DidNotReceive().Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateWarehouseCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task W4_CodeOnlyCollision_RequiresApprovalWithoutAdoptionOrWrite()
    {
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        var local = Local(10, Guid.NewGuid(), "WH-04", sapCode: null, "Bodega local");
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([local]);
        var processor = new SapWarehouseRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(Snapshot("WH-04", "Bodega SAP"), null, "worker");

        result.Action.Should().Be(SapSyncExecutionDetailActions.Approval);
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.ApprovalRequired);
        result.ResultCode.Should().Be(SapWarehouseResultCodes.CodeCollisionApprovalRequired);
        result.LocalWarehouseId.Should().Be(local.Id);
        result.LocalGlobalId.Should().Be(local.GlobalId);
        await sender.DidNotReceive().Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateWarehouseCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task W5_InactiveSapWarehouse_LinkedToActiveLocal_RequiresApprovalAndRemainsActive()
    {
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        var local = Local(10, Guid.NewGuid(), "WH-05", "WH-05", "Bodega WH-05");
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([local]);
        var processor = new SapWarehouseRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(Snapshot("WH-05", "Bodega WH-05", isActive: false), null, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.ApprovalRequired);
        result.ResultCode.Should().Be(SapWarehouseResultCodes.ApprovalRequired);
        local.IsActive.Should().BeTrue();
        await sender.DidNotReceive().Send(Arg.Any<UpdateWarehouseCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task W6_NewInactiveWarehouse_IsSkippedWithoutCreate()
    {
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        var processor = new SapWarehouseRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(Snapshot("WH-06", "Bodega WH-06", isActive: false), null, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Skipped);
        result.ResultCode.Should().Be(SapWarehouseResultCodes.Inactive);
        await sender.DidNotReceive().Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("", "Bodega")]
    [InlineData("WH-07", " ")]
    public async Task W7_InvalidCodeOrName_IsSkippedWithSafeTerminalCode(string code, string name)
    {
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        var processor = new SapWarehouseRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(Snapshot(code, name), null, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Skipped);
        result.ResultCode.Should().Be(SapWarehouseResultCodes.Invalid);
        result.SafeMessage.Should().NotBeNullOrWhiteSpace();
        await repository.DidNotReceiveWithAnyArgs().GetAllAsync(default);
        await sender.DidNotReceive().Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task W9_SecondFullCycle_DoesNotCreateOrUpdateDuplicate()
    {
        var repository = Substitute.For<IWarehouseRepository>();
        var sender = Substitute.For<ISender>();
        var created = Local(101, Guid.NewGuid(), "WH-09", "WH-09", "Bodega WH-09");
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([], [created]);
        sender.Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<WarehouseDto>.Success(created));
        var processor = new SapWarehouseRecordProcessor(repository, sender);

        var first = await processor.ProcessAsync(Snapshot("WH-09", "Bodega WH-09"), null, "worker");
        var second = await processor.ProcessAsync(Snapshot("WH-09", "Bodega WH-09"), null, "worker");

        first.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        second.Status.Should().Be(SapSyncExecutionDetailStatuses.Unchanged);
        second.LocalGlobalId.Should().Be(created.GlobalId);
        await sender.Received(1).Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateWarehouseCommand>(), Arg.Any<CancellationToken>());
    }

    private static SapWarehouseSnapshot Snapshot(
        string code,
        string name,
        string? street = "Direccion",
        string? city = "Cuenca",
        bool isActive = true) =>
        new(code, name, street, city, "Azuay", "EC", isActive);

    private static WarehouseDto Local(
        int id,
        Guid globalId,
        string code,
        string? sapCode,
        string name) => new()
        {
            Id = id,
            GlobalId = globalId,
            Code = code,
            Name = name,
            Description = "Descripcion local",
            BranchCode = "BR-LOCAL",
            Address = "Direccion",
            City = "Cuenca",
            Province = "Azuay",
            Country = "EC",
            Phone = "0999999999",
            Email = "local@example.test",
            ManagerName = "Responsable local",
            AllowsSales = true,
            AllowsPurchases = false,
            AllowsTransfers = true,
            AllowsProduction = true,
            IsDefault = true,
            ExternalSystem = sapCode is null ? null : "SAP_B1",
            ExternalCode = sapCode,
            SapCode = sapCode,
            IsActive = true
        };
}
