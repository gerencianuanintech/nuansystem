namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record SaveSecurityFormAccessOperationData(
    int OperationId,
    bool IsAllowed);
