namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record SaveSecurityFormAccessRequest(
    IReadOnlyCollection<SaveSecurityFormAccessOperationRequest> Operations);
