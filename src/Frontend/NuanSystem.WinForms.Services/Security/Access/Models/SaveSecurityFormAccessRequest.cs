namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record SaveSecurityFormAccessRequest(
    IReadOnlyCollection<SaveSecurityFormAccessOperationRequest> Operations);
