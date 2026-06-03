using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Services;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSupplierImportServiceTests
{
    [Fact]
    public void FindLocalMatch_ReturnsNew_WhenSupplierDoesNotExist()
    {
        var index = SapSupplierImportService.BuildLocalIndex([]);
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        var result = SapSupplierImportService.FindLocalMatch(supplier, index);

        result.Status.Should().Be("New");
    }

    [Fact]
    public void FindLocalMatch_MatchesBySapCardCode()
    {
        var local = LocalSupplier(10, "LOCAL001", "099001", "P001");
        var index = SapSupplierImportService.BuildLocalIndex([local]);
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        var result = SapSupplierImportService.FindLocalMatch(supplier, index);

        result.Status.Should().Be("Matched");
        result.Supplier?.Id.Should().Be(10);
    }

    [Fact]
    public void FindLocalMatch_MatchesByLocalCode()
    {
        var local = LocalSupplier(11, "P001", "099001", null);
        var index = SapSupplierImportService.BuildLocalIndex([local]);
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        var result = SapSupplierImportService.FindLocalMatch(supplier, index);

        result.Status.Should().Be("Matched");
        result.Supplier?.Id.Should().Be(11);
    }

    [Fact]
    public void FindLocalMatch_ReturnsConflict_WhenIdentificationExistsWithoutSapRelation()
    {
        var local = LocalSupplier(12, "LOCAL001", "099001", null);
        var index = SapSupplierImportService.BuildLocalIndex([local]);
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        var result = SapSupplierImportService.FindLocalMatch(supplier, index);

        result.Status.Should().Be("Conflict");
        result.Supplier?.Id.Should().Be(12);
    }

    [Fact]
    public async Task ImportAsync_LoadsLocalSuppliersOncePerBatch()
    {
        var sapReader = Substitute.For<ISapSupplierReader>();
        var repository = Substitute.For<IBusinessPartnerRepository>();
        var syncLogRepository = Substitute.For<ISapSyncLogRepository>();
        var suppliers = new[] { Supplier("P001", "Proveedor Uno", "099001"), Supplier("P002", "Proveedor Dos", "099002") };

        sapReader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns(suppliers);
        repository.GetAllAsync(null, Arg.Any<CancellationToken>()).Returns(Array.Empty<BusinessPartnerDto>());
        repository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                var data = call.Arg<BusinessPartnerSapImportData>();
                return new BusinessPartnerSapImportResultData(data.CardCode == "P001" ? 101 : 102, "Created", "Creado.");
            });

        var service = new SapSupplierImportService(sapReader, repository, syncLogRepository);

        var result = await service.ImportAsync(1, ManualOptions());

        result.Created.Should().Be(2);
        await repository.Received(1).GetAllAsync(null, Arg.Any<CancellationToken>());
        await repository.Received(2).ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_ReturnsUnchanged_WhenExistingSupplierHasNoRelevantDifferences()
    {
        var sapReader = Substitute.For<ISapSupplierReader>();
        var repository = Substitute.For<IBusinessPartnerRepository>();
        var syncLogRepository = Substitute.For<ISapSyncLogRepository>();
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        sapReader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns([supplier]);
        repository.GetAllAsync(null, Arg.Any<CancellationToken>()).Returns([LocalSupplier(10, "LOCAL001", "099001", "P001", name: "Proveedor Uno")]);

        var service = new SapSupplierImportService(sapReader, repository, syncLogRepository);

        var result = await service.ImportAsync(1, ManualOptions());

        result.Unchanged.Should().Be(1);
        await repository.DidNotReceive().ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_UpdatesExistingSupplier_WhenAllowedFieldsDiffer()
    {
        var sapReader = Substitute.For<ISapSupplierReader>();
        var repository = Substitute.For<IBusinessPartnerRepository>();
        var syncLogRepository = Substitute.For<ISapSyncLogRepository>();
        var supplier = Supplier("P001", "Proveedor Uno Actualizado", "099001");

        sapReader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns([supplier]);
        repository.GetAllAsync(null, Arg.Any<CancellationToken>()).Returns([LocalSupplier(10, "LOCAL001", "099001", "P001", name: "Proveedor Uno")]);
        repository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapImportResultData(10, "Updated", "Actualizado."));

        var service = new SapSupplierImportService(sapReader, repository, syncLogRepository);

        var result = await service.ImportAsync(1, ManualOptions());

        result.Updated.Should().Be(1);
        await repository.Received(1).ImportSupplierFromSapAsync(
            Arg.Is<BusinessPartnerSapImportData>(item => item.CardCode == "P001" && item.CardName == "Proveedor Uno Actualizado"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_DoesNotDuplicateSupplier_OnSecondExecution()
    {
        var sapReader = Substitute.For<ISapSupplierReader>();
        var repository = Substitute.For<IBusinessPartnerRepository>();
        var syncLogRepository = Substitute.For<ISapSyncLogRepository>();
        var supplier = Supplier("P001", "Proveedor Uno", "099001");
        var localAfterFirstImport = LocalSupplier(101, "P001", "099001", "P001", name: "Proveedor Uno");

        sapReader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns([supplier]);
        repository.GetAllAsync(null, Arg.Any<CancellationToken>())
            .Returns(
                Array.Empty<BusinessPartnerDto>(),
                [localAfterFirstImport]);
        repository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapImportResultData(101, "Created", "Creado."));

        var service = new SapSupplierImportService(sapReader, repository, syncLogRepository);

        var first = await service.ImportAsync(1, ManualOptions());
        var second = await service.ImportAsync(1, ManualOptions());

        first.Created.Should().Be(1);
        second.Unchanged.Should().Be(1);
        await repository.Received(1).ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>());
    }

    private static SapSupplierRecord Supplier(string code, string name, string? identification)
    {
        return new SapSupplierRecord(
            code,
            name,
            identification,
            "S",
            null,
            "099999999",
            "proveedor@demo.com",
            "USD",
            true,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date);
    }

    private static SapSupplierImportOptions ManualOptions()
    {
        return new SapSupplierImportOptions(
            AuditUserId: 1,
            AuditUserName: "tester",
            WritePublicSapLog: false,
            WriteInbox: false,
            UseIncrementalWatermark: false,
            WorkerInstance: "Test",
            CorrelationId: "test-correlation");
    }

    private static BusinessPartnerDto LocalSupplier(
        int id,
        string code,
        string identification,
        string? sapCardCode,
        string name = "Proveedor Local")
    {
        return new BusinessPartnerDto
        {
            Id = id,
            Code = code,
            Name = name,
            PartnerType = "Supplier",
            IdentificationNumber = identification,
            SapCardCode = sapCardCode,
            Email = "proveedor@demo.com",
            Phone = "099999999",
            PreferredCurrencyCode = "USD",
            IsActive = true
        };
    }
}
