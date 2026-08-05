using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.SapIntegration.Provinces;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapServiceLayerProvinceReaderTests
{
    [Fact]
    public async Task GetProvincesAsync_ShouldReadAllPagesUsingFullOrderedQuery()
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
        var handler = new SapProvinceHttpHandler();
        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("SapServiceLayer").Returns(new HttpClient(handler));
        var queryClient = new SapServiceLayerQueryClient(clientFactory, settingsRepository, protector);
        var reader = new SapServiceLayerProvinceReader(queryClient);
        using var cancellation = new CancellationTokenSource();

        var rows = await reader.GetProvincesAsync(1, cancellation.Token);

        rows.Should().BeEquivalentTo([
            new SapProvinceRecord("EC", "A", "Azuay"),
            new SapProvinceRecord("EC", "P", "Pichincha")
        ], options => options.WithStrictOrdering());
        handler.RequestPaths.Should().ContainInOrder(
            "/b1s/v1/Login",
            "/b1s/v1/States?$orderby=Country,Code",
            "/b1s/v1/States?$skip=1",
            "/b1s/v1/Logout");
        handler.RequestPaths.Should().NotContain(path => path.Contains("$filter", StringComparison.OrdinalIgnoreCase));
        handler.RequestPaths.Should().NotContain(path => path.Contains("plain-password", StringComparison.Ordinal));
        await settingsRepository.Received(1).GetByCompanyIdAsync(1, cancellation.Token);
    }

    [Fact]
    public void FullQuery_ShouldOrderByCountryAndCodeWithoutFilter()
    {
        SapProvinceQuery.Full.Should().Be("States?$orderby=Country,Code");
        SapProvinceQuery.Full.Should().NotContain("$filter");
        SapProvinceQuery.ReadOptions.MaxPages.Should().Be(100);
    }

    [Fact]
    public void Map_ShouldTrimOfficialStateFields()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "Country": " EC ",
              "Code": " A ",
              "Name": " Azuay "
            }
            """);

        var result = SapProvinceMapper.Map(document.RootElement);

        result.Should().Be(new SapProvinceRecord("EC", "A", "Azuay"));
    }

    [Fact]
    public void Map_ShouldAcceptCompatiblePropertyCasing()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "country": "EC",
              "code": "P",
              "name": "Pichincha"
            }
            """);

        var result = SapProvinceMapper.Map(document.RootElement);

        result.Should().Be(new SapProvinceRecord("EC", "P", "Pichincha"));
    }

    private sealed class SapProvinceHttpHandler : HttpMessageHandler
    {
        public List<string> RequestPaths { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var path = request.RequestUri!.PathAndQuery;
            RequestPaths.Add(path);

            if (path.EndsWith("/Login", StringComparison.Ordinal))
            {
                var response = Json(HttpStatusCode.OK, "{}");
                response.Headers.TryAddWithoutValidation("Set-Cookie", "B1SESSION=test-session; Path=/; HttpOnly");
                return Task.FromResult(response);
            }

            if (path.Contains("$orderby", StringComparison.Ordinal))
            {
                return Task.FromResult(Json(HttpStatusCode.OK,
                    """
                    {
                      "value": [
                        { "Country": "EC", "Code": "A", "Name": "Azuay" }
                      ],
                      "odata.nextLink": "States?$skip=1"
                    }
                    """));
            }

            if (path.Contains("$skip=1", StringComparison.Ordinal))
            {
                return Task.FromResult(Json(HttpStatusCode.OK,
                    """
                    {
                      "value": [
                        { "Country": "EC", "Code": "P", "Name": "Pichincha" }
                      ]
                    }
                    """));
            }

            if (path.EndsWith("/Logout", StringComparison.Ordinal))
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        private static HttpResponseMessage Json(HttpStatusCode statusCode, string json) => new(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}
