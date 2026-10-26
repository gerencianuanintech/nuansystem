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
        var importRepository = Substitute.For<ISapSupplierImportRepository>();
        var syncLogRepository = Substitute.For<ISapSyncLogRepository>();
        var suppliers = new[] { Supplier("P001", "Proveedor Uno", "099001"), Supplier("P002", "Proveedor Dos", "099002") };

        sapReader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns(suppliers);
        repository.GetAllAsync(null, Arg.Any<CancellationToken>()).Returns(Array.Empty<BusinessPartnerDto>());
        importRepository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                var data = call.Arg<BusinessPartnerSapImportData>();
                return new BusinessPartnerSapImportResultData(data.CardCode == "P001" ? 101 : 102, "Created", "Creado.");
            });

        var service = new SapSupplierImportService(sapReader, repository, importRepository, syncLogRepository);

        var result = await service.ImportAsync(1, ManualOptions());

        result.Created.Should().Be(2);
        await repository.Received(1).GetAllAsync(null, Arg.Any<CancellationToken>());
        await importRepository.Received(2).ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_ReturnsUnchanged_WhenExistingSupplierHasNoRelevantDifferences()
    {
        var sapReader = Substitute.For<ISapSupplierReader>();
        var repository = Substitute.For<IBusinessPartnerRepository>();
        var importRepository = Substitute.For<ISapSupplierImportRepository>();
        var syncLogRepository = Substitute.For<ISapSyncLogRepository>();
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        sapReader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns([supplier]);
        repository.GetAllAsync(null, Arg.Any<CancellationToken>()).Returns([LocalSupplier(10, "LOCAL001", "099001", "P001", name: "Proveedor Uno")]);

        var service = new SapSupplierImportService(sapReader, repository, importRepository, syncLogRepository);

        var result = await service.ImportAsync(1, ManualOptions());

        result.Unchanged.Should().Be(1);
        await importRepository.DidNotReceive().ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_UpdatesExistingSupplier_WhenAllowedFieldsDiffer()
    {
        var sapReader = Substitute.For<ISapSupplierReader>();
        var repository = Substitute.For<IBusinessPartnerRepository>();
        var importRepository = Substitute.For<ISapSupplierImportRepository>();
        var syncLogRepository = Substitute.For<ISapSyncLogRepository>();
        var supplier = Supplier("P001", "Proveedor Uno Actualizado", "099001");

        sapReader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns([supplier]);
        repository.GetAllAsync(null, Arg.Any<CancellationToken>()).Returns([LocalSupplier(10, "LOCAL001", "099001", "P001", name: "Proveedor Uno")]);
        importRepository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapImportResultData(10, "Updated", "Actualizado."));

        var service = new SapSupplierImportService(sapReader, repository, importRepository, syncLogRepository);

        var result = await service.ImportAsync(1, ManualOptions());

        result.Updated.Should().Be(1);
        await importRepository.Received(1).ImportSupplierFromSapAsync(
            Arg.Is<BusinessPartnerSapImportData>(item => item.CardCode == "P001" && item.CardName == "Proveedor Uno Actualizado"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportAsync_DoesNotDuplicateSupplier_OnSecondExecution()
    {
        var sapReader = Substitute.For<ISapSupplierReader>();
        var repository = Substitute.For<IBusinessPartnerRepository>();
        var importRepository = Substitute.For<ISapSupplierImportRepository>();
        var syncLogRepository = Substitute.For<ISapSyncLogRepository>();
        var supplier = Supplier("P001", "Proveedor Uno", "099001");
        var localAfterFirstImport = LocalSupplier(101, "P001", "099001", "P001", name: "Proveedor Uno");

        sapReader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns([supplier]);
        repository.GetAllAsync(null, Arg.Any<CancellationToken>())
            .Returns(
                Array.Empty<BusinessPartnerDto>(),
                [localAfterFirstImport]);
        importRepository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapImportResultData(101, "Created", "Creado."));

        var service = new SapSupplierImportService(sapReader, repository, importRepository, syncLogRepository);

        var first = await service.ImportAsync(1, ManualOptions());
        var second = await service.ImportAsync(1, ManualOptions());

        first.Created.Should().Be(1);
        second.Unchanged.Should().Be(1);
        await importRepository.Received(1).ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(false, "Created")]
    [InlineData(true, "Updated")]
    [InlineData(true, "Conflict")]
    public async Task Segregated_import_preserves_local_identity_audit_result_and_cancellation(bool existing, string action)
    {
        var reader = Substitute.For<ISapSupplierReader>();
        var crud = Substitute.For<IBusinessPartnerRepository>();
        var imports = Substitute.For<ISapSupplierImportRepository>();
        var logs = Substitute.For<ISapSyncLogRepository>();
        var supplier = Supplier("P001", "Proveedor Actualizado", "099001");
        crud.GetAllAsync(null, Arg.Any<CancellationToken>()).Returns(existing
            ? new[] { LocalSupplier(42, "LOCAL-STABLE", "099001", "P001") }
            : Array.Empty<BusinessPartnerDto>());
        imports.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapImportResultData(42, action, "Resultado persistido"));
        using var cancellation = new CancellationTokenSource();
        var service = new SapSupplierImportService(reader, crud, imports, logs);
        var result = await service.ImportOneAsync(supplier, 7, "auditor", cancellation.Token);
        Assert.Equal(action, result.Status);
        Assert.Equal(42, result.LocalBusinessPartnerId);
        Assert.Equal("P001", result.SapCardCode);
        Assert.Equal("Resultado persistido", result.Message);
        await imports.Received(1).ImportSupplierFromSapAsync(Arg.Is<BusinessPartnerSapImportData>(data =>
            data.CardCode == "P001" && data.CardName == "Proveedor Actualizado" && data.TaxIdentification == "099001"
            && data.AuditUserId == 7 && data.AuditUserName == "auditor"
            && (existing ? data.InternalCode == "LOCAL-STABLE" : !string.IsNullOrWhiteSpace(data.InternalCode))), cancellation.Token);
        await crud.Received(1).GetAllAsync(null, cancellation.Token);
        Assert.DoesNotContain(crud.ReceivedCalls(), call => call.GetMethodInfo().Name != "GetAllAsync");
        Assert.Empty(reader.ReceivedCalls());
        Assert.Empty(logs.ReceivedCalls());
    }

    [Fact]
    public async Task Preview_queries_local_suppliers_without_using_import_contract()
    {
        var reader = Substitute.For<ISapSupplierReader>();
        var crud = Substitute.For<IBusinessPartnerRepository>();
        var imports = Substitute.For<ISapSupplierImportRepository>();
        var logs = Substitute.For<ISapSyncLogRepository>();
        reader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns([Supplier("P001", "Proveedor", "099001")]);
        crud.GetAllAsync("Supplier", Arg.Any<CancellationToken>()).Returns(Array.Empty<BusinessPartnerDto>());
        var result = await new SapSupplierImportService(reader, crud, imports, logs).PreviewAsync(1);
        Assert.Single(result);
        await crud.Received(1).GetAllAsync("Supplier", Arg.Any<CancellationToken>());
        Assert.Empty(imports.ReceivedCalls());
        Assert.Empty(logs.ReceivedCalls());
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
