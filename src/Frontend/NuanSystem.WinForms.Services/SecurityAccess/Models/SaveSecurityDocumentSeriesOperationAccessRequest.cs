namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record SaveSecurityDocumentSeriesOperationAccessRequest(
    int? OperationId,
    string ActionKey,
    bool IsAllowed);
