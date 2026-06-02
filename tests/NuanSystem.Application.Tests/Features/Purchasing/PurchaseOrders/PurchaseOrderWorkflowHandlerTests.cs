using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Tests.Features.Purchasing.PurchaseOrders;

public sealed class PurchaseOrderWorkflowHandlerTests
{
    private const int OrderId = 10;
    private const int AuditUserId = 42;
    private const string AuditUserName = "tester";

    private readonly IPurchaseOrderRepository _repository = Substitute.For<IPurchaseOrderRepository>();

    [Fact]
    public async Task SendToApproval_ReturnsFailure_WhenOrderIsApproved()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Approved));
        var handler = new SendPurchaseOrderToApprovalCommandHandler(_repository);

        var result = await handler.Handle(CreateSendToApprovalCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.SendToApprovalInvalidMessage);
        await _repository.DidNotReceive()
            .UpdateStatusAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Draft)]
    [InlineData(PurchaseOrderStatuses.Rejected)]
    public async Task SendToApproval_UpdatesStatusToPendingApproval_WhenOrderCanBeConfirmed(string status)
    {
        SetupStatusChange(status, PurchaseOrderStatuses.PendingApproval);
        var handler = new SendPurchaseOrderToApprovalCommandHandler(_repository);

        var result = await handler.Handle(CreateSendToApprovalCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden enviada a aprobacion.");
        await _repository.Received(1)
            .UpdateStatusAsync(OrderId, PurchaseOrderStatuses.PendingApproval, AuditUserId, AuditUserName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Approve_ReturnsFailure_WhenOrderIsDraft()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Draft));
        var handler = new ApprovePurchaseOrderCommandHandler(_repository);

        var result = await handler.Handle(CreateApproveCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.ApproveInvalidMessage);
        await _repository.DidNotReceive()
            .UpdateStatusAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Approve_UpdatesStatusToApproved_WhenOrderIsPendingApproval()
    {
        SetupStatusChange(PurchaseOrderStatuses.PendingApproval, PurchaseOrderStatuses.Approved);
        var handler = new ApprovePurchaseOrderCommandHandler(_repository);

        var result = await handler.Handle(CreateApproveCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden aprobada correctamente.");
        await _repository.Received(1)
            .UpdateStatusAsync(OrderId, PurchaseOrderStatuses.Approved, AuditUserId, AuditUserName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Reject_ReturnsFailure_WhenOrderIsDraft()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Draft));
        var handler = new RejectPurchaseOrderCommandHandler(_repository);

        var result = await handler.Handle(CreateRejectCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.RejectInvalidMessage);
        await _repository.DidNotReceive()
            .UpdateStatusAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Reject_UpdatesStatusToRejected_WhenOrderIsPendingApproval()
    {
        SetupStatusChange(PurchaseOrderStatuses.PendingApproval, PurchaseOrderStatuses.Rejected);
        var handler = new RejectPurchaseOrderCommandHandler(_repository);

        var result = await handler.Handle(CreateRejectCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden rechazada.");
        await _repository.Received(1)
            .UpdateStatusAsync(OrderId, PurchaseOrderStatuses.Rejected, AuditUserId, AuditUserName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SyncSap_ReturnsFailureAndLogsSkipped_WhenOrderIsDraft()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Draft));
        SetupSapLog();
        var handler = new SyncPurchaseOrderSapCommandHandler(_repository);

        var result = await handler.Handle(CreateSyncSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.SapSyncInvalidMessage);
        await _repository.Received(1)
            .AddSapLogAsync(OrderId, "PurchaseOrderSync", "Skipped", PurchaseOrderWorkflowPolicy.SapSyncInvalidMessage, AuditUserId, AuditUserName, Arg.Any<CancellationToken>());
        await _repository.DidNotReceive()
            .UpdateStatusAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SyncSap_ReturnsFailure_WhenOrderIsSapSynced()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.SapSynced, PurchaseOrderSapStatuses.Synced));
        SetupSapLog();
        var handler = new SyncPurchaseOrderSapCommandHandler(_repository);

        var result = await handler.Handle(CreateSyncSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.SapSyncInvalidMessage);
        await _repository.DidNotReceive()
            .UpdateStatusAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Approved)]
    [InlineData(PurchaseOrderStatuses.SapError)]
    public async Task SyncSap_MarksOrderAsSapPending_WhenStatusAllowsRequestOrRetry(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status), CreateOrder(PurchaseOrderStatuses.SapPending));
        _repository.UpdateStatusAsync(OrderId, PurchaseOrderStatuses.SapPending, AuditUserId, AuditUserName, Arg.Any<CancellationToken>())
            .Returns(true);
        SetupSapLog();
        var handler = new SyncPurchaseOrderSapCommandHandler(_repository);

        var result = await handler.Handle(CreateSyncSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden marcada como pendiente de sincronizacion SAP.");
        await _repository.Received(1)
            .AddSapLogAsync(OrderId, "PurchaseOrderSync", "Pending", "Pendiente de envio a SAP Business One. ObjectType 22.", AuditUserId, AuditUserName, Arg.Any<CancellationToken>());
        await _repository.Received(1)
            .UpdateStatusAsync(OrderId, PurchaseOrderStatuses.SapPending, AuditUserId, AuditUserName, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.PendingApproval)]
    [InlineData(PurchaseOrderStatuses.Approved)]
    public async Task Update_ReturnsFailure_WhenOrderCannotBeEdited(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status));
        var handler = new UpdatePurchaseOrderCommandHandler(_repository);

        var result = await handler.Handle(CreateUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.EditInvalidMessage);
        await _repository.DidNotReceive()
            .UpdateAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Draft)]
    [InlineData(PurchaseOrderStatuses.Rejected)]
    [InlineData(PurchaseOrderStatuses.SapError)]
    public async Task Update_CallsRepositoryUpdate_WhenOrderCanBeEdited(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status), CreateOrder(status));
        _repository.UpdateAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = new UpdatePurchaseOrderCommandHandler(_repository);

        var result = await handler.Handle(CreateUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden de compra actualizada correctamente.");
        await _repository.Received(1)
            .UpdateAsync(Arg.Is<PurchaseOrderPersistData>(data => data.Id == OrderId && data.Status == status), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Approved)]
    [InlineData(PurchaseOrderStatuses.SapError)]
    public async Task Delete_ReturnsFailure_WhenOrderCannotBeDeleted(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status));
        var handler = new DeletePurchaseOrderCommandHandler(_repository);

        var result = await handler.Handle(CreateDeleteCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.DeleteInvalidMessage);
        await _repository.DidNotReceive()
            .DeleteAsync(Arg.Any<int>(), Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Draft)]
    [InlineData(PurchaseOrderStatuses.Rejected)]
    public async Task Delete_CallsRepositoryDelete_WhenOrderCanBeDeleted(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status));
        _repository.DeleteAsync(OrderId, AuditUserId, AuditUserName, Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = new DeletePurchaseOrderCommandHandler(_repository);

        var result = await handler.Handle(CreateDeleteCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden de compra eliminada correctamente.");
        await _repository.Received(1)
            .DeleteAsync(OrderId, AuditUserId, AuditUserName, Arg.Any<CancellationToken>());
    }

    private void SetupStatusChange(string currentStatus, string nextStatus)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(currentStatus), CreateOrder(nextStatus));
        _repository.UpdateStatusAsync(OrderId, nextStatus, AuditUserId, AuditUserName, Arg.Any<CancellationToken>())
            .Returns(true);
    }

    private void SetupSapLog()
    {
        _repository.AddSapLogAsync(OrderId, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), AuditUserId, AuditUserName, Arg.Any<CancellationToken>())
            .Returns(new PurchaseOrderSapSyncLogDto());
    }

    private static SendPurchaseOrderToApprovalCommand CreateSendToApprovalCommand()
    {
        return new SendPurchaseOrderToApprovalCommand(OrderId, AuditUserId, AuditUserName);
    }

    private static ApprovePurchaseOrderCommand CreateApproveCommand()
    {
        return new ApprovePurchaseOrderCommand(OrderId, "ok", AuditUserId, AuditUserName);
    }

    private static RejectPurchaseOrderCommand CreateRejectCommand()
    {
        return new RejectPurchaseOrderCommand(OrderId, "no", AuditUserId, AuditUserName);
    }

    private static SyncPurchaseOrderSapCommand CreateSyncSapCommand()
    {
        return new SyncPurchaseOrderSapCommand(OrderId, AuditUserId, AuditUserName);
    }

    private static DeletePurchaseOrderCommand CreateDeleteCommand()
    {
        return new DeletePurchaseOrderCommand(OrderId, AuditUserId, AuditUserName);
    }

    private static UpdatePurchaseOrderCommand CreateUpdateCommand()
    {
        return new UpdatePurchaseOrderCommand(
            OrderId,
            BranchId: 1,
            DocumentSeriesId: 1,
            SeriesCode: "OC-2026",
            DocumentNumber: "OC-000010",
            SupplierId: 100,
            SupplierCode: "SUP-001",
            SupplierName: "Proveedor de prueba",
            SupplierTaxId: "0999999999001",
            ContactName: "Compras",
            Phone: "0999999999",
            Email: "compras@example.com",
            DocumentDate: new DateTime(2026, 6, 2),
            DeliveryDate: new DateTime(2026, 6, 5),
            CurrencyCode: "USD",
            ExchangeRate: 1m,
            PaymentTermId: 1,
            PriceListId: 1,
            BuyerId: 1,
            MainWarehouseId: 1,
            ProjectId: null,
            CostCenterId: null,
            PurchaseTypeId: 1,
            Comments: "Orden de prueba",
            DiscountPercent: 0m,
            Lines: [CreateLine()],
            Addresses: [CreateAddress("Delivery"), CreateAddress("Billing")],
            RelatedDocuments: [],
            Attachments: [],
            AuditUserId,
            AuditUserName);
    }

    private static PurchaseOrderDto CreateOrder(string status, string sapStatus = PurchaseOrderSapStatuses.Pending)
    {
        return new PurchaseOrderDto
        {
            Id = OrderId,
            BranchId = 1,
            DocumentSeriesId = 1,
            SeriesCode = "OC-2026",
            DocumentNumber = "OC-000010",
            SupplierId = 100,
            SupplierCode = "SUP-001",
            SupplierName = "Proveedor de prueba",
            DocumentDate = new DateTime(2026, 6, 2),
            DeliveryDate = new DateTime(2026, 6, 5),
            CurrencyCode = "USD",
            ExchangeRate = 1m,
            PaymentTermId = 1,
            BuyerId = 1,
            MainWarehouseId = 1,
            Status = status,
            SapStatus = sapStatus,
            Lines =
            [
                new PurchaseOrderLineDto
                {
                    Id = 1,
                    LineNumber = 1,
                    ItemId = 1,
                    ItemCode = "ART-001",
                    ItemName = "Articulo de prueba",
                    UnitId = 1,
                    UnitCode = "UND",
                    Quantity = 2m,
                    OpenQuantity = 2m,
                    UnitPrice = 10m,
                    TaxId = 1,
                    TaxCode = "IVA",
                    TaxRate = 12m,
                    WarehouseId = 1,
                    WarehouseCode = "BOD-001",
                    DeliveryDate = new DateTime(2026, 6, 5),
                    Status = PurchaseOrderLineStatuses.Open
                }
            ],
            Addresses =
            [
                new PurchaseOrderAddressDto { Id = 1, AddressType = "Delivery" },
                new PurchaseOrderAddressDto { Id = 2, AddressType = "Billing" }
            ]
        };
    }

    private static PurchaseOrderLineSaveRequest CreateLine()
    {
        return new PurchaseOrderLineSaveRequest(
            Id: 1,
            LineNumber: 1,
            ItemId: 1,
            ItemCode: "ART-001",
            ItemName: "Articulo de prueba",
            UnitId: 1,
            UnitCode: "UND",
            Quantity: 2m,
            UnitPrice: 10m,
            DiscountPercent: 0m,
            TaxId: 1,
            TaxCode: "IVA",
            TaxRate: 12m,
            WarehouseId: 1,
            WarehouseCode: "BOD-001",
            DeliveryDate: new DateTime(2026, 6, 5),
            CostCenterId: null,
            ProjectId: null);
    }

    private static PurchaseOrderAddressSaveRequest CreateAddress(string addressType)
    {
        return new PurchaseOrderAddressSaveRequest(
            Id: null,
            AddressType: addressType,
            SourceAddressId: null,
            AddressName: $"{addressType} address",
            Street: "Main street",
            Reference: null,
            City: "Quito",
            State: "Pichincha",
            ZipCode: null,
            Country: "EC",
            Phone: null,
            Email: null,
            IsModified: false);
    }
}
