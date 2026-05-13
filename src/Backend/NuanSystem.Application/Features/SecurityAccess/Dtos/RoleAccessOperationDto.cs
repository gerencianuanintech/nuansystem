namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record RoleAccessOperationDto(
    int FormId,
    string FormCode,
    string FormName,
    string FormKey,
    int OperationId,
    string OperationCode,
    string OperationName,
    string? ActionKey,
    bool IsAllowed);
