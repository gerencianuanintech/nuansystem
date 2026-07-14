using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Warehouses;

public sealed class WarehouseSyncPublishingTests
{
    private readonly IWarehouseRepository _repository = Substitute.For<IWarehouseRepository>();
    private readonly ISyncEventPublisher _syncEventPublisher = Substitute.For<ISyncEventPublisher>();
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();

    [Fact]
    public async Task Create_PublishesWarehouseSyncEvent_WithGlobalIdAndCode()
    {
        SyncPublishRequest? captured = null;
        var warehouse = CreateWarehouse();
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.ExistsByCodeAsync("BOD-AME", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateWarehouseData>(), Arg.Any<CancellationToken>()).Returns(warehouse.Id);
        _repository.GetByIdAsync(warehouse.Id, Arg.Any<CancellationToken>()).Returns(warehouse);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.CompanyId.Should().Be(10);
        captured.EntityName.Should().Be("Warehouse");
        captured.EntityGlobalId.Should().Be(warehouse.GlobalId);
        captured.EntityCode.Should().Be(warehouse.Code);
        captured.Operation.Should().Be(SyncOperation.Created);
        captured.Payload.Should().BeOfType<WarehouseSyncPayload>();
    }

    [Fact]
    public async Task Update_PublishesDisabledWarehouseSyncEvent_WhenWarehouseBecomesInactive()
    {
        SyncPublishRequest? captured = null;
        var current = CreateWarehouse();
        var inactive = CreateWarehouse(isActive: false);
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.GetByIdAsync(current.Id, Arg.Any<CancellationToken>()).Returns(current, inactive);
        _repository.ExistsByCodeAsync("BOD-AME", current.Id, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateWarehouseData>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(UpdateCommand(current.Id, isActive: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityGlobalId.Should().Be(inactive.GlobalId);
        captured.Operation.Should().Be(SyncOperation.Disabled);
    }

    [Fact]
    public async Task Create_KeepsStandaloneCrudWorking_WhenPublisherSkipsForDisabledSync()
    {
        var warehouse = CreateWarehouse();
        ConfigureActiveCompany(syncEnabled: false);
        _syncEventPublisher.PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(false, null, "La empresa no tiene sincronizacion habilitada.")));
        _repository.ExistsByCodeAsync("BOD-AME", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateWarehouseData>(), Arg.Any<CancellationToken>()).Returns(warehouse.Id);
        _repository.GetByIdAsync(warehouse.Id, Arg.Any<CancellationToken>()).Returns(warehouse);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _syncEventPublisher.Received(1).PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_DoesNotPublish_WhenNoActiveCompanyContext()
    {
        var warehouse = CreateWarehouse();
        ConfigureNoActiveCompany();
        _repository.ExistsByCodeAsync("BOD-AME", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateWarehouseData>(), Arg.Any<CancellationToken>()).Returns(warehouse.Id);
        _repository.GetByIdAsync(warehouse.Id, Arg.Any<CancellationToken>()).Returns(warehouse);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _syncEventPublisher.DidNotReceiveWithAnyArgs().PublishAsync(default!, default);
    }

    [Fact]
    public async Task Create_PayloadKeepsSapCodeOptionalAndExcludesOperationalInventoryValues()
    {
        SyncPublishRequest? captured = null;
        var warehouse = CreateWarehouse(sapCode: null);
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.ExistsByCodeAsync("BOD-AME", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateWarehouseData>(), Arg.Any<CancellationToken>()).Returns(warehouse.Id);
        _repository.GetByIdAsync(warehouse.Id, Arg.Any<CancellationToken>()).Returns(warehouse);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var payload = captured!.Payload.Should().BeOfType<WarehouseSyncPayload>().Subject;
        payload.SapCode.Should().BeNull();
        payload.GlobalId.Should().Be(warehouse.GlobalId);
        payload.GetType().GetProperty("Stock").Should().BeNull();
        payload.GetType().GetProperty("OnHand").Should().BeNull();
        payload.GetType().GetProperty("Cost").Should().BeNull();
        payload.GetType().GetProperty("Kardex").Should().BeNull();
    }

    private CreateWarehouseCommandHandler CreateCreateHandler() => new(_repository, _syncEventPublisher, _companyContext);

    private UpdateWarehouseCommandHandler CreateUpdateHandler() => new(_repository, _syncEventPublisher, _companyContext);

    private void ConfigureSyncPublisher(Action<SyncPublishRequest> capture)
    {
        _syncEventPublisher.PublishAsync(Arg.Do(capture), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(true, 45, "Evento publicado.")));
    }

    private void ConfigureActiveCompany(bool syncEnabled)
    {
        _companyContext.HasActiveCompany.Returns(true);
        _companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            CompanyId: 10,
            CompanyCode: "MASTER",
            CommercialName: "Empresa Master",
            DatabaseEngine: DatabaseEngine.SqlServer,
            ConnectionString: "Server=(local);Database=NuanSystem_Tenant;",
            SapIntegrationMode: SapIntegrationMode.None,
            OperationMode: CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: syncEnabled));
    }

    private void ConfigureNoActiveCompany()
    {
        _companyContext.HasActiveCompany.Returns(false);
        _companyContext.CurrentCompany.Returns((CompanyConnectionInfo?)null);
    }

    private static WarehouseDto CreateWarehouse(bool isActive = true, string? sapCode = "SAP-BOD-AME")
    {
        return new WarehouseDto
        {
            Id = 25,
            GlobalId = Guid.NewGuid(),
            Code = "BOD-AME",
            Name = "Bodega Mega Americas",
            Description = "Bodega comercial principal",
            BranchCode = "AME",
            Address = "Av. Americas",
            City = "Cuenca",
            Province = "Azuay",
            Country = "EC",
            Phone = "0999999999",
            Email = "bodega@example.com",
            ManagerName = "Administrador",
            AllowsSales = true,
            AllowsPurchases = true,
            AllowsTransfers = true,
            AllowsProduction = false,
            IsDefault = true,
            IsActive = isActive,
            ExternalSystem = "ExternalApi",
            ExternalCode = "EXT-BOD-AME",
            SapCode = sapCode,
            CreatedAt = new DateTime(2026, 7, 10, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = isActive ? null : new DateTime(2026, 7, 10, 11, 0, 0, DateTimeKind.Utc)
        };
    }

    private static CreateWarehouseCommand CreateCommand()
    {
        return new CreateWarehouseCommand(
            GlobalId: null,
            Code: "BOD-AME",
            Name: "Bodega Mega Americas",
            Description: "Bodega comercial principal",
            BranchCode: "AME",
            Address: "Av. Americas",
            City: "Cuenca",
            Province: "Azuay",
            Country: "EC",
            Phone: "0999999999",
            Email: "bodega@example.com",
            ManagerName: "Administrador",
            AllowsSales: true,
            AllowsPurchases: true,
            AllowsTransfers: true,
            AllowsProduction: false,
            IsDefault: true,
            ExternalSystem: "ExternalApi",
            ExternalCode: "EXT-BOD-AME",
            SapCode: "SAP-BOD-AME",
            IsActive: true,
            AuditUserId: 7,
            AuditUserName: "admin");
    }

    private static UpdateWarehouseCommand UpdateCommand(int id, bool isActive = true)
    {
        var create = CreateCommand();
        return new UpdateWarehouseCommand(
            id,
            create.GlobalId,
            create.Code,
            create.Name,
            create.Description,
            create.BranchCode,
            create.Address,
            create.City,
            create.Province,
            create.Country,
            create.Phone,
            create.Email,
            create.ManagerName,
            create.AllowsSales,
            create.AllowsPurchases,
            create.AllowsTransfers,
            create.AllowsProduction,
            create.IsDefault,
            create.ExternalSystem,
            create.ExternalCode,
            create.SapCode,
            isActive,
            create.AuditUserId,
            create.AuditUserName);
    }
}
