using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Mapping;

public sealed record GetSapQueryDefinitionsQuery(
    string? EntityCode,
    string? Transport,
    bool ActiveOnly,
    int UserId) : IQuery<IReadOnlyCollection<SapQueryDefinitionDto>>;

public sealed class GetSapQueryDefinitionsQueryHandler(ISapMasterDataMappingRepository repository)
    : IQueryHandler<GetSapQueryDefinitionsQuery, IReadOnlyCollection<SapQueryDefinitionDto>>
{
    public async Task<Result<IReadOnlyCollection<SapQueryDefinitionDto>>> Handle(
        GetSapQueryDefinitionsQuery request,
        CancellationToken cancellationToken)
        => Result<IReadOnlyCollection<SapQueryDefinitionDto>>.Success(
            await repository.GetQueryDefinitionsAsync(request.EntityCode, request.Transport, request.ActiveOnly, cancellationToken));
}

public sealed record GetSapFieldMappingsQuery(
    long ProfileId,
    long ProfileEntityId,
    SapMappingDirection? Direction,
    bool ActiveOnly,
    int UserId) : IQuery<IReadOnlyCollection<SapFieldMappingDto>>;

public sealed class GetSapFieldMappingsQueryHandler(
    ISapSyncProfileRepository profileRepository,
    ISapMasterDataMappingRepository mappingRepository)
    : IQueryHandler<GetSapFieldMappingsQuery, IReadOnlyCollection<SapFieldMappingDto>>
{
    public async Task<Result<IReadOnlyCollection<SapFieldMappingDto>>> Handle(
        GetSapFieldMappingsQuery request,
        CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.ProfileId, cancellationToken);
        if (profile is null)
        {
            return Result<IReadOnlyCollection<SapFieldMappingDto>>.Failure(
                "El perfil SAP no existe.",
                [new ApiError("SAP_PROFILE_NOT_FOUND", "El perfil SAP no existe.", nameof(request.ProfileId))]);
        }

        if (!await SapMappingAuthorization.HasCompanyAccessAsync(profile.CompanyId, request.UserId, profileRepository, cancellationToken))
            return Result<IReadOnlyCollection<SapFieldMappingDto>>.Failure("El usuario no tiene acceso al perfil SAP.", [new ApiError("SAP_PROFILE_ACCESS_DENIED", "El usuario no tiene acceso al perfil SAP.")]);

        var entity = profile.Entities.SingleOrDefault(item => item.Id == request.ProfileEntityId);
        if (entity is null)
        {
            return Result<IReadOnlyCollection<SapFieldMappingDto>>.Failure(
                "La entidad del perfil SAP no existe.",
                [new ApiError("SAP_PROFILE_ENTITY_NOT_FOUND", "La entidad del perfil SAP no existe.", nameof(request.ProfileEntityId))]);
        }

        return Result<IReadOnlyCollection<SapFieldMappingDto>>.Success(
            await mappingRepository.GetFieldMappingsAsync(request.ProfileEntityId, request.Direction, request.ActiveOnly, cancellationToken));
    }
}

public sealed record ReplaceSapFieldMappingsCommand(
    long ProfileId,
    long ProfileEntityId,
    IReadOnlyCollection<SapFieldMappingWriteModel> Mappings,
    int UserId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SapMappingWriteResult>;

public sealed class ReplaceSapFieldMappingsCommandHandler(
    ISapSyncProfileRepository profileRepository,
    ISapMasterDataMappingRepository mappingRepository)
    : ICommandHandler<ReplaceSapFieldMappingsCommand, SapMappingWriteResult>
{
    public async Task<Result<SapMappingWriteResult>> Handle(
        ReplaceSapFieldMappingsCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.ProfileId, cancellationToken);
        if (profile is null)
        {
            return Result<SapMappingWriteResult>.Failure(
                "El perfil SAP no existe.",
                [new ApiError("SAP_PROFILE_NOT_FOUND", "El perfil SAP no existe.", nameof(request.ProfileId))]);
        }

        if (!await SapMappingAuthorization.HasCompanyAccessAsync(profile.CompanyId, request.UserId, profileRepository, cancellationToken))
            return Result<SapMappingWriteResult>.Failure("El usuario no tiene acceso al perfil SAP.", [new ApiError("SAP_PROFILE_ACCESS_DENIED", "El usuario no tiene acceso al perfil SAP.")]);

        if (profile.Entities.All(item => item.Id != request.ProfileEntityId))
        {
            return Result<SapMappingWriteResult>.Failure(
                "La entidad del perfil SAP no existe.",
                [new ApiError("SAP_PROFILE_ENTITY_NOT_FOUND", "La entidad del perfil SAP no existe.", nameof(request.ProfileEntityId))]);
        }

        var definitions = request.Mappings.Select(mapping => new SapFieldMappingDefinition(
            mapping.SapField,
            mapping.NuanField,
            mapping.Direction,
            mapping.IsRequired,
            mapping.IsReadOnly,
            mapping.TransformationKey)).ToArray();
        var validation = SapMappingValidationService.ValidateMappings(definitions);
        if (!validation.IsValid)
        {
            return Result<SapMappingWriteResult>.Failure(
                "Los mapeos SAP contienen datos inválidos.",
                validation.Errors.Select(error => new ApiError(error.Code, error.Message, error.Field)).ToArray());
        }

        var result = await mappingRepository.ReplaceFieldMappingsAsync(
            request.ProfileEntityId,
            request.Mappings,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
        return Result<SapMappingWriteResult>.Success(result, "Mapeos SAP guardados correctamente.");
    }
}

public sealed record CreateSapQueryDefinitionCommand(
    SapQueryDefinitionWriteModel Model,
    int UserId) : ICommand<SapMappingWriteResult>;

public sealed class CreateSapQueryDefinitionCommandHandler(ISapMasterDataMappingRepository repository)
    : ICommandHandler<CreateSapQueryDefinitionCommand, SapMappingWriteResult>
{
    public async Task<Result<SapMappingWriteResult>> Handle(
        CreateSapQueryDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        if (!SapMasterDataMappingContract.EntityCodes.Contains(request.Model.EntityCode, StringComparer.OrdinalIgnoreCase))
        {
            return Result<SapMappingWriteResult>.Failure(
                "La entidad SAP no está permitida.",
                [new ApiError(SapMappingValidationCodes.EntityUnknown, "La entidad SAP no está permitida.", nameof(request.Model.EntityCode))]);
        }

        if (!SapMappingValidationService.IsDataTypeAllowed("Text"))
        {
            return Result<SapMappingWriteResult>.Failure("La configuración SAP no es válida.");
        }

        var result = await repository.CreateQueryDefinitionAsync(request.Model, cancellationToken);
        return Result<SapMappingWriteResult>.Success(result, "Definición de consulta SAP creada correctamente.");
    }
}

public sealed record DeactivateSapQueryDefinitionCommand(
    long QueryDefinitionId,
    byte[] ExpectedRowVersion,
    int UserId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<SapMappingWriteResult>;

public sealed class DeactivateSapQueryDefinitionCommandHandler(ISapMasterDataMappingRepository repository)
    : ICommandHandler<DeactivateSapQueryDefinitionCommand, SapMappingWriteResult>
{
    public async Task<Result<SapMappingWriteResult>> Handle(
        DeactivateSapQueryDefinitionCommand request,
        CancellationToken cancellationToken)
    {
        var result = await repository.DeactivateQueryDefinitionAsync(
            request.QueryDefinitionId,
            request.ExpectedRowVersion,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
        return Result<SapMappingWriteResult>.Success(result, "Definición de consulta SAP desactivada.");
    }
}

public sealed record SapMappingPreviewDto(
    long ProfileId,
    long ProfileEntityId,
    string EntityCode,
    SapMappingDirection Direction,
    bool IsValid,
    IReadOnlyCollection<SapMappingValidationMessage> Errors,
    IReadOnlyCollection<SapFieldMappingDto> Mappings);

public sealed record PreviewSapEntityMappingQuery(
    long ProfileId,
    long ProfileEntityId,
    int UserId) : IQuery<SapMappingPreviewDto>;

public sealed class PreviewSapEntityMappingQueryHandler(
    ISapSyncProfileRepository profileRepository,
    ISapMasterDataMappingRepository mappingRepository)
    : IQueryHandler<PreviewSapEntityMappingQuery, SapMappingPreviewDto>
{
    public async Task<Result<SapMappingPreviewDto>> Handle(
        PreviewSapEntityMappingQuery request,
        CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.ProfileId, cancellationToken);
        if (profile is not null && !await SapMappingAuthorization.HasCompanyAccessAsync(profile.CompanyId, request.UserId, profileRepository, cancellationToken))
            return Result<SapMappingPreviewDto>.Failure("El usuario no tiene acceso al perfil SAP.", [new ApiError("SAP_PROFILE_ACCESS_DENIED", "El usuario no tiene acceso al perfil SAP.")]);
        var entity = profile?.Entities.SingleOrDefault(item => item.Id == request.ProfileEntityId);
        if (entity is null)
        {
            return Result<SapMappingPreviewDto>.Failure(
                "La entidad del perfil SAP no existe.",
                [new ApiError("SAP_PROFILE_ENTITY_NOT_FOUND", "La entidad del perfil SAP no existe.", nameof(request.ProfileEntityId))]);
        }

        var mappings = await mappingRepository.GetFieldMappingsAsync(
            request.ProfileEntityId,
            entity.Direction switch
            {
                SapSyncDirection.SapToErp => SapMappingDirection.SapToErp,
                SapSyncDirection.ErpToSap => SapMappingDirection.ErpToSap,
                _ => SapMappingDirection.Both
            },
            activeOnly: true,
            cancellationToken);
        var definitions = mappings.Select(mapping => new SapFieldMappingDefinition(
            mapping.SapField,
            mapping.NuanField,
            mapping.Direction,
            mapping.IsRequired,
            mapping.IsReadOnly,
            mapping.TransformationKey)).ToArray();
        var validation = SapMappingValidationService.ValidateActivation(
            new SapMappingActivationSnapshot(
                QueryApproved: true,
                QueryActive: true,
                entity.EntityCode,
                entity.Direction switch
                {
                    SapSyncDirection.SapToErp => SapMappingDirection.SapToErp,
                    SapSyncDirection.ErpToSap => SapMappingDirection.ErpToSap,
                    _ => SapMappingDirection.Both
                },
                ImportExecutable: true,
                ExportExecutable: false,
                definitions));

        return Result<SapMappingPreviewDto>.Success(
            new SapMappingPreviewDto(
                request.ProfileId,
                request.ProfileEntityId,
                entity.EntityCode,
                definitions.FirstOrDefault()?.Direction ?? SapMappingDirection.SapToErp,
                validation.IsValid,
                validation.Errors,
                mappings),
            "Vista previa de configuración generada correctamente.");
    }

    private static async Task<bool> HasCompanyAccessAsync(int companyId, int userId, ISapSyncProfileRepository repository, CancellationToken cancellationToken)
        => (await repository.GetCompanyAccessAsync(userId, companyId, cancellationToken)).Count > 0;
}

internal static class SapMappingAuthorization
{
    public static async Task<bool> HasCompanyAccessAsync(int companyId, int userId, ISapSyncProfileRepository repository, CancellationToken cancellationToken)
        => (await repository.GetCompanyAccessAsync(userId, companyId, cancellationToken)).Count > 0;
}
