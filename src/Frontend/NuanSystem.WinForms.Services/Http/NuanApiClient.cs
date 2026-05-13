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
                $"La API respondio {(int)response.StatusCode} {response.ReasonPhrase} sin contenido.",
                (int)response.StatusCode);
        }

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<TResponse>>(content, JsonOptions);
        if (apiResponse is null)
        {
            throw new ApiClientException("No fue posible interpretar la respuesta de la API.", (int)response.StatusCode);
        }

        if (!response.IsSuccessStatusCode || !apiResponse.Success)
        {
            throw new ApiClientException(apiResponse.Message, (int)response.StatusCode);
        }

        return apiResponse.Data!;
    }
}
