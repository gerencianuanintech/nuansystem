using System.Net;
using System.Net.Http.Json;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Tests.Features.Purchasing.PurchaseOrders;
using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.Application.Tests.Features.Integration;

[Trait("Category", "InMemoryIntegration")]
public sealed class BackendIntegratedHttpTests
{
    [Fact]
    public async Task Real_routes_and_WinForms_client_preserve_404_and_separate_company_contexts()
    {
        await using var f = await BackendApiFixture.CreateAsync();
        var first = await Assert.ThrowsAsync<ApiClientException>(() => f.PurchaseOrders.GetByIdAsync(7));
        Assert.Equal(404, first.StatusCode);
        Assert.Equal("PURCHASE_ORDER_NOT_FOUND", Assert.Single(first.Errors).Code);
        f.Session.SelectCompany(new UserCompanyResponse(2, "TEST-2", "Second", null, null, null));
        var second = await Assert.ThrowsAsync<ApiClientException>(() => f.PurchaseOrders.GetByIdAsync(7));
        Assert.Equal(404, second.StatusCode);
        Assert.Equal(new[] { 1, 2 }, f.RepositoryCompanyIds);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Company_or_form_denial_returns_403_without_resolving_purchase_repository(bool denyForm)
    {
        await using var f = await BackendApiFixture.CreateAsync();
        f.AllowForm = !denyForm;
        f.AllowCompany = denyForm;
        var error = await Assert.ThrowsAsync<ApiClientException>(() => f.PurchaseOrders.GetByIdAsync(7));
        Assert.Equal(403, error.StatusCode);
        Assert.Empty(f.RepositoryCompanyIds);
        Assert.DoesNotContain(f.Orders.ReceivedCalls(), call => call.GetMethodInfo().Name == "GetByIdAsync");
    }

    [Fact]
    public async Task Unauthenticated_request_is_rejected_before_repository_resolution()
    {
        await using var f = await BackendApiFixture.CreateAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/purchase-orders/7");
        request.Headers.Add("X-Company-Code", "TEST-1");
        using var response = await f.Http.SendAsync(request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Empty(f.RepositoryCompanyIds);
    }

    [Fact]
    public async Task Server_exception_is_safe_for_the_real_client()
    {
        await using var f = await BackendApiFixture.CreateAsync();
        f.Orders.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns(Task.FromException<PurchaseOrderDto?>(new InvalidOperationException("PHASE9_PRIVATE_DIAGNOSTIC")));
        var error = await Assert.ThrowsAsync<ApiClientException>(() => f.PurchaseOrders.GetByIdAsync(7));
        Assert.Equal(500, error.StatusCode);
        Assert.DoesNotContain("PHASE9_PRIVATE_DIAGNOSTIC", error.Message);
        Assert.DoesNotContain(error.Errors, item => item.Message.Contains("PHASE9_PRIVATE_DIAGNOSTIC", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Supplier_import_crosses_actual_endpoint_mediator_and_service_with_audit_from_claims()
    {
        await using var f = await BackendApiFixture.CreateAsync();
        SapSupplierImportResultDto result;
        try { result = await f.Api.PostAsync<object, SapSupplierImportResultDto>("/api/sap/suppliers/import", new { AuditUserId = 999 }); }
        catch (ApiClientException) { throw new Xunit.Sdk.XunitException(f.LastDiagnostic ?? "No API diagnostic"); }
        Assert.Equal(1, result.Created);
        await f.Reader.Received(1).GetSuppliersAsync(1, Arg.Any<CancellationToken>());
        await f.Imports.Received(1).ImportSupplierFromSapAsync(Arg.Is<BusinessPartnerSapImportData>(data => data.AuditUserId == 42 && data.AuditUserName == "phase9-user"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Missing_SAP_permission_never_calls_the_reader()
    {
        await using var f = await BackendApiFixture.CreateAsync();
        f.AllowSapPermission = false;
        var error = await Assert.ThrowsAsync<ApiClientException>(() => f.Api.PostAsync<object, SapSupplierImportResultDto>("/api/sap/suppliers/import", new { }));
        Assert.Equal(403, error.StatusCode);
        Assert.Empty(f.Reader.ReceivedCalls());
    }

    [Fact]
    public async Task In_flight_import_cancellation_reaches_client_without_persisting_a_supplier()
    {
        await using var f = await BackendApiFixture.CreateAsync();
        using var cancellation = new CancellationTokenSource();
        f.Reader.GetSuppliersAsync(1, Arg.Any<CancellationToken>()).Returns(call =>
        { cancellation.Cancel(); return Task.FromCanceled<IReadOnlyCollection<SapSupplierRecord>>(call.Arg<CancellationToken>()); });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => f.Api.PostAsync<object, SapSupplierImportResultDto>("/api/sap/suppliers/import", new { }, cancellation.Token));
        await f.Reader.Received(1).GetSuppliersAsync(1, Arg.Is<CancellationToken>(token => token.IsCancellationRequested));
        Assert.Empty(f.Imports.ReceivedCalls());
    }

    [Fact]
    public async Task Purchase_creation_keeps_calculation_numbering_and_trusted_audit_across_HTTP()
    {
        await using var f = await BackendApiFixture.CreateAsync();
        var data = PurchaseOrderCalculationCharacterizationTests.Request(PurchaseOrderCalculationCharacterizationTests.Line(2m, 100m, 10m, 0.15m)) with { DiscountPercent = 10m };
        var address = new PurchaseOrderAddressSaveRequest(null, "Delivery", null, null, "Street", null, null, null, null, "EC", null, null, false);
        data = data with { Addresses = [address, address with { AddressType = "Billing" }] };
        var command = new CreatePurchaseOrderCommand(data.BranchId, data.DocumentSeriesId, data.SeriesCode, "CLIENT-NUMBER",
            data.SupplierId, data.SupplierCode, data.SupplierName, data.SupplierTaxId, data.ContactName, data.Phone, data.Email,
            data.DocumentDate, data.DeliveryDate, data.CurrencyCode, data.ExchangeRate, data.PaymentTermId, data.PriceListId,
            data.BuyerId, data.MainWarehouseId, data.ProjectId, data.CostCenterId, data.PurchaseTypeId, data.Comments,
            data.DiscountPercent, data.Lines, data.Addresses, data.RelatedDocuments, data.Attachments, 999, "Untrusted");
        f.Orders.CreateAsync(Arg.Any<PurchaseOrderPersistData>(), Arg.Any<CancellationToken>()).Returns(7);
        f.Orders.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns(new PurchaseOrderDto { Id = 7, TotalAmount = 186.3m });
        var result = await f.Api.PostAsync<CreatePurchaseOrderCommand, PurchaseOrderDto>("/api/purchase-orders", command);
        Assert.Equal(7, result.Id);
        await f.Orders.Received(1).CreateAsync(Arg.Is<PurchaseOrderPersistData>(order => order.TotalAmount == 186.3m
            && order.SeriesCode == "AUTHORIZED" && order.DocumentNumber == "RESERVED-42" && order.AuditUserId == 42 && order.AuditUserName == "phase9-user"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Invalid_purchase_is_400_before_number_reservation_or_write()
    {
        await using var f = await BackendApiFixture.CreateAsync();
        var error = await Assert.ThrowsAsync<ApiClientException>(() => f.Api.PostAsync<object, PurchaseOrderDto>("/api/purchase-orders", new { SupplierId = 0, Lines = Array.Empty<object>(), Addresses = Array.Empty<object>() }));
        Assert.True(error.StatusCode == 400, $"Expected 400, got {error.StatusCode}: {f.LastDiagnostic}");
        Assert.NotEmpty(error.Errors);
        Assert.Empty(f.Numbering.ReceivedCalls());
        Assert.DoesNotContain(f.Orders.ReceivedCalls(), call => call.GetMethodInfo().Name == "CreateAsync");
    }
}
