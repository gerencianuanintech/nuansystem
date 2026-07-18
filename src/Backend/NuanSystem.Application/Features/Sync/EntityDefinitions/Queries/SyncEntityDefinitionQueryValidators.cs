using FluentValidation;

namespace NuanSystem.Application.Features.Sync.EntityDefinitions.Queries;

public sealed class GetSyncEntityDefinitionsQueryValidator : AbstractValidator<GetSyncEntityDefinitionsQuery>
{
    public GetSyncEntityDefinitionsQueryValidator()
    {
        RuleFor(query => query.Search).MaximumLength(200);
        RuleFor(query => query.PageNumber).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 500);
    }
}

public sealed class GetSyncEntityDefinitionByIdQueryValidator : AbstractValidator<GetSyncEntityDefinitionByIdQuery>
{
    public GetSyncEntityDefinitionByIdQueryValidator()
    {
        RuleFor(query => query.Id).GreaterThan(0);
    }
}

public sealed class GetSyncEntityDefinitionLookupQueryValidator : AbstractValidator<GetSyncEntityDefinitionLookupQuery>
{
    public GetSyncEntityDefinitionLookupQueryValidator()
    {
        RuleFor(query => query.IncludeId).GreaterThan(0).When(query => query.IncludeId.HasValue);
    }
}
