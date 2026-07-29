namespace NuanSystem.Application.Features.SriTxtImports;

public static class SriTxtImportLimits
{
    public const long MaxFileSizeBytes = 10L * 1024L * 1024L;
    public const int MaxDataRows = 50_000;
    public const int MaxLineBytes = 8 * 1024;
    public const int MaxProcessingSeconds = 60;
    public const int MaxFileNameLength = 260;
}

public static class SriTxtEncodingCodes
{
    public const string Utf8 = "UTF-8";
    public const string Windows1252 = "Windows-1252";
}

public static class SriTxtImportStatusCodes
{
    public const string Validated = "Validated";
    public const string ValidatedWithErrors = "ValidatedWithErrors";
    public const string Completed = "Completed";
    public const string CompletedWithErrors = "CompletedWithErrors";
    public static readonly IReadOnlyCollection<string> All =
        [Validated, ValidatedWithErrors, Completed, CompletedWithErrors];

    public static bool IsValid(string? value) =>
        string.IsNullOrWhiteSpace(value)
        || All.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : All.Single(item => item.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
}

public static class SriTxtRowValidationStatusCodes
{
    public const string Valid = "Valid";
    public const string Invalid = "Invalid";
    public const string DuplicateInFile = "DuplicateInFile";
    public const string Conflict = "Conflict";
}

public static class SriTxtRowEnqueueStatusCodes
{
    public const string NotEligible = "NotEligible";
    public const string Staged = "Staged";
    public const string LinkedExisting = "LinkedExisting";
    public const string LinkedAuthorized = "LinkedAuthorized";
    public const string Enqueued = "Enqueued";
    public const string Conflict = "Conflict";
}

public static class SriTxtRowValidityCodes
{
    public const string All = "All";
    public const string Valid = "Valid";
    public const string Invalid = "Invalid";
    public static readonly IReadOnlyCollection<string> Values = [All, Valid, Invalid];

    public static bool IsValid(string? value) =>
        string.IsNullOrWhiteSpace(value)
        || Values.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? All
            : Values.Single(item => item.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
}
