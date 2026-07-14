namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record SaveSecurityFormFieldAccessRequest(
    IReadOnlyCollection<SaveSecurityFormFieldAccessItemRequest> Fields);
