namespace NuanSystem.Application.Features.Audit.Dtos;

public sealed record SecurityChangeDto(
    long Id,
    string EntityName,
    string RecordId,
    string Action,
    string FieldName,
    string? OldValue,
    string? NewValue,
    int? UserId,
    string? UserName,
    string Source,
    DateTime CreatedAt);
