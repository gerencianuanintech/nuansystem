using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.SapIntegration.Countries;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapServiceLayerCountryReaderTests
{
    [Fact]
    public async Task GetCountriesAsync_ShouldReadAllPagesUsingFullQueryAndMapIsoCodes()
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
        var handler = new SapCountryHttpHandler();
        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("SapServiceLayer").Returns(new HttpClient(handler));
        var queryClient = new SapServiceLayerQueryClient(
            clientFactory,
            settingsRepository,
            protector);
        var reader = new SapServiceLayerCountryReader(queryClient);
        using var cancellation = new CancellationTokenSource();

        var rows = await reader.GetCountriesAsync(1, cancellation.Token);

        rows.Should().BeEquivalentTo([
            new SapCountryRecord("EC", "Ecuador", "EC", "ECU"),
            new SapCountryRecord("PE", "Peru", "PE", "PER")
        ], options => options.WithStrictOrdering());
        handler.RequestPaths.Should().ContainInOrder(
            "/b1s/v1/Login",
            "/b1s/v1/Countries?$orderby=Code",
            "/b1s/v1/Countries?$skip=1",
            "/b1s/v1/Logout");
        handler.RequestPaths.Should().NotContain(path => path.Contains("$filter", StringComparison.OrdinalIgnoreCase));
        handler.RequestPaths.Should().NotContain(path => path.Contains("plain-password", StringComparison.Ordinal));
        await settingsRepository.Received(1).GetByCompanyIdAsync(1, cancellation.Token);
    }

    [Fact]
    public void FullQuery_ShouldBeOrderedAndNeverContainAFilter()
    {
        SapCountryQuery.Full.Should().Be("Countries?$orderby=Code");
        SapCountryQuery.Full.Should().NotContain("$filter");
        SapCountryQuery.ReadOptions.MaxPages.Should().Be(100);
    }

    [Fact]
    public void Map_ShouldTrimValuesAndTreatBlankOptionalIsoCodesAsNull()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "Code": " EC ",
              "Name": " Ecuador ",
              "ISOAlpha2Code": "  ",
              "ISOAlpha3Code": null
            }
            """);

        var result = SapCountryMapper.Map(document.RootElement);

        result.Should().Be(new SapCountryRecord("EC", "Ecuador", null, null));
    }

    [Fact]
    public void Map_ShouldAcceptCompatibleIsoPropertyCasingUntilMetadataIsValidated()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "Code": "PE",
              "Name": "Peru",
              "IsoAlpha2Code": "PE",
              "IsoAlpha3Code": "PER"
            }
            """);

        var result = SapCountryMapper.Map(document.RootElement);

        result.Should().Be(new SapCountryRecord("PE", "Peru", "PE", "PER"));
    }

    private sealed class SapCountryHttpHandler : HttpMessageHandler
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
                        { "Code": "EC", "Name": "Ecuador", "ISOAlpha2Code": "EC", "ISOAlpha3Code": "ECU" }
                      ],
                      "odata.nextLink": "Countries?$skip=1"
                    }
                    """));
            }

            if (path.Contains("$skip=1", StringComparison.Ordinal))
            {
                return Task.FromResult(Json(HttpStatusCode.OK,
                    """
                    {
                      "value": [
                        { "Code": "PE", "Name": "Peru", "ISOAlpha2Code": "PE", "ISOAlpha3Code": "PER" }
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
