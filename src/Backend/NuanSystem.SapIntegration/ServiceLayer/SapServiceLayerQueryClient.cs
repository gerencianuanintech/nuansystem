using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Common.Exceptions;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.SapIntegration.ServiceLayer;

public sealed class SapServiceLayerQueryClient(
    IHttpClientFactory httpClientFactory,
    ISapCompanySettingsRepository settingsRepository,
    ISecretProtector secretProtector)
{
    public Task<IReadOnlyCollection<JsonElement>> ReadAllAsync(
        int companyId,
        string relativeQuery,
        CancellationToken cancellationToken)
        => ReadAllAsync(
            companyId,
            relativeQuery,
            SapServiceLayerReadOptions.Default,
            cancellationToken);

    internal async Task<IReadOnlyCollection<JsonElement>> ReadAllAsync(
        int companyId,
        string relativeQuery,
        SapServiceLayerReadOptions options,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeQuery);
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        var settings = await settingsRepository.GetByCompanyIdAsync(companyId, cancellationToken);
        ValidateSettings(settings);

        var baseUri = BuildBaseUri(settings!.ServiceLayerUrl!);
        var queryUri = ResolveQueryUri(baseUri, relativeQuery);
        var client = httpClientFactory.CreateClient("SapServiceLayer");
        var cookie = await LoginAsync(client, baseUri, settings, cancellationToken);

        try
        {
            return await ReadAllPagesAsync(
                client,
                baseUri,
                queryUri,
                cookie,
                options,
                cancellationToken);
        }
        finally
        {
            await TryLogoutAsync(client, baseUri, cookie, cancellationToken);
        }
    }

    private static async Task<IReadOnlyCollection<JsonElement>> ReadAllPagesAsync(
        HttpClient client,
        Uri baseUri,
        Uri queryUri,
        string cookie,
        SapServiceLayerReadOptions options,
        CancellationToken cancellationToken)
    {
        var result = new List<JsonElement>();
        Uri? nextUri = queryUri;

        for (var page = 0; nextUri is not null && page < options.MaxPages; page++)
        {
            using var request = CreateAuthenticatedRequest(HttpMethod.Get, nextUri, cookie);
            using var response = await SendAsync(
                client,
                request,
                options.Operation,
                cancellationToken);
            using var payload = await ReadJsonAsync(
                response,
                options.EntityDisplayName,
                cancellationToken);

            if (!payload.RootElement.TryGetProperty("value", out var value)
                || value.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException(
                    $"SAP Service Layer devolvio un formato inesperado para {options.EntityDisplayName}.");
            }

            result.AddRange(value.EnumerateArray().Select(item => item.Clone()));
            nextUri = ResolveNextPage(payload.RootElement, baseUri);
        }

        if (nextUri is not null)
        {
            throw new InvalidOperationException(
                $"SAP Service Layer excedio el limite de paginas permitido para {options.EntityDisplayName}.");
        }

        return result;
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
            throw new InvalidOperationException(
                "No fue posible abrir la credencial protegida de SAP para la empresa.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, "Login"))
        {
            Content = JsonContent.Create(new LoginRequest(
                settings.SapCompanyDb!,
                settings.SapUser!,
                password))
        };
        using var response = await SendAsync(
            client,
            request,
            "iniciar sesion",
            cancellationToken);

        var session = ReadCookie(response, "B1SESSION");
        if (string.IsNullOrWhiteSpace(session))
        {
            throw new InvalidOperationException(
                "SAP Service Layer no devolvio una sesion valida.");
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
            using var request = CreateAuthenticatedRequest(
                HttpMethod.Post,
                new Uri(baseUri, "Logout"),
                cookie);
            using var response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            // Best effort: a logout error must not hide a completed query.
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
            response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new SapServiceLayerException(
                operation,
                sapErrorMessage: "La solicitud supero el tiempo de espera.");
        }
        catch (HttpRequestException exception)
        {
            throw new SapServiceLayerException(
                operation,
                sapErrorMessage: "No fue posible conectar con el servidor SAP.",
                innerException: exception);
        }

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        var statusCode = (int)response.StatusCode;
        var sapError = await ReadSapErrorAsync(response, cancellationToken);
        response.Dispose();
        throw new SapServiceLayerException(
            operation,
            statusCode,
            sapError.Code,
            sapError.Message ?? "SAP rechazo la operacion.");
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

            return (
                SanitizeSapError(ReadErrorValue(error, "code"), 80),
                SanitizeSapError(ReadErrorMessage(error), 300));
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

    private static string? ReadErrorMessage(JsonElement error)
    {
        if (!error.TryGetProperty("message", out var message))
        {
            return null;
        }

        return message.ValueKind switch
        {
            JsonValueKind.String => message.GetString(),
            JsonValueKind.Object => ReadErrorValue(message, "value"),
            _ => null
        };
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

        var sanitized = string.Join(' ', value.Split(
            ['\r', '\n', '\t'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        return sanitized.Length <= maxLength ? sanitized : sanitized[..maxLength];
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response,
        string entityDisplayName,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);
        }
        catch (JsonException)
        {
            throw new InvalidOperationException(
                $"SAP Service Layer devolvio datos invalidos al leer {entityDisplayName}.");
        }
    }

    private static Uri ResolveQueryUri(Uri baseUri, string relativeQuery)
    {
        if (Uri.TryCreate(relativeQuery, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException(
                "La consulta SAP debe ser relativa al Service Layer configurado.");
        }

        var queryUri = new Uri(baseUri, relativeQuery);
        ValidateServiceLayerUri(queryUri, baseUri);
        return queryUri;
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
        ValidateServiceLayerUri(resolved, baseUri);
        return resolved;
    }

    private static void ValidateServiceLayerUri(Uri uri, Uri baseUri)
    {
        if (!string.Equals(uri.Scheme, baseUri.Scheme, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(uri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase)
            || uri.Port != baseUri.Port
            || !uri.AbsolutePath.StartsWith(baseUri.AbsolutePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "SAP Service Layer devolvio una paginacion fuera del servidor configurado.");
        }
    }

    private static string? ReadCookie(HttpResponseMessage response, string name)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var values))
        {
            return null;
        }

        foreach (var segment in values.SelectMany(value => value.Split(
                     ';',
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)))
        {
            var separator = segment.IndexOf('=');
            if (separator <= 0
                || !segment[..separator].Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var cookie = segment[(separator + 1)..].Trim();
            return cookie.Contains('\r') || cookie.Contains('\n') ? null : cookie;
        }

        return null;
    }

    private static string? ReadOptionalString(JsonElement element, string name)
        => element.TryGetProperty(name, out var property)
           && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static Uri BuildBaseUri(string serviceLayerUrl)
    {
        if (!Uri.TryCreate(serviceLayerUrl.TrimEnd('/') + "/", UriKind.Absolute, out var uri)
            || !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            || !string.IsNullOrEmpty(uri.UserInfo))
        {
            throw new InvalidOperationException(
                "La URL de SAP Service Layer debe ser una direccion HTTPS valida y sin credenciales.");
        }

        return uri;
    }

    private static void ValidateSettings(SapCompanySettingsDto? settings)
    {
        if (settings is null
            || !settings.IsEnabled
            || settings.IntegrationMode != SapIntegrationMode.ServiceLayer
            || string.IsNullOrWhiteSpace(settings.ServiceLayerUrl)
            || string.IsNullOrWhiteSpace(settings.SapCompanyDb)
            || string.IsNullOrWhiteSpace(settings.SapUser)
            || string.IsNullOrWhiteSpace(settings.SapPasswordEncrypted))
        {
            throw new InvalidOperationException(
                "La empresa no tiene configuracion completa de SAP Service Layer.");
        }
    }

    private sealed record LoginRequest(
        [property: JsonPropertyName("CompanyDB")] string CompanyDb,
        [property: JsonPropertyName("UserName")] string User,
        [property: JsonPropertyName("Password")] string Password);
}
