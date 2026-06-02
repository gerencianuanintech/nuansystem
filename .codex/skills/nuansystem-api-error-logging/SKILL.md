---
name: nuansystem-api-error-logging
description: Define and implement NuanSystem API error handling, Result<T>, ApiErrorResponse, ProblemDetails, validation errors, global exception middleware, Serilog structured logging, TraceId correlation, SQL/SAP error normalization, and production-safe frontend messages. Use when touching API responses, exception handling, logging, middleware, handlers, validation, or frontend error contracts.
---

# NuanSystem API Error Logging

## Response Standard

All API errors must use a stable response shape that WinForms can deserialize and present clearly:

```csharp
public sealed class ApiErrorResponse
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public string? Detail { get; init; }
    public string? TraceId { get; init; }
    public IReadOnlyList<ApiFieldError> Errors { get; init; } = [];
}

public sealed class ApiFieldError
{
    public required string Field { get; init; }
    public required string Message { get; init; }
}
```

Example JSON:

```json
{
  "code": "CUSTOMER_ALREADY_EXISTS",
  "message": "Ya existe un cliente con el mismo codigo.",
  "detail": null,
  "traceId": "00-abc123",
  "errors": []
}
```

## Rules

- Handlers must return `Result<T>` for business errors.
- Validation failures must return structured field errors.
- Unexpected exceptions must pass through global exception middleware/handler.
- Do not add repetitive `try/catch` blocks to every endpoint.
- Do not return `ex.Message` directly in production.
- Do not expose SQL errors, SAP stack traces, connection strings, tokens, or credentials to the frontend.
- Each relevant error must have a stable code for frontend handling and support diagnostics.
- Include `TraceId` in error responses and logs.
- Frontend messages must be clear in Spanish and not require technical interpretation by the user.

## Result Pattern

```csharp
public sealed record Error(string Code, string Message, string? Detail = null);

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string code, string message, string? detail = null) =>
        new(false, default, new Error(code, message, detail));

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
}
```

Business example:

```csharp
if (await _customers.ExistsByCodeAsync(command.Code, cancellationToken))
{
    return Result<CreateCustomerResponse>.Failure(
        "CUSTOMER_ALREADY_EXISTS",
        "Ya existe un cliente con el mismo codigo.");
}
```

## Global Exception Handler

Use one global handler that logs unexpected failures and returns a production-safe response:

```csharp
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        _logger.LogError(exception,
            "Unhandled API error. TraceId: {TraceId}, Path: {Path}",
            traceId,
            httpContext.Request.Path);

        var response = new ApiErrorResponse
        {
            Code = "UNEXPECTED_ERROR",
            Message = "Ocurrio un error inesperado. Intente nuevamente o contacte soporte.",
            Detail = _environment.IsDevelopment() ? exception.Message : null,
            TraceId = traceId
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
```

## Program.cs

```csharp
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "NuanSystem.Api");
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
```

## Serilog Configuration

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/nuansystem-api-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ],
    "Enrich": [ "FromLogContext" ]
  }
}
```

## Logging Rules

- Log critical business operations: login, company selection, permission changes, document posting/cancellation, stock movements, cash movements, SAP sync attempts.
- Log SAP integration with `CompanyCode`, local document id/type, status, `TraceId`, and SAP identifiers when available.
- Log SQL failures with procedure name and safe parameter context only; never include raw connection strings or secrets.
- Use structured properties instead of string concatenation:

```csharp
_logger.LogInformation(
    "Posting document {DocumentType} {DocumentId} for company {CompanyCode}",
    document.Type,
    document.Id,
    company.CompanyCode);
```

## Error Categories

- Validation: `VALIDATION_ERROR`, status 400, field-level errors.
- Business: stable domain code, status 400 or 409 depending on conflict.
- Authentication: `AUTHENTICATION_REQUIRED` or `INVALID_CREDENTIALS`, status 401.
- Authorization: `ACCESS_DENIED`, status 403.
- Not found: `{ENTITY}_NOT_FOUND`, status 404.
- SQL/infrastructure: `DATABASE_ERROR`, status 500, safe message.
- SAP: `SAP_SYNC_FAILED`, status 502/500 depending on operation contract, safe message plus logged technical detail.
