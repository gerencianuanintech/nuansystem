using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Tests.Features.Purchasing.PurchaseOrders;

public sealed class PurchaseOrderWorkflowHandlerTests
{
    private const int OrderId = 10;
    private const int AuditUserId = 42;
    private const string AuditUserName = "tester";
    private const string CompanyCode = "EMPRESA01";

    private readonly IPurchaseOrderRepository _repository = Substitute.For<IPurchaseOrderRepository>();
    private readonly ISecurityDocumentSeriesAccessRepository _seriesAccessRepository = Substitute.For<ISecurityDocumentSeriesAccessRepository>();
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();
    private readonly ISapSyncProcessorRegistry _sapSyncProcessors = Substitute.For<ISapSyncProcessorRegistry>();

    public PurchaseOrderWorkflowHandlerTests()
    {
        _companyContext.HasActiveCompany.Returns(true);
        _companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            CompanyId: 1,
            CompanyCode,
            CommercialName: "Empresa de prueba",
            DatabaseEngine: DatabaseEngine.SqlServer,
            ConnectionString: "Server=(local);Database=NuanSystem_Test;Trusted_Connection=True;",
            SapIntegrationMode: SapIntegrationMode.None));

        _seriesAccessRepository.ValidateUserOperationAsync(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(true);

        _sapSyncProcessors.FindExporter(SapSyncEntityCode.PurchaseOrders)
            .Returns(Substitute.For<ISapSyncExporter>());
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("OC-AUTH", false)]
    public async Task Creation_uses_authorized_series_and_reserved_number_or_rejects_missing_code(string code, bool reject)
    {
        var seriesRepository = Substitute.For<ISecurityDocumentSeriesRepository>();
        var numbering = Substitute.For<ISecurityDocumentNumberingService>();
        seriesRepository.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(
            new NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos.SecurityDocumentSeriesDto
            { Id = 3, Code = code, DocumentType = PurchaseOrderSecurity.DocumentType, IsActive = true });
        numbering.ReserveNumberAsync(3, AuditUserId, AuditUserName, Arg.Any<CancellationToken>()).Returns(
            new NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos.ReserveSecurityDocumentNumberResult(
                true, 42, "OC-RESERVED-42", "42", "Reserved"));
        _repository.CreateAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<CancellationToken>()).Returns(OrderId);
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>()).Returns(CreateOrder(PurchaseOrderStatuses.Draft));
        var handler = new CreatePurchaseOrderCommandHandler(_repository, seriesRepository, numbering, _seriesAccessRepository, _companyContext);
        var data = PurchaseOrderCalculationCharacterizationTests.Request(PurchaseOrderCalculationCharacterizationTests.Line(2m, 100m, 10m, 0.15m)) with { DiscountPercent = 10m };
        var command = new CreatePurchaseOrderCommand(
            data.BranchId, data.DocumentSeriesId, "UNTRUSTED-SERIES", "UNTRUSTED-NUMBER",
            data.SupplierId, data.SupplierCode, data.SupplierName, data.SupplierTaxId, data.ContactName, data.Phone, data.Email,
            data.DocumentDate, data.DeliveryDate, data.CurrencyCode, data.ExchangeRate, data.PaymentTermId, data.PriceListId,
            data.BuyerId, data.MainWarehouseId, data.ProjectId, data.CostCenterId, data.PurchaseTypeId, data.Comments,
            data.DiscountPercent, data.Lines, data.Addresses, data.RelatedDocuments, data.Attachments, AuditUserId, AuditUserName);
        var result = await handler.Handle(command, CancellationToken.None);
        Assert.Equal(!reject, result.IsSuccess);
        if (reject)
        {
            Assert.Empty(numbering.ReceivedCalls());
            await _repository.DidNotReceive().CreateAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<CancellationToken>());
        }
        else
        {
            await _repository.Received(1).CreateAsync(Arg.Is<PurchaseOrderPersistData>(order =>
                order.DocumentSeriesId == 3 && order.SeriesCode == "OC-AUTH" && order.DocumentNumber == "OC-RESERVED-42"
                && order.TotalAmount == 186.3m && order.SapObjectType == "22"
                && order.AuditUserId == AuditUserId && order.AuditUserName == AuditUserName), Arg.Any<CancellationToken>());
        }
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    public async Task Missing_persisted_numbering_is_rejected_without_inventing_a_number(bool missingSeries, bool collection)
    {
        var order = CreateOrder(PurchaseOrderStatuses.Draft);
        if (missingSeries) order.SeriesCode = " "; else order.DocumentNumber = " ";
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>()).Returns(order);
        _repository.UpdateIfEditableAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>()).Returns(true);
        _repository.UpdateAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<CancellationToken>()).Returns(true);
        var result = collection
            ? await PurchaseOrderCollections.UpdateAsync(_repository, OrderId, current => PurchaseOrderMapping.ToSaveRequest(current),
                "Updated", AuditUserId, AuditUserName, CancellationToken.None)
            : await CreateUpdateHandler().Handle(CreateUpdateCommand(), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == "PURCHASE_ORDER_NUMBERING_MISSING");
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().UpdateIfEditableAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendToApproval_ReturnsFailure_WhenOrderIsApproved()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Approved));
        var handler = CreateSendToApprovalHandler();

        var result = await handler.Handle(CreateSendToApprovalCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.SendToApprovalInvalidMessage);
        await _repository.DidNotReceive()
            .TransitionWorkflowAsync(Arg.Any<PurchaseOrderWorkflowTransitionData>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Draft)]
    [InlineData(PurchaseOrderStatuses.Rejected)]
    public async Task SendToApproval_UpdatesStatusToPendingApproval_WhenOrderCanBeConfirmed(string status)
    {
        SetupStatusChange(status, PurchaseOrderStatuses.PendingApproval);
        var handler = CreateSendToApprovalHandler();

        var result = await handler.Handle(CreateSendToApprovalCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden enviada a aprobacion.");
        await _repository.Received(1)
            .TransitionWorkflowAsync(
                Arg.Is<PurchaseOrderWorkflowTransitionData>(data =>
                    data.OrderId == OrderId
                    && data.NextStatus == PurchaseOrderStatuses.PendingApproval
                    && Matches(data.ExpectedCurrentStatuses, PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected)),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendToApproval_ReturnsFailure_WhenAtomicTransitionDoesNotUpdateRows()
    {
        SetupStatusChange(PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.PendingApproval, updated: false);
        var handler = CreateSendToApprovalHandler();

        var result = await handler.Handle(CreateSendToApprovalCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("No se pudo actualizar el estado de la orden.");
    }

    [Fact]
    public async Task SendToApproval_CreatesOnePendingLevelOneApprovalWithTrustedIdentity()
    {
        var current = CreateOrder(PurchaseOrderStatuses.Draft);
        var transitioned = CreateOrder(PurchaseOrderStatuses.PendingApproval);
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>()).Returns(current);
        _repository.TransitionWorkflowAsync(
                Arg.Any<PurchaseOrderWorkflowTransitionData>(),
                Arg.Any<CancellationToken>())
            .Returns(PurchaseOrderWorkflowTransitionResult.Updated(transitioned));

        var result = await CreateSendToApprovalHandler().Handle(CreateSendToApprovalCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).TransitionWorkflowAsync(
            Arg.Is<PurchaseOrderWorkflowTransitionData>(data =>
                data.OrderId == OrderId
                && data.Action == PurchaseOrderWorkflowActions.RequestApproval
                && data.ApprovalLevel == 1
                && data.NextStatus == PurchaseOrderStatuses.PendingApproval
                && data.Observation == null
                && data.UserId == AuditUserId
                && data.UserName == AuditUserName),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(true, PurchaseOrderStatuses.Approved, PurchaseOrderWorkflowActions.Approve, "ok")]
    [InlineData(false, PurchaseOrderStatuses.Rejected, PurchaseOrderWorkflowActions.Reject, "no")]
    public async Task Decision_UpdatesThePendingApprovalAtomically(
        bool approve,
        string nextStatus,
        string action,
        string observation)
    {
        var current = CreateOrder(PurchaseOrderStatuses.PendingApproval);
        var transitioned = CreateOrder(nextStatus);
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>()).Returns(current);
        _repository.TransitionWorkflowAsync(
                Arg.Any<PurchaseOrderWorkflowTransitionData>(),
                Arg.Any<CancellationToken>())
            .Returns(PurchaseOrderWorkflowTransitionResult.Updated(transitioned));

        var result = approve
            ? await CreateApproveHandler().Handle(CreateApproveCommand(), CancellationToken.None)
            : await CreateRejectHandler().Handle(CreateRejectCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).TransitionWorkflowAsync(
            Arg.Is<PurchaseOrderWorkflowTransitionData>(data =>
                data.Action == action
                && data.NextStatus == nextStatus
                && data.Observation == observation
                && data.ExpectedCurrentStatuses.SequenceEqual(new[] { PurchaseOrderStatuses.PendingApproval })
                && data.UserId == AuditUserId
                && data.UserName == AuditUserName),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Approve_ReturnsConflict_WhenAtomicWorkflowTransitionLosesTheRace()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.PendingApproval));
        _repository.TransitionWorkflowAsync(
                Arg.Any<PurchaseOrderWorkflowTransitionData>(),
                Arg.Any<CancellationToken>())
            .Returns(PurchaseOrderWorkflowTransitionResult.Conflict());

        var result = await CreateApproveHandler().Handle(CreateApproveCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.FailureKind.Should().Be(FailureKind.Conflict);
        result.Errors.Should().ContainSingle(error => error.Code == "PURCHASE_ORDER_STATE_CONFLICT");
    }

    [Fact]
    public async Task Approve_ReturnsFailure_WhenOrderIsDraft()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Draft));
        var handler = CreateApproveHandler();

        var result = await handler.Handle(CreateApproveCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.ApproveInvalidMessage);
        await _repository.DidNotReceive()
            .TransitionWorkflowAsync(Arg.Any<PurchaseOrderWorkflowTransitionData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Approve_UpdatesStatusToApproved_WhenOrderIsPendingApproval()
    {
        SetupStatusChange(PurchaseOrderStatuses.PendingApproval, PurchaseOrderStatuses.Approved);
        var handler = CreateApproveHandler();

        var result = await handler.Handle(CreateApproveCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden aprobada correctamente.");
        await _repository.Received(1)
            .TransitionWorkflowAsync(
                Arg.Is<PurchaseOrderWorkflowTransitionData>(data =>
                    data.OrderId == OrderId
                    && data.NextStatus == PurchaseOrderStatuses.Approved
                    && Matches(data.ExpectedCurrentStatuses, PurchaseOrderStatuses.PendingApproval)),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Reject_ReturnsFailure_WhenOrderIsDraft()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Draft));
        var handler = CreateRejectHandler();

        var result = await handler.Handle(CreateRejectCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.RejectInvalidMessage);
        await _repository.DidNotReceive()
            .TransitionWorkflowAsync(Arg.Any<PurchaseOrderWorkflowTransitionData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Reject_UpdatesStatusToRejected_WhenOrderIsPendingApproval()
    {
        SetupStatusChange(PurchaseOrderStatuses.PendingApproval, PurchaseOrderStatuses.Rejected);
        var handler = CreateRejectHandler();

        var result = await handler.Handle(CreateRejectCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden rechazada.");
        await _repository.Received(1)
            .TransitionWorkflowAsync(
                Arg.Is<PurchaseOrderWorkflowTransitionData>(data =>
                    data.OrderId == OrderId
                    && data.NextStatus == PurchaseOrderStatuses.Rejected
                    && Matches(data.ExpectedCurrentStatuses, PurchaseOrderStatuses.PendingApproval)),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SyncSap_ReturnsFailureAndLogsSkipped_WhenOrderIsDraft()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Draft));
        SetupSapLog();
        var handler = CreateSyncSapHandler();

        var result = await handler.Handle(CreateSyncSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.SapSyncInvalidMessage);
        await _repository.Received(1)
            .AddSapLogAsync(OrderId, "PurchaseOrderSync", "Skipped", PurchaseOrderWorkflowPolicy.SapSyncInvalidMessage, AuditUserId, AuditUserName, Arg.Any<CancellationToken>());
        await _repository.DidNotReceive()
            .UpdateStatusIfCurrentAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SyncSap_ReturnsFailure_WhenOrderIsSapSynced()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.SapSynced, PurchaseOrderSapStatuses.Synced));
        SetupSapLog();
        var handler = CreateSyncSapHandler();

        var result = await handler.Handle(CreateSyncSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.SapSyncInvalidMessage);
        await _repository.DidNotReceive()
            .UpdateStatusIfCurrentAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Approved)]
    [InlineData(PurchaseOrderStatuses.SapError)]
    public async Task SyncSap_MarksOrderAsSapPending_WhenStatusAllowsRequestOrRetry(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status), CreateOrder(PurchaseOrderStatuses.SapPending));
        _repository.UpdateStatusIfCurrentAsync(
                OrderId,
                PurchaseOrderStatuses.SapPending,
                Arg.Is<IReadOnlyCollection<string>>(statuses => Matches(statuses, PurchaseOrderStatuses.Approved, PurchaseOrderStatuses.SapError)),
                AuditUserId,
                AuditUserName,
                Arg.Any<CancellationToken>())
            .Returns(true);
        SetupSapLog();
        var handler = CreateSyncSapHandler();

        var result = await handler.Handle(CreateSyncSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden marcada como pendiente de sincronizacion SAP.");
        await _repository.Received(1)
            .AddSapLogAsync(OrderId, "PurchaseOrderSync", "Pending", "Pendiente de envio a SAP Business One. ObjectType 22.", AuditUserId, AuditUserName, Arg.Any<CancellationToken>());
        await _repository.Received(1)
            .UpdateStatusIfCurrentAsync(
                OrderId,
                PurchaseOrderStatuses.SapPending,
                Arg.Is<IReadOnlyCollection<string>>(statuses => Matches(statuses, PurchaseOrderStatuses.Approved, PurchaseOrderStatuses.SapError)),
                AuditUserId,
                AuditUserName,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SyncSap_ReturnsFailure_WhenAtomicTransitionDoesNotUpdateRows()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Approved));
        _repository.UpdateStatusIfCurrentAsync(
                OrderId,
                PurchaseOrderStatuses.SapPending,
                Arg.Is<IReadOnlyCollection<string>>(statuses => Matches(statuses, PurchaseOrderStatuses.Approved, PurchaseOrderStatuses.SapError)),
                AuditUserId,
                AuditUserName,
                Arg.Any<CancellationToken>())
            .Returns(false);
        SetupSapLog();
        var handler = CreateSyncSapHandler();

        var result = await handler.Handle(CreateSyncSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("No se pudo actualizar el estado de la orden.");
    }

    [Fact]
    public async Task SyncSap_WhenExporterIsUnavailable_DoesNotMutateOrderOrLogPending()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Approved));
        _sapSyncProcessors.FindExporter(SapSyncEntityCode.PurchaseOrders)
            .Returns((ISapSyncExporter?)null);
        var handler = CreateSyncSapHandler();

        var result = await handler.Handle(CreateSyncSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.FailureKind.Should().Be(FailureKind.Unavailable);
        result.Errors.Should().ContainSingle(error => error.Code == "SAP_EXPORT_NOT_AVAILABLE");
        await _repository.DidNotReceive().AddSapLogAsync(
            Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(),
            Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().UpdateStatusIfCurrentAsync(
            Arg.Any<int>(), Arg.Any<string>(), Arg.Any<IReadOnlyCollection<string>>(),
            Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.PendingApproval)]
    [InlineData(PurchaseOrderStatuses.Approved)]
    public async Task Update_ReturnsFailure_WhenOrderCannotBeEdited(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status));
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(CreateUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.EditInvalidMessage);
        await _repository.DidNotReceive()
            .UpdateIfEditableAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Draft)]
    [InlineData(PurchaseOrderStatuses.Rejected)]
    [InlineData(PurchaseOrderStatuses.SapError)]
    public async Task Update_CallsRepositoryUpdate_WhenOrderCanBeEdited(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status), CreateOrder(status));
        _repository.UpdateIfEditableAsync(
                Arg.Any<PurchaseOrderPersistData>(),
                Arg.Is<IReadOnlyCollection<string>>(statuses => Matches(statuses, PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected, PurchaseOrderStatuses.SapError)),
                Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(CreateUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden de compra actualizada correctamente.");
        await _repository.Received(1)
            .UpdateIfEditableAsync(
                Arg.Is<PurchaseOrderPersistData>(data => data.Id == OrderId && data.Status == status),
                Arg.Is<IReadOnlyCollection<string>>(statuses => Matches(statuses, PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected, PurchaseOrderStatuses.SapError)),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ReturnsFailure_WhenAtomicEditableUpdateDoesNotUpdateRows()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Draft));
        _repository.UpdateIfEditableAsync(
                Arg.Any<PurchaseOrderPersistData>(),
                Arg.Is<IReadOnlyCollection<string>>(statuses => Matches(statuses, PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected, PurchaseOrderStatuses.SapError)),
                Arg.Any<CancellationToken>())
            .Returns(false);
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(CreateUpdateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("No se pudo actualizar la orden de compra.");
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Approved)]
    [InlineData(PurchaseOrderStatuses.SapError)]
    public async Task Delete_ReturnsFailure_WhenOrderCannotBeDeleted(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status));
        var handler = CreateDeleteHandler();

        var result = await handler.Handle(CreateDeleteCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be(PurchaseOrderWorkflowPolicy.DeleteInvalidMessage);
        await _repository.DidNotReceive()
            .DeleteIfCurrentAsync(Arg.Any<int>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<int?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Draft)]
    [InlineData(PurchaseOrderStatuses.Rejected)]
    public async Task Delete_CallsRepositoryDeleteIfCurrent_WhenOrderCanBeDeleted(string status)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(status));
        _repository.DeleteIfCurrentAsync(
                OrderId,
                Arg.Is<IReadOnlyCollection<string>>(statuses => Matches(statuses, PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected)),
                AuditUserId,
                AuditUserName,
                Arg.Any<CancellationToken>())
            .Returns(true);
        var handler = CreateDeleteHandler();

        var result = await handler.Handle(CreateDeleteCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Orden de compra eliminada correctamente.");
        await _repository.Received(1)
            .DeleteIfCurrentAsync(
                OrderId,
                Arg.Is<IReadOnlyCollection<string>>(statuses => Matches(statuses, PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected)),
                AuditUserId,
                AuditUserName,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_ReturnsFailure_WhenAtomicCancellationDoesNotUpdateRows()
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(PurchaseOrderStatuses.Draft));
        _repository.DeleteIfCurrentAsync(
                OrderId,
                Arg.Is<IReadOnlyCollection<string>>(statuses => Matches(statuses, PurchaseOrderStatuses.Draft, PurchaseOrderStatuses.Rejected)),
                AuditUserId,
                AuditUserName,
                Arg.Any<CancellationToken>())
            .Returns(false);
        var handler = CreateDeleteHandler();

        var result = await handler.Handle(CreateDeleteCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("No se pudo eliminar la orden de compra.");
    }

    private void SetupStatusChange(string currentStatus, string nextStatus, bool updated = true)
    {
        _repository.GetByIdAsync(OrderId, Arg.Any<CancellationToken>())
            .Returns(CreateOrder(currentStatus));
        _repository.TransitionWorkflowAsync(
                Arg.Any<PurchaseOrderWorkflowTransitionData>(),
                Arg.Any<CancellationToken>())
            .Returns(updated
                ? PurchaseOrderWorkflowTransitionResult.Updated(CreateOrder(nextStatus))
                : PurchaseOrderWorkflowTransitionResult.Conflict());
    }

    private void SetupSapLog()
    {
        _repository.AddSapLogAsync(OrderId, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>(), AuditUserId, AuditUserName, Arg.Any<CancellationToken>())
            .Returns(new PurchaseOrderSapSyncLogDto());
    }

    private SendPurchaseOrderToApprovalCommandHandler CreateSendToApprovalHandler()
    {
        return new SendPurchaseOrderToApprovalCommandHandler(_repository, _seriesAccessRepository, _companyContext);
    }

    private ApprovePurchaseOrderCommandHandler CreateApproveHandler()
    {
        return new ApprovePurchaseOrderCommandHandler(_repository, _seriesAccessRepository, _companyContext);
    }

    private RejectPurchaseOrderCommandHandler CreateRejectHandler()
    {
        return new RejectPurchaseOrderCommandHandler(_repository, _seriesAccessRepository, _companyContext);
    }

    private SyncPurchaseOrderSapCommandHandler CreateSyncSapHandler()
    {
        return new SyncPurchaseOrderSapCommandHandler(
            _repository,
            _seriesAccessRepository,
            _companyContext,
            _sapSyncProcessors);
    }

    private UpdatePurchaseOrderCommandHandler CreateUpdateHandler()
    {
        return new UpdatePurchaseOrderCommandHandler(_repository, _seriesAccessRepository, _companyContext);
    }

    private DeletePurchaseOrderCommandHandler CreateDeleteHandler()
    {
        return new DeletePurchaseOrderCommandHandler(_repository, _seriesAccessRepository, _companyContext);
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

    private static bool Matches(IReadOnlyCollection<string> actual, params string[] expected)
    {
        return actual.Count == expected.Length && !expected.Except(actual).Any();
    }
}
