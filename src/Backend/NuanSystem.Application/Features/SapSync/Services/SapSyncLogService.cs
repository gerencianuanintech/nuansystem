using System.Text.Json;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncLogService(ISapSyncTechnicalLogRepository repository) : ISapSyncLogService
{
    private static readonly string[] SensitiveNames = ["password", "token", "cookie", "session", "secret", "connectionstring"];

    public Task WriteAsync(SapSyncLogWriteDto log, CancellationToken cancellationToken = default)
        => repository.WriteAsync(log with { RequestJson = SanitizeJson(log.RequestJson), ResponseJson = SanitizeJson(log.ResponseJson) }, cancellationToken);

    private static string? SanitizeJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return json;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                WriteElement(document.RootElement, writer);
            }

            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
        catch (JsonException)
        {
            return ContainsSensitiveName(json) ? "[REDACTED]" : json;
        }
    }

    private static void WriteElement(JsonElement element, Utf8JsonWriter writer)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            writer.WriteStartObject();
            foreach (var property in element.EnumerateObject())
            {
                writer.WritePropertyName(property.Name);
                if (ContainsSensitiveName(property.Name))
                {
                    writer.WriteStringValue("[REDACTED]");
                }
                else
                {
                    WriteElement(property.Value, writer);
                }
            }
            writer.WriteEndObject();
            return;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            writer.WriteStartArray();
            foreach (var item in element.EnumerateArray())
            {
                WriteElement(item, writer);
            }
            writer.WriteEndArray();
            return;
        }

        element.WriteTo(writer);
    }

    private static bool ContainsSensitiveName(string value)
        => SensitiveNames.Any(name => value.Contains(name, StringComparison.OrdinalIgnoreCase));
}
