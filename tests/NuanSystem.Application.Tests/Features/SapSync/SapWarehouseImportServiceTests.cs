using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Services;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapWarehouseImportServiceTests
{
    [Fact]
    public async Task PreviewAsync_ReturnsNew_WhenSapCodeDoesNotExistLocally()
    {
        var (reader, repository, logRepository, sender) = CreateDependencies();
        reader.GetWarehousesAsync(1, Arg.Any<CancellationToken>()).Returns([SapWarehouse("B01")]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<WarehouseDto>());
        var service = new SapWarehouseImportService(reader, repository, logRepository, sender);

        var result = await service.PreviewAsync(1);

        result.Should().ContainSingle().Which.Status.Should().Be("New");
    }

    [Fact]
    public async Task PreviewAsync_ReturnsConflict_WhenLocalCodeHasNoConfirmedSapRelation()
    {
        var (reader, repository, logRepository, sender) = CreateDependencies();
        reader.GetWarehousesAsync(1, Arg.Any<CancellationToken>()).Returns([SapWarehouse("B01")]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([LocalWarehouse(10, "B01", sapCode: null)]);
        var service = new SapWarehouseImportService(reader, repository, logRepository, sender);

        var result = await service.PreviewAsync(1);

        result.Should().ContainSingle().Which.Status.Should().Be("Conflict");
    }

    [Fact]
    public async Task ImportAsync_DoesNotAutomaticallyDeactivateExistingWarehouse()
    {
        var (reader, repository, logRepository, sender) = CreateDependencies();
        reader.GetWarehousesAsync(1, Arg.Any<CancellationToken>()).Returns([SapWarehouse("B01", isActive: false)]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([LocalWarehouse(10, "B01", "B01", isActive: true)]);
        var service = new SapWarehouseImportService(reader, repository, logRepository, sender);

        var result = await service.ImportAsync(1, [], 1, "tester");

        result.Unchanged.Should().Be(1);
        result.Items.Should().ContainSingle().Which.Message.Should().Contain("aprobacion manual");
        await sender.DidNotReceive().Send(Arg.Any<UpdateWarehouseCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_CreatesWarehouseWithSapIdentityMapping()
    {
        var (reader, repository, logRepository, sender) = CreateDependencies();
        reader.GetWarehousesAsync(1, Arg.Any<CancellationToken>()).Returns([SapWarehouse("B01"), SapWarehouse("B02")]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<WarehouseDto>());
        sender.Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                var command = call.Arg<CreateWarehouseCommand>();
                var id = command.Code == "B01" ? 101 : 102;
                return Result<WarehouseDto>.Success(LocalWarehouse(id, command.Code, command.SapCode, branchCode: command.BranchCode));
            });
        var service = new SapWarehouseImportService(reader, repository, logRepository, sender);

        var result = await service.ImportAsync(
            1,
            [new SapWarehouseBranchMappingDto("B01", "REMIGIO")],
            1,
            "tester");

        result.TotalRead.Should().Be(2);
        result.Created.Should().Be(2);
        await sender.Received(1).Send(
            Arg.Is<CreateWarehouseCommand>(command =>
                command.Code == "B01"
                && command.SapCode == "B01"
                && command.ExternalCode == "B01"
                && command.ExternalSystem == "SAP_B1"
                && command.BranchCode == "REMIGIO"),
            Arg.Any<CancellationToken>());
        await sender.Received(1).Send(
            Arg.Is<CreateWarehouseCommand>(command =>
                command.Code == "B02"
                && command.SapCode == "B02"
                && command.BranchCode == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_RepeatedExistingWarehouseIsIdempotent()
    {
        var (reader, repository, logRepository, sender) = CreateDependencies();
        reader.GetWarehousesAsync(1, Arg.Any<CancellationToken>()).Returns([SapWarehouse("B01")]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([LocalWarehouse(10, "B01", "B01")]);
        var service = new SapWarehouseImportService(reader, repository, logRepository, sender);

        var first = await service.ImportAsync(1, [], 1, "tester");
        var second = await service.ImportAsync(1, [], 1, "tester");

        first.Should().Match<SapWarehouseImportResultDto>(result =>
            result.TotalRead == 1 && result.Unchanged == 1 && result.Created == 0 && result.Updated == 0 && result.Failed == 0);
        second.Should().Match<SapWarehouseImportResultDto>(result =>
            result.TotalRead == 1 && result.Unchanged == 1 && result.Created == 0 && result.Updated == 0 && result.Failed == 0);
        await sender.DidNotReceive().Send(Arg.Any<CreateWarehouseCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateWarehouseCommand>(), Arg.Any<CancellationToken>());
        await logRepository.Received(2).CreateAsync(
            Arg.Is<CreateSapSyncLogData>(log => log.Status == "Succeeded" && log.ErrorMessage == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_UpdatesMappedFieldsAndPreservesGlobalIdentityAndApprovedActiveState()
    {
        var (reader, repository, logRepository, sender) = CreateDependencies();
        var local = LocalWarehouse(10, "B01", "B01", isActive: true);
        local.Name = "Nombre anterior";
        local.City = "Loja";
        reader.GetWarehousesAsync(1, Arg.Any<CancellationToken>())
            .Returns([new SapWarehouseRecord("B01", "Nombre SAP", "Calle SAP", "Cuenca", "Azuay", "EC", false)]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([local]);
        sender.Send(Arg.Any<UpdateWarehouseCommand>(), Arg.Any<CancellationToken>())
            .Returns(call => Result<WarehouseDto>.Success(local));
        var service = new SapWarehouseImportService(reader, repository, logRepository, sender);

        var result = await service.ImportAsync(1, [], 1, "tester");

        result.Updated.Should().Be(1);
        result.Items.Should().ContainSingle().Which.Message.Should().Contain("aprobacion manual");
        await sender.Received(1).Send(
            Arg.Is<UpdateWarehouseCommand>(command =>
                command.Id == local.Id
                && command.GlobalId == local.GlobalId
                && command.Name == "Nombre SAP"
                && command.Address == "Calle SAP"
                && command.City == "Cuenca"
                && command.Province == "Azuay"
                && command.Country == "EC"
                && command.SapCode == "B01"
                && command.ExternalSystem == "SAP_B1"
                && command.ExternalCode == "B01"
                && command.IsActive),
            Arg.Any<CancellationToken>());
    }

    private static (
        ISapWarehouseReader Reader,
        IWarehouseRepository Repository,
        ISapSyncLogRepository LogRepository,
        ISender Sender) CreateDependencies()
        => (
            Substitute.For<ISapWarehouseReader>(),
            Substitute.For<IWarehouseRepository>(),
            Substitute.For<ISapSyncLogRepository>(),
            Substitute.For<ISender>());

    private static SapWarehouseRecord SapWarehouse(string code, bool isActive = true)
        => new(code, $"Bodega {code}", "Direccion", "Cuenca", "Azuay", "EC", isActive);

    private static WarehouseDto LocalWarehouse(
        int id,
        string code,
        string? sapCode,
        bool isActive = true,
        string? branchCode = null)
        => new()
        {
            Id = id,
            GlobalId = Guid.NewGuid(),
            Code = code,
            Name = $"Bodega {code}",
            BranchCode = branchCode,
            Address = "Direccion",
            City = "Cuenca",
            Province = "Azuay",
            Country = "EC",
            AllowsSales = true,
            AllowsPurchases = true,
            AllowsTransfers = true,
            ExternalSystem = sapCode is null ? null : "SAP_B1",
            ExternalCode = sapCode,
            SapCode = sapCode,
            IsActive = isActive
        };
}
