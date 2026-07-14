namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record SaveSecurityDocumentSeriesAccessRequest(
    bool IsSelected,
    IReadOnlyCollection<SaveSecurityDocumentSeriesOperationAccessRequest> Operations);
