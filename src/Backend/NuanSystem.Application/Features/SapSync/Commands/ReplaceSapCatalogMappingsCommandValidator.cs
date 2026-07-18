using FluentValidation;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class ReplaceSapCatalogMappingsCommandValidator : AbstractValidator<ReplaceSapCatalogMappingsCommand>
{
    public ReplaceSapCatalogMappingsCommandValidator()
    {
        RuleFor(x => x.Mappings).NotNull().Must(x => x.Count <= 5000).WithMessage("Puede configurar hasta 5000 equivalencias.");
        RuleForEach(x => x.Mappings).ChildRules(row =>
        {
            row.RuleFor(x => x.MappingType).NotEmpty().Must(SapCatalogMappingTypes.All.Contains).WithMessage("Tipo de equivalencia no soportado.");
            row.RuleFor(x => x.SapCode).NotEmpty().MaximumLength(120);
            row.RuleFor(x => x.NuanCode).NotEmpty().MaximumLength(120);
        });
        RuleFor(x => x.Mappings).Must(rows => rows
            .Select(row => $"{row.MappingType.Trim()}|{row.SapCode.Trim()}")
            .Distinct(StringComparer.OrdinalIgnoreCase).Count() == rows.Count)
            .WithMessage("Existen equivalencias SAP duplicadas para el mismo tipo.");
    }
}
