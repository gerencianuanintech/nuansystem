using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Common.Exceptions;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.SapIntegration.Items;

public sealed class SapServiceLayerItemReader(
    IHttpClientFactory httpClientFactory,
    ISapCompanySettingsRepository settingsRepository,
    ISecretProtector secretProtector) : ISapItemReader
{
    private const int MaxPages = 10000;
    private const int ItemCodeBatchSize = 25;
    private const string SelectFields =
        "ItemCode,ItemName,ItemsGroupCode,PurchaseItem,SalesItem,InventoryItem,Valid,Frozen," +
        "InventoryUOM,PurchaseUnit,SalesUnit,BarCode,PurchaseVATGroup,SalesVATGroup," +
        "ManageSerialNumbers,ManageBatchNumbers,ItemType";

    public async Task<IReadOnlyCollection<SapItemRecord>> GetItemsAsync(
        int companyId,
        SapItemReadOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var settings = await settingsRepository.GetByCompanyIdAsync(companyId, cancellationToken);
        ValidateSettings(settings);

        var baseUri = BuildBaseUri(settings!.ServiceLayerUrl!);
        var client = httpClientFactory.CreateClient("SapServiceLayer");
        var cookie = await LoginAsync(client, baseUri, settings, cancellationToken);

        try
        {
            if (options?.ItemCodes is not { Count: > 0 })
            {
                return await ReadAllPagesAsync(client, baseUri, cookie, options, cancellationToken);
            }

            var rows = new Dictionary<string, SapItemRecord>(StringComparer.OrdinalIgnoreCase);
            foreach (var batch in options.ItemCodes
                         .Where(code => !string.IsNullOrWhiteSpace(code))
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .Chunk(ItemCodeBatchSize))
            {
                var batchRows = await ReadAllPagesAsync(
                    client,
                    baseUri,
                    cookie,
                    options with { ItemCodes = batch },
                    cancellationToken);
                foreach (var row in batchRows)
                {
                    rows[row.ItemCode] = row;
                }
            }

            return rows.Values.OrderBy(row => row.ItemCode).ToArray();
        }
        finally
        {
            await TryLogoutAsync(client, baseUri, cookie, cancellationToken);
        }
    }

    private static async Task<IReadOnlyCollection<SapItemRecord>> ReadAllPagesAsync(
        HttpClient client,
        Uri baseUri,
        string cookie,
        SapItemReadOptions? options,
        CancellationToken cancellationToken)
    {
        var rows = new List<SapItemRecord>();
        Uri? nextUri = BuildItemsUri(baseUri, options);

        for (var page = 0; nextUri is not null && page < MaxPages; page++)
        {
            using var request = CreateAuthenticatedRequest(HttpMethod.Get, nextUri, cookie);
            request.Headers.TryAddWithoutValidation("Prefer", "odata.maxpagesize=1000");
            using var response = await SendAsync(client, request, "consultar los articulos", cancellationToken);
            using var payload = await ReadJsonAsync(response, "leer los articulos", cancellationToken);

            if (!payload.RootElement.TryGetProperty("value", out var value) || value.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException("SAP Service Layer devolvio un formato inesperado para los articulos.");
            }

            rows.AddRange(value.EnumerateArray().Select(Map));
            if (options?.MaxRecords is int maxRecords && rows.Count >= maxRecords)
            {
                return rows.Take(maxRecords).ToArray();
            }
            nextUri = ResolveNextPage(payload.RootElement, baseUri);
        }

        if (nextUri is not null)
        {
            throw new InvalidOperationException("SAP Service Layer excedio el limite de paginas permitido para articulos.");
        }

        return rows;
    }

    private static Uri BuildItemsUri(Uri baseUri, SapItemReadOptions? options)
    {
        var query = new List<string>
        {
            $"$select={SelectFields}",
            "$orderby=ItemCode"
        };

        if (options?.MaxRecords is int maxRecords)
        {
            query.Add($"$top={Math.Clamp(maxRecords, 1, 1000)}");
        }

        var filters = new List<string>();
        if (!string.IsNullOrWhiteSpace(options?.Search))
        {
            var search = EscapeODataString(options.Search);
            filters.Add($"(contains(ItemCode,'{search}') or contains(ItemName,'{search}'))");
        }

        if (options?.ItemCodes is { Count: > 0 })
        {
            filters.Add("(" + string.Join(" or ", options.ItemCodes
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(code => $"ItemCode eq '{EscapeODataString(code)}'")) + ")");
        }

        if (filters.Count > 0)
        {
            query.Add("$filter=" + Uri.EscapeDataString(string.Join(" and ", filters)));
        }

        return new Uri(baseUri, "Items?" + string.Join("&", query));
    }

    private static string EscapeODataString(string value)
        => value.Trim().Replace("'", "''", StringComparison.Ordinal);

    private async Task<string> LoginAsync(
        HttpClient client,
        Uri baseUri,
        SapCompanySettingsDto settings,
        CancellationToken cancellationToken)
    {
        string password;
        try
        {
            password = secretProtector.Unprotect(settings.SapPasswordEncrypted!);
        }
        catch
        {
            throw new InvalidOperationException("No fue posible abrir la credencial protegida de SAP para la empresa.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, "Login"))
        {
            Content = JsonContent.Create(new SapLoginRequest(settings.SapCompanyDb!, settings.SapUser!, password))
        };
        using var response = await SendAsync(client, request, "iniciar sesion", cancellationToken);

        var session = ReadCookie(response, "B1SESSION");
        if (string.IsNullOrWhiteSpace(session))
        {
            throw new InvalidOperationException("SAP Service Layer no devolvio una sesion valida.");
        }

        var route = ReadCookie(response, "ROUTEID");
        return string.IsNullOrWhiteSpace(route) ? $"B1SESSION={session}" : $"B1SESSION={session}; ROUTEID={route}";
    }

    private static async Task TryLogoutAsync(HttpClient client, Uri baseUri, string cookie, CancellationToken cancellationToken)
    {
        try
        {
            using var request = CreateAuthenticatedRequest(HttpMethod.Post, new Uri(baseUri, "Logout"), cookie);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            // Best effort: a logout error must not hide a completed import.
        }
    }

    private static HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method, Uri uri, string cookie)
    {
        var request = new HttpRequestMessage(method, uri);
        request.Headers.TryAddWithoutValidation("Cookie", cookie);
        return request;
    }

    private static async Task<HttpResponseMessage> SendAsync(
        HttpClient client,
        HttpRequestMessage request,
        string operation,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new SapServiceLayerException(operation, sapErrorMessage: "La solicitud supero el tiempo de espera.");
        }
        catch (HttpRequestException exception)
        {
            throw new SapServiceLayerException(operation, sapErrorMessage: "No fue posible conectar con el servidor SAP.", innerException: exception);
        }

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        var statusCode = (int)response.StatusCode;
        var sapError = await ReadSapErrorAsync(response, cancellationToken);
        response.Dispose();
        throw new SapServiceLayerException(operation, statusCode, sapError.Code, sapError.Message);
    }

    private static async Task<(string? Code, string? Message)> ReadSapErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var payload = await JsonDocument.ParseAsync(stream, new JsonDocumentOptions { MaxDepth = 16 }, cancellationToken);
            if (!payload.RootElement.TryGetProperty("error", out var error))
            {
                return default;
            }

            var code = ReadValue(error, "code");
            var message = error.TryGetProperty("message", out var messageNode)
                ? messageNode.ValueKind == JsonValueKind.String ? messageNode.GetString() : ReadValue(messageNode, "value")
                : null;
            return (Sanitize(code, 80), Sanitize(message, 300));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return default;
        }
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response,
        string operation,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        }
        catch (JsonException)
        {
            throw new InvalidOperationException($"SAP Service Layer devolvio datos invalidos al {operation}.");
        }
    }

    private static SapItemRecord Map(JsonElement item)
        => new(
            ReadString(item, "ItemCode"),
            ReadString(item, "ItemName"),
            ReadInt32(item, "ItemsGroupCode"),
            ReadOptionalString(item, "InventoryUOM"),
            ReadOptionalString(item, "PurchaseUnit"),
            ReadOptionalString(item, "SalesUnit"),
            ReadOptionalString(item, "BarCode"),
            ReadOptionalString(item, "PurchaseVATGroup"),
            ReadOptionalString(item, "SalesVATGroup"),
            ReadYesNo(item, "PurchaseItem"),
            ReadYesNo(item, "SalesItem"),
            ReadYesNo(item, "InventoryItem"),
            ReadYesNo(item, "ManageSerialNumbers"),
            ReadYesNo(item, "ManageBatchNumbers"),
            ReadOptionalString(item, "ItemType") ?? "itItems",
            !ReadNo(item, "Valid") && !ReadYesNo(item, "Frozen"));

    private static Uri? ResolveNextPage(JsonElement root, Uri baseUri)
    {
        var next = ReadOptionalString(root, "odata.nextLink") ?? ReadOptionalString(root, "@odata.nextLink");
        if (next is null)
        {
            return null;
        }

        var resolved = Uri.TryCreate(next, UriKind.Absolute, out var absolute) ? absolute : new Uri(baseUri, next);
        if (!string.Equals(resolved.Scheme, baseUri.Scheme, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(resolved.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase)
            || resolved.Port != baseUri.Port
            || !resolved.AbsolutePath.StartsWith(baseUri.AbsolutePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("SAP Service Layer devolvio una paginacion fuera del servidor configurado.");
        }

        return resolved;
    }

    private static string? ReadCookie(HttpResponseMessage response, string name)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var values))
        {
            return null;
        }

        foreach (var segment in values.SelectMany(value => value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)))
        {
            var separator = segment.IndexOf('=');
            if (separator > 0 && segment[..separator].Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                var value = segment[(separator + 1)..].Trim();
                return value.Contains('\r') || value.Contains('\n') ? null : value;
            }
        }

        return null;
    }

    private static string ReadString(JsonElement element, string name) => ReadOptionalString(element, name) ?? string.Empty;
    private static string? ReadOptionalString(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(value.GetString())
            ? value.GetString()!.Trim()
            : null;
    private static int? ReadInt32(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.TryGetInt32(out var number) ? number : null;
    private static bool ReadYesNo(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && (value.ValueKind == JsonValueKind.True
            || value.ValueKind == JsonValueKind.String && value.GetString()?.Trim().ToUpperInvariant() is "Y" or "YES" or "TYES" or "TRUE" or "1");
    private static bool ReadNo(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            && value.GetString()?.Trim().ToUpperInvariant() is "N" or "NO" or "TNO" or "FALSE" or "0";
    private static string? ReadValue(JsonElement element, string name)
        => element.TryGetProperty(name, out var value)
            ? value.ValueKind == JsonValueKind.String ? value.GetString() : value.ValueKind == JsonValueKind.Number ? value.GetRawText() : null
            : null;
    private static string? Sanitize(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var sanitized = string.Join(' ', value.Split(['\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        return sanitized.Length <= maxLength ? sanitized : sanitized[..maxLength];
    }

    private static Uri BuildBaseUri(string serviceLayerUrl)
    {
        if (!Uri.TryCreate(serviceLayerUrl.TrimEnd('/') + "/", UriKind.Absolute, out var uri)
            || uri.Scheme != Uri.UriSchemeHttps || !string.IsNullOrEmpty(uri.UserInfo))
        {
            throw new InvalidOperationException("La URL de SAP Service Layer debe ser una direccion HTTPS valida y sin credenciales.");
        }
        return uri;
    }

    private static void ValidateSettings(SapCompanySettingsDto? settings)
    {
        if (settings is null || !settings.IsEnabled || settings.IntegrationMode == SapIntegrationMode.None)
            throw new InvalidOperationException("La empresa no tiene integracion SAP activa.");
        if (settings.IntegrationMode != SapIntegrationMode.ServiceLayer)
            throw new InvalidOperationException("La importacion de articulos requiere SAP Service Layer.");
        if (string.IsNullOrWhiteSpace(settings.ServiceLayerUrl) || string.IsNullOrWhiteSpace(settings.SapCompanyDb)
            || string.IsNullOrWhiteSpace(settings.SapUser) || string.IsNullOrWhiteSpace(settings.SapPasswordEncrypted))
            throw new InvalidOperationException("La configuracion de SAP Service Layer para la empresa esta incompleta.");
    }

    private sealed record SapLoginRequest(
        [property: JsonPropertyName("CompanyDB")] string CompanyDb,
        [property: JsonPropertyName("UserName")] string UserName,
        [property: JsonPropertyName("Password")] string Password);
}
