using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Items.Commands;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Services;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapItemImportServiceTests
{
    [Fact]
    public async Task PreviewAsync_ReturnsConflict_WhenLocalCodeHasNoConfirmedSapRelation()
    {
        var (reader, repository, mappings, logs, sender, logger) = CreateDependencies();
        reader.GetItemsAsync(1, Arg.Any<SapItemReadOptions?>(), Arg.Any<CancellationToken>()).Returns([SapItem("A01")]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([LocalItem(10, "A01", null)]);
        mappings.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>()).Returns([]);
        var service = new SapItemImportService(reader, repository, mappings, logs, sender, logger);

        var result = await service.PreviewAsync(1, 200, null);

        result.Should().ContainSingle().Which.Status.Should().Be("Conflict");
    }

    [Fact]
    public async Task ImportAsync_ImportsOnlySelectedCodes_AndSetsSapIdentity()
    {
        var (reader, repository, mappings, logs, sender, logger) = CreateDependencies();
        reader.GetItemsAsync(1, Arg.Any<SapItemReadOptions?>(), Arg.Any<CancellationToken>()).Returns([SapItem("A01"), SapItem("A02")]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<ItemDto>());
        repository.GetLookupsAsync(Arg.Any<CancellationToken>()).Returns(EmptyLookups());
        sender.Send(Arg.Any<CreateItemCommand>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            var command = call.Arg<CreateItemCommand>();
            return Result<ItemDto>.Success(LocalItem(101, command.Code, command.SapCode));
        });
        mappings.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>()).Returns([]);
        var service = new SapItemImportService(reader, repository, mappings, logs, sender, logger);

        var result = await service.ImportAsync(1, ["A02"], 1, "tester");

        result.TotalRead.Should().Be(2);
        result.Selected.Should().Be(1);
        result.Created.Should().Be(1);
        await sender.Received(1).Send(
            Arg.Is<CreateItemCommand>(command =>
                command.Code == "A02"
                && command.ExternalSystem == "SAP_B1"
                && command.ExternalCode == "A02"
                && command.SapCode == "A02"
                && command.IsExternalImport),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_DoesNotAutomaticallyDeactivateExistingItem()
    {
        var (reader, repository, mappings, logs, sender, logger) = CreateDependencies();
        reader.GetItemsAsync(1, Arg.Any<SapItemReadOptions?>(), Arg.Any<CancellationToken>()).Returns([SapItem("A01", isActive: false)]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([LocalItem(10, "A01", "A01", true)]);
        repository.GetLookupsAsync(Arg.Any<CancellationToken>()).Returns(EmptyLookups());
        mappings.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>()).Returns([]);
        var service = new SapItemImportService(reader, repository, mappings, logs, sender, logger);

        var result = await service.ImportAsync(1, null, 1, "tester");

        result.Unchanged.Should().Be(1);
        result.Items.Should().ContainSingle().Which.Message.Should().Contain("aprobacion manual");
        await sender.DidNotReceive().Send(Arg.Any<UpdateItemCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_AppliesCompanyCatalogMappings()
    {
        var (reader, repository, mappings, logs, sender, logger) = CreateDependencies();
        reader.GetItemsAsync(1, Arg.Any<SapItemReadOptions?>(), Arg.Any<CancellationToken>())
            .Returns([SapItem("A01", groupCode: 115, unitCode: "Unidad")]);
        repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        repository.GetLookupsAsync(Arg.Any<CancellationToken>()).Returns(new ItemLookupsDto(
            [new ItemGroupLookupDto(7, "INV-ABR", "Abarrotes")], [],
            [new UnitOfMeasureLookupDto(8, "UND", "Unidad")], [], []));
        mappings.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>()).Returns([
            new SapCatalogMappingDto(1, 1, SapCatalogMappingTypes.ItemGroup, "115", "INV-ABR", true, null),
            new SapCatalogMappingDto(2, 1, SapCatalogMappingTypes.UnitOfMeasure, "Unidad", "UND", true, null)
        ]);
        sender.Send(Arg.Any<CreateItemCommand>(), Arg.Any<CancellationToken>()).Returns(call =>
            Result<ItemDto>.Success(LocalItem(101, call.Arg<CreateItemCommand>().Code, "A01")));
        var service = new SapItemImportService(reader, repository, mappings, logs, sender, logger);

        await service.ImportAsync(1, ["A01"], 1, "tester", false);

        await sender.Received(1).Send(Arg.Is<CreateItemCommand>(command =>
            command.ItemGroupId == 7
            && command.InventoryUnitOfMeasureId == 8
            && command.PurchaseUnitOfMeasureId == 8
            && command.SalesUnitOfMeasureId == 8), Arg.Any<CancellationToken>());
    }

    private static (ISapItemReader Reader, IItemRepository Repository, ISapCatalogMappingRepository Mappings, ISapSyncLogRepository Logs, ISender Sender, ILogger<SapItemImportService> Logger) CreateDependencies()
        => (Substitute.For<ISapItemReader>(), Substitute.For<IItemRepository>(), Substitute.For<ISapCatalogMappingRepository>(), Substitute.For<ISapSyncLogRepository>(), Substitute.For<ISender>(), Substitute.For<ILogger<SapItemImportService>>());

    private static SapItemRecord SapItem(string code, bool isActive = true, int? groupCode = null, string unitCode = "UND")
        => new(code, $"Articulo {code}", groupCode, unitCode, unitCode, unitCode, null, null, null,
            true, true, true, false, false, "itItems", isActive);

    private static ItemDto LocalItem(int id, string code, string? sapCode, bool isActive = true)
        => new()
        {
            Id = id,
            GlobalId = Guid.NewGuid(),
            Code = code,
            Name = $"Articulo {code}",
            ItemType = "Product",
            IsPurchaseItem = true,
            IsSalesItem = true,
            IsInventoryItem = true,
            ValuationMethod = "MovingAverage",
            ManagedBy = "None",
            BatchSerialManagementMethod = "EveryTransaction",
            PurchaseFactor = 1,
            SalesFactor = 1,
            AllowDiscount = true,
            ExternalSystem = sapCode is null ? null : "SAP_B1",
            ExternalCode = sapCode,
            SapCode = sapCode,
            IsActive = isActive
        };

    private static ItemLookupsDto EmptyLookups()
        => new([], [], [], [], []);
}
