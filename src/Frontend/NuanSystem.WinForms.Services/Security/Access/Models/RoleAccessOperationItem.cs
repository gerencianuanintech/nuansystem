namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record RoleAccessOperationItem(
    int FormId,
    string FormCode,
    string FormName,
    string FormKey,
    int OperationId,
    string OperationCode,
    string OperationName,
    string? ActionKey,
    bool IsAllowed);
