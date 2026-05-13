namespace NuanSystem.WinForms.Services.Sap.Models;

public sealed record SapSendResult(
    bool Success,
    string Status,
    string Message,
    int? SapDocEntry,
    int? SapDocNum);
