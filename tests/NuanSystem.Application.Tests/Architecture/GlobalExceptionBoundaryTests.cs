using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NuanSystem.Api.Middleware;
using NuanSystem.Application.Abstractions.Data;
using FluentAssertions;
using NuanSystem.Application.Common.Exceptions;
using NuanSystem.Application.Features.Audit.Dtos;
using NuanSystem.Infrastructure.Diagnostics;
using NuanSystem.Persistence.Diagnostics;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class GlobalExceptionBoundaryTests
{
    [Theory]
    [InlineData("Password=SuperSecret!;Server=db.internal", "SuperSecret!")]
    [InlineData("Authorization: Bearer eyJhbGciOiJIUzI1NiJ9.secret.signature", "eyJhbGci")]
    [InlineData("Cookie: session=private-cookie", "private-cookie")]
    [InlineData("access_token=token-value", "token-value")]
    public void Sanitizer_removes_sensitive_values(string diagnostic, string forbidden)
    {
        var sanitizer = new ErrorSanitizer();

        var sanitized = sanitizer.Sanitize(diagnostic);

        sanitized.Should().NotContain(forbidden);
        sanitized.Should().Contain("[REDACTED]");
    }

    [Fact]
    public void Sanitizer_bounds_diagnostics()
    {
        var sanitizer = new ErrorSanitizer();

        sanitizer.Sanitize(new string('x', 10_000))!.Length.Should().BeLessThanOrEqualTo(4_000);
    }

    [Fact]
    public void Dependency_failure_exposes_only_stable_safe_contract()
    {
        var failure = new DependencyFailureException(
            DependencyKind.Database,
            "DATABASE_UNAVAILABLE",
            "La base de datos no esta disponible.",
            isRetryable: true,
            diagnosticContext: "SQL error 53");

        failure.Dependency.Should().Be(DependencyKind.Database);
        failure.SafeErrorCode.Should().Be("DATABASE_UNAVAILABLE");
        failure.Message.Should().Be("La base de datos no esta disponible.");
        failure.IsRetryable.Should().BeTrue();
        failure.DiagnosticContext.Should().Be("SQL error 53");
    }

    [Fact]
    public void Api_middleware_does_not_reference_provider_exceptions_or_raw_messages()
    {
        var source = string.Join(
            Environment.NewLine,
            ReadSource("src", "Backend", "NuanSystem.Api", "Middleware", "GlobalExceptionMiddleware.cs"),
            ReadSource("src", "Backend", "NuanSystem.Api", "Middleware", "ExceptionResponseMapper.cs"),
            ReadSource("src", "Backend", "NuanSystem.Api", "Middleware", "ExceptionAuditWriter.cs"));

        source.Should().NotContain("Microsoft.Data.SqlClient");
        source.Should().NotContain("SapServiceLayerException");
        source.Should().NotContain("exception.Message");
        source.Should().Contain("IErrorSanitizer");
        source.Should().Contain("TraceIdentifier");
    }

    [Fact]
    public async Task Middleware_redacts_dependency_secrets_from_response_and_audit()
    {
        const string secret = "TopSecret-987";
        CreateAuditErrorLogData? capturedAudit = null;
        var audit = Substitute.For<IAuditLogRepository>();
        audit.AddErrorAsync(Arg.Any<CreateAuditErrorLogData>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                capturedAudit = call.Arg<CreateAuditErrorLogData>();
                return Task.CompletedTask;
            });
        var environment = Substitute.For<IHostEnvironment>();
        environment.EnvironmentName.Returns(Environments.Development);
        var middleware = new GlobalExceptionMiddleware(
            _ => throw new DependencyFailureException(
                DependencyKind.Network,
                "DEPENDENCY_NETWORK_UNAVAILABLE",
                "No fue posible conectar con el servicio externo.",
                isRetryable: true,
                diagnosticContext: $"Authorization: Bearer {secret}",
                innerException: new InvalidOperationException($"Password={secret};Server=db.internal")));
        var sanitizer = new ErrorSanitizer();
        var responseMapper = new ExceptionResponseMapper(
            new DependencyFailureClassifier(),
            sanitizer,
            environment);
        var auditWriter = new ExceptionAuditWriter(
            audit,
            sanitizer,
            NullLogger<ExceptionAuditWriter>.Instance);
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-safe-123"
        };
        context.Request.Path = $"/failure/Password={secret}";
        context.Request.QueryString = new QueryString($"?access_token={secret}");
        context.Request.Headers.UserAgent = $"Cookie: session={secret}";
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(
            context,
            responseMapper,
            auditWriter);

        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().NotContain(secret);
        responseBody.Should().Contain("trace-safe-123");
        context.Response.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);
        capturedAudit.Should().NotBeNull();
        capturedAudit!.ErrorMessage.Should().NotContain(secret);
        capturedAudit.StackTrace.Should().NotContain(secret);
        capturedAudit.Path.Should().NotContain(secret);
        capturedAudit.QueryString.Should().NotContain(secret);
        capturedAudit.UserAgent.Should().NotContain(secret);
        capturedAudit.TraceId.Should().Be("trace-safe-123");
    }

    private static string ReadSource(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. pathParts]);
            if (File.Exists(path)) return File.ReadAllText(path);
            directory = directory.Parent;
        }

        throw new FileNotFoundException(Path.Combine(pathParts));
    }
}
