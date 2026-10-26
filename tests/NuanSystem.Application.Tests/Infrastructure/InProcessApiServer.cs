using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace NuanSystem.Application.Tests.Infrastructure;

// Drives the actual ASP.NET application without a socket, TestHost package or production Program.
internal sealed class InProcessApiServer : IServer
{
    private Func<IFeatureCollection, Task>? process;
    public IFeatureCollection Features { get; } = new FeatureCollection();

    public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken) where TContext : notnull
    {
        process = async features =>
        {
            var context = application.CreateContext(features);
            Exception? error = null;
            try { await application.ProcessRequestAsync(context); }
            catch (Exception exception) { error = exception; throw; }
            finally { application.DisposeContext(context, error); }
        };
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) { process = null; return Task.CompletedTask; }
    public void Dispose() => process = null;
    public HttpMessageHandler CreateHandler() => new Transport(this);

    private sealed class Transport(InProcessApiServer server) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var dispatch = server.process ?? throw new InvalidOperationException("The in-process API is not started.");
            var headers = new HeaderDictionary();
            foreach (var header in request.Headers) headers[header.Key] = new StringValues(header.Value.ToArray());
            if (request.Content is not null)
                foreach (var header in request.Content.Headers) headers[header.Key] = new StringValues(header.Value.ToArray());
            var bytes = request.Content is null ? [] : await request.Content.ReadAsByteArrayAsync(cancellationToken);
            await using var body = new MemoryStream(bytes);
            await using var responseBody = new MemoryStream();
            var features = new FeatureCollection();
            features.Set<IHttpRequestFeature>(new HttpRequestFeature
            {
                Method = request.Method.Method, Scheme = request.RequestUri!.Scheme,
                Path = request.RequestUri.AbsolutePath, QueryString = request.RequestUri.Query,
                RawTarget = request.RequestUri.PathAndQuery, Protocol = "HTTP/1.1", Headers = headers, Body = body
            });
            var responseFeature = new HttpResponseFeature();
            features.Set<IHttpResponseFeature>(responseFeature);
            features.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(responseBody));
            features.Set<IHttpRequestLifetimeFeature>(new Lifetime(cancellationToken));
            features.Set<IHttpRequestBodyDetectionFeature>(new BodyDetection(bytes.Length > 0));
            await dispatch(features);
            cancellationToken.ThrowIfCancellationRequested();
            var response = new HttpResponseMessage((System.Net.HttpStatusCode)responseFeature.StatusCode)
            { RequestMessage = request, Content = new ByteArrayContent(responseBody.ToArray()) };
            foreach (var header in responseFeature.Headers)
                if (!response.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                    response.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            return response;
        }
    }

    private sealed class BodyDetection(bool hasBody) : IHttpRequestBodyDetectionFeature
    {
        public bool CanHaveBody => hasBody;
    }

    private sealed class Lifetime(CancellationToken token) : IHttpRequestLifetimeFeature
    {
        public CancellationToken RequestAborted { get; set; } = token;
        public void Abort() { }
    }
}
