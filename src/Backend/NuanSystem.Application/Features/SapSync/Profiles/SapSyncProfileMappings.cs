using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Profiles;

internal static class SapSyncProfileMappings
{
    public static SapSyncProfileDto ToApiDto(this SapSyncProfileDetailDto profile) =>
        new(
            profile.Id,
            profile.CompanyId,
            profile.CompanyCode,
            profile.CompanyName,
            profile.Code,
            profile.Name,
            profile.Description,
            profile.IsActive,
            profile.CreatedByUserId,
            profile.CreatedByUserName,
            profile.CreatedAtUtc,
            profile.UpdatedByUserId,
            profile.UpdatedByUserName,
            profile.UpdatedAtUtc,
            profile.RowVersion,
            profile.Entities.Select(entity => new SapSyncProfileEntityDto(
                entity.Id,
                entity.EntityCode,
                entity.Direction.ToString(),
                entity.SyncMode,
                entity.BatchSize,
                entity.MaxAttempts,
                entity.ExecutionOrder,
                entity.ContinueOnError,
                entity.ExecutionTimeoutMinutes,
                entity.IsActive,
                new SapSyncScheduleDto(
                    entity.Schedule.Id,
                    entity.Schedule.ScheduleType,
                    entity.Schedule.IntervalMinutes,
                    entity.Schedule.ExecutionTime,
                    entity.Schedule.TimeZoneId,
                    entity.Schedule.PreventConcurrentExecutions,
                    entity.Schedule.IsActive,
                    entity.Schedule.NextExecutionAtUtc,
                    entity.Schedule.LastScheduledAtUtc,
                    entity.Schedule.LastExecutionAtUtc,
                    entity.Schedule.LastSuccessfulExecutionAtUtc,
                    entity.Schedule.RowVersion),
                entity.RowVersion)).ToArray());

    public static SaveSapSyncProfileRequest ToSaveRequest(this SapSyncProfileDetailDto profile) =>
        new(
            profile.CompanyId,
            profile.Code,
            profile.Name,
            profile.Description,
            profile.Entities.Select(entity => new SaveSapSyncProfileEntityRequest(
                entity.Id,
                entity.EntityCode,
                entity.Direction.ToString(),
                entity.SyncMode,
                entity.BatchSize,
                entity.MaxAttempts,
                entity.ExecutionOrder,
                entity.ContinueOnError,
                entity.ExecutionTimeoutMinutes,
                entity.IsActive,
                new SaveSapSyncScheduleRequest(
                    entity.Schedule.Id,
                    entity.Schedule.ScheduleType,
                    entity.Schedule.IntervalMinutes,
                    entity.Schedule.ExecutionTime,
                    entity.Schedule.TimeZoneId,
                    entity.Schedule.PreventConcurrentExecutions,
                    entity.Schedule.IsActive,
                    entity.Schedule.RowVersion),
                entity.RowVersion)).ToArray());
}

internal static class SapSyncProfileResults
{
    public static Result<T> NotFound<T>(long id) =>
        Failure<T>(
            SapSyncProfileErrorCodes.NotFound,
            $"El perfil SAP {id} no existe o fue eliminado.",
            "Id",
            "No se encontro el perfil SAP.");

    public static Result<T> CompanyImmutable<T>(long id) =>
        Failure<T>(
            SapSyncProfileErrorCodes.CompanyImmutable,
            $"La empresa propietaria del perfil SAP {id} no puede cambiarse.",
            "CompanyId",
            "La empresa propietaria del perfil SAP es inmutable.");

    public static Result<T> MapWrite<T>(
        SapSyncProfileWriteResult writeResult,
        Func<SapSyncProfileWriteResult, T> successFactory,
        string successMessage)
    {
        return writeResult.ResultCode switch
        {
            SapSyncProfilePersistenceCodes.Created
                or SapSyncProfilePersistenceCodes.Updated
                or SapSyncProfilePersistenceCodes.Activated
                or SapSyncProfilePersistenceCodes.Deactivated
                or SapSyncProfilePersistenceCodes.Deleted
                => Result<T>.Success(successFactory(writeResult), successMessage),
            SapSyncProfilePersistenceCodes.NotFound
                => NotFound<T>(writeResult.Id ?? 0),
            SapSyncProfilePersistenceCodes.DuplicateCode
                => Failure<T>(
                    SapSyncProfileErrorCodes.DuplicateCode,
                    "Ya existe un perfil SAP con el mismo codigo en la empresa.",
                    "Code",
                    "El codigo del perfil SAP esta duplicado."),
            SapSyncProfilePersistenceCodes.ConcurrencyConflict
                => Failure<T>(
                    SapSyncProfileErrorCodes.ConcurrencyConflict,
                    "El perfil fue modificado por otro proceso. Recargue e intente nuevamente.",
                    "RowVersion",
                    "Se detecto un conflicto de concurrencia."),
            SapSyncProfilePersistenceCodes.CompanyImmutable
                => CompanyImmutable<T>(writeResult.Id ?? 0),
            "UnsupportedCapability" or "NoActiveSupportedEntities"
                => Failure<T>(
                    writeResult.ResultCode == "NoActiveSupportedEntities"
                        ? SapSyncProfileErrorCodes.NoActiveSupportedEntities
                        : SapSyncProfileErrorCodes.UnsupportedCapability,
                    "La configuracion contiene una capacidad SAP no soportada.",
                    "Entities",
                    "La capacidad SAP solicitada no esta soportada."),
            "InvalidSchedule"
                => Failure<T>(
                    SapSyncProfileErrorCodes.ScheduleInvalid,
                    "La agenda del perfil SAP no es valida.",
                    "Entities",
                    "La agenda del perfil SAP no es valida."),
            _ => Failure<T>(
                SapSyncProfileErrorCodes.PersistenceRejected,
                "Persistencia rechazo la operacion solicitada.",
                null,
                "No se pudo guardar el perfil SAP.")
        };
    }

    public static Result<T> FromFailure<TSource, T>(Result<TSource> failure) =>
        Result<T>.Failure(failure.Message, failure.Errors);

    private static Result<T> Failure<T>(
        string code,
        string detail,
        string? field,
        string message) =>
        Result<T>.Failure(message, [new ApiError(code, detail, field)]);
}
