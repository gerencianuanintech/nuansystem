namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record SaveSecurityDocumentSeriesOperationAccessRequest(
    int? OperationId,
    string ActionKey,
    bool IsAllowed);
