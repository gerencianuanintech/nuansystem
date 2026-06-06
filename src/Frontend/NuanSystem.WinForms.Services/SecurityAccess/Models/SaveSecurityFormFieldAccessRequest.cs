namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record SaveSecurityFormFieldAccessRequest(
    IReadOnlyCollection<SaveSecurityFormFieldAccessItemRequest> Fields);
