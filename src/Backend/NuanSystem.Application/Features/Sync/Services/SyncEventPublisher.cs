using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Services;

public sealed class SyncEventPublisher(
    IReplicableEntityMetadataProvider metadataProvider,
    ISyncEventPayloadFactory payloadFactory,
    ISyncRoutingService routingService,
    ISyncOutboxRepository outboxRepository) : ISyncEventPublisher
{
    public async Task<Result<SyncPublishResult>> PublishAsync(
        SyncPublishRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.EntityGlobalId == Guid.Empty)
        {
            return Result<SyncPublishResult>.Failure("El evento de sincronizacion requiere GlobalId.");
        }

        if (string.IsNullOrWhiteSpace(request.EntityName))
        {
            return Result<SyncPublishResult>.Failure("El evento de sincronizacion requiere entidad.");
        }

        var metadata = await metadataProvider.GetAsync(request.CompanyId, request.EntityName.Trim(), cancellationToken);
        var skipReason = GetSkipReason(metadata, request.SyncProfileId.HasValue);
        if (skipReason is not null)
        {
            return Result<SyncPublishResult>.Success(new SyncPublishResult(false, null, skipReason));
        }

        var payloadJson = payloadFactory.CreatePayloadJson(request);
        var entityName = request.EntityName.Trim();
        var outboxId = await outboxRepository.CreateAsync(
            new CreateSyncOutboxEventData(
                Guid.NewGuid(),
                request.CompanyId,
                entityName,
                request.EntityGlobalId,
                request.EntityCode,
                request.Operation,
                payloadJson,
                request.SourceSystem,
                request.SourceReference,
                metadata.MaxAttemptsOrDefault()),
            cancellationToken);

        var routingEvaluation = await routingService.ResolveTargetsAsync(
            new SyncRoutingContext(
                request.CompanyId,
                entityName,
                request.SyncProfileId),
            cancellationToken);

        var targetCount = 0;
        foreach (var target in routingEvaluation.Targets.GroupBy(target => target.BranchCompanyId).Select(group => group.First()))
        {
            await outboxRepository.CreateTargetAsync(
                new CreateSyncOutboxTargetData(
                    outboxId,
                    target.BranchCompanyId,
                    MaxAttemptsFromRetries(target.MaxRetries)),
                cancellationToken);
            targetCount++;
        }

        var message = targetCount == 0
            ? $"Evento SyncOutbox publicado sin targets: {routingEvaluation.Reason ?? "no hay perfiles activos aplicables."}"
            : $"Evento SyncOutbox publicado con {targetCount} target(s).";

        return Result<SyncPublishResult>.Success(
            new SyncPublishResult(true, outboxId, message));
    }

    private static int MaxAttemptsFromRetries(int maxRetries) => Math.Clamp(maxRetries + 1, 1, 11);

    private static string? GetSkipReason(ReplicableEntityMetadata metadata, bool isAdministrativeExecution)
    {
        if (!metadata.SyncEnabled)
        {
            return "La empresa no tiene sincronizacion habilitada.";
        }

        if (!metadata.IsMaster)
        {
            return "La empresa activa no es Master para publicar MasterToBranch.";
        }

        if (isAdministrativeExecution)
        {
            return null;
        }

        if (!metadata.IsConfigured)
        {
            return "La entidad no tiene configuracion de sincronizacion.";
        }

        if (!metadata.IsEnabled)
        {
            return "La configuracion de sincronizacion de la entidad esta deshabilitada.";
        }

        if (metadata.Direction is not (SyncDirection.MasterToBranch or SyncDirection.Bidirectional))
        {
            return "La direccion configurada no permite publicar MasterToBranch.";
        }

        return null;
    }
}

file static class ReplicableEntityMetadataExtensions
{
    public static int MaxAttemptsOrDefault(this ReplicableEntityMetadata _) => 3;
}
