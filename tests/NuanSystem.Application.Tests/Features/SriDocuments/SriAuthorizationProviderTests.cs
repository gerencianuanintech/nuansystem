using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Sri;
using NuanSystem.SriWorker.Options;
using NuanSystem.SriWorker.Services;

namespace NuanSystem.Application.Tests.Features.SriDocuments;

public sealed class SriAuthorizationProviderTests
{
    [Fact]
    public async Task QueryAsync_StoresOnlyAuthorizedXmlWhoseIdentityMatchesRequest()
    {
        var key = SriAccessKeyTests.BuildKey("01", '2');
        var innerXml = $"<factura><infoTributaria><ruc>{key.Substring(10, 13)}</ruc><claveAcceso>{key}</claveAcceso></infoTributaria></factura>";
        var handler = new StubHttpHandler(HttpStatusCode.OK, AuthorizedResponse(innerXml));
        var provider = CreateProvider(handler);

        var result = await provider.QueryAsync("Production", key);

        result.Outcome.Should().Be(SriAuthorizationOutcome.Authorized);
        result.AuthorizationNumber.Should().Be("AUTH-001");
        result.DocumentTypeCode.Should().Be("01");
        result.IssuerRuc.Should().Be(key.Substring(10, 13));
        result.XmlContent.Should().Equal(Encoding.UTF8.GetBytes(innerXml));
        result.Sha256.Should().HaveCount(32);
        handler.LastRequestUri.Should().Be("https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline");
        handler.LastBody.Should().Contain(key).And.Contain("autorizacionComprobante");
    }

    [Fact]
    public async Task QueryAsync_RejectsAuthorizedXmlWithDifferentAccessKey()
    {
        var requested = SriAccessKeyTests.BuildKey("01", '2');
        var different = SriAccessKeyTests.BuildKey("04", '2');
        var innerXml = $"<factura><infoTributaria><ruc>{requested.Substring(10, 13)}</ruc><claveAcceso>{different}</claveAcceso></infoTributaria></factura>";
        var provider = CreateProvider(new StubHttpHandler(HttpStatusCode.OK, AuthorizedResponse(innerXml)));

        var result = await provider.QueryAsync("Production", requested);

        result.Outcome.Should().Be(SriAuthorizationOutcome.PermanentFailure);
        result.ErrorCategory.Should().Be("Integrity");
        result.ErrorCode.Should().Be("SRI_XML_INTEGRITY");
        result.XmlContent.Should().BeNull();
    }

    [Fact]
    public async Task QueryAsync_ClassifiesEmptyAuthorizationListAsNotFound()
    {
        var key = SriAccessKeyTests.BuildKey("07", '1');
        var provider = CreateProvider(new StubHttpHandler(HttpStatusCode.OK,
            "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><respuestaAutorizacionComprobante><autorizaciones /></respuestaAutorizacionComprobante></soap:Body></soap:Envelope>"));

        var result = await provider.QueryAsync("Test", key);

        result.Outcome.Should().Be(SriAuthorizationOutcome.NotFound);
    }

    [Fact]
    public async Task QueryAsync_RejectsOversizedResponseBeforeParsing()
    {
        var options = new SriProviderOptions { MaxXmlBytes = 128 };
        var handler = new StubHttpHandler(HttpStatusCode.OK, new string('x', 129));
        var provider = new SriAuthorizationProvider(new HttpClient(handler), Options.Create(options));

        var result = await provider.QueryAsync("Test", SriAccessKeyTests.BuildKey("01", '1'));

        result.Outcome.Should().Be(SriAuthorizationOutcome.PermanentFailure);
        result.ErrorCode.Should().Be("SRI_RESPONSE_SIZE");
    }

    [Theory]
    [InlineData("https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline", "celcer.sri.gob.ec", true)]
    [InlineData("https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline", "cel.sri.gob.ec", true)]
    [InlineData("http://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline", "cel.sri.gob.ec", false)]
    [InlineData("https://example.org/comprobantes-electronicos-ws/AutorizacionComprobantesOffline", "cel.sri.gob.ec", false)]
    [InlineData("https://cel.sri.gob.ec/otro", "cel.sri.gob.ec", false)]
    public void EndpointValidation_AllowsOnlyExactOfficialHttpsEndpoint(string value, string host, bool expected)
    {
        SriProviderOptions.IsOfficialEndpoint(value, host).Should().Be(expected);
    }

    private static SriAuthorizationProvider CreateProvider(HttpMessageHandler handler) =>
        new(new HttpClient(handler), Options.Create(new SriProviderOptions()));

    private static string AuthorizedResponse(string innerXml) =>
        $"<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><respuestaAutorizacionComprobante><autorizaciones><autorizacion><estado>AUTORIZADO</estado><numeroAutorizacion>AUTH-001</numeroAutorizacion><fechaAutorizacion>2026-07-20T10:00:00-05:00</fechaAutorizacion><ambiente>PRODUCCIÓN</ambiente><comprobante><![CDATA[{innerXml}]]></comprobante></autorizacion></autorizaciones></respuestaAutorizacionComprobante></soap:Body></soap:Envelope>";

    private sealed class StubHttpHandler(HttpStatusCode statusCode, string body) : HttpMessageHandler
    {
        public string? LastRequestUri { get; private set; }
        public string? LastBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri?.ToString();
            LastBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(statusCode) { Content = new StringContent(body, Encoding.UTF8, "text/xml") };
        }
    }
}
