using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Sync.EntityDefinitions.Commands;

public sealed class CreateSyncEntityDefinitionCommandHandler(ISyncEntityDefinitionRepository repository)
    : ICommandHandler<CreateSyncEntityDefinitionCommand, SyncEntityDefinitionDetailDto>
{
    public async Task<Result<SyncEntityDefinitionDetailDto>> Handle(
        CreateSyncEntityDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();
        if (await repository.GetByCodeAsync(code, cancellationToken) is not null)
        {
            return Failure(
                "SyncEntityDefinitionCodeAlreadyExists",
                "Ya existe una definicion con el codigo indicado.",
                nameof(request.Code));
        }

        var dependencyIds = NormalizeDependencies(request.DependencyDefinitionIds);
        if (await ValidateDependenciesAsync(repository, dependencyIds, null, cancellationToken) is { } dependencyError)
        {
            return dependencyError;
        }

        var mutation = await repository.CreateAsync(
            new CreateSyncEntityDefinitionData(
                code,
                request.Name.Trim(),
                NormalizeOptional(request.Description),
                request.DefaultExecutionOrder,
                request.SupportsIncremental,
                request.SupportsInsert,
                request.SupportsUpdate,
                request.SupportsDeactivate,
                NormalizeTechnicalField(request.DefaultKeyField),
                NormalizeTechnicalField(request.DefaultModifiedAtField),
                request.IsActive,
                request.AuditUserId,
                NormalizeOptional(request.AuditUserName),
                dependencyIds),
            cancellationToken);

        if (!mutation.Succeeded)
        {
            return SyncEntityDefinitionMutationErrorMapper.ToFailure<SyncEntityDefinitionDetailDto>(mutation.Error);
        }

        var definition = await repository.GetByIdAsync(mutation.Id!.Value, cancellationToken)
            ?? throw new InvalidOperationException("La definicion fue creada pero no pudo consultarse.");
        return Result<SyncEntityDefinitionDetailDto>.Success(
            SyncEntityDefinitionMapper.ToDetailDto(definition),
            "Definicion de entidad creada correctamente.");
    }

    internal static async Task<Result<SyncEntityDefinitionDetailDto>?> ValidateDependenciesAsync(
        ISyncEntityDefinitionRepository repository,
        IReadOnlyCollection<int> dependencyIds,
        int? currentId,
        CancellationToken cancellationToken)
    {
        if (currentId.HasValue && dependencyIds.Contains(currentId.Value))
        {
            return Failure(
                "SyncEntityDefinitionSelfDependency",
                "Una entidad no puede depender de si misma.",
                nameof(CreateSyncEntityDefinitionCommand.DependencyDefinitionIds));
        }

        if (dependencyIds.Count == 0)
        {
            return null;
        }

        var definitions = await repository.GetLookupAsync(null, true, cancellationToken);
        var activeIds = definitions
            .Where(definition => definition.Definition.IsActive)
            .Select(definition => definition.Definition.Id)
            .ToHashSet();

        return dependencyIds.Any(id => !activeIds.Contains(id))
            ? Failure(
                "SyncEntityDefinitionInvalidDependency",
                "Una dependencia no existe o esta inactiva.",
                nameof(CreateSyncEntityDefinitionCommand.DependencyDefinitionIds))
            : null;
    }

    internal static IReadOnlyCollection<int> NormalizeDependencies(IReadOnlyCollection<int> values)
    {
        return values.Distinct().Order().ToArray();
    }

    internal static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    internal static string? NormalizeTechnicalField(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static Result<SyncEntityDefinitionDetailDto> Failure(string code, string message, string field)
    {
        return Result<SyncEntityDefinitionDetailDto>.Failure(message, [new ApiError(code, message, field)]);
    }
}

public sealed class UpdateSyncEntityDefinitionCommandHandler(ISyncEntityDefinitionRepository repository)
    : ICommandHandler<UpdateSyncEntityDefinitionCommand, SyncEntityDefinitionDetailDto>
{
    public async Task<Result<SyncEntityDefinitionDetailDto>> Handle(
        UpdateSyncEntityDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        if (await repository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return SyncEntityDefinitionMutationErrorMapper.ToFailure<SyncEntityDefinitionDetailDto>(
                SyncEntityDefinitionMutationError.NotFound);
        }

        var dependencyIds = CreateSyncEntityDefinitionCommandHandler.NormalizeDependencies(request.DependencyDefinitionIds);
        if (await CreateSyncEntityDefinitionCommandHandler.ValidateDependenciesAsync(
                repository,
                dependencyIds,
                request.Id,
                cancellationToken) is { } dependencyError)
        {
            return dependencyError;
        }

        var mutation = await repository.UpdateAsync(
            new UpdateSyncEntityDefinitionData(
                request.Id,
                request.Name.Trim(),
                CreateSyncEntityDefinitionCommandHandler.NormalizeOptional(request.Description),
                request.DefaultExecutionOrder,
                request.SupportsIncremental,
                request.SupportsInsert,
                request.SupportsUpdate,
                request.SupportsDeactivate,
                CreateSyncEntityDefinitionCommandHandler.NormalizeTechnicalField(request.DefaultKeyField),
                CreateSyncEntityDefinitionCommandHandler.NormalizeTechnicalField(request.DefaultModifiedAtField),
                request.IsActive,
                request.AuditUserId,
                CreateSyncEntityDefinitionCommandHandler.NormalizeOptional(request.AuditUserName),
                dependencyIds),
            cancellationToken);

        if (!mutation.Succeeded)
        {
            return SyncEntityDefinitionMutationErrorMapper.ToFailure<SyncEntityDefinitionDetailDto>(mutation.Error);
        }

        var definition = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("La definicion fue actualizada pero no pudo consultarse.");
        return Result<SyncEntityDefinitionDetailDto>.Success(
            SyncEntityDefinitionMapper.ToDetailDto(definition),
            "Definicion de entidad actualizada correctamente.");
    }
}

public sealed class DeleteSyncEntityDefinitionCommandHandler(ISyncEntityDefinitionRepository repository)
    : ICommandHandler<DeleteSyncEntityDefinitionCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteSyncEntityDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return SyncEntityDefinitionMutationErrorMapper.ToFailure<bool>(SyncEntityDefinitionMutationError.NotFound);
        }

        if (existing.Definition.IsSystem)
        {
            return SyncEntityDefinitionMutationErrorMapper.ToFailure<bool>(SyncEntityDefinitionMutationError.SystemDefinition);
        }

        var mutation = await repository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            CreateSyncEntityDefinitionCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        return mutation.Succeeded
            ? Result<bool>.Success(true, "Definicion de entidad eliminada correctamente.")
            : SyncEntityDefinitionMutationErrorMapper.ToFailure<bool>(mutation.Error);
    }
}

internal static class SyncEntityDefinitionMutationErrorMapper
{
    public static Result<T> ToFailure<T>(SyncEntityDefinitionMutationError error)
    {
        var (code, message, field) = error switch
        {
            SyncEntityDefinitionMutationError.DuplicateCode => ("SyncEntityDefinitionCodeAlreadyExists", "Ya existe una definicion con el codigo indicado.", "Code"),
            SyncEntityDefinitionMutationError.InvalidDependency => ("SyncEntityDefinitionInvalidDependency", "Una dependencia no existe o esta inactiva.", "DependencyDefinitionIds"),
            SyncEntityDefinitionMutationError.DependencyCycle => ("SyncEntityDefinitionDependencyCycle", "Las dependencias forman un ciclo.", "DependencyDefinitionIds"),
            SyncEntityDefinitionMutationError.SystemDefinition => ("SyncEntityDefinitionSystemProtected", "Las definiciones del sistema no pueden eliminarse; puede desactivarlas.", "Id"),
            SyncEntityDefinitionMutationError.ReferencedByProfile => ("SyncEntityDefinitionInUse", "La definicion esta referenciada por uno o mas perfiles.", "Id"),
            SyncEntityDefinitionMutationError.RequiredByDefinition => ("SyncEntityDefinitionRequired", "La definicion es dependencia de otra entidad.", "Id"),
            SyncEntityDefinitionMutationError.InvalidData => ("SyncEntityDefinitionInvalidData", "Los datos de la definicion no son validos.", null),
            _ => ("SyncEntityDefinitionNotFound", "La definicion de entidad no existe.", "Id")
        };

        return Result<T>.Failure(message, [new ApiError(code, message, field)]);
    }
}
