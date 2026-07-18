using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Common.Exceptions;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.SapIntegration.Warehouses;

public sealed class SapServiceLayerWarehouseReader(
    IHttpClientFactory httpClientFactory,
    ISapCompanySettingsRepository settingsRepository,
    ISecretProtector secretProtector) : ISapWarehouseReader
{
    private const int MaxPages = 100;

    public async Task<IReadOnlyCollection<SapWarehouseRecord>> GetWarehousesAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var settings = await settingsRepository.GetByCompanyIdAsync(companyId, cancellationToken);
        ValidateSettings(settings);

        var baseUri = BuildBaseUri(settings!.ServiceLayerUrl!);
        var client = httpClientFactory.CreateClient("SapServiceLayer");
        var cookie = await LoginAsync(client, baseUri, settings, cancellationToken);

        try
        {
            return await ReadAllPagesAsync(client, baseUri, cookie, cancellationToken);
        }
        finally
        {
            await TryLogoutAsync(client, baseUri, cookie, cancellationToken);
        }
    }

    private async Task<IReadOnlyCollection<SapWarehouseRecord>> ReadAllPagesAsync(
        HttpClient client,
        Uri baseUri,
        string cookie,
        CancellationToken cancellationToken)
    {
        var rows = new List<SapWarehouseRecord>();
        Uri? nextUri = new Uri(baseUri, "Warehouses?$orderby=WarehouseCode");

        for (var page = 0; nextUri is not null && page < MaxPages; page++)
        {
            using var request = CreateAuthenticatedRequest(HttpMethod.Get, nextUri, cookie);
            using var response = await SendAsync(client, request, "consultar las bodegas", cancellationToken);
            using var payload = await ReadJsonAsync(response, "leer las bodegas", cancellationToken);

            if (!payload.RootElement.TryGetProperty("value", out var value)
                || value.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException("SAP Service Layer devolvio un formato inesperado para las bodegas.");
            }

            foreach (var item in value.EnumerateArray())
            {
                rows.Add(Map(item));
            }

            nextUri = ResolveNextPage(payload.RootElement, baseUri);
        }

        if (nextUri is not null)
        {
            throw new InvalidOperationException("SAP Service Layer excedio el limite de paginas permitido para bodegas.");
        }

        return rows;
    }

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
            Content = JsonContent.Create(new SapLoginRequest(
                settings.SapCompanyDb!,
                settings.SapUser!,
                password))
        };
        using var response = await SendAsync(client, request, "iniciar sesion", cancellationToken);

        var session = ReadCookie(response, "B1SESSION");
        if (string.IsNullOrWhiteSpace(session))
        {
            throw new InvalidOperationException("SAP Service Layer no devolvio una sesion valida.");
        }

        var route = ReadCookie(response, "ROUTEID");
        return string.IsNullOrWhiteSpace(route)
            ? $"B1SESSION={session}"
            : $"B1SESSION={session}; ROUTEID={route}";
    }

    private static async Task TryLogoutAsync(
        HttpClient client,
        Uri baseUri,
        string cookie,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = CreateAuthenticatedRequest(HttpMethod.Post, new Uri(baseUri, "Logout"), cookie);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            // Closing a best-effort SAP session must not hide the import result.
        }
    }

    private static HttpRequestMessage CreateAuthenticatedRequest(
        HttpMethod method,
        Uri requestUri,
        string cookie)
    {
        var request = new HttpRequestMessage(method, requestUri);
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
            using var payload = await JsonDocument.ParseAsync(
                stream,
                new JsonDocumentOptions { MaxDepth = 16 },
                cancellationToken);

            if (!payload.RootElement.TryGetProperty("error", out var error)
                || error.ValueKind != JsonValueKind.Object)
            {
                return default;
            }

            var code = ReadErrorValue(error, "code");
            var message = ReadErrorMessage(error);
            return (SanitizeSapError(code, 80), SanitizeSapError(message, 300));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return default;
        }
    }

    private static string? ReadErrorMessage(JsonElement error)
    {
        if (!error.TryGetProperty("message", out var message))
        {
            return null;
        }

        if (message.ValueKind == JsonValueKind.String)
        {
            return message.GetString();
        }

        return message.ValueKind == JsonValueKind.Object
            ? ReadErrorValue(message, "value")
            : null;
    }

    private static string? ReadErrorValue(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString(),
            JsonValueKind.Number => property.GetRawText(),
            _ => null
        };
    }

    private static string? SanitizeSapError(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var sanitized = string.Join(' ', value
            .Split(['\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        return sanitized.Length <= maxLength ? sanitized : sanitized[..maxLength];
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

    private static SapWarehouseRecord Map(JsonElement item)
    {
        var inactive = ReadBooleanFlag(item, "Inactive") || ReadBooleanFlag(item, "Locked");
        return new SapWarehouseRecord(
            ReadString(item, "WarehouseCode"),
            ReadString(item, "WarehouseName"),
            ReadOptionalString(item, "Street"),
            ReadOptionalString(item, "City"),
            ReadFirstOptionalString(item, "State", "County", "StateCode"),
            ReadOptionalString(item, "Country"),
            !inactive);
    }

    private static Uri? ResolveNextPage(JsonElement root, Uri baseUri)
    {
        var next = ReadOptionalString(root, "odata.nextLink")
            ?? ReadOptionalString(root, "@odata.nextLink");
        if (string.IsNullOrWhiteSpace(next))
        {
            return null;
        }

        var resolved = Uri.TryCreate(next, UriKind.Absolute, out var absolute)
            ? absolute
            : new Uri(baseUri, next);

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

        foreach (var header in values)
        {
            foreach (var segment in header.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var separator = segment.IndexOf('=');
                if (separator <= 0 || !segment[..separator].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var value = segment[(separator + 1)..].Trim();
                return value.Contains('\r') || value.Contains('\n') ? null : value;
            }
        }

        return null;
    }

    private static string ReadString(JsonElement element, string name)
        => ReadOptionalString(element, name) ?? string.Empty;

    private static string? ReadFirstOptionalString(JsonElement element, params string[] names)
        => names.Select(name => ReadOptionalString(element, name)).FirstOrDefault(value => value is not null);

    private static string? ReadOptionalString(JsonElement element, string name)
        => element.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.String
            ? string.IsNullOrWhiteSpace(property.GetString()) ? null : property.GetString()!.Trim()
            : null;

    private static bool ReadBooleanFlag(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property))
        {
            return false;
        }

        if (property.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            return property.GetBoolean();
        }

        var value = property.ValueKind == JsonValueKind.String ? property.GetString() : null;
        return value is not null
            && value.Trim().ToUpperInvariant() is ("Y" or "YES" or "TYES" or "TRUE" or "1");
    }

    private static Uri BuildBaseUri(string serviceLayerUrl)
    {
        if (!Uri.TryCreate(serviceLayerUrl.TrimEnd('/') + "/", UriKind.Absolute, out var uri)
            || !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            || !string.IsNullOrEmpty(uri.UserInfo))
        {
            throw new InvalidOperationException("La URL de SAP Service Layer debe ser una direccion HTTPS valida y sin credenciales.");
        }

        return uri;
    }

    private static void ValidateSettings(SapCompanySettingsDto? settings)
    {
        if (settings is null || !settings.IsEnabled || settings.IntegrationMode == SapIntegrationMode.None)
        {
            throw new InvalidOperationException("La empresa no tiene integracion SAP activa.");
        }

        if (settings.IntegrationMode != SapIntegrationMode.ServiceLayer)
        {
            throw new InvalidOperationException("La importacion de bodegas requiere SAP Service Layer.");
        }

        if (string.IsNullOrWhiteSpace(settings.ServiceLayerUrl)
            || string.IsNullOrWhiteSpace(settings.SapCompanyDb)
            || string.IsNullOrWhiteSpace(settings.SapUser)
            || string.IsNullOrWhiteSpace(settings.SapPasswordEncrypted))
        {
            throw new InvalidOperationException("La configuracion de SAP Service Layer para la empresa esta incompleta.");
        }
    }

    private sealed record SapLoginRequest(
        [property: JsonPropertyName("CompanyDB")] string CompanyDb,
        [property: JsonPropertyName("UserName")] string UserName,
        [property: JsonPropertyName("Password")] string Password);
}
