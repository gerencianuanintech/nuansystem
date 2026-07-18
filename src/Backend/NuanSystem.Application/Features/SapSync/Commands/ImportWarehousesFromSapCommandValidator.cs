using FluentValidation;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class ImportWarehousesFromSapCommandValidator : AbstractValidator<ImportWarehousesFromSapCommand>
{
    public ImportWarehousesFromSapCommandValidator()
    {
        RuleFor(command => command.Mappings)
            .NotNull()
            .Must(HaveUniqueSapCodes)
            .WithMessage("No puede repetir el codigo de una bodega SAP en la matriz de distribucion.");

        RuleForEach(command => command.Mappings).ChildRules(mapping =>
        {
            mapping.RuleFor(item => item.SapWarehouseCode)
                .NotEmpty()
                .MaximumLength(100);

            mapping.RuleFor(item => item.BranchCode)
                .NotEmpty()
                .MaximumLength(50);
        });
    }

    private static bool HaveUniqueSapCodes(IReadOnlyCollection<Dtos.SapWarehouseBranchMappingDto>? mappings)
        => mappings is null
           || mappings
               .Select(item => item.SapWarehouseCode?.Trim())
               .Where(code => !string.IsNullOrWhiteSpace(code))
               .Distinct(StringComparer.OrdinalIgnoreCase)
               .Count() == mappings.Count(item => !string.IsNullOrWhiteSpace(item.SapWarehouseCode));
}
