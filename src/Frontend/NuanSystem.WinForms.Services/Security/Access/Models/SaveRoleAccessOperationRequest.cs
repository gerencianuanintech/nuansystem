namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record SaveRoleAccessOperationRequest(int FormId, int OperationId, bool IsAllowed);
