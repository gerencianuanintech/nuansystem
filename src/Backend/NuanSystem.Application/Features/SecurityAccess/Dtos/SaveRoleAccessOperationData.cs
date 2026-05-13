namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record SaveRoleAccessOperationData(int FormId, int OperationId, bool IsAllowed);
