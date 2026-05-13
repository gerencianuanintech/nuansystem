namespace NuanSystem.SapIntegration.Documents;

public sealed record SapClientResult(
    bool Success,
    string Status,
    string? ErrorMessage,
    string? RequestJson,
    string? ResponseJson,
    int? SapDocEntry,
    int? SapDocNum);
