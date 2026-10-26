using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace NuanSystem.Application.Tests.Infrastructure;

// Executes the real HTTP result and serializer, without a listener or application startup.
internal sealed class InMemoryResultHandler(Func<CancellationToken, Task<IResult>> resultFactory) : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var services = new ServiceCollection().AddLogging().ConfigureHttpJsonOptions(_ => { }).BuildServiceProvider();
        await using var body = new MemoryStream();
        var context = new DefaultHttpContext { RequestServices = services, RequestAborted = cancellationToken };
        context.Response.Body = body;
        await (await resultFactory(cancellationToken)).ExecuteAsync(context);
        return new HttpResponseMessage((System.Net.HttpStatusCode)context.Response.StatusCode)
        {
            RequestMessage = request,
            Content = new StringContent(Encoding.UTF8.GetString(body.ToArray()), Encoding.UTF8, "application/json")
        };
    }
}
