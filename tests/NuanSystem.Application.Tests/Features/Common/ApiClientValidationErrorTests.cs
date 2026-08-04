using System.Net;
using System.Text;
using FluentAssertions;
using NuanSystem.Shared.Responses;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.Application.Tests.Features.Common;

public sealed class ApiClientValidationErrorTests
{
    [Fact]
    public async Task Failed_response_preserves_structured_errors_and_formats_user_message()
    {
        var response = ApiResponse<object>.Fail(
            "El perfil SAP contiene datos invalidos.",
            [
                new ApiError(
                    "SAP_SYNC_PROFILE_SYNC_MODE_UNSUPPORTED",
                    "La entidad 'Warehouses' no soporta el modo 'Incremental'.",
                    "Entities[0].SyncMode"),
                new ApiError(
                    "SAP_SYNC_PROFILE_SCHEDULE_INVALID",
                    "La agenda debe respetar la forma de campos permitida.",
                    "Entities[0].Schedule")
            ]);
        using var httpClient = new HttpClient(new JsonResponseHandler(
            HttpStatusCode.BadRequest,
            System.Text.Json.JsonSerializer.Serialize(response)))
        {
            BaseAddress = new Uri("https://localhost")
        };
        var client = new NuanApiClient(httpClient, new ApiSession());

        var action = () => client.GetAsync<object>("/api/sap/sync-profiles/1");

        var exception = await action.Should().ThrowAsync<ApiClientException>();
        exception.Which.StatusCode.Should().Be(400);
        exception.Which.Errors.Should().HaveCount(2);
        ApiClientErrorMessageFormatter.Format(exception.Which).Should().Be(
            "El perfil SAP contiene datos invalidos."
            + Environment.NewLine + Environment.NewLine
            + "• Entidad 1 > Modo: La entidad 'Warehouses' no soporta el modo 'Incremental'."
            + Environment.NewLine
            + "• Entidad 1 > Programación: La agenda debe respetar la forma de campos permitida.");
    }

    [Fact]
    public void Formatter_sanitizes_control_characters_and_does_not_display_error_codes()
    {
        var exception = new ApiClientException(
            "Solicitud invalida.\r\nDetalle",
            400,
            [new ApiError("TECHNICAL_CODE", "Valor\r\nno permitido", "Entities[0].SyncMode\t")]);

        var message = ApiClientErrorMessageFormatter.Format(exception);

        message.Should().Be(
            "Solicitud invalida. Detalle"
            + Environment.NewLine + Environment.NewLine
            + "• Entidad 1 > Modo: Valor no permitido");
        message.Should().NotContain("TECHNICAL_CODE");
    }

    [Fact]
    public void Formatter_keeps_existing_behavior_when_response_has_no_structured_errors()
    {
        var exception = new ApiClientException("No tienes permiso.", 403);

        ApiClientErrorMessageFormatter.Format(exception).Should().Be("No tienes permiso.");
    }

    [Fact]
    public void Formatter_can_show_structured_errors_when_header_is_empty()
    {
        var exception = new ApiClientException(
            string.Empty,
            400,
            [new ApiError("INVALID", "Seleccione una empresa.", "CompanyId")]);

        ApiClientErrorMessageFormatter.Format(exception).Should().Be(
            "• Empresa SAP: Seleccione una empresa.");
    }

    private sealed class JsonResponseHandler(HttpStatusCode statusCode, string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                RequestMessage = request,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
        }
    }
}
