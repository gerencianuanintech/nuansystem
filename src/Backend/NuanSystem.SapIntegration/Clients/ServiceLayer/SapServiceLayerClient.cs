using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using NuanSystem.SapIntegration.Abstractions;
using NuanSystem.SapIntegration.Documents;

namespace NuanSystem.SapIntegration.Clients.ServiceLayer;

public sealed class SapServiceLayerClient(IHttpClientFactory httpClientFactory, IConfiguration configuration) : ISapClient
{
    public async Task<SapClientResult> SendDocumentAsync(
        SapDocumentPayload document,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = configuration["Sap:ServiceLayer:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return Failure(document, "Sap:ServiceLayer:BaseUrl no esta configurado.");
        }

        var endpoint = ResolveEndpoint(document.DocumentType);
        if (endpoint is null)
        {
            return Failure(document, $"Tipo de documento SAP no soportado: {document.DocumentType}.");
        }

        var request = BuildServiceLayerPayload(document);
        var requestJson = JsonSerializer.Serialize(request);

        try
        {
            var client = httpClientFactory.CreateClient("SapServiceLayer");
            client.BaseAddress ??= new Uri(baseUrl);

            var response = await client.PostAsJsonAsync(endpoint, request, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new SapClientResult(false, "Failed", responseJson, requestJson, responseJson, null, null);
            }

            var docEntry = TryReadInt(responseJson, "DocEntry");
            var docNum = TryReadInt(responseJson, "DocNum");

            return new SapClientResult(true, "Synced", null, requestJson, responseJson, docEntry, docNum);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return new SapClientResult(false, "Failed", exception.Message, requestJson, null, null, null);
        }
    }

    private static string? ResolveEndpoint(string documentType)
    {
        return documentType switch
        {
            "SalesOrder" => "Orders",
            "Delivery" => "DeliveryNotes",
            "Invoice" => "Invoices",
            _ => null
        };
    }

    private static object BuildServiceLayerPayload(SapDocumentPayload document)
    {
        return new
        {
            CardCode = document.CustomerCode,
            DocDate = document.DocumentDate.ToString("yyyy-MM-dd"),
            DocCurrency = document.Currency,
            Comments = $"NuanSystem {document.DocumentType} {document.DocumentNumber}",
            DocumentLines = document.Lines.Select(line => new
            {
                ItemCode = line.ItemCode,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice
            })
        };
    }

    private static SapClientResult Failure(SapDocumentPayload document, string message)
    {
        var requestJson = JsonSerializer.Serialize(document);
        return new SapClientResult(false, "Failed", message, requestJson, null, null, null);
    }

    private static int? TryReadInt(string json, string propertyName)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.TryGetProperty(propertyName, out var property)
                && property.TryGetInt32(out var value)
                    ? value
                    : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
