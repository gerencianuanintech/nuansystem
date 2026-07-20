using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Carriers.Commands;

public sealed class CreateCarrierCommandHandler(ICarrierRepository repository) : ICommandHandler<CreateCarrierCommand, CarrierDetailDto>
{
    public async Task<Result<CarrierDetailDto>> Handle(CreateCarrierCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        if (await repository.ExistsByCodeAsync(code, cancellationToken: cancellationToken))
        {
            return DuplicateCode(request.Code);
        }

        var createResult = await repository.CreateAsync(new CreateCarrierData(
            code,
            request.Name.Trim(),
            request.IdentificationTypeCode.Trim(),
            request.IdentificationNumber.Trim(),
            NormalizeOptional(request.Description),
            request.IsActive,
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName)), cancellationToken);

        if (createResult.DuplicateCode)
        {
            return DuplicateCode(request.Code);
        }

        var id = createResult.Id
            ?? throw new InvalidOperationException("El transportista fue creado sin devolver un identificador valido.");

        var created = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El transportista fue creado pero no pudo consultarse.");

        return Result<CarrierDetailDto>.Success(created, "Transportista creado correctamente.");
    }

    internal static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    internal static Result<CarrierDetailDto> DuplicateCode(string code) =>
        Result<CarrierDetailDto>.Failure(
            "Ya existe un transportista con el codigo indicado.",
            [new ApiError("CARRIER_DUPLICATED_CODE", $"El codigo '{code.Trim()}' ya existe.", "Code")]);
}

public sealed class UpdateCarrierCommandHandler(ICarrierRepository repository) : ICommandHandler<UpdateCarrierCommand, CarrierDetailDto>
{
    public async Task<Result<CarrierDetailDto>> Handle(UpdateCarrierCommand request, CancellationToken cancellationToken)
    {
        if (await repository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return NotFound(request.Id);
        }

        var code = CreateCarrierCommandHandler.NormalizeCode(request.Code);
        if (await repository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return CreateCarrierCommandHandler.DuplicateCode(request.Code);
        }

        var updateResult = await repository.UpdateAsync(new UpdateCarrierData(
            request.Id,
            code,
            request.Name.Trim(),
            request.IdentificationTypeCode.Trim(),
            request.IdentificationNumber.Trim(),
            CreateCarrierCommandHandler.NormalizeOptional(request.Description),
            request.IsActive,
            request.AuditUserId,
            CreateCarrierCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);

        if (updateResult.DuplicateCode)
        {
            return CreateCarrierCommandHandler.DuplicateCode(request.Code);
        }

        if (!updateResult.Updated)
        {
            return NotFound(request.Id);
        }

        var carrier = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El transportista fue actualizado pero no pudo consultarse.");

        return Result<CarrierDetailDto>.Success(carrier, "Transportista actualizado correctamente.");
    }

    internal static Result<CarrierDetailDto> NotFound(int id) =>
        Result<CarrierDetailDto>.Failure(
            "No se encontro el transportista.",
            [new ApiError("CARRIER_NOT_FOUND", $"El transportista {id} no existe o fue eliminado.", "Id")]);
}

public sealed class DeleteCarrierCommandHandler(ICarrierRepository repository) : ICommandHandler<DeleteCarrierCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteCarrierCommand request, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(
            new DeleteCarrierData(request.Id, request.AuditUserId, CreateCarrierCommandHandler.NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Transportista eliminado correctamente.")
            : Result<bool>.Failure("No se pudo eliminar el transportista.", [new ApiError("CARRIER_NOT_FOUND", "El transportista no existe o fue eliminado.", "Id")]);
    }
}
