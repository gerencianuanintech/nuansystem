using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.General.Countries.Queries;

public sealed class SearchCountriesQueryValidator : AbstractValidator<SearchCountriesQuery>
{
    public SearchCountriesQueryValidator()
    {
        RuleFor(query => query.Search)
            .MaximumLength(120).WithErrorCode("GEOGRAPHY_COUNTRY_SEARCH_MAX_LENGTH");
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithErrorCode("GEOGRAPHY_COUNTRY_PAGE_NUMBER_INVALID");
        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithErrorCode("GEOGRAPHY_COUNTRY_PAGE_SIZE_INVALID");
    }
}
