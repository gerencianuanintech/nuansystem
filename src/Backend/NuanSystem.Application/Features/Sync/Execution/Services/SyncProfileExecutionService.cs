using System.Text.Json;
using NuanSystem.Application.Abstractions.Common;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Configuration.Services;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Services;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Execution.Services;

public sealed class SyncProfileExecutionService(
    ISyncProfileRepository profileRepository,
    ISyncProfileValidationService validationService,
    ISyncProfileExecutionRepository executionRepository,
    IEnumerable<ISyncFullEntitySource> entitySources,
    ISyncEventPublisher eventPublisher,
    ISyncEntityCatalogService entityCatalogService,
    ISystemClock clock) : ISyncProfileExecutionService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IReadOnlyDictionary<string, ISyncFullEntitySource> entitySourcesByCode = entitySources
        .GroupBy(source => source.EntityCode, StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.Single(), StringComparer.OrdinalIgnoreCase);

    public async Task<Result<CreateSyncProfileExecutionResultDto>> RequestExecutionAsync(
        int syncProfileId,
        SyncProfileExecutionRequest request,
        int? auditUserId = null,
        string? auditUserName = null,
        CancellationToken cancellationToken = default)
    {
        var profile = await profileRepository.GetByIdAsync(syncProfileId, cancellationToken);
        if (profile is null)
        {
            return Failure<CreateSyncProfileExecutionResultDto>("SyncProfileNotFound", "El perfil no existe.", nameof(syncProfileId));
        }

        if (!profile.IsActive)
        {
            return Failure<CreateSyncProfileExecutionResultDto>("SyncProfileInactive", "El perfil debe estar activo para ejecutarse.", nameof(syncProfileId));
        }

        if (string.Equals(profile.Direction, "BranchToMaster", StringComparison.OrdinalIgnoreCase))
        {
            return Failure<CreateSyncProfileExecutionResultDto>(
                "SYNC_BRANCH_TO_MASTER_INCREMENTAL_ONLY",
                "BranchToMaster solo autoriza el enrutamiento incremental del relay y no admite ejecucion administrativa.",
                nameof(profile.Direction));
        }

        if (!string.Equals(profile.Direction, "MasterToBranch", StringComparison.OrdinalIgnoreCase))
        {
            return Failure<CreateSyncProfileExecutionResultDto>("SyncDirectionNotSupported", "Solo se ejecuta MasterToBranch.", nameof(profile.Direction));
        }

        if (string.Equals(request.ExecutionType, "Scheduled", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(profile.ExecutionMode, "Full", StringComparison.OrdinalIgnoreCase))
        {
            return Failure<CreateSyncProfileExecutionResultDto>("SyncScheduledRequiresFull", "La programacion automatica solo ejecuta perfiles Full.", nameof(profile.ExecutionMode));
        }

        var validation = await validationService.ValidatePersistedAsync(syncProfileId, auditUserId, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<CreateSyncProfileExecutionResultDto>.Failure(
                "No se puede ejecutar el perfil porque la configuracion no es valida.",
                validation.Errors.Select(error => new ApiError(error.Code, error.Message, error.Field)).ToArray());
        }

        var requestedCodes = NormalizeEntityCodes(request.EntityCodes);
        var catalog = await entityCatalogService.GetAsync(true, cancellationToken: cancellationToken);
        var entities = GetExecutableEntities(profile, requestedCodes, entitySourcesByCode.Keys, catalog);
        if (entities.Count == 0)
        {
            return Failure<CreateSyncProfileExecutionResultDto>("SyncExecutionNoEntities", "No hay entidades operativas activas para ejecutar.", nameof(request.EntityCodes));
        }

        if (request.MaxRecords is <= 0)
        {
            return Failure<CreateSyncProfileExecutionResultDto>("SyncExecutionMaxRecordsInvalid", "El maximo de registros debe ser mayor a cero.", nameof(request.MaxRecords));
        }

        if (profile.Schedule?.PreventConcurrentExecutions != false)
        {
            var activeExecutionId = await executionRepository.GetActiveExecutionAsync(syncProfileId, cancellationToken);
            if (activeExecutionId.HasValue)
            {
                return Failure<CreateSyncProfileExecutionResultDto>("SyncExecutionAlreadyActive", $"Ya existe una ejecucion activa ({activeExecutionId.Value}) para el perfil.", nameof(syncProfileId));
            }
        }

        var correlationId = $"sync-profile-{syncProfileId}-{Guid.NewGuid():N}";
        var executionId = await executionRepository.CreateAsync(
            new CreateSyncProfileExecutionData(
                syncProfileId,
                NormalizeExecutionType(request.ExecutionType),
                string.IsNullOrWhiteSpace(request.RequestedBy) ? auditUserName ?? "System" : request.RequestedBy.Trim(),
                correlationId,
                requestedCodes.Count == 0 ? null : JsonSerializer.Serialize(requestedCodes, JsonOptions),
                request.FromKey,
                request.MaxRecords,
                entities.Count,
                auditUserId,
                auditUserName),
            cancellationToken);

        await profileRepository.RecordAuditAsync(
            syncProfileId,
            "ExecutionRequested",
            "ExecutionId",
            null,
            executionId.ToString(),
            auditUserId,
            auditUserName,
            cancellationToken);

        return Result<CreateSyncProfileExecutionResultDto>.Success(
            new CreateSyncProfileExecutionResultDto(
                executionId,
                syncProfileId,
                "Pending",
                NormalizeExecutionType(request.ExecutionType),
                correlationId,
                clock.UtcNow),
            "Ejecucion de sincronizacion encolada.");
    }

    public async Task ProcessPendingAsync(CancellationToken cancellationToken = default)
    {
        var pending = await executionRepository.GetPendingAsync(5, cancellationToken);
        foreach (var execution in pending)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ProcessExecutionAsync(execution, cancellationToken);
        }
    }

    public async Task<Result<CancelSyncProfileExecutionResultDto>> CancelAsync(
        int executionId,
        string? requestedBy,
        CancellationToken cancellationToken = default)
    {
        var cancelled = await executionRepository.CancelAsync(executionId, requestedBy, cancellationToken);
        if (!cancelled)
        {
            return Failure<CancelSyncProfileExecutionResultDto>("SyncProfileExecutionNotFound", "La ejecucion no existe o ya finalizo.", nameof(executionId));
        }

        return Result<CancelSyncProfileExecutionResultDto>.Success(
            new CancelSyncProfileExecutionResultDto(executionId, "Cancelling", clock.UtcNow),
            "Cancelacion solicitada.");
    }

    public async Task<Result<RetrySyncProfileExecutionResultDto>> RetryAsync(
        int executionId,
        string? requestedBy,
        CancellationToken cancellationToken = default)
    {
        var original = await executionRepository.GetByIdAsync(executionId, cancellationToken);
        if (original is null)
        {
            return Failure<RetrySyncProfileExecutionResultDto>("SyncProfileExecutionNotFound", "La ejecucion no existe.", nameof(executionId));
        }

        var request = new SyncProfileExecutionRequest
        {
            ExecutionType = "Retry",
            RequestedBy = requestedBy ?? original.RequestedBy ?? "System",
            EntityCodes = DeserializeEntityCodes(original.EntityCodesJson),
            FromKey = original.FromKey,
            MaxRecords = original.MaxRecords
        };

        var retry = await RequestExecutionAsync(original.SyncProfileId, request, null, requestedBy, cancellationToken);
        if (!retry.IsSuccess || retry.Value is null)
        {
            return Result<RetrySyncProfileExecutionResultDto>.Failure(retry.Message, retry.Errors);
        }

        return Result<RetrySyncProfileExecutionResultDto>.Success(
            new RetrySyncProfileExecutionResultDto(original.Id, retry.Value.ExecutionId, retry.Value.Status, retry.Value.CorrelationId),
            "Reintento encolado.");
    }

    private async Task ProcessExecutionAsync(
        SyncProfileExecutionDetailDto execution,
        CancellationToken cancellationToken)
    {
        if (!await executionRepository.StartAsync(execution.Id, cancellationToken))
        {
            return;
        }

        var profile = await profileRepository.GetByIdAsync(execution.SyncProfileId, cancellationToken);
        if (profile is null)
        {
            await executionRepository.CompleteAsync(
                new CompleteSyncProfileExecutionData(execution.Id, "Failed", 0, 0, 0, 1, "El perfil ya no existe."),
                cancellationToken);
            return;
        }

        var requestedCodes = NormalizeEntityCodes(DeserializeEntityCodes(execution.EntityCodesJson));
        var catalog = await entityCatalogService.GetAsync(true, cancellationToken: cancellationToken);
        var entities = GetExecutableEntities(profile, requestedCodes, entitySourcesByCode.Keys, catalog);
        var totalRead = 0;
        var totalPublished = 0;
        var totalSkipped = 0;
        var totalErrors = 0;
        var failed = false;

        foreach (var entity in entities)
        {
            var detailStatus = "Completed";
            var detailRead = 0;
            var detailPublished = 0;
            var detailSkipped = 0;
            var detailErrors = 0;
            var lastKey = execution.FromKey;
            string? message = null;

            if (!entitySourcesByCode.TryGetValue(entity.EntityCode, out var source))
            {
                detailStatus = "Skipped";
                detailSkipped++;
                message = "La entidad no tiene lector Full registrado.";
            }
            else
            {
                try
                {
                    var pageSize = Math.Clamp(entity.BatchSize ?? profile.BatchSize, 1, 10000);
                    var hasMore = true;

                    while (hasMore && !cancellationToken.IsCancellationRequested)
                    {
                        var current = await executionRepository.GetByIdAsync(execution.Id, cancellationToken);
                        if (current is not null && string.Equals(current.Status, "Cancelling", StringComparison.OrdinalIgnoreCase))
                        {
                            detailStatus = "Cancelled";
                            message = "Ejecucion cancelada por solicitud administrativa.";
                            break;
                        }

                        var remaining = execution.MaxRecords.HasValue
                            ? Math.Max(execution.MaxRecords.Value - totalRead, 0)
                            : (int?)null;
                        if (remaining == 0)
                        {
                            hasMore = false;
                            break;
                        }

                        var page = await source.ReadPageAsync(
                            new SyncSourceReadContext(profile.CompanyId, lastKey, pageSize, remaining),
                            cancellationToken);

                        foreach (var record in page.Records)
                        {
                            var operation = record.IsActive ? SyncOperation.Updated : SyncOperation.Disabled;
                            var publish = await eventPublisher.PublishAsync(
                                new SyncPublishRequest(
                                    profile.CompanyId,
                                    entity.EntityCode,
                                    record.GlobalId,
                                    record.EntityKey,
                                    operation,
                                    record.Payload,
                                    "SyncProfileExecution",
                                    execution.CorrelationId,
                                    profile.Id,
                                    execution.CorrelationId,
                                    record.TargetBranchCode,
                                    record.RequireTargetBranchMatch),
                                cancellationToken);

                            detailRead++;
                            if (publish.IsSuccess && publish.Value?.Published == true)
                            {
                                detailPublished++;
                            }
                            else
                            {
                                detailSkipped++;
                            }

                            lastKey = record.EntityKey;
                        }

                        hasMore = page.HasMore;
                        if (!string.IsNullOrWhiteSpace(page.LastKey))
                        {
                            lastKey = page.LastKey;
                        }

                        await executionRepository.UpsertDetailAsync(
                            new SyncProfileExecutionDetailUpdate(
                                execution.Id,
                                entity.Id,
                                entity.EntityCode,
                                detailStatus,
                                detailRead,
                                detailPublished,
                                detailSkipped,
                                detailErrors,
                                lastKey,
                                message),
                            cancellationToken);
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    detailStatus = "Failed";
                    detailErrors++;
                    message = ex.Message;
                    failed = true;
                    if (!entity.ContinueOnError)
                    {
                        await executionRepository.UpsertDetailAsync(
                            new SyncProfileExecutionDetailUpdate(execution.Id, entity.Id, entity.EntityCode, detailStatus, detailRead, detailPublished, detailSkipped, detailErrors, lastKey, message),
                            cancellationToken);
                        break;
                    }
                }
            }

            totalRead += detailRead;
            totalPublished += detailPublished;
            totalSkipped += detailSkipped;
            totalErrors += detailErrors;

            await executionRepository.UpsertDetailAsync(
                new SyncProfileExecutionDetailUpdate(
                    execution.Id,
                    entity.Id,
                    entity.EntityCode,
                    detailStatus,
                    detailRead,
                    detailPublished,
                    detailSkipped,
                    detailErrors,
                    lastKey,
                    message),
                cancellationToken);
        }

        var latest = await executionRepository.GetByIdAsync(execution.Id, cancellationToken);
        var status = latest is not null && string.Equals(latest.Status, "Cancelling", StringComparison.OrdinalIgnoreCase)
            ? "Cancelled"
            : failed || totalErrors > 0
                ? "CompletedWithErrors"
                : "Completed";

        await executionRepository.CompleteAsync(
            new CompleteSyncProfileExecutionData(
                execution.Id,
                status,
                totalRead,
                totalPublished,
                totalSkipped,
                totalErrors,
                status == "Completed" ? null : "La ejecucion finalizo con incidencias."),
            cancellationToken);
    }

    private static IReadOnlyCollection<SyncProfileEntityRecord> GetExecutableEntities(
        SyncProfileDetailDto profile,
        IReadOnlyCollection<string> requestedCodes,
        IEnumerable<string> registeredSourceCodes,
        IReadOnlyCollection<NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos.SyncEntityDefinitionLookupDto> catalog)
    {
        var operativeEntities = profile.Entities
            .Where(entity => SyncMasterBranchEntityCodes.IsOperative(entity.EntityCode))
            .ToArray();

        return SyncEntityDependencyPlanner.Plan(
            operativeEntities,
            requestedCodes,
            registeredSourceCodes,
            catalog);
    }

    private static IReadOnlyCollection<string> NormalizeEntityCodes(IReadOnlyCollection<string>? entityCodes)
    {
        return entityCodes?
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
            ?? Array.Empty<string>();
    }

    private static IReadOnlyCollection<string>? DeserializeEntityCodes(string? entityCodesJson)
    {
        return string.IsNullOrWhiteSpace(entityCodesJson)
            ? null
            : JsonSerializer.Deserialize<IReadOnlyCollection<string>>(entityCodesJson, JsonOptions);
    }

    private static string NormalizeExecutionType(string? executionType)
    {
        return string.IsNullOrWhiteSpace(executionType) ? "Manual" : executionType.Trim();
    }

    private static Result<T> Failure<T>(string code, string message, string? field)
    {
        return Result<T>.Failure(message, [new ApiError(code, message, field)]);
    }
}
