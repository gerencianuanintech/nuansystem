using System.Text.Json;
using System.Text.Json.Nodes;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Features.Sync.Services;

public sealed class SyncEventPayloadFactory : ISyncEventPayloadFactory
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    private static readonly string[] SensitiveNameFragments =
    [
        "password",
        "secret",
        "token",
        "credential",
        "connectionstring",
        "encryptedpassword",
        "apikey",
        "api_key"
    ];

    public string CreatePayloadJson(SyncPublishRequest request)
    {
        var payloadNode = JsonSerializer.SerializeToNode(request.Payload, SerializerOptions) ?? new JsonObject();
        RemoveSensitiveValues(payloadNode);

        var root = new JsonObject
        {
            ["entityName"] = request.EntityName,
            ["globalId"] = JsonValue.Create(request.EntityGlobalId),
            ["code"] = request.EntityCode,
            ["operation"] = request.Operation.ToString(),
            ["payload"] = payloadNode
        };

        if (!string.IsNullOrWhiteSpace(request.CorrelationId))
        {
            root["correlationId"] = request.CorrelationId;
        }

        return root.ToJsonString(SerializerOptions);
    }

    private static void RemoveSensitiveValues(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject jsonObject:
                foreach (var propertyName in jsonObject.Select(property => property.Key).ToArray())
                {
                    if (IsSensitive(propertyName))
                    {
                        jsonObject.Remove(propertyName);
                        continue;
                    }

                    RemoveSensitiveValues(jsonObject[propertyName]);
                }

                break;

            case JsonArray jsonArray:
                foreach (var item in jsonArray)
                {
                    RemoveSensitiveValues(item);
                }

                break;
        }
    }

    private static bool IsSensitive(string propertyName)
    {
        var normalized = propertyName.Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();

        return SensitiveNameFragments.Any(fragment => normalized.Contains(fragment, StringComparison.Ordinal));
    }
}
