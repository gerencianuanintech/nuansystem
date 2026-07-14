using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Configuration.Services;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Sync.Configuration.Commands;

public sealed class CreateSyncProfileCommandHandler(
    ISyncProfileRepository repository,
    ISyncProfileValidationService validationService)
    : ICommandHandler<CreateSyncProfileCommand, int>
{
    public async Task<Result<int>> Handle(CreateSyncProfileCommand request, CancellationToken cancellationToken)
    {
        var validation = await validationService.ValidateAsync(request.Request, null, request.AuditUserId, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<int>.Failure("El perfil de sincronizacion no es valido.", SyncValidationApiErrorMapper.ToApiErrors(validation));
        }

        var aggregate = SyncProfileMapper.ToAggregate(0, request.Request, request.AuditUserId, request.AuditUserName);
        var id = await repository.CreateAsync(aggregate, cancellationToken);
        await repository.RecordAuditAsync(
            id,
            "SyncProfileCreated",
            "Code",
            null,
            request.Request.Code,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);

        return Result<int>.Success(id, "Perfil de sincronizacion creado correctamente.");
    }
}

public sealed class UpdateSyncProfileCommandHandler(
    ISyncProfileRepository repository,
    ISyncProfileValidationService validationService)
    : ICommandHandler<UpdateSyncProfileCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateSyncProfileCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<bool>.Failure(
                "Perfil de sincronizacion no encontrado.",
                [new ApiError("SyncProfileNotFound", "El perfil no existe.", nameof(request.Id))]);
        }

        if (existing.CompanyId != request.Request.CompanyId
            && await repository.HasOperationalHistoryAsync(request.Id, cancellationToken))
        {
            return Result<bool>.Failure(
                "No se puede cambiar la empresa maestra porque el perfil tiene historial operativo.",
                [new ApiError("SyncProfileCompanyChangeBlocked", "Existe historial relacionado en SyncOutbox o SyncAudit.", nameof(request.Request.CompanyId))]);
        }

        var validation = await validationService.ValidateAsync(request.Request, request.Id, request.AuditUserId, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<bool>.Failure("El perfil de sincronizacion no es valido.", SyncValidationApiErrorMapper.ToApiErrors(validation));
        }

        var aggregate = SyncProfileMapper.ToAggregate(request.Id, request.Request, request.AuditUserId, request.AuditUserName);
        var updated = await repository.UpdateAsync(aggregate, cancellationToken);
        if (updated)
        {
            await repository.RecordAuditAsync(
                request.Id,
                "SyncProfileUpdated",
                "Code",
                existing.Code,
                request.Request.Code,
                request.AuditUserId,
                request.AuditUserName,
                cancellationToken);
        }

        return updated
            ? Result<bool>.Success(true, "Perfil de sincronizacion actualizado correctamente.")
            : Result<bool>.Failure(
                "Perfil de sincronizacion no encontrado.",
                [new ApiError("SyncProfileNotFound", "El perfil no existe.", nameof(request.Id))]);
    }
}

public sealed class ActivateSyncProfileCommandHandler(
    ISyncProfileRepository repository,
    ISyncProfileValidationService validationService)
    : ICommandHandler<ActivateSyncProfileCommand, bool>
{
    public async Task<Result<bool>> Handle(ActivateSyncProfileCommand request, CancellationToken cancellationToken)
    {
        var validation = await validationService.ValidatePersistedAsync(request.Id, request.AuditUserId, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<bool>.Failure("No se puede activar el perfil porque la configuracion no es valida.", SyncValidationApiErrorMapper.ToApiErrors(validation));
        }

        var updated = await repository.SetActiveAsync(request.Id, true, request.AuditUserId, request.AuditUserName, cancellationToken);
        if (updated)
        {
            await repository.RecordAuditAsync(
                request.Id,
                "SyncProfileActivated",
                "IsActive",
                "false",
                "true",
                request.AuditUserId,
                request.AuditUserName,
                cancellationToken);
        }

        return updated
            ? Result<bool>.Success(true, "Perfil de sincronizacion activado correctamente.")
            : Result<bool>.Failure("Perfil de sincronizacion no encontrado.", [new ApiError("SyncProfileNotFound", "El perfil no existe.", nameof(request.Id))]);
    }
}

public sealed class DeactivateSyncProfileCommandHandler(ISyncProfileRepository repository)
    : ICommandHandler<DeactivateSyncProfileCommand, bool>
{
    public async Task<Result<bool>> Handle(DeactivateSyncProfileCommand request, CancellationToken cancellationToken)
    {
        var updated = await repository.SetActiveAsync(request.Id, false, request.AuditUserId, request.AuditUserName, cancellationToken);
        if (updated)
        {
            await repository.RecordAuditAsync(
                request.Id,
                "ProfileDeactivated",
                "IsActive",
                "true",
                "false",
                request.AuditUserId,
                request.AuditUserName,
                cancellationToken);
        }

        return updated
            ? Result<bool>.Success(true, "Perfil de sincronizacion desactivado correctamente.")
            : Result<bool>.Failure("Perfil de sincronizacion no encontrado.", [new ApiError("SyncProfileNotFound", "El perfil no existe.", nameof(request.Id))]);
    }
}

public sealed class DeleteSyncProfileCommandHandler(ISyncProfileRepository repository)
    : ICommandHandler<DeleteSyncProfileCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteSyncProfileCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<bool>.Failure("Perfil de sincronizacion no encontrado.", [new ApiError("SyncProfileNotFound", "El perfil no existe.", nameof(request.Id))]);
        }

        if (await repository.HasOperationalHistoryAsync(request.Id, cancellationToken))
        {
            return Result<bool>.Failure(
                "El perfil tiene historial operativo; se debe desactivar en lugar de eliminar.",
                [new ApiError("SyncProfileHasOperationalHistory", "Existe historial relacionado en SyncOutbox o SyncAudit.", nameof(request.Id))]);
        }

        var deleted = await repository.DeleteAsync(request.Id, request.AuditUserId, request.AuditUserName, cancellationToken);
        if (deleted)
        {
            await repository.RecordAuditAsync(
                request.Id,
                "SyncProfileDeleted",
                "IsDeleted",
                "false",
                "true",
                request.AuditUserId,
                request.AuditUserName,
                cancellationToken);
        }

        return deleted
            ? Result<bool>.Success(true, "Perfil de sincronizacion eliminado correctamente.")
            : Result<bool>.Failure("Perfil de sincronizacion no encontrado.", [new ApiError("SyncProfileNotFound", "El perfil no existe.", nameof(request.Id))]);
    }
}

public sealed class ValidateSyncProfileCommandHandler(
    ISyncProfileValidationService validationService,
    ISyncProfileRepository repository)
    : ICommandHandler<ValidateSyncProfileCommand, SyncProfileValidationResultDto>
{
    public async Task<Result<SyncProfileValidationResultDto>> Handle(
        ValidateSyncProfileCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await validationService.ValidateAsync(request.Request, request.ProfileId, request.UserId, cancellationToken);
        await repository.RecordAuditAsync(
            request.ProfileId,
            "SyncProfileValidated",
            "Result",
            null,
            validation.IsValid ? "Valid" : "Invalid",
            request.UserId,
            null,
            cancellationToken);

        return Result<SyncProfileValidationResultDto>.Success(validation, "Validacion completada.");
    }
}

public sealed class ValidatePersistedSyncProfileCommandHandler(
    ISyncProfileValidationService validationService,
    ISyncProfileRepository repository)
    : ICommandHandler<ValidatePersistedSyncProfileCommand, SyncProfileValidationResultDto>
{
    public async Task<Result<SyncProfileValidationResultDto>> Handle(
        ValidatePersistedSyncProfileCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await validationService.ValidatePersistedAsync(request.Id, request.UserId, cancellationToken);
        await repository.RecordAuditAsync(
            request.Id,
            "SyncProfileValidated",
            "Result",
            null,
            validation.IsValid ? "Valid" : "Invalid",
            request.UserId,
            null,
            cancellationToken);

        return Result<SyncProfileValidationResultDto>.Success(validation, "Validacion completada.");
    }
}

internal static class SyncValidationApiErrorMapper
{
    public static IReadOnlyCollection<ApiError> ToApiErrors(SyncProfileValidationResultDto validation)
    {
        return validation.Errors
            .Select(error => new ApiError(error.Code, error.Message, error.Field))
            .ToArray();
    }
}
