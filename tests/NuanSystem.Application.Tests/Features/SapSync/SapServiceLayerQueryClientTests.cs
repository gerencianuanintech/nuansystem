using System.Net;
using System.Text;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapServiceLayerQueryClientTests
{
    [Fact]
    public async Task ReadAllAsync_ShouldFailInsteadOfTruncatingWhenPageLimitIsExceeded()
    {
        var handler = new StubServiceLayerHandler(request =>
        {
            var path = request.RequestUri!.PathAndQuery;
            if (path.EndsWith("/Login", StringComparison.Ordinal))
            {
                return LoginResponse();
            }

            if (path.EndsWith("/Logout", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            return Json(
                HttpStatusCode.OK,
                """
                {
                  "value": [{ "WarehouseCode": "01" }],
                  "odata.nextLink": "Warehouses?$skip=1"
                }
                """);
        });
        var client = CreateClient(handler);

        var action = () => client.ReadAllAsync(
            1,
            "Warehouses?$orderby=WarehouseCode",
            new SapServiceLayerReadOptions(1, "consultar las bodegas", "las bodegas"),
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*excedio el limite de paginas*");
        handler.RequestPaths.Should().EndWith("/b1s/v1/Logout");
    }

    [Fact]
    public async Task ReadAllAsync_ShouldRejectPaginationOutsideConfiguredServiceLayerRoot()
    {
        var handler = new StubServiceLayerHandler(request =>
        {
            var path = request.RequestUri!.PathAndQuery;
            if (path.EndsWith("/Login", StringComparison.Ordinal))
            {
                return LoginResponse();
            }

            if (path.EndsWith("/Logout", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            return Json(
                HttpStatusCode.OK,
                """
                {
                  "value": [{ "WarehouseCode": "01" }],
                  "odata.nextLink": "https://outside.local/b1s/v1/Warehouses?$skip=1"
                }
                """);
        });
        var client = CreateClient(handler);

        var action = () => client.ReadAllAsync(
            1,
            "Warehouses?$orderby=WarehouseCode",
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*paginacion fuera del servidor configurado*");
        handler.RequestPaths.Should().EndWith("/b1s/v1/Logout");
    }

    [Fact]
    public async Task ReadAllAsync_ShouldRejectAbsoluteInitialQuery()
    {
        var handler = new StubServiceLayerHandler(_ =>
            throw new InvalidOperationException("No debe invocar SAP."));
        var client = CreateClient(handler);

        var action = () => client.ReadAllAsync(
            1,
            "https://outside.local/b1s/v1/Warehouses",
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*consulta SAP debe ser relativa*");
        handler.RequestPaths.Should().BeEmpty();
    }

    private static SapServiceLayerQueryClient CreateClient(HttpMessageHandler handler)
    {
        var settingsRepository = Substitute.For<ISapCompanySettingsRepository>();
        settingsRepository.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new SapCompanySettingsDto
            {
                CompanyId = 1,
                CompanyCode = "DEMO",
                IsEnabled = true,
                IntegrationMode = SapIntegrationMode.ServiceLayer,
                ServiceLayerUrl = "https://sap.local/b1s/v1/",
                SapCompanyDb = "COMPANY_DB",
                SapUser = "integration-user",
                SapPasswordEncrypted = "protected-password"
            });

        var protector = Substitute.For<ISecretProtector>();
        protector.Unprotect("protected-password").Returns("plain-password");

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("SapServiceLayer").Returns(new HttpClient(handler));

        return new SapServiceLayerQueryClient(
            clientFactory,
            settingsRepository,
            protector);
    }

    private static HttpResponseMessage LoginResponse()
    {
        var response = Json(HttpStatusCode.OK, "{}");
        response.Headers.TryAddWithoutValidation(
            "Set-Cookie",
            "B1SESSION=test-session; Path=/; HttpOnly");
        return response;
    }

    private static HttpResponseMessage Json(HttpStatusCode statusCode, string json)
        => new(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    private sealed class StubServiceLayerHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        public List<string> RequestPaths { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestPaths.Add(request.RequestUri!.PathAndQuery);
            return Task.FromResult(responseFactory(request));
        }
    }
}
