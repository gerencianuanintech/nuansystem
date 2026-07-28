namespace NuanSystem.WinForms.Services.Http;

public interface INuanApiClient
{
    Task<TResponse> GetAsync<TResponse>(string path, CancellationToken cancellationToken = default);
    Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken = default);
    Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken = default);
    Task<TResponse> DeleteAsync<TResponse>(string path, CancellationToken cancellationToken = default);
    Task<TResponse> PostFileAsync<TResponse>(
        string path,
        Stream content,
        string fileName,
        string formFieldName = "file",
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default);
    Task<ApiFileResponse> GetFileAsync(string path, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(string path = "/health", CancellationToken cancellationToken = default);
}

public sealed record ApiFileResponse(byte[] Content, string ContentType, string FileName);
