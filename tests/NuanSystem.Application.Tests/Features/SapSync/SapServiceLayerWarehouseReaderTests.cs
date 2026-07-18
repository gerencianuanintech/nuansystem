using System.Net;
using System.Text;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.SapIntegration.Warehouses;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapServiceLayerWarehouseReaderTests
{
    [Fact]
    public async Task GetWarehousesAsync_ShouldReadAllPagesAndMapActiveStateWithoutExposingCredentials()
    {
        var settingsRepository = Substitute.For<ISapCompanySettingsRepository>();
        settingsRepository.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>()).Returns(new SapCompanySettingsDto
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
        var handler = new SapWarehouseHttpHandler();
        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("SapServiceLayer").Returns(new HttpClient(handler));
        var reader = new SapServiceLayerWarehouseReader(clientFactory, settingsRepository, protector);

        var rows = await reader.GetWarehousesAsync(1);

        rows.Should().HaveCount(2);
        rows.Should().ContainEquivalentOf(new SapWarehouseRecord(
            "11", "MEGA TOTORACOCHA", null, "CUENCA", "AZUAY", "EC", true));
        rows.Should().ContainEquivalentOf(new SapWarehouseRecord(
            "20", "MEGA REMIGIO", "CALLE 1", null, null, null, false));
        handler.RequestPaths.Should().ContainInOrder(
            "/b1s/v1/Login",
            "/b1s/v1/Warehouses?$orderby=WarehouseCode",
            "/b1s/v1/Warehouses?$skip=1",
            "/b1s/v1/Logout");
        handler.LoginBody.Should().Contain("plain-password");
        handler.RequestPaths.Should().NotContain(path => path.Contains("plain-password", StringComparison.Ordinal));
    }

    private sealed class SapWarehouseHttpHandler : HttpMessageHandler
    {
        public List<string> RequestPaths { get; } = [];
        public string LoginBody { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri!.PathAndQuery;
            RequestPaths.Add(path);

            if (path.EndsWith("/Login", StringComparison.Ordinal))
            {
                LoginBody = await request.Content!.ReadAsStringAsync(cancellationToken);
                var response = Json(HttpStatusCode.OK, "{}");
                response.Headers.TryAddWithoutValidation("Set-Cookie", "B1SESSION=test-session; Path=/; HttpOnly");
                return response;
            }

            if (path.Contains("$orderby", StringComparison.Ordinal))
            {
                return Json(HttpStatusCode.OK,
                    """
                    {
                      "value": [
                        { "WarehouseCode": "11", "WarehouseName": "MEGA TOTORACOCHA", "City": "CUENCA", "State": "AZUAY", "Country": "EC", "Inactive": "tNO", "Locked": "tNO" }
                      ],
                      "odata.nextLink": "Warehouses?$skip=1"
                    }
                    """);
            }

            if (path.Contains("$skip=1", StringComparison.Ordinal))
            {
                return Json(HttpStatusCode.OK,
                    """
                    {
                      "value": [
                        { "WarehouseCode": "20", "WarehouseName": "MEGA REMIGIO", "Street": "CALLE 1", "Inactive": "tNO", "Locked": "tYES" }
                      ]
                    }
                    """);
            }

            if (path.EndsWith("/Logout", StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        private static HttpResponseMessage Json(HttpStatusCode statusCode, string json) => new(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}
