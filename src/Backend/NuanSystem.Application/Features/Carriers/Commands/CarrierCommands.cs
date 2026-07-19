using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Carriers.Dtos;

namespace NuanSystem.Application.Features.Carriers.Commands;

public sealed record CreateCarrierCommand(string Code, string Name, string IdentificationTypeCode, string IdentificationNumber, string? Description, bool IsActive, int? AuditUserId, string? AuditUserName) : ICommand<CarrierDetailDto>;
public sealed record UpdateCarrierCommand(int Id, string Code, string Name, string IdentificationTypeCode, string IdentificationNumber, string? Description, bool IsActive, int? AuditUserId, string? AuditUserName) : ICommand<CarrierDetailDto>;
public sealed record DeleteCarrierCommand(int Id, int? AuditUserId, string? AuditUserName) : ICommand<bool>;
