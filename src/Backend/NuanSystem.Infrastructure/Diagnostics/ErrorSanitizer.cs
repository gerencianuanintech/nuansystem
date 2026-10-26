using System.Text.RegularExpressions;
using NuanSystem.Application.Abstractions.Diagnostics;

namespace NuanSystem.Infrastructure.Diagnostics;

public sealed class ErrorSanitizer : IErrorSanitizer
{
    private const int MaxInputLength = 12_000;
    private const int MaxOutputLength = 4_000;

    private static readonly Regex SensitiveAssignment = new(
        "(?ix)\\b(password|pwd|token|access_token|refresh_token|authorization|cookie|client_secret|api[-_]?key|user\\s*id|uid)\\b\\s*[:=]\\s*(?:bearer\\s+)?(?:\"[^\"\\r\\n]*\"|'[^'\\r\\n]*'|[^;\\s,\\r\\n]+)",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    private static readonly Regex Jwt = new(
        @"\beyJ[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\b",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    private static readonly Regex ConnectionString = new(
        @"(?i)\b(server|data\s+source)\s*=\s*[^\r\n]+",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    public string? Sanitize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var bounded = value.Length <= MaxInputLength ? value : value[..MaxInputLength];
        var sanitized = SensitiveAssignment.Replace(bounded, "$1=[REDACTED]");
        sanitized = Jwt.Replace(sanitized, "[REDACTED]");
        sanitized = ConnectionString.Replace(sanitized, "[CONNECTION_STRING_REDACTED]");
        return sanitized.Length <= MaxOutputLength ? sanitized : sanitized[..MaxOutputLength];
    }
}
