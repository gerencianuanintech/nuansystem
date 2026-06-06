namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record SaveSecurityFormFieldAccessRequest(
    IReadOnlyCollection<SaveSecurityFormFieldAccessData> Fields);
