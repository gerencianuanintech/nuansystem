namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record SaveSecurityFormAccessRequest(
    IReadOnlyCollection<SaveSecurityFormAccessOperationData> Operations);
