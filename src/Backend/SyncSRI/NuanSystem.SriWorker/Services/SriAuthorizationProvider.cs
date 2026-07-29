using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Sri;
using NuanSystem.SriWorker.Options;

namespace NuanSystem.SriWorker.Services;

public sealed class SriAuthorizationProvider(HttpClient httpClient, IOptions<SriProviderOptions> options) : ISriAuthorizationProvider
{
    private static readonly XNamespace Soap = "http://schemas.xmlsoap.org/soap/envelope/";
    private static readonly XNamespace Authorization = "http://ec.gob.sri.ws.autorizacion";

    public async Task<SriAuthorizationResult> QueryAsync(string environment, string accessKey, CancellationToken cancellationToken = default)
    {
        var current = options.Value;
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, current.GetEndpoint(environment));
            request.Headers.TryAddWithoutValidation("SOAPAction", "\"\"");
            request.Content = new StringContent(BuildEnvelope(accessKey), Encoding.UTF8, "text/xml");
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var correlation = ReadCorrelation(response);
            if (!response.IsSuccessStatusCode)
            {
                var transient = response.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests || (int)response.StatusCode >= 500;
                return Failure(transient, "Http", $"SRI_HTTP_{(int)response.StatusCode}", "El servicio SRI devolvio un estado HTTP no exitoso.", correlation);
            }

            var bytes = await ReadBoundedAsync(response.Content, current.MaxXmlBytes, cancellationToken);
            return ParseResponse(bytes, environment, accessKey, current.MaxXmlBytes, correlation);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Failure(true, "Timeout", "SRI_TIMEOUT", "La consulta al SRI excedio el tiempo permitido.", null);
        }
        catch (HttpRequestException)
        {
            return Failure(true, "Transport", "SRI_TRANSPORT", "No fue posible establecer comunicacion segura con el SRI.", null);
        }
        catch (InvalidDataException exception)
        {
            return Failure(false, "Contract", "SRI_RESPONSE_SIZE", exception.Message, null);
        }
        catch (XmlException)
        {
            return Failure(false, "Contract", "SRI_XML_INVALID", "El SRI devolvio una respuesta XML invalida.", null);
        }
    }

    internal static SriAuthorizationResult ParseResponse(byte[] responseBytes, string environment, string accessKey, int maxXmlBytes, string? correlation)
    {
        var response = LoadSecure(responseBytes, maxXmlBytes);
        var authorizations = response.Descendants().Where(element => element.Name.LocalName == "autorizacion").ToArray();
        if (authorizations.Length == 0) return new SriAuthorizationResult(SriAuthorizationOutcome.NotFound, RemoteCorrelationId: correlation);

        var authorized = authorizations.FirstOrDefault(element => Value(element, "estado").Equals("AUTORIZADO", StringComparison.OrdinalIgnoreCase));
        if (authorized is null)
        {
            return Failure(false, "Business", "SRI_NOT_AUTHORIZED", "El SRI reporto el comprobante como no autorizado.", correlation);
        }

        var authorizationNumber = Value(authorized, "numeroAutorizacion");
        var authorizationDate = Value(authorized, "fechaAutorizacion");
        var providerEnvironment = Value(authorized, "ambiente");
        var innerXml = Value(authorized, "comprobante");
        if (string.IsNullOrWhiteSpace(authorizationNumber) || !DateTimeOffset.TryParse(authorizationDate, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var authorizedAt) || string.IsNullOrWhiteSpace(innerXml))
            return Failure(false, "Contract", "SRI_AUTHORIZATION_INCOMPLETE", "La autorizacion SRI no contiene todos los datos obligatorios.", correlation);

        var xmlBytes = Encoding.UTF8.GetBytes(innerXml);
        if (xmlBytes.Length > maxXmlBytes) return Failure(false, "Contract", "SRI_XML_TOO_LARGE", "El XML autorizado supera el limite configurado.", correlation);
        var document = LoadSecure(xmlBytes, maxXmlBytes);
        var root = document.Root ?? throw new XmlException("Documento sin raiz.");
        var documentType = root.Name.LocalName switch { "factura" => "01", "notaCredito" => "04", "comprobanteRetencion" => "07", _ => string.Empty };
        var info = root.Descendants().FirstOrDefault(element => element.Name.LocalName == "infoTributaria");
        var issuerRuc = info is null ? string.Empty : Value(info, "ruc");
        var innerAccessKey = info is null ? string.Empty : Value(info, "claveAcceso");
        if (documentType.Length == 0 || innerAccessKey != accessKey || issuerRuc.Length != 13 || accessKey.Substring(10, 13) != issuerRuc || !EnvironmentMatches(environment, providerEnvironment))
            return Failure(false, "Integrity", "SRI_XML_INTEGRITY", "El XML autorizado no coincide con la clave, el emisor o el ambiente solicitado.", correlation);

        return new SriAuthorizationResult(SriAuthorizationOutcome.Authorized, authorizationNumber, authorizedAt,
            providerEnvironment, issuerRuc, documentType, xmlBytes, SHA256.HashData(xmlBytes), RemoteCorrelationId: correlation);
    }

    private static string BuildEnvelope(string accessKey) => new XDocument(
        new XElement(Soap + "Envelope", new XAttribute(XNamespace.Xmlns + "soapenv", Soap), new XAttribute(XNamespace.Xmlns + "ec", Authorization),
            new XElement(Soap + "Header"), new XElement(Soap + "Body",
                new XElement(Authorization + "autorizacionComprobante", new XElement("claveAccesoComprobante", accessKey))))).ToString(SaveOptions.DisableFormatting);

    private static XDocument LoadSecure(byte[] bytes, int maxBytes)
    {
        using var stream = new MemoryStream(bytes, writable: false);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = maxBytes * 2L });
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    private static async Task<byte[]> ReadBoundedAsync(HttpContent content, int maxBytes, CancellationToken cancellationToken)
    {
        if (content.Headers.ContentLength > maxBytes) throw new InvalidDataException("La respuesta SRI supera el limite configurado.");
        await using var stream = await content.ReadAsStreamAsync(cancellationToken);
        using var memory = new MemoryStream();
        var buffer = new byte[81920];
        int read;
        while ((read = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            if (memory.Length + read > maxBytes) throw new InvalidDataException("La respuesta SRI supera el limite configurado.");
            await memory.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
        return memory.ToArray();
    }

    private static string Value(XElement parent, string localName) => parent.Descendants().FirstOrDefault(element => element.Name.LocalName == localName)?.Value.Trim() ?? string.Empty;
    private static bool EnvironmentMatches(string requested, string provider) => requested.Equals("Test", StringComparison.OrdinalIgnoreCase)
        ? provider.Contains("PRUEBA", StringComparison.OrdinalIgnoreCase)
        : provider.Normalize(NormalizationForm.FormD).Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark).Aggregate(new StringBuilder(), (builder, ch) => builder.Append(ch)).ToString().Contains("PRODUCCION", StringComparison.OrdinalIgnoreCase);
    private static string? ReadCorrelation(HttpResponseMessage response) => response.Headers.TryGetValues("X-Correlation-ID", out var values) ? values.FirstOrDefault() : null;
    private static SriAuthorizationResult Failure(bool transient, string category, string code, string message, string? correlation) =>
        new(transient ? SriAuthorizationOutcome.TransientFailure : SriAuthorizationOutcome.PermanentFailure,
            ErrorCategory: category, ErrorCode: code, ErrorMessage: message, RemoteCorrelationId: correlation);
}
