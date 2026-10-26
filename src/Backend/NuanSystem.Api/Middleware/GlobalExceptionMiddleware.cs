namespace NuanSystem.Api.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IExceptionResponseMapper responseMapper,
        IExceptionAuditWriter auditWriter)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var response = responseMapper.Map(exception, context.TraceIdentifier);
            if (response.ShouldAudit)
            {
                await auditWriter.WriteAsync(context, exception, response.StatusCode);
            }

            context.Response.StatusCode = response.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response.Body);
        }
    }
}
