using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Services;

namespace NuanSystem.Application.Features.Sync.Configuration.Services;

public sealed class SyncProfileValidationService(
    ISyncProfileRepository repository,
    ISyncRoutingRepository routingRepository,
    ISyncEntityCatalogService entityCatalogService,
    IBusinessPartnerSapCodePolicyRepository? sapCodePolicyRepository = null) : ISyncProfileValidationService
{
    private static readonly HashSet<string> SupportedExecutionModes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Incremental",
        "Full",
        "Manual"
    };

    private static readonly HashSet<string> SupportedScheduleTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Manual",
        "Interval",
        "Daily"
    };

    public async Task<SyncProfileValidationResultDto> ValidateAsync(
        SaveSyncProfileRequest request,
        int? profileId,
        int? userId,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<SyncValidationMessageDto>();
        var warnings = new List<SyncValidationMessageDto>();
        var companies = await repository.GetCompanyLookupsAsync(userId, cancellationToken);
        var companyById = companies.ToDictionary(company => company.Id);
        var entityCatalog = (await entityCatalogService.GetAsync(true, cancellationToken: cancellationToken))
            .ToDictionary(entity => entity.Code, StringComparer.OrdinalIgnoreCase);

        ValidateHeader(request, profileId, companyById, errors, warnings);
        await ValidateDuplicateCodeAsync(request, profileId, errors, cancellationToken);
        ValidateBranches(request, companyById, errors, warnings);
        ValidateEntities(request, entityCatalog, errors, warnings);
        ValidateDirectionalEntities(request, errors);
        ValidateMatrix(request, errors, warnings);
        ValidateSchedule(request.Schedule, errors, warnings);
        if (string.Equals(request.Direction, "BranchToMaster", StringComparison.OrdinalIgnoreCase)
            && (request.Schedule is null
                || !string.Equals(request.Schedule.ScheduleType, "Manual", StringComparison.OrdinalIgnoreCase)))
        {
            errors.Add(Message(
                "SyncBranchToMasterManualScheduleOnly",
                nameof(request.Schedule.ScheduleType),
                "BranchToMaster solo admite programacion Manual para autorizar la ruta incremental."));
        }
        await ValidateProposalCodePolicyAsync(request, companyById, errors, cancellationToken);
        await ValidateActiveRoutingConflictsAsync(request, profileId, errors, cancellationToken);

        if (!request.IsActive)
        {
            warnings.Add(Message("SyncProfileInactive", nameof(request.IsActive), "El perfil se guardara inactivo."));
        }

        return new SyncProfileValidationResultDto
        {
            IsValid = errors.Count == 0,
            Errors = errors,
            Warnings = warnings
        };
    }

    public async Task<SyncProfileValidationResultDto> ValidatePersistedAsync(
        int profileId,
        int? userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByIdAsync(profileId, cancellationToken);
        if (profile is null)
        {
            return new SyncProfileValidationResultDto
            {
                IsValid = false,
                Errors = [Message("SyncProfileNotFound", nameof(profileId), "El perfil no existe.")]
            };
        }

        var request = new SaveSyncProfileRequest
        {
            Code = profile.Code,
            Name = profile.Name,
            Description = profile.Description,
            CompanyId = profile.CompanyId,
            Direction = profile.Direction,
            ExecutionMode = profile.ExecutionMode,
            ConflictStrategy = profile.ConflictStrategy,
            BatchSize = profile.BatchSize,
            MaxRetries = profile.MaxRetries,
            RetryDelaySeconds = profile.RetryDelaySeconds,
            TimeoutMinutes = profile.TimeoutMinutes,
            IsActive = true,
            Branches = profile.Branches.Select(branch => new SaveSyncProfileBranchRequest
            {
                BranchCompanyId = branch.BranchCompanyId,
                BatchSize = branch.BatchSize,
                MaxRetries = branch.MaxRetries,
                IsActive = branch.IsActive
            }).ToArray(),
            Entities = profile.Entities.Select(entity => new SaveSyncProfileEntityRequest
            {
                EntityCode = entity.EntityCode,
                EntityName = entity.EntityName,
                ExecutionOrder = entity.ExecutionOrder,
                SyncMode = entity.SyncMode,
                KeyField = entity.KeyField,
                ModifiedAtField = entity.ModifiedAtField,
                VersionField = entity.VersionField,
                ActiveField = entity.ActiveField,
                AllowInsert = entity.AllowInsert,
                AllowUpdate = entity.AllowUpdate,
                AllowDeactivate = entity.AllowDeactivate,
                ContinueOnError = entity.ContinueOnError,
                BatchSize = entity.BatchSize,
                IsActive = entity.IsActive,
                Branches = profile.EntityBranches
                    .Where(link => string.Equals(link.EntityCode, entity.EntityCode, StringComparison.OrdinalIgnoreCase))
                    .Select(link => new SaveSyncEntityBranchRequest
                    {
                        BranchCompanyId = link.BranchCompanyId,
                        IsEnabled = link.IsEnabled,
                        BatchSize = link.BatchSize
                    })
                    .ToArray()
            }).ToArray(),
            Schedule = profile.Schedule is null
                ? null
                : new SaveSyncScheduleRequest
                {
                    ScheduleType = profile.Schedule.ScheduleType,
                    IntervalMinutes = profile.Schedule.IntervalMinutes,
                    ExecutionTime = profile.Schedule.ExecutionTime,
                    TimeZoneId = profile.Schedule.TimeZoneId,
                    PreventConcurrentExecutions = profile.Schedule.PreventConcurrentExecutions,
                    IsActive = profile.Schedule.IsActive
                }
        };

        return await ValidateAsync(request, profileId, userId, cancellationToken);
    }

    private static void ValidateHeader(
        SaveSyncProfileRequest request,
        int? profileId,
        IReadOnlyDictionary<int, SyncCompanyLookupRecord> companyById,
        List<SyncValidationMessageDto> errors,
        List<SyncValidationMessageDto> warnings)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            errors.Add(Message("SyncProfileCodeRequired", nameof(request.Code), "El codigo es obligatorio."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add(Message("SyncProfileNameRequired", nameof(request.Name), "El nombre es obligatorio."));
        }

        if (!companyById.TryGetValue(request.CompanyId, out var master))
        {
            errors.Add(Message("SyncMasterCompanyNotFound", nameof(request.CompanyId), "La empresa maestra no existe o no esta permitida para el usuario."));
        }
        else
        {
            if (!master.IsMaster)
            {
                errors.Add(Message("SyncMasterCompanyRequired", nameof(request.CompanyId), "La empresa seleccionada no esta marcada como maestra."));
            }

            if (!master.IsActive)
            {
                errors.Add(Message("SyncMasterCompanyInactive", nameof(request.CompanyId), "La empresa maestra esta inactiva."));
            }

            if (!master.SyncEnabled)
            {
                errors.Add(Message("SyncMasterCompanySyncDisabled", nameof(request.CompanyId), "La empresa maestra no tiene sincronizacion habilitada."));
            }
        }

        var masterToBranch = string.Equals(request.Direction, "MasterToBranch", StringComparison.OrdinalIgnoreCase);
        var branchToMaster = string.Equals(request.Direction, "BranchToMaster", StringComparison.OrdinalIgnoreCase);
        if (!masterToBranch && !branchToMaster)
        {
            errors.Add(Message("SyncDirectionNotSupported", nameof(request.Direction), "Solo se soportan MasterToBranch y BranchToMaster."));
        }

        var expectedStrategy = branchToMaster ? "CentralReview" : "MasterWins";
        if (!string.Equals(request.ConflictStrategy, expectedStrategy, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(Message("SyncConflictStrategyNotSupported", nameof(request.ConflictStrategy), $"La direccion {request.Direction} requiere {expectedStrategy}."));
        }

        if (!SupportedExecutionModes.Contains(request.ExecutionMode))
        {
            errors.Add(Message("SyncExecutionModeNotSupported", nameof(request.ExecutionMode), "El modo de ejecucion no esta soportado."));
        }
        else if (string.Equals(request.ExecutionMode, "Full", StringComparison.OrdinalIgnoreCase))
        {
            warnings.Add(Message("SyncExecutionModeFull", nameof(request.ExecutionMode), "El modo Full puede procesar mas datos que Incremental."));
        }

        if (branchToMaster && !string.Equals(request.ExecutionMode, "Incremental", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(Message("SyncBranchToMasterIncrementalOnly", nameof(request.ExecutionMode), "BranchToMaster solo admite ejecucion Incremental por relay."));
        }

        ValidateRange(request.BatchSize, 1, 10000, nameof(request.BatchSize), "SyncBatchSizeInvalid", errors);
        ValidateRange(request.MaxRetries, 0, 10, nameof(request.MaxRetries), "SyncMaxRetriesInvalid", errors);
        ValidateRange(request.RetryDelaySeconds, 0, 3600, nameof(request.RetryDelaySeconds), "SyncRetryDelayInvalid", errors);
        ValidateRange(request.TimeoutMinutes, 1, 1440, nameof(request.TimeoutMinutes), "SyncTimeoutInvalid", errors);

        if (request.BatchSize > 5000)
        {
            warnings.Add(Message("SyncBatchSizeHigh", nameof(request.BatchSize), "El lote configurado es alto; revise capacidad de red y base de datos."));
        }

        _ = profileId;
    }

    private async Task ValidateDuplicateCodeAsync(
        SaveSyncProfileRequest request,
        int? profileId,
        List<SyncValidationMessageDto> errors,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || request.CompanyId <= 0)
        {
            return;
        }

        var existing = await repository.GetByCodeAsync(request.CompanyId, request.Code.Trim().ToUpperInvariant(), cancellationToken);
        if (existing is not null && existing.Id != profileId)
        {
            errors.Add(Message("SyncProfileCodeDuplicated", nameof(request.Code), "Ya existe un perfil con el mismo codigo para la empresa maestra."));
        }
    }

    private static void ValidateBranches(
        SaveSyncProfileRequest request,
        IReadOnlyDictionary<int, SyncCompanyLookupRecord> companyById,
        List<SyncValidationMessageDto> errors,
        List<SyncValidationMessageDto> warnings)
    {
        var activeBranches = request.Branches.Where(branch => branch.IsActive).ToArray();
        if (activeBranches.Length == 0)
        {
            errors.Add(Message("SyncBranchesRequired", nameof(request.Branches), "Debe configurar al menos una sucursal activa."));
        }

        foreach (var duplicate in request.Branches.GroupBy(branch => branch.BranchCompanyId).Where(group => group.Count() > 1))
        {
            errors.Add(Message("SyncBranchDuplicated", nameof(request.Branches), $"La sucursal {duplicate.Key} esta duplicada."));
        }

        foreach (var branch in request.Branches)
        {
            if (branch.BranchCompanyId == request.CompanyId)
            {
                errors.Add(Message("SyncBranchEqualsMaster", nameof(branch.BranchCompanyId), "La sucursal no puede ser igual a la empresa maestra."));
            }

            if (!companyById.TryGetValue(branch.BranchCompanyId, out var company))
            {
                errors.Add(Message("SyncBranchNotFound", nameof(branch.BranchCompanyId), $"La sucursal {branch.BranchCompanyId} no existe o no esta permitida para el usuario."));
            }
            else
            {
                if (company.IsMaster || company.ParentCompanyId != request.CompanyId)
                {
                    errors.Add(Message("SyncBranchNotBelongsToMaster", nameof(branch.BranchCompanyId), $"La sucursal {company.Code} no pertenece a la empresa maestra."));
                }

                if (!company.IsActive)
                {
                    errors.Add(Message("SyncBranchInactive", nameof(branch.BranchCompanyId), $"La sucursal {company.Code} esta inactiva."));
                }

                if (!company.SyncEnabled)
                {
                    errors.Add(Message("SyncBranchSyncDisabled", nameof(branch.BranchCompanyId), $"La sucursal {company.Code} no tiene sincronizacion habilitada."));
                }
            }

            ValidateNullableRange(branch.BatchSize, 1, 10000, nameof(branch.BatchSize), "SyncBranchBatchSizeInvalid", errors);
            ValidateNullableRange(branch.MaxRetries, 0, 10, nameof(branch.MaxRetries), "SyncBranchMaxRetriesInvalid", errors);
        }

        _ = warnings;
    }

    private static void ValidateEntities(
        SaveSyncProfileRequest request,
        IReadOnlyDictionary<string, SyncEntityDefinitionLookupDto> entityCatalog,
        List<SyncValidationMessageDto> errors,
        List<SyncValidationMessageDto> warnings)
    {
        var activeEntities = request.Entities.Where(entity => entity.IsActive).ToArray();
        if (activeEntities.Length == 0)
        {
            errors.Add(Message("SyncEntitiesRequired", nameof(request.Entities), "Debe configurar al menos una entidad activa."));
        }

        foreach (var duplicate in request.Entities.GroupBy(entity => entity.EntityCode, StringComparer.OrdinalIgnoreCase).Where(group => group.Count() > 1))
        {
            errors.Add(Message("SyncEntityDuplicated", nameof(request.Entities), $"La entidad {duplicate.Key} esta duplicada."));
        }

        foreach (var entity in request.Entities)
        {
            if (!entityCatalog.TryGetValue(entity.EntityCode, out var catalogItem))
            {
                errors.Add(Message("SyncEntityUnknown", nameof(entity.EntityCode), $"La entidad {entity.EntityCode} no esta en el catalogo permitido."));
            }
            else
            {
                if (entity.IsActive && !catalogItem.IsActive)
                {
                    var message = Message("SyncEntityDefinitionInactive", nameof(entity.EntityCode), $"La entidad {entity.EntityCode} esta inactiva en el catalogo.");
                    if (request.IsActive)
                    {
                        errors.Add(message);
                    }
                    else
                    {
                        warnings.Add(message);
                    }
                }

                ValidateEntityOperability(request, entity, catalogItem, errors, warnings);
                ValidateEntityCapabilities(request, entity, catalogItem, errors, warnings);
            }

            if (entity.ExecutionOrder < 0)
            {
                errors.Add(Message("SyncEntityExecutionOrderInvalid", nameof(entity.ExecutionOrder), "El orden de ejecucion no puede ser negativo."));
            }

            if (!SupportedExecutionModes.Contains(entity.SyncMode))
            {
                errors.Add(Message("SyncEntityModeNotSupported", nameof(entity.SyncMode), $"El modo de la entidad {entity.EntityCode} no esta soportado."));
            }

            ValidateNullableRange(entity.BatchSize, 1, 10000, nameof(entity.BatchSize), "SyncEntityBatchSizeInvalid", errors);
            ValidateTechnicalField(entity.KeyField, nameof(entity.KeyField), errors);
            ValidateTechnicalField(entity.ModifiedAtField, nameof(entity.ModifiedAtField), errors);
            ValidateTechnicalField(entity.VersionField, nameof(entity.VersionField), errors);
            ValidateTechnicalField(entity.ActiveField, nameof(entity.ActiveField), errors);

            if (entity.AllowDeactivate)
            {
                warnings.Add(Message("SyncEntityAllowsDeactivate", nameof(entity.AllowDeactivate), $"La entidad {entity.EntityCode} puede desactivar registros en sucursal."));
            }
        }

        var activeByCode = request.Entities
            .Where(entity => entity.IsActive)
            .GroupBy(entity => entity.EntityCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        foreach (var entity in activeByCode.Values)
        {
            entityCatalog.TryGetValue(entity.EntityCode, out var catalogItem);
            foreach (var dependency in catalogItem?.Dependencies ?? Array.Empty<string>())
            {
                if (!activeByCode.TryGetValue(dependency, out var requiredEntity))
                {
                    errors.Add(Message(
                        "SyncEntityDependencyMissing",
                        nameof(request.Entities),
                        $"La entidad {entity.EntityCode} requiere que {dependency} este configurada y activa."));
                    continue;
                }

                if (requiredEntity.ExecutionOrder >= entity.ExecutionOrder)
                {
                    warnings.Add(Message(
                        "SyncEntityDependencyOrderAdjusted",
                        nameof(entity.ExecutionOrder),
                        $"La ejecucion colocara {dependency} antes de {entity.EntityCode}, independientemente del orden manual."));
                }

                var requiredBranches = requiredEntity.Branches
                    .Where(branch => branch.IsEnabled)
                    .Select(branch => branch.BranchCompanyId)
                    .ToHashSet();
                foreach (var branchCompanyId in entity.Branches
                             .Where(branch => branch.IsEnabled)
                             .Select(branch => branch.BranchCompanyId)
                             .Where(branchCompanyId => !requiredBranches.Contains(branchCompanyId)))
                {
                    errors.Add(Message(
                        "SyncEntityDependencyBranchMissing",
                        nameof(entity.Branches),
                        $"La entidad {entity.EntityCode} requiere {dependency} habilitada en la sucursal {branchCompanyId}."));
                }
            }
        }
    }

    private static void ValidateMatrix(
        SaveSyncProfileRequest request,
        List<SyncValidationMessageDto> errors,
        List<SyncValidationMessageDto> warnings)
    {
        var activeBranchIds = request.Branches.Where(branch => branch.IsActive).Select(branch => branch.BranchCompanyId).ToHashSet();
        var entityCodes = request.Entities.Select(entity => entity.EntityCode).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var enabledCombinations = 0;
        var enabledByBranch = new HashSet<int>();

        foreach (var entity in request.Entities)
        {
            var seenBranches = new HashSet<int>();
            var enabledForEntity = 0;

            foreach (var link in entity.Branches)
            {
                if (!activeBranchIds.Contains(link.BranchCompanyId))
                {
                    errors.Add(Message("SyncMatrixBranchNotInProfile", nameof(link.BranchCompanyId), $"La sucursal {link.BranchCompanyId} no esta activa en el perfil."));
                }

                if (!entityCodes.Contains(entity.EntityCode))
                {
                    errors.Add(Message("SyncMatrixEntityNotInProfile", nameof(entity.EntityCode), $"La entidad {entity.EntityCode} no esta activa en el perfil."));
                }

                if (!seenBranches.Add(link.BranchCompanyId))
                {
                    errors.Add(Message("SyncMatrixDuplicated", nameof(entity.Branches), $"La matriz de {entity.EntityCode} tiene duplicada la sucursal {link.BranchCompanyId}."));
                }

                ValidateNullableRange(link.BatchSize, 1, 10000, nameof(link.BatchSize), "SyncMatrixBatchSizeInvalid", errors);

                if (link.IsEnabled)
                {
                    enabledCombinations++;
                    enabledForEntity++;
                    enabledByBranch.Add(link.BranchCompanyId);
                }
            }

            if (entity.IsActive && enabledForEntity == 0)
            {
                errors.Add(Message("SyncEntityWithoutEnabledBranch", nameof(entity.Branches), $"La entidad {entity.EntityCode} no tiene sucursales habilitadas."));
            }
        }

        if (enabledCombinations == 0)
        {
            errors.Add(Message("SyncMatrixEnabledRequired", nameof(request.Entities), "Debe existir al menos una combinacion entidad-sucursal habilitada."));
        }

        foreach (var branch in request.Branches.Where(branch => branch.IsActive && !enabledByBranch.Contains(branch.BranchCompanyId)))
        {
            errors.Add(Message("SyncBranchWithoutEnabledEntity", nameof(request.Branches), $"La sucursal {branch.BranchCompanyId} no tiene entidades habilitadas."));
        }

        foreach (var branch in request.Branches.Where(branch => branch.IsActive))
        {
            var enabledCount = request.Entities.SelectMany(entity => entity.Branches)
                .Count(link => link.BranchCompanyId == branch.BranchCompanyId && link.IsEnabled);
            if (enabledCount == 1)
            {
                warnings.Add(Message("SyncBranchFewEntities", nameof(request.Branches), $"La sucursal {branch.BranchCompanyId} solo tiene una entidad habilitada."));
            }
        }
    }

    private static void ValidateDirectionalEntities(
        SaveSyncProfileRequest request,
        List<SyncValidationMessageDto> errors)
    {
        if (!string.Equals(request.Direction, "BranchToMaster", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        foreach (var entity in request.Entities.Where(entity => entity.IsActive))
        {
            if (!string.Equals(entity.EntityCode, SyncMasterBranchEntityCodes.BusinessPartnerProposal, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(Message(
                    "SyncBranchToMasterEntityNotSupported",
                    nameof(entity.EntityCode),
                    "BranchToMaster solo autoriza BusinessPartnerProposal."));
            }

            if (!string.Equals(entity.SyncMode, "Incremental", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(Message(
                    "SyncBranchToMasterIncrementalOnly",
                    nameof(entity.SyncMode),
                    "BusinessPartnerProposal solo admite modo Incremental."));
            }
        }
    }

    private async Task ValidateProposalCodePolicyAsync(
        SaveSyncProfileRequest request,
        IReadOnlyDictionary<int, SyncCompanyLookupRecord> companyById,
        List<SyncValidationMessageDto> errors,
        CancellationToken cancellationToken)
    {
        if (!request.IsActive
            || !string.Equals(request.Direction, "BranchToMaster", StringComparison.OrdinalIgnoreCase)
            || !request.Entities.Any(entity => entity.IsActive
                && string.Equals(entity.EntityCode, SyncMasterBranchEntityCodes.BusinessPartnerProposal, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        if (!companyById.TryGetValue(request.CompanyId, out var company)
            || !company.IsMaster
            || !company.IsActive
            || !company.SyncEnabled)
        {
            return;
        }

        var policy = sapCodePolicyRepository is null
            ? null
            : await sapCodePolicyRepository.GetByCompanyIdAsync(request.CompanyId, cancellationToken);
        if (policy is not { IsEnabled: true })
        {
            errors.Add(Message(
                "SyncBusinessPartnerSapCodePolicyRequired",
                nameof(request.CompanyId),
                "La politica de codigo SAP de socios debe estar habilitada antes de activar propuestas."));
        }
    }

    private static void ValidateSchedule(
        SaveSyncScheduleRequest? schedule,
        List<SyncValidationMessageDto> errors,
        List<SyncValidationMessageDto> warnings)
    {
        if (schedule is null)
        {
            return;
        }

        if (!SupportedScheduleTypes.Contains(schedule.ScheduleType))
        {
            errors.Add(Message("SyncScheduleTypeNotSupported", nameof(schedule.ScheduleType), "El tipo de programacion no esta soportado."));
            return;
        }

        if (string.Equals(schedule.ScheduleType, "Manual", StringComparison.OrdinalIgnoreCase)
            && (schedule.IntervalMinutes.HasValue || schedule.ExecutionTime.HasValue))
        {
            errors.Add(Message("SyncScheduleManualShapeInvalid", nameof(schedule.ScheduleType), "La programacion Manual no debe tener intervalo ni hora."));
        }

        if (string.Equals(schedule.ScheduleType, "Interval", StringComparison.OrdinalIgnoreCase))
        {
            if (!schedule.IntervalMinutes.HasValue)
            {
                errors.Add(Message("SyncScheduleIntervalRequired", nameof(schedule.IntervalMinutes), "La programacion por intervalo requiere minutos."));
            }
            else
            {
                ValidateRange(schedule.IntervalMinutes.Value, 1, 1440, nameof(schedule.IntervalMinutes), "SyncScheduleIntervalInvalid", errors);
                if (schedule.IntervalMinutes.Value < 5)
                {
                    warnings.Add(Message("SyncScheduleIntervalFrequent", nameof(schedule.IntervalMinutes), "El intervalo configurado es muy frecuente."));
                }
            }

            if (schedule.ExecutionTime.HasValue)
            {
                errors.Add(Message("SyncScheduleIntervalTimeInvalid", nameof(schedule.ExecutionTime), "La programacion por intervalo no debe tener hora fija."));
            }
        }

        if (string.Equals(schedule.ScheduleType, "Daily", StringComparison.OrdinalIgnoreCase)
            && !schedule.ExecutionTime.HasValue)
        {
            errors.Add(Message("SyncScheduleDailyTimeRequired", nameof(schedule.ExecutionTime), "La programacion diaria requiere hora."));
        }

        if (string.IsNullOrWhiteSpace(schedule.TimeZoneId))
        {
            errors.Add(Message("SyncScheduleTimeZoneRequired", nameof(schedule.TimeZoneId), "La zona horaria es obligatoria."));
        }
        else
        {
            try
            {
                _ = TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZoneId.Trim());
            }
            catch (TimeZoneNotFoundException)
            {
                errors.Add(Message("SyncScheduleTimeZoneInvalid", nameof(schedule.TimeZoneId), "La zona horaria no es valida para .NET."));
            }
            catch (InvalidTimeZoneException)
            {
                errors.Add(Message("SyncScheduleTimeZoneInvalid", nameof(schedule.TimeZoneId), "La zona horaria no es valida para .NET."));
            }
        }
    }

    private async Task ValidateActiveRoutingConflictsAsync(
        SaveSyncProfileRequest request,
        int? profileId,
        List<SyncValidationMessageDto> errors,
        CancellationToken cancellationToken)
    {
        if (!request.IsActive
            || request.CompanyId <= 0
            || !string.Equals(request.Direction, "MasterToBranch", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(request.ExecutionMode, "Incremental", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(request.ConflictStrategy, "MasterWins", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var activeBranchIds = request.Branches
            .Where(branch => branch.IsActive)
            .Select(branch => branch.BranchCompanyId)
            .ToHashSet();

        var combinations = request.Entities
            .Where(entity => entity.IsActive)
            .SelectMany(entity => entity.Branches
                .Where(link => link.IsEnabled && activeBranchIds.Contains(link.BranchCompanyId))
                .Select(link => new SyncRoutingConflictCheckItem(entity.EntityCode.Trim(), link.BranchCompanyId)))
            .GroupBy(item => $"{item.EntityCode}|{item.BranchCompanyId}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();

        if (combinations.Length == 0)
        {
            return;
        }

        var conflicts = await routingRepository.FindActiveConflictsAsync(
            profileId,
            request.CompanyId,
            combinations,
            cancellationToken);

        foreach (var conflict in conflicts)
        {
            errors.Add(Message(
                "SyncRoutingActiveConflict",
                nameof(request.Entities),
                $"Ya existe un perfil activo ({conflict.SyncProfileCode}) para la sucursal {conflict.BranchCompanyId} y entidad {conflict.EntityCode}."));
        }
    }

    private static void ValidateEntityOperability(
        SaveSyncProfileRequest request,
        SaveSyncProfileEntityRequest entity,
        SyncEntityDefinitionLookupDto catalogItem,
        List<SyncValidationMessageDto> errors,
        List<SyncValidationMessageDto> warnings)
    {
        if (!entity.IsActive || catalogItem.IsOperative)
        {
            return;
        }

        var message = $"La entidad {entity.EntityCode} no tiene productor/aplicador Master-Branch operativo.";
        if (request.IsActive)
        {
            errors.Add(Message("SyncEntityNotOperative", nameof(entity.EntityCode), message));
            return;
        }

        warnings.Add(Message("SyncEntityDraftOnly", nameof(entity.EntityCode), $"{message} Se permite guardarla como borrador inactivo."));
    }

    private static void ValidateEntityCapabilities(
        SaveSyncProfileRequest request,
        SaveSyncProfileEntityRequest entity,
        SyncEntityDefinitionLookupDto catalogItem,
        List<SyncValidationMessageDto> errors,
        List<SyncValidationMessageDto> warnings)
    {
        AddCapabilityMessage(entity.AllowInsert && !catalogItem.SupportsInsert, "SyncEntityInsertNotSupported", "insertar");
        AddCapabilityMessage(entity.AllowUpdate && !catalogItem.SupportsUpdate, "SyncEntityUpdateNotSupported", "actualizar");
        AddCapabilityMessage(entity.AllowDeactivate && !catalogItem.SupportsDeactivate, "SyncEntityDeactivateNotSupported", "desactivar");

        void AddCapabilityMessage(bool condition, string code, string operation)
        {
            if (!condition)
            {
                return;
            }

            var message = $"La entidad {entity.EntityCode} no soporta {operation} registros en el flujo Master-Branch actual.";
            if (request.IsActive)
            {
                errors.Add(Message(code, nameof(entity.EntityCode), message));
                return;
            }

            warnings.Add(Message(code, nameof(entity.EntityCode), message));
        }
    }

    private static void ValidateTechnicalField(string? value, string field, List<SyncValidationMessageDto> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var trimmed = value.Trim();
        if (trimmed.Contains(';', StringComparison.Ordinal)
            || trimmed.Contains("--", StringComparison.Ordinal)
            || trimmed.Contains("/*", StringComparison.Ordinal)
            || trimmed.Contains("*/", StringComparison.Ordinal)
            || trimmed.Contains('(', StringComparison.Ordinal)
            || trimmed.Contains(')', StringComparison.Ordinal)
            || trimmed.Any(char.IsWhiteSpace))
        {
            errors.Add(Message("SyncTechnicalFieldExecutable", field, "Los campos tecnicos no aceptan SQL libre ni expresiones ejecutables."));
        }

        if (trimmed.Any(character => !char.IsLetterOrDigit(character) && character != '_'))
        {
            errors.Add(Message("SyncTechnicalFieldInvalid", field, "Los campos tecnicos solo aceptan letras, numeros y guion bajo."));
        }
    }

    private static void ValidateRange(
        int value,
        int minimum,
        int maximum,
        string field,
        string code,
        List<SyncValidationMessageDto> errors)
    {
        if (value < minimum || value > maximum)
        {
            errors.Add(Message(code, field, $"El valor debe estar entre {minimum} y {maximum}."));
        }
    }

    private static void ValidateNullableRange(
        int? value,
        int minimum,
        int maximum,
        string field,
        string code,
        List<SyncValidationMessageDto> errors)
    {
        if (value.HasValue)
        {
            ValidateRange(value.Value, minimum, maximum, field, code, errors);
        }
    }

    private static SyncValidationMessageDto Message(string code, string? field, string message)
    {
        return new SyncValidationMessageDto(code, field, message);
    }
}
