using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Profiles.Services;

namespace NuanSystem.Application.Features.SapSync.Profiles.Commands;

public sealed class CreateSapSyncProfileCommandHandler(
    ISapSyncProfileRepository repository,
    ISapSyncProfileValidationService validationService)
    : ICommandHandler<CreateSapSyncProfileCommand, SapSyncProfileWriteDto>
{
    public async Task<Result<SapSyncProfileWriteDto>> Handle(
        CreateSapSyncProfileCommand request,
        CancellationToken cancellationToken)
    {
        var aggregate = await validationService.BuildAggregateAsync(
            id: null,
            request.Profile,
            request.UserId,
            request.AuditUserId,
            request.AuditUserName,
            profileIsActive: false,
            forceChildrenInactive: true,
            rowVersion: null,
            cancellationToken);
        if (!aggregate.IsSuccess)
        {
            return SapSyncProfileResults.FromFailure<SapSyncProfileAggregate, SapSyncProfileWriteDto>(aggregate);
        }

        var writeResult = await repository.CreateAsync(aggregate.Value!, cancellationToken);
        return SapSyncProfileResults.MapWrite(
            writeResult,
            result => new SapSyncProfileWriteDto(
                result.Id!.Value,
                IsActive: false,
                result.RowVersion ?? []),
            "Perfil SAP creado inactivo correctamente.");
    }
}

public sealed class UpdateSapSyncProfileCommandHandler(
    ISapSyncProfileRepository repository,
    ISapSyncProfileValidationService validationService)
    : ICommandHandler<UpdateSapSyncProfileCommand, SapSyncProfileWriteDto>
{
    public async Task<Result<SapSyncProfileWriteDto>> Handle(
        UpdateSapSyncProfileCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return SapSyncProfileResults.NotFound<SapSyncProfileWriteDto>(request.Id);
        }

        var currentAccess = await validationService.ValidateCompanyAsync(
            existing.CompanyId,
            request.UserId,
            requireSapReady: false,
            cancellationToken);
        if (!currentAccess.IsSuccess)
        {
            return SapSyncProfileResults.FromFailure<SapSyncProfileCompanyAccessDto, SapSyncProfileWriteDto>(
                currentAccess);
        }

        if (existing.CompanyId != request.Request.Profile.CompanyId)
        {
            return SapSyncProfileResults.CompanyImmutable<SapSyncProfileWriteDto>(request.Id);
        }

        var aggregate = await validationService.BuildAggregateAsync(
            request.Id,
            request.Request.Profile,
            request.UserId,
            request.AuditUserId,
            request.AuditUserName,
            existing.IsActive,
            forceChildrenInactive: false,
            request.Request.RowVersion,
            cancellationToken);
        if (!aggregate.IsSuccess)
        {
            return SapSyncProfileResults.FromFailure<SapSyncProfileAggregate, SapSyncProfileWriteDto>(aggregate);
        }

        var writeResult = await repository.UpdateAsync(aggregate.Value!, cancellationToken);
        return SapSyncProfileResults.MapWrite(
            writeResult,
            result => new SapSyncProfileWriteDto(
                result.Id!.Value,
                existing.IsActive,
                result.RowVersion ?? []),
            "Perfil SAP actualizado correctamente.");
    }
}

public sealed class DeleteSapSyncProfileCommandHandler(
    ISapSyncProfileRepository repository,
    ISapSyncProfileValidationService validationService)
    : ICommandHandler<DeleteSapSyncProfileCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteSapSyncProfileCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return SapSyncProfileResults.NotFound<bool>(request.Id);
        }

        var access = await validationService.ValidateCompanyAsync(
            existing.CompanyId,
            request.UserId,
            requireSapReady: false,
            cancellationToken);
        if (!access.IsSuccess)
        {
            return SapSyncProfileResults.FromFailure<SapSyncProfileCompanyAccessDto, bool>(access);
        }

        var writeResult = await repository.DeleteAsync(
            request.Id,
            request.RowVersion,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
        return SapSyncProfileResults.MapWrite(
            writeResult,
            _ => true,
            "Perfil SAP eliminado logicamente.");
    }
}

public sealed class ValidateSapSyncProfileCommandHandler(
    ISapSyncProfileRepository repository,
    ISapSyncProfileValidationService validationService)
    : ICommandHandler<ValidateSapSyncProfileCommand, SapSyncProfileValidationResultDto>
{
    public async Task<Result<SapSyncProfileValidationResultDto>> Handle(
        ValidateSapSyncProfileCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return SapSyncProfileResults.NotFound<SapSyncProfileValidationResultDto>(request.Id);
        }

        var access = await validationService.ValidateCompanyAsync(
            existing.CompanyId,
            request.UserId,
            requireSapReady: false,
            cancellationToken);
        if (!access.IsSuccess)
        {
            return SapSyncProfileResults.FromFailure<
                SapSyncProfileCompanyAccessDto,
                SapSyncProfileValidationResultDto>(access);
        }

        var validation = await validationService.ValidateAsync(
            existing.ToSaveRequest(),
            request.UserId,
            requireActiveEntity: true,
            cancellationToken);
        return Result<SapSyncProfileValidationResultDto>.Success(
            validation,
            validation.IsValid
                ? "El perfil SAP es valido para activacion."
                : "El perfil SAP requiere correcciones antes de activarse.");
    }
}

public sealed class ActivateSapSyncProfileCommandHandler(
    ISapSyncProfileRepository repository,
    ISapSyncProfileValidationService validationService)
    : ICommandHandler<ActivateSapSyncProfileCommand, SapSyncProfileWriteDto>
{
    public async Task<Result<SapSyncProfileWriteDto>> Handle(
        ActivateSapSyncProfileCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return SapSyncProfileResults.NotFound<SapSyncProfileWriteDto>(request.Id);
        }

        var access = await validationService.ValidateCompanyAsync(
            existing.CompanyId,
            request.UserId,
            requireSapReady: true,
            cancellationToken);
        if (!access.IsSuccess)
        {
            return SapSyncProfileResults.FromFailure<SapSyncProfileCompanyAccessDto, SapSyncProfileWriteDto>(
                access);
        }

        var validation = await validationService.ValidateAsync(
            existing.ToSaveRequest(),
            request.UserId,
            requireActiveEntity: true,
            cancellationToken);
        if (!validation.IsValid)
        {
            return Result<SapSyncProfileWriteDto>.Failure(
                "El perfil SAP no puede activarse.",
                validation.Errors.Select(error =>
                    new NuanSystem.Shared.Responses.ApiError(error.Code, error.Message, error.Field)).ToArray());
        }

        var writeResult = await repository.SetActiveAsync(
            request.Id,
            isActive: true,
            request.RowVersion,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
        return SapSyncProfileResults.MapWrite(
            writeResult,
            result => new SapSyncProfileWriteDto(
                result.Id!.Value,
                IsActive: true,
                result.RowVersion ?? []),
            "Perfil SAP activado correctamente.");
    }
}

public sealed class DeactivateSapSyncProfileCommandHandler(
    ISapSyncProfileRepository repository,
    ISapSyncProfileValidationService validationService)
    : ICommandHandler<DeactivateSapSyncProfileCommand, SapSyncProfileWriteDto>
{
    public async Task<Result<SapSyncProfileWriteDto>> Handle(
        DeactivateSapSyncProfileCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return SapSyncProfileResults.NotFound<SapSyncProfileWriteDto>(request.Id);
        }

        var access = await validationService.ValidateCompanyAsync(
            existing.CompanyId,
            request.UserId,
            requireSapReady: false,
            cancellationToken);
        if (!access.IsSuccess)
        {
            return SapSyncProfileResults.FromFailure<SapSyncProfileCompanyAccessDto, SapSyncProfileWriteDto>(
                access);
        }

        var writeResult = await repository.SetActiveAsync(
            request.Id,
            isActive: false,
            request.RowVersion,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
        return SapSyncProfileResults.MapWrite(
            writeResult,
            result => new SapSyncProfileWriteDto(
                result.Id!.Value,
                IsActive: false,
                result.RowVersion ?? []),
            "Perfil SAP desactivado; solo se impiden disparos futuros.");
    }
}
