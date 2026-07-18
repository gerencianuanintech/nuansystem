using System.Globalization;
using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Features.Sync.Services;

public sealed class SyncDistributionPolicyEvaluator : ISyncDistributionPolicyEvaluator
{
    private static readonly HashSet<string> WarehouseFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "code", "branchCode", "sapCode", "isActive", "allowsSales", "allowsPurchases"
    };

    public SyncDistributionDecisionDto Evaluate(SyncRoutingTargetDto target, SyncRoutingContext context)
    {
        var mode = string.IsNullOrWhiteSpace(target.DistributionMode) ? "All" : target.DistributionMode.Trim();
        return mode.ToUpperInvariant() switch
        {
            "NONE" => Create(false, "La politica no distribuye esta entidad."),
            "ALL" => Create(true, "La politica distribuye todos los registros."),
            "SELECTED" => Create(
                target.IsSelected,
                target.IsSelected ? "El GlobalId esta seleccionado." : "El GlobalId no esta seleccionado; permanece en Master."),
            "RULE" => EvaluateRule(target, context),
            _ => Create(false, "Modo de distribucion no reconocido; el registro permanece en Master.")
        };

        SyncDistributionDecisionDto Create(bool matched, string reason) => new(
            target.SyncProfileEntityBranchId,
            target.BranchCompanyId,
            mode,
            matched,
            reason,
            target.RuleVersion);
    }

    private static SyncDistributionDecisionDto EvaluateRule(SyncRoutingTargetDto target, SyncRoutingContext context)
    {
        try
        {
            using var ruleDocument = JsonDocument.Parse(target.RuleExpressionJson ?? string.Empty);
            using var payloadDocument = JsonDocument.Parse(context.PayloadJson ?? "{}");
            var rule = ruleDocument.RootElement;
            var facts = payloadDocument.RootElement.TryGetProperty("payload", out var payload)
                ? payload
                : payloadDocument.RootElement;
            var allowedFields = GetAllowedFields(context.EntityCode);
            var match = rule.TryGetProperty("match", out var matchElement) ? matchElement.GetString() : "All";

            if (!rule.TryGetProperty("conditions", out var conditions) || conditions.ValueKind != JsonValueKind.Array)
            {
                return Create(false, "La regla no contiene condiciones validas.");
            }

            var results = new List<bool>();
            foreach (var condition in conditions.EnumerateArray())
            {
                var field = condition.TryGetProperty("field", out var fieldElement) ? fieldElement.GetString() : null;
                var operation = condition.TryGetProperty("operator", out var operatorElement) ? operatorElement.GetString() : null;
                if (string.IsNullOrWhiteSpace(field) || !allowedFields.Contains(field) || string.IsNullOrWhiteSpace(operation))
                {
                    return Create(false, "La regla usa un campo u operador no autorizado.");
                }

                results.Add(EvaluateCondition(facts, field, operation, condition));
            }

            if (results.Count == 0)
            {
                return Create(false, "La regla no contiene condiciones.");
            }

            var matched = string.Equals(match, "Any", StringComparison.OrdinalIgnoreCase)
                ? results.Any(value => value)
                : results.All(value => value);
            return Create(
                matched,
                matched ? "La regla de distribucion coincide." : "La regla no coincide; el registro permanece en Master.");
        }
        catch (JsonException)
        {
            return Create(false, "La regla configurada no contiene JSON valido.");
        }

        SyncDistributionDecisionDto Create(bool matched, string reason) => new(
            target.SyncProfileEntityBranchId,
            target.BranchCompanyId,
            target.DistributionMode,
            matched,
            reason,
            target.RuleVersion);
    }

    private static bool EvaluateCondition(JsonElement facts, string field, string operation, JsonElement condition)
    {
        if (!TryGetProperty(facts, field, out var fact))
        {
            return false;
        }

        return operation.ToUpperInvariant() switch
        {
            "EQUALS" => condition.TryGetProperty("value", out var value) && ValuesEqual(fact, value),
            "NOTEQUALS" => condition.TryGetProperty("value", out var value) && !ValuesEqual(fact, value),
            "IN" => condition.TryGetProperty("values", out var values)
                && values.ValueKind == JsonValueKind.Array
                && values.EnumerateArray().Any(value => ValuesEqual(fact, value)),
            "ISTRUE" => fact.ValueKind == JsonValueKind.True,
            "ISFALSE" => fact.ValueKind == JsonValueKind.False,
            _ => false
        };
    }

    private static bool ValuesEqual(JsonElement left, JsonElement right)
    {
        if (left.ValueKind is JsonValueKind.True or JsonValueKind.False
            && right.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            return left.GetBoolean() == right.GetBoolean();
        }

        return string.Equals(ToInvariantString(left), ToInvariantString(right), StringComparison.OrdinalIgnoreCase);
    }

    private static string ToInvariantString(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.String => value.GetString() ?? string.Empty,
        JsonValueKind.Number => value.GetDecimal().ToString(CultureInfo.InvariantCulture),
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        JsonValueKind.Null => string.Empty,
        _ => value.GetRawText()
    };

    private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    private static IReadOnlySet<string> GetAllowedFields(string entityCode)
    {
        return string.Equals(entityCode, "Warehouse", StringComparison.OrdinalIgnoreCase)
            || string.Equals(entityCode, "Warehouses", StringComparison.OrdinalIgnoreCase)
            ? WarehouseFields
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "code", "isActive" };
    }
}
