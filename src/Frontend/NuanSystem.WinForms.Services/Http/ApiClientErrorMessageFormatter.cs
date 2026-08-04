using NuanSystem.Shared.Responses;
using System.Text.RegularExpressions;

namespace NuanSystem.WinForms.Services.Http;

public static class ApiClientErrorMessageFormatter
{
    private const int MaximumVisibleErrors = 10;
    private const int MaximumTextLength = 500;
    private static readonly IReadOnlyDictionary<string, string> FieldNames =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["CompanyId"] = "Empresa SAP",
            ["Entities"] = "Entidad",
            ["EntityCode"] = "Entidad",
            ["Direction"] = "Dirección",
            ["SyncMode"] = "Modo",
            ["BatchSize"] = "Lote",
            ["MaxAttempts"] = "Intentos",
            ["ExecutionOrder"] = "Orden",
            ["ContinueOnError"] = "Continuar con error",
            ["ExecutionTimeoutMinutes"] = "Timeout (min)",
            ["IsActive"] = "Activa",
            ["Schedule"] = "Programación",
            ["ScheduleType"] = "Agenda",
            ["IntervalMinutes"] = "Minutos",
            ["ExecutionTime"] = "Hora",
            ["TimeZoneId"] = "Zona horaria",
            ["PreventConcurrentExecutions"] = "Evitar simultáneas"
        };

    public static string Format(ApiClientException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var header = Sanitize(exception.Message);
        var errors = exception.Errors
            .Where(error => !string.IsNullOrWhiteSpace(error.Message))
            .Select(error => new
            {
                Field = FormatField(Sanitize(error.Field)),
                Message = Sanitize(error.Message)
            })
            .Where(error => !string.IsNullOrWhiteSpace(error.Message))
            .Distinct()
            .ToArray();

        if (errors.Length == 0)
        {
            return header;
        }

        var details = errors
            .Take(MaximumVisibleErrors)
            .Select(error => string.IsNullOrWhiteSpace(error.Field)
                ? $"• {error.Message}"
                : $"• {error.Field}: {error.Message}")
            .ToList();

        if (errors.Length > MaximumVisibleErrors)
        {
            details.Add($"• Hay {errors.Length - MaximumVisibleErrors} errores adicionales.");
        }

        return string.IsNullOrWhiteSpace(header)
            ? string.Join(Environment.NewLine, details)
            : $"{header}{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, details)}";
    }

    private static string Sanitize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var sanitized = string.Join(
            ' ',
            value.Split(
                ['\r', '\n', '\t'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        return sanitized.Length <= MaximumTextLength
            ? sanitized
            : sanitized[..MaximumTextLength];
    }

    private static string FormatField(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return string.Join(
            " > ",
            value.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(segment =>
                {
                    var match = Regex.Match(segment, "^(?<name>[^\\[]+)\\[(?<index>\\d+)\\]$");
                    if (!match.Success)
                    {
                        return FieldNames.TryGetValue(segment, out var translated)
                            ? translated
                            : segment;
                    }

                    var name = match.Groups["name"].Value;
                    var index = int.Parse(match.Groups["index"].Value) + 1;
                    var label = FieldNames.TryGetValue(name, out var translatedName)
                        ? translatedName
                        : name;
                    return $"{label} {index}";
                }));
    }
}
