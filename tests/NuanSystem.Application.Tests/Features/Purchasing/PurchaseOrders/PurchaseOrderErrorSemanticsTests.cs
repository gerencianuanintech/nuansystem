using Microsoft.AspNetCore.Http;
using NSubstitute;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Queries;
using NuanSystem.Application.Tests.Infrastructure;
using NuanSystem.Domain.Tenancy;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Purchasing.PurchaseOrders;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.Application.Tests.Features.Purchasing.PurchaseOrders;

public sealed class PurchaseOrderErrorSemanticsTests
{
    [Fact]
    public async Task Missing_query_is_404_and_is_readable_by_the_real_typed_WinForms_client()
    {
        var repository = Substitute.For<IPurchaseOrderRepository>();
        var handler = new GetPurchaseOrderByIdQueryHandler(repository);
        using var http = new HttpClient(new InMemoryResultHandler(async token =>
            (await handler.Handle(new GetPurchaseOrderByIdQuery(7), token)).ToHttpResult()))
        { BaseAddress = new Uri("https://phase3.invalid") };

        var exception = await Assert.ThrowsAsync<ApiClientException>(() =>
            new PurchaseOrderClient(new NuanApiClient(http, new ApiSession())).GetByIdAsync(7));

        Assert.Equal(404, exception.StatusCode);
        Assert.Equal("PURCHASE_ORDER_NOT_FOUND", Assert.Single(exception.Errors).Code);
    }

    [Fact]
    public async Task Delete_missing_order_is_NotFound_and_does_not_attempt_a_write()
    {
        var fixture = new Fixture();
        var result = await fixture.Delete.Handle(new DeletePurchaseOrderCommand(7, 1, "tester"), CancellationToken.None);
        Assert.Equal(FailureKind.NotFound, result.FailureKind);
        Assert.Equal(404, Status(result.ToHttpResult()));
        Assert.DoesNotContain(fixture.Repository.ReceivedCalls(), call => call.GetMethodInfo().Name == "DeleteIfCurrentAsync");
    }

    [Fact]
    public async Task Invalid_delete_state_is_Conflict_instead_of_validation()
    {
        var fixture = new Fixture(PurchaseOrderStatuses.Approved);
        var result = await fixture.Delete.Handle(new DeletePurchaseOrderCommand(7, 1, "tester"), CancellationToken.None);
        Assert.Equal(FailureKind.Conflict, result.FailureKind);
        Assert.Equal(409, Status(result.ToHttpResult()));
        Assert.DoesNotContain(fixture.Access.ReceivedCalls(), call => call.GetMethodInfo().Name == "ValidateUserOperationAsync");
    }

    [Fact]
    public async Task Denied_series_permission_reaches_the_outer_handler_as_Forbidden()
    {
        var fixture = new Fixture(PurchaseOrderStatuses.Draft);
        fixture.AllowSeries(false);
        var result = await fixture.Delete.Handle(new DeletePurchaseOrderCommand(7, 1, "tester"), CancellationToken.None);
        Assert.Equal(FailureKind.Forbidden, result.FailureKind);
        Assert.Equal(403, Status(result.ToHttpResult()));
        Assert.Equal("PURCHASE_ORDER_SERIES_FORBIDDEN", Assert.Single(result.Errors).Code);
        Assert.DoesNotContain(fixture.Repository.ReceivedCalls(), call => call.GetMethodInfo().Name == "DeleteIfCurrentAsync");
    }

    [Fact]
    public async Task Lost_atomic_transition_is_Conflict()
    {
        var fixture = new Fixture(PurchaseOrderStatuses.Draft);
        var result = await fixture.Send.Handle(new SendPurchaseOrderToApprovalCommand(7, 1, "tester"), CancellationToken.None);
        Assert.Equal(FailureKind.Conflict, result.FailureKind);
        Assert.Equal(409, Status(result.ToHttpResult()));
    }

    [Fact]
    public async Task Missing_read_after_a_successful_transition_is_Unexpected_not_NotFound()
    {
        var fixture = new Fixture(PurchaseOrderStatuses.Draft);
        fixture.Repository.TransitionWorkflowAsync(
                Arg.Any<PurchaseOrderWorkflowTransitionData>(),
                Arg.Any<CancellationToken>())
            .Returns(new PurchaseOrderWorkflowTransitionResult(
                PurchaseOrderWorkflowTransitionOutcome.Updated,
                null));

        var result = await fixture.Send.Handle(new SendPurchaseOrderToApprovalCommand(7, 1, "tester"), CancellationToken.None);

        Assert.Equal(FailureKind.Unexpected, result.FailureKind);
        Assert.Equal(500, Status(result.ToHttpResult()));
    }

    private static int? Status(IResult result) => Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode;

    private sealed class Fixture
    {
        public IPurchaseOrderRepository Repository { get; } = Substitute.For<IPurchaseOrderRepository>();
        public ISecurityDocumentSeriesAccessRepository Access { get; } = Substitute.For<ISecurityDocumentSeriesAccessRepository>();
        public DeletePurchaseOrderCommandHandler Delete { get; }
        public SendPurchaseOrderToApprovalCommandHandler Send { get; }

        public Fixture(string? status = null)
        {
            var context = Substitute.For<ICompanyContext>();
            context.CurrentCompany.Returns(new CompanyConnectionInfo(1, "TEST", "Test", DatabaseEngine.SqlServer,
                string.Empty, SapIntegrationMode.None));
            if (status is not null)
            {
                Repository.GetByIdAsync(7, Arg.Any<CancellationToken>()).Returns(new PurchaseOrderDto { Id = 7, DocumentSeriesId = 3, Status = status });
                Repository.TransitionWorkflowAsync(
                        Arg.Any<PurchaseOrderWorkflowTransitionData>(),
                        Arg.Any<CancellationToken>())
                    .Returns(PurchaseOrderWorkflowTransitionResult.Conflict());
            }
            AllowSeries(true);
            Delete = new(Repository, Access, context);
            Send = new(Repository, Access, context);
        }

        public void AllowSeries(bool allowed) => Access.ValidateUserOperationAsync(Arg.Any<int>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(allowed);
    }
}
