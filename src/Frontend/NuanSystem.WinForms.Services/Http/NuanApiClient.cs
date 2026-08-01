using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using NuanSystem.Shared.Responses;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.WinForms.Services.Http;

public sealed class NuanApiClient : INuanApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient httpClient;
    private readonly ApiSession session;

    public NuanApiClient(HttpClient httpClient, ApiSession session)
    {
        this.httpClient = httpClient;
        this.session = session;
    }

    public async Task<TResponse> GetAsync<TResponse>(string path, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, path);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken = default)
    {
        using var message = CreateRequest(HttpMethod.Post, path);
        message.Content = JsonContent.Create(request, options: JsonOptions);
        return await SendAsync<TResponse>(message, cancellationToken);
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken = default)
    {
        using var message = CreateRequest(HttpMethod.Put, path);
        message.Content = JsonContent.Create(request, options: JsonOptions);
        return await SendAsync<TResponse>(message, cancellationToken);
    }

    public async Task<TResponse> DeleteAsync<TResponse>(string path, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Delete, path);
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public async Task<TResponse> DeleteAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = CreateRequest(HttpMethod.Delete, path);
        message.Content = JsonContent.Create(request, options: JsonOptions);
        return await SendAsync<TResponse>(message, cancellationToken);
    }

    public async Task<TResponse> PostFileAsync<TResponse>(
        string path,
        Stream content,
        string fileName,
        string formFieldName = "file",
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(formFieldName);

        using var request = CreateRequest(HttpMethod.Post, path);
        using var multipart = new MultipartFormDataContent();
        using var fileContent = new StreamContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        multipart.Add(fileContent, formFieldName, Path.GetFileName(fileName));
        request.Content = multipart;
        return await SendAsync<TResponse>(request, cancellationToken);
    }

    public async Task<bool> IsAvailableAsync(string path = "/health", CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, path);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<ApiFileResponse> GetFileAsync(string path, CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Get, path);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _ = await ReadResponseAsync<object>(response, cancellationToken);
        }

        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
            ?? "download.bin";
        return new ApiFileResponse(content, contentType, Path.GetFileName(fileName));
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, path);

        if (!string.IsNullOrWhiteSpace(session.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        }

        if (!string.IsNullOrWhiteSpace(session.CompanyCode))
        {
            request.Headers.TryAddWithoutValidation("X-Company-Code", session.CompanyCode);
        }

        return request;
    }

    private async Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ReadResponseAsync<TResponse>(response, cancellationToken);
    }

    private static async Task<TResponse> ReadResponseAsync<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
        {
            if (response.IsSuccessStatusCode)
            {
                return default!;
            }

            throw new ApiClientException(
                AppendRequestPath(ResolveEmptyErrorMessage(response), response),
                (int)response.StatusCode);
        }

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<TResponse>>(content, JsonOptions);
        if (apiResponse is null)
        {
            throw new ApiClientException("No fue posible interpretar la respuesta de la API.", (int)response.StatusCode);
        }

        if (!response.IsSuccessStatusCode || !apiResponse.Success)
        {
            throw new ApiClientException(AppendRequestPath(apiResponse.Message, response), (int)response.StatusCode);
        }

        return apiResponse.Data!;
    }

    private static string AppendRequestPath(string message, HttpResponseMessage response)
    {
        if (response.StatusCode is not (System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden))
        {
            return message;
        }

        var path = response.RequestMessage?.RequestUri?.PathAndQuery;
        return string.IsNullOrWhiteSpace(path)
            ? message
            : $"{message} Ruta: {path}";
    }

    private static string ResolveEmptyErrorMessage(HttpResponseMessage response)
    {
        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Tu sesion no es valida o expiro. Inicia sesion nuevamente.",
            System.Net.HttpStatusCode.Forbidden => "No tienes permiso para realizar esta accion. Solicita al administrador que revise tus accesos.",
            System.Net.HttpStatusCode.TooManyRequests => "Se realizaron demasiados intentos. Espera un momento e intenta nuevamente.",
            _ => $"La API respondio {(int)response.StatusCode} {response.ReasonPhrase} sin detalle adicional."
        };
    }
}
