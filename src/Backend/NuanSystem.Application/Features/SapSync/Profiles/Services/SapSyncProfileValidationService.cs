using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Profiles.Services;

public interface ISapSyncProfileValidationService
{
    Task<Result<SapSyncProfileCompanyAccessDto>> ValidateCompanyAsync(
        int companyId,
        int userId,
        bool requireSapReady,
        CancellationToken cancellationToken = default);

    Task<SapSyncProfileValidationResultDto> ValidateAsync(
        SaveSapSyncProfileRequest request,
        int userId,
        bool requireActiveEntity,
        CancellationToken cancellationToken = default);

    Task<Result<SapSyncProfileAggregate>> BuildAggregateAsync(
        long? id,
        SaveSapSyncProfileRequest request,
        int userId,
        int? auditUserId,
        string? auditUserName,
        bool profileIsActive,
        bool forceChildrenInactive,
        byte[]? rowVersion,
        CancellationToken cancellationToken = default);
}

public sealed class SapSyncProfileValidationService(ISapSyncProfileRepository repository)
    : ISapSyncProfileValidationService
{
    public const string DefaultTimeZoneId = "America/Guayaquil";

    public async Task<Result<SapSyncProfileCompanyAccessDto>> ValidateCompanyAsync(
        int companyId,
        int userId,
        bool requireSapReady,
        CancellationToken cancellationToken = default)
    {
        var company = (await repository.GetCompanyAccessAsync(userId, companyId, cancellationToken))
            .SingleOrDefault();
        if (company is null)
        {
            return CompanyFailure(
                SapSyncProfileErrorCodes.CompanyNotFound,
                "La empresa indicada no existe.");
        }

        if (!company.IsUserAuthorized)
        {
            return CompanyFailure(
                SapSyncProfileErrorCodes.CompanyAccessDenied,
                "La empresa no pertenece al alcance autorizado del usuario.");
        }

        if (!requireSapReady)
        {
            return Result<SapSyncProfileCompanyAccessDto>.Success(company);
        }

        if (!company.IsCompanyActive)
        {
            return CompanyFailure(
                SapSyncProfileErrorCodes.CompanyInactive,
                "La empresa indicada esta inactiva.");
        }

        if (company.SapIntegrationMode == 0
            || !company.HasSapSettings
            || !company.IsSapEnabled
            || company.SapSettingsIntegrationMode == 0)
        {
            return CompanyFailure(
                SapSyncProfileErrorCodes.CompanySapDisabled,
                "La empresa no tiene una configuracion SAP habilitada.");
        }

        return Result<SapSyncProfileCompanyAccessDto>.Success(company);
    }

    public async Task<SapSyncProfileValidationResultDto> ValidateAsync(
        SaveSapSyncProfileRequest request,
        int userId,
        bool requireActiveEntity,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<SapSyncProfileValidationMessageDto>();
        var company = await ValidateCompanyAsync(
            request.CompanyId,
            userId,
            requireSapReady: true,
            cancellationToken);
        if (!company.IsSuccess)
        {
            errors.AddRange(company.Errors.Select(error =>
                new SapSyncProfileValidationMessageDto(error.Code, error.Message, error.Field)));
        }

        var capabilities = await repository.GetHandlerCapabilitiesAsync(
            activeOnly: false,
            cancellationToken);
        var capabilityByCode = capabilities.ToDictionary(
            capability => capability.EntityCode,
            StringComparer.OrdinalIgnoreCase);

        if (request.Entities.Count == 0)
        {
            errors.Add(Error(
                SapSyncProfileErrorCodes.EntityRequired,
                "El perfil debe contener al menos una entidad.",
                nameof(request.Entities)));
        }

        var duplicates = request.Entities
            .GroupBy(
                entity => $"{entity.EntityCode.Trim()}|{entity.Direction.Trim()}",
                StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1);
        foreach (var duplicate in duplicates)
        {
            errors.Add(Error(
                SapSyncProfileErrorCodes.DuplicateEntityDirection,
                $"La combinacion entidad/direccion '{duplicate.Key}' esta duplicada.",
                nameof(request.Entities)));
        }

        var activeSupportedEntities = 0;
        for (var index = 0; index < request.Entities.Count; index++)
        {
            var entity = request.Entities.ElementAt(index);
            var prefix = $"{nameof(request.Entities)}[{index}]";
            var entityCode = entity.EntityCode.Trim();

            if (!capabilityByCode.TryGetValue(entityCode, out var capability))
            {
                errors.Add(Error(
                    SapSyncProfileErrorCodes.EntityUnknown,
                    $"La entidad '{entityCode}' no esta registrada en SapSyncHandlerCapabilities.",
                    $"{prefix}.{nameof(entity.EntityCode)}"));
                ValidateSchedule(entity.Schedule, prefix, errors);
                continue;
            }

            if (entityCode.Equals("PurchaseOrders", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(Error(
                    SapSyncProfileErrorCodes.PurchaseOrdersUnsupported,
                    "PurchaseOrders no esta implementado para perfiles SAP.",
                    $"{prefix}.{nameof(entity.EntityCode)}"));
            }
            else if (!capability.IsActive || !capability.IsImplemented)
            {
                errors.Add(Error(
                    SapSyncProfileErrorCodes.EntityNotImplemented,
                    $"El handler de '{entityCode}' no esta implementado y activo.",
                    $"{prefix}.{nameof(entity.EntityCode)}"));
            }

            if (!TryParseDirection(entity.Direction, out var direction))
            {
                errors.Add(Error(
                    SapSyncProfileErrorCodes.DirectionInvalid,
                    $"La direccion '{entity.Direction}' no es valida.",
                    $"{prefix}.{nameof(entity.Direction)}"));
            }
            else if (direction == SapSyncDirection.Both)
            {
                errors.Add(Error(
                    SapSyncProfileErrorCodes.DirectionBothUnsupported,
                    "La direccion Both no esta habilitada mientras ambos sentidos no esten implementados.",
                    $"{prefix}.{nameof(entity.Direction)}"));
            }
            else if (!capability.Supports(direction))
            {
                errors.Add(Error(
                    SapSyncProfileErrorCodes.DirectionUnsupported,
                    $"La entidad '{entityCode}' no soporta la direccion '{direction}'.",
                    $"{prefix}.{nameof(entity.Direction)}"));
            }

            var syncMode = entity.SyncMode.Trim();
            if (!syncMode.Equals(SapSyncModes.Full, StringComparison.OrdinalIgnoreCase)
                && !syncMode.Equals(SapSyncModes.Incremental, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(Error(
                    SapSyncProfileErrorCodes.SyncModeInvalid,
                    $"El modo '{entity.SyncMode}' no es valido.",
                    $"{prefix}.{nameof(entity.SyncMode)}"));
            }
            else if ((syncMode.Equals(SapSyncModes.Full, StringComparison.OrdinalIgnoreCase) && !capability.SupportsFull)
                || (syncMode.Equals(SapSyncModes.Incremental, StringComparison.OrdinalIgnoreCase) && !capability.SupportsIncremental))
            {
                errors.Add(Error(
                    SapSyncProfileErrorCodes.SyncModeUnsupported,
                    $"La entidad '{entityCode}' no soporta el modo '{syncMode}'.",
                    $"{prefix}.{nameof(entity.SyncMode)}"));
            }

            ValidateLimits(entity, prefix, errors);
            ValidateSchedule(entity.Schedule, prefix, errors);

            if (entity.IsActive
                && capability.IsActive
                && capability.IsImplemented
                && TryParseDirection(entity.Direction, out direction)
                && direction != SapSyncDirection.Both
                && capability.Supports(direction)
                && IsSupportedMode(entity.SyncMode, capability))
            {
                activeSupportedEntities++;
            }
        }

        if (requireActiveEntity && activeSupportedEntities == 0)
        {
            errors.Add(Error(
                SapSyncProfileErrorCodes.NoActiveSupportedEntities,
                "La activacion requiere al menos una entidad activa, implementada y compatible.",
                nameof(request.Entities)));
        }

        return new SapSyncProfileValidationResultDto(errors.Count == 0, errors);
    }

    public async Task<Result<SapSyncProfileAggregate>> BuildAggregateAsync(
        long? id,
        SaveSapSyncProfileRequest request,
        int userId,
        int? auditUserId,
        string? auditUserName,
        bool profileIsActive,
        bool forceChildrenInactive,
        byte[]? rowVersion,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(
            request,
            userId,
            requireActiveEntity: false,
            cancellationToken);
        if (!validation.IsValid)
        {
            return Result<SapSyncProfileAggregate>.Failure(
                "El perfil SAP contiene datos invalidos.",
                validation.Errors.Select(error =>
                    new ApiError(error.Code, error.Message, error.Field)).ToArray());
        }

        var entities = request.Entities.Select(entity =>
        {
            _ = TryParseDirection(entity.Direction, out var direction);
            var schedule = entity.Schedule;
            return new SapSyncProfileEntityData(
                entity.Id,
                entity.EntityCode.Trim(),
                direction,
                NormalizeSyncMode(entity.SyncMode),
                entity.BatchSize,
                entity.MaxAttempts,
                entity.ExecutionOrder,
                entity.ContinueOnError,
                entity.ExecutionTimeoutMinutes,
                forceChildrenInactive ? false : entity.IsActive,
                new SapSyncScheduleData(
                    schedule.Id,
                    NormalizeScheduleType(schedule.ScheduleType),
                    schedule.IntervalMinutes,
                    schedule.ExecutionTime,
                    NormalizeTimeZone(schedule.TimeZoneId),
                    schedule.PreventConcurrentExecutions,
                    null,
                    null,
                    null,
                    null,
                    forceChildrenInactive ? false : schedule.IsActive,
                    schedule.RowVersion),
                entity.RowVersion);
        }).ToArray();

        return Result<SapSyncProfileAggregate>.Success(new SapSyncProfileAggregate(
            id,
            request.CompanyId,
            request.Code.Trim().ToUpperInvariant(),
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            profileIsActive,
            entities,
            auditUserId,
            NormalizeOptional(auditUserName),
            rowVersion));
    }

    internal static bool TryParseDirection(string? value, out SapSyncDirection direction) =>
        Enum.TryParse(value?.Trim(), ignoreCase: true, out direction)
        && Enum.IsDefined(direction);

    internal static string NormalizeTimeZone(string? value) =>
        string.IsNullOrWhiteSpace(value) ? DefaultTimeZoneId : value.Trim();

    private static void ValidateLimits(
        SaveSapSyncProfileEntityRequest entity,
        string prefix,
        ICollection<SapSyncProfileValidationMessageDto> errors)
    {
        AddRangeError(entity.BatchSize, 1, 10000, nameof(entity.BatchSize));
        AddRangeError(entity.MaxAttempts, 1, 20, nameof(entity.MaxAttempts));
        AddRangeError(entity.ExecutionOrder, 0, 100000, nameof(entity.ExecutionOrder));
        AddRangeError(entity.ExecutionTimeoutMinutes, 1, 1440, nameof(entity.ExecutionTimeoutMinutes));
        return;

        void AddRangeError(int value, int minimum, int maximum, string field)
        {
            if (value < minimum || value > maximum)
            {
                errors.Add(Error(
                    SapSyncProfileErrorCodes.UnsupportedCapability,
                    $"{field} debe estar entre {minimum} y {maximum}.",
                    $"{prefix}.{field}"));
            }
        }
    }

    private static void ValidateSchedule(
        SaveSapSyncScheduleRequest schedule,
        string entityPrefix,
        ICollection<SapSyncProfileValidationMessageDto> errors)
    {
        var prefix = $"{entityPrefix}.{nameof(SaveSapSyncProfileEntityRequest.Schedule)}";
        var type = schedule.ScheduleType?.Trim();
        var validShape = type?.ToUpperInvariant() switch
        {
            "MANUAL" => schedule.IntervalMinutes is null && schedule.ExecutionTime is null,
            "INTERVAL" => schedule.IntervalMinutes is >= 1 and <= 525600 && schedule.ExecutionTime is null,
            "DAILY" => schedule.IntervalMinutes is null
                       && schedule.ExecutionTime is not null
                       && schedule.ExecutionTime >= TimeSpan.Zero
                       && schedule.ExecutionTime < TimeSpan.FromDays(1),
            _ => false
        };

        if (!validShape)
        {
            errors.Add(Error(
                SapSyncProfileErrorCodes.ScheduleInvalid,
                "La agenda debe ser Manual, Interval o Daily y respetar la forma de campos permitida.",
                prefix));
        }

        var timeZoneId = NormalizeTimeZone(schedule.TimeZoneId);
        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            AddTimeZoneError();
        }
        catch (InvalidTimeZoneException)
        {
            AddTimeZoneError();
        }

        if (!schedule.PreventConcurrentExecutions)
        {
            errors.Add(Error(
                SapSyncProfileErrorCodes.ConcurrentExecutionRequired,
                "La prevencion de ejecuciones concurrentes es obligatoria.",
                $"{prefix}.{nameof(schedule.PreventConcurrentExecutions)}"));
        }

        return;

        void AddTimeZoneError() =>
            errors.Add(Error(
                SapSyncProfileErrorCodes.TimeZoneInvalid,
                $"La zona horaria '{timeZoneId}' no es valida.",
                $"{prefix}.{nameof(schedule.TimeZoneId)}"));
    }

    private static bool IsSupportedMode(string value, SapSyncHandlerCapabilityDto capability) =>
        value.Trim().Equals(SapSyncModes.Full, StringComparison.OrdinalIgnoreCase)
            ? capability.SupportsFull
            : value.Trim().Equals(SapSyncModes.Incremental, StringComparison.OrdinalIgnoreCase)
              && capability.SupportsIncremental;

    private static string NormalizeSyncMode(string value) =>
        value.Trim().Equals(SapSyncModes.Full, StringComparison.OrdinalIgnoreCase)
            ? SapSyncModes.Full
            : SapSyncModes.Incremental;

    private static string NormalizeScheduleType(string value) =>
        SapSyncScheduleTypes.All.First(type => type.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SapSyncProfileValidationMessageDto Error(string code, string message, string? field) =>
        new(code, message, field);

    private static Result<SapSyncProfileCompanyAccessDto> CompanyFailure(string code, string message) =>
        Result<SapSyncProfileCompanyAccessDto>.Failure(
            "La empresa del perfil SAP no esta disponible.",
            [new ApiError(code, message, "CompanyId")]);
}
