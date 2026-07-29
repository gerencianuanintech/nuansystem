namespace NuanSystem.Application.Features.SriDocuments;

public static class SriEnvironmentCodes
{
    public const string Test = "Test";
    public const string Production = "Production";
    public static readonly IReadOnlyCollection<string> All = [Test, Production];
    public static bool IsValid(string? value) => All.Contains(value?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);
    public static string Normalize(string value) => All.Single(item => item.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
    public static char GetAccessKeyCode(string environment) => Normalize(environment) == Test ? '1' : '2';
}

public static class SriSourceTypeCodes
{
    public const string NuanSystem = "NuanSystem";
    public const string Txt = "Txt";
    public const string SapAddOn = "SapAddOn";
    public const string Manual = "Manual";
    public const string ExternalApi = "ExternalApi";
    public static readonly IReadOnlyCollection<string> All = [NuanSystem, Txt, SapAddOn, Manual, ExternalApi];
    public static bool IsValid(string? value) => All.Contains(value?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);
    public static string Normalize(string value) => All.Single(item => item.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
}

public static class SriDocumentTypeCodes
{
    public const string Invoice = "01";
    public const string CreditNote = "04";
    public const string Withholding = "07";
    public static readonly IReadOnlyCollection<string> Pilot = [Invoice, CreditNote, Withholding];
}

public static class SriDocumentQueueStatusCodes
{
    public const string Staged = "Staged";
    public const string Pending = "Pending";
    public const string Querying = "Querying";
    public const string RetryScheduled = "RetryScheduled";
    public const string Authorized = "Authorized";
    public const string NotFound = "NotFound";
    public const string Failed = "Failed";
    public const string DeadLetter = "DeadLetter";
    public const string Cancelled = "Cancelled";
    public static readonly IReadOnlyCollection<string> All = [Staged, Pending, Querying, RetryScheduled, Authorized, NotFound, Failed, DeadLetter, Cancelled];
    public static bool IsValid(string? value) => string.IsNullOrWhiteSpace(value) || All.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);
    public static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : All.Single(item => item.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
    public static string GetDisplayName(string? value) =>
        value switch
        {
            Staged => "Preparado",
            Pending => "Pendiente de consulta",
            Querying => "Consultando",
            RetryScheduled => "Reintento programado",
            Authorized => "Autorizado",
            NotFound => "No encontrado",
            Failed => "Fallido",
            DeadLetter => "Requiere intervencion",
            Cancelled => "Cancelado",
            _ => value ?? string.Empty
        };
}

public static class SriAccessKey
{
    public const int Length = 49;
    public static bool HasValidFormat(string? value) => value is { Length: Length } && value.All(char.IsAsciiDigit);

    public static bool HasValidCheckDigit(string? value)
    {
        if (!HasValidFormat(value)) return false;
        var sum = 0;
        var factor = 2;
        for (var index = Length - 2; index >= 0; index--)
        {
            sum += (value![index] - '0') * factor;
            factor = factor == 7 ? 2 : factor + 1;
        }
        var expected = 11 - (sum % 11);
        expected = expected switch { 11 => 0, 10 => 1, _ => expected };
        return value![^1] - '0' == expected;
    }

    public static bool MatchesEnvironment(string? value, string? environment) =>
        HasValidFormat(value) && SriEnvironmentCodes.IsValid(environment) && value![23] == SriEnvironmentCodes.GetAccessKeyCode(environment!);

    public static bool IsSupportedPilotDocument(string? value) =>
        HasValidFormat(value) && SriDocumentTypeCodes.Pilot.Contains(value!.Substring(8, 2), StringComparer.Ordinal);

    public static string GetDocumentType(string value) => value.Substring(8, 2);
}
