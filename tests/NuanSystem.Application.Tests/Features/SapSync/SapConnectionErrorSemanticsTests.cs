using System.Text.Json;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Queries;
using NuanSystem.Application.Tests.Infrastructure;
using NuanSystem.Domain.Tenancy;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Sap;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapConnectionErrorSemanticsTests
{
    [Fact]
    public async Task Failed_connection_is_safe_Unavailable_instead_of_a_successful_envelope()
    {
        var tester = Substitute.For<ISapServiceLayerConnectionTester>();
        tester.TestAsync(1, Arg.Any<CancellationToken>()).Returns(new SapConnectionTestResult(false, "Password=private-secret"));
        var handler = new TestSapServiceLayerConnectionQueryHandler(Context(), tester);

        var result = await handler.Handle(new TestSapServiceLayerConnectionQuery(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(FailureKind.Unavailable, result.FailureKind);
        Assert.Null(result.Value);
        Assert.Equal("SAP_CONNECTION_UNAVAILABLE", Assert.Single(result.Errors).Code);
        Assert.DoesNotContain("private-secret", JsonSerializer.Serialize(result));
    }

    [Fact]
    public async Task Typed_SAP_client_and_ViewModel_handle_503_and_restore_busy_state()
    {
        var tester = Substitute.For<ISapServiceLayerConnectionTester>();
        tester.TestAsync(1, Arg.Any<CancellationToken>()).Returns(new SapConnectionTestResult(false, "remote details"));
        var handler = new TestSapServiceLayerConnectionQueryHandler(Context(), tester);
        using var http = new HttpClient(new InMemoryResultHandler(async token =>
            (await handler.Handle(new TestSapServiceLayerConnectionQuery(), token)).ToHttpResult()))
        { BaseAddress = new Uri("https://phase3.invalid") };
        var viewModel = new SapIntegrationSettingsViewModel(new SapIntegrationSettingsClient(new NuanApiClient(http, new ApiSession())));

        var error = await Assert.ThrowsAsync<ApiClientException>(() => viewModel.TestConnectionAsync());

        Assert.Equal(503, error.StatusCode);
        Assert.Equal("SAP_CONNECTION_UNAVAILABLE", Assert.Single(error.Errors).Code);
        Assert.False(viewModel.IsBusy);
        Assert.DoesNotContain("remote details", error.Message);
    }

    [Fact]
    public async Task Successful_connection_keeps_200_and_its_existing_data()
    {
        var tester = Substitute.For<ISapServiceLayerConnectionTester>();
        tester.TestAsync(1, Arg.Any<CancellationToken>()).Returns(new SapConnectionTestResult(true, "Conectado."));
        var result = await new TestSapServiceLayerConnectionQueryHandler(Context(), tester)
            .Handle(new TestSapServiceLayerConnectionQuery(), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsSuccessful);
        Assert.Equal(200, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result.ToHttpResult()).StatusCode);
    }

    [Fact]
    public async Task Missing_company_is_Validation_and_never_invokes_the_connection_tester()
    {
        var tester = Substitute.For<ISapServiceLayerConnectionTester>();
        var result = await new TestSapServiceLayerConnectionQueryHandler(Substitute.For<ICompanyContext>(), tester)
            .Handle(new TestSapServiceLayerConnectionQuery(), CancellationToken.None);
        Assert.Equal(FailureKind.Validation, result.FailureKind);
        Assert.Equal(400, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result.ToHttpResult()).StatusCode);
        Assert.Empty(tester.ReceivedCalls());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Exceptions_and_cancellation_are_not_disguised_as_dependency_unavailability(bool cancellation)
    {
        var tester = Substitute.For<ISapServiceLayerConnectionTester>();
        Exception failure = cancellation ? new OperationCanceledException() : new InvalidOperationException("technical failure");
        tester.TestAsync(1, Arg.Any<CancellationToken>()).Returns(Task.FromException<SapConnectionTestResult>(failure));
        var action = () => new TestSapServiceLayerConnectionQueryHandler(Context(), tester)
            .Handle(new TestSapServiceLayerConnectionQuery(), CancellationToken.None);

        var actual = await Record.ExceptionAsync(action);
        Assert.Same(failure, actual);
    }

    private static ICompanyContext Context()
    {
        var context = Substitute.For<ICompanyContext>();
        context.HasActiveCompany.Returns(true);
        context.CurrentCompany.Returns(new CompanyConnectionInfo(1, "TEST", "Test", DatabaseEngine.SqlServer,
            string.Empty, SapIntegrationMode.None));
        return context;
    }
}
