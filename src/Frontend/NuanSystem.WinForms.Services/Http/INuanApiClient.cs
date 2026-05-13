namespace NuanSystem.WinForms.Services.Http;

public interface INuanApiClient
{
    Task<TResponse> GetAsync<TResponse>(string path, CancellationToken cancellationToken = default);
    Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken = default);
    Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest request, CancellationToken cancellationToken = default);
    Task<TResponse> DeleteAsync<TResponse>(string path, CancellationToken cancellationToken = default);
}
