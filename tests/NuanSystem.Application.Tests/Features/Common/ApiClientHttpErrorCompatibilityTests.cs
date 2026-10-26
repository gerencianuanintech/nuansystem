using Microsoft.AspNetCore.Http;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Tests.Infrastructure;
using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.Shared.Responses;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.Application.Tests.Features.Common;

public sealed class ApiClientHttpErrorCompatibilityTests
{
    [Theory]
    [InlineData(FailureKind.Validation, 400)]
    [InlineData(FailureKind.NotFound, 404)]
    [InlineData(FailureKind.Conflict, 409)]
    [InlineData(FailureKind.Forbidden, 403)]
    [InlineData(FailureKind.Unavailable, 503)]
    public async Task WinForms_preserves_status_errors_and_session_for_each_business_failure(FailureKind kind, int status)
    {
        var session = new ApiSession();
        var company = new UserCompanyResponse(1, "PHASE3", "Test", null, null, null);
        var user = new LoginResponse(1, "tester", "Test", "phase3-test-token", DateTime.UtcNow.AddMinutes(1), false, [], [], [company]);
        session.SetUser(user);
        session.SelectCompany(company);
        using var http = new HttpClient(new InMemoryResultHandler(_ => Task.FromResult(Result<string>.Failure(
            "Mensaje seguro.", kind, [new ApiError("FEATURE_CODE", "Detalle del campo.", "Name")]).ToHttpResult())))
        { BaseAddress = new Uri("https://phase3.invalid") };
        var client = new NuanApiClient(http, session);

        var exception = await Assert.ThrowsAsync<ApiClientException>(() => client.GetAsync<string>("/api/test"));

        Assert.Equal(status, exception.StatusCode);
        Assert.StartsWith("Mensaje seguro.", exception.Message);
        var error = Assert.Single(exception.Errors);
        Assert.Equal("FEATURE_CODE", error.Code);
        Assert.Equal("Name", error.Field);
        Assert.Equal("Detalle del campo.", error.Message);
        Assert.Contains("Detalle del campo.", ApiClientErrorMessageFormatter.Format(exception));
        Assert.Same(user, session.CurrentUser);
        Assert.Same(company, session.CurrentCompany);
    }

    [Fact]
    public async Task Unexpected_error_reaches_WinForms_as_safe_500_instead_of_raw_exception_details()
    {
        using var http = new HttpClient(new InMemoryResultHandler(_ => Task.FromResult(Result<string>.Failure(
            "Password=private-secret", FailureKind.Unexpected,
            [new ApiError("SQL", "private-secret", "private-table")]).ToHttpResult())))
        { BaseAddress = new Uri("https://phase3.invalid") };
        var client = new NuanApiClient(http, new ApiSession());

        var exception = await Assert.ThrowsAsync<ApiClientException>(() => client.GetAsync<string>("/api/test"));

        Assert.Equal(500, exception.StatusCode);
        Assert.Equal("UNEXPECTED_ERROR", Assert.Single(exception.Errors).Code);
        Assert.DoesNotContain("private-secret", ApiClientErrorMessageFormatter.Format(exception));
        Assert.DoesNotContain("private-table", ApiClientErrorMessageFormatter.Format(exception));
    }

    [Fact]
    public async Task Legacy_400_and_successful_200_keep_the_existing_WinForms_behavior()
    {
        using var successHttp = new HttpClient(new InMemoryResultHandler(_ => Task.FromResult(Result<string>.Success("value").ToHttpResult())))
        { BaseAddress = new Uri("https://phase3.invalid") };
        Assert.Equal("value", await new NuanApiClient(successHttp, new ApiSession()).GetAsync<string>("/api/test"));
        using var failureHttp = new HttpClient(new InMemoryResultHandler(_ => Task.FromResult(Result<string>.Failure("Legacy.").ToHttpResult())))
        { BaseAddress = new Uri("https://phase3.invalid") };
        var exception = await Assert.ThrowsAsync<ApiClientException>(() =>
            new NuanApiClient(failureHttp, new ApiSession()).GetAsync<string>("/api/test"));
        Assert.Equal(400, exception.StatusCode);
        Assert.Equal("Legacy.", exception.Message);
        Assert.Empty(exception.Errors);
    }
}
