using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed record ImportSuppliersFromSapCommand(
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<SapSupplierImportResultDto>;
