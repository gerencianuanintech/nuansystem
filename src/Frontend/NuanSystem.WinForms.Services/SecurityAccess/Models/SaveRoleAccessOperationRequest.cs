namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record SaveRoleAccessOperationRequest(int FormId, int OperationId, bool IsAllowed);
