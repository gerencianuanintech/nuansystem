namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record SaveSecurityFormAccessOperationRequest(
    int OperationId,
    bool IsAllowed);
