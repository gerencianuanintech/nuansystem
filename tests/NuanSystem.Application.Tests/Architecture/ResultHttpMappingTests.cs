using System.Text.Json;
using Microsoft.AspNetCore.Http;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class ResultHttpMappingTests
{
    [Theory]
    [InlineData(FailureKind.Validation, 400)]
    [InlineData(FailureKind.NotFound, 404)]
    [InlineData(FailureKind.Conflict, 409)]
    [InlineData(FailureKind.Forbidden, 403)]
    [InlineData(FailureKind.Unavailable, 503)]
    [InlineData(FailureKind.Unexpected, 500)]
    public void Explicit_failure_category_controls_HTTP_status(FailureKind kind, int status)
    {
        var result = Result<object>.Failure("Mensaje seguro.", kind, [new ApiError("TEST", "Detalle seguro.", "Id")]);

        var response = result.ToHttpResult();

        Assert.Equal(status, Assert.IsAssignableFrom<IStatusCodeHttpResult>(response).StatusCode);
        Assert.False(Envelope<object>(response).Success);
        Assert.Equal(kind, result.FailureKind);
    }

    [Fact]
    public void Legacy_failure_keeps_400_and_preserves_message_errors_and_four_value_deconstruction()
    {
        ApiError[] errors = [new("OLD_CODE", "Detalle.", "Name")];
        var result = Result<string>.Failure("Validacion anterior.", errors);
        var (success, message, value, existingErrors) = result;
        var response = result.ToHttpResult();

        Assert.False(success);
        Assert.Equal("Validacion anterior.", message);
        Assert.Null(value);
        Assert.Same(errors, existingErrors);
        Assert.Equal(FailureKind.Validation, result.FailureKind);
        Assert.Equal(400, Assert.IsAssignableFrom<IStatusCodeHttpResult>(response).StatusCode);
        Assert.Equal(message, Envelope<string>(response).Message);
        Assert.Same(errors, Envelope<string>(response).Errors);
    }

    [Fact]
    public void Existing_constructor_without_category_and_legacy_null_errors_still_map_to_400()
    {
        var result = new Result<string>(false, "Legacy.", null, []);
        Assert.Null(result.FailureKind);
        Assert.Equal(400, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result.ToHttpResult()).StatusCode);
        Assert.Equal(400, Assert.IsAssignableFrom<IStatusCodeHttpResult>(Result<string>.Failure("Legacy.", null).ToHttpResult()).StatusCode);
    }

    [Theory]
    [InlineData("No encontrado")]
    [InlineData("conflict timeout forbidden HTTP 503")]
    public void Legacy_status_is_never_inferred_from_message_text(string message)
    {
        Assert.Equal(400, Assert.IsAssignableFrom<IStatusCodeHttpResult>(Result<string>.Failure(message).ToHttpResult()).StatusCode);
    }

    [Theory]
    [InlineData(FailureKind.Unexpected)]
    [InlineData((FailureKind)999)]
    public void Technical_or_unknown_failure_never_leaks_original_message_errors_or_value(FailureKind kind)
    {
        const string secret = "Password=private-secret; SELECT * FROM private_table";
        var result = new Result<string>(false, secret, secret, [new ApiError(secret, secret, secret)]) { FailureKind = kind };

        var response = result.ToHttpResult();
        var envelope = Envelope<string>(response);
        var json = JsonSerializer.Serialize(envelope, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.Equal(500, Assert.IsAssignableFrom<IStatusCodeHttpResult>(response).StatusCode);
        Assert.DoesNotContain("private-secret", json);
        Assert.Null(envelope.Data);
        Assert.Equal("UNEXPECTED_ERROR", Assert.Single(envelope.Errors).Code);
        Assert.False(string.IsNullOrWhiteSpace(envelope.Message));
    }

    [Fact]
    public void Success_keeps_200_data_and_the_existing_envelope_fields()
    {
        var result = Result<int>.Success(17, "Guardado.");
        var response = result.ToHttpResult();
        var envelope = Envelope<int>(response);
        var json = JsonSerializer.SerializeToElement(envelope, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.Null(result.FailureKind);
        Assert.Equal(200, Assert.IsAssignableFrom<IStatusCodeHttpResult>(response).StatusCode);
        Assert.True(envelope.Success);
        Assert.Equal(17, envelope.Data);
        Assert.Empty(envelope.Errors);
        Assert.Equal(["success", "message", "data", "errors"], json.EnumerateObject().Select(item => item.Name).ToArray());
    }

    [Fact]
    public void Unexpected_error_does_not_advise_repeating_a_write_that_may_already_be_committed()
    {
        var result = Result<string>.Failure("No se pudo recuperar el documento creado.", FailureKind.Unexpected);
        var envelope = Envelope<string>(result.ToHttpResult());
        Assert.DoesNotContain("Intente nuevamente", envelope.Message);
        Assert.Contains("estado de la operacion", envelope.Message);
    }

    private static ApiResponse<T> Envelope<T>(IResult response) => Assert.IsType<ApiResponse<T>>(
        Assert.IsAssignableFrom<IValueHttpResult>(response).Value);
}
