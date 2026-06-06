namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record SaveSecurityDocumentSeriesAccessRequest(
    bool IsSelected,
    IReadOnlyCollection<SaveSecurityDocumentSeriesOperationAccessRequest> Operations);
