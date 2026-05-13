using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityFields.Dtos;

namespace NuanSystem.Application.Features.SecurityFields.Commands;

public sealed record CreateSecurityFieldCommand(
    int FormId,
    string Code,
    string Name,
    string FieldKey,
    string? Description,
    string ControlType,
    string DataType,
    bool IsRequired,
    string? ValidationMessage,
    bool IsReadOnly,
    bool IsVisible,
    bool IsCustom,
    int DisplayOrder,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<SecurityFieldDto>;
