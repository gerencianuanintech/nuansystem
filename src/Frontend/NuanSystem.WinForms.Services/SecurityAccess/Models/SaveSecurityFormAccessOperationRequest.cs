namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record SaveSecurityFormAccessOperationRequest(
    int OperationId,
    bool IsAllowed);
