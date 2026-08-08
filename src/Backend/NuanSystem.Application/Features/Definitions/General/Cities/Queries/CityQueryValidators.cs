using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.General.Cities.Queries;

public sealed class SearchCitiesQueryValidator : AbstractValidator<SearchCitiesQuery>
{
    public SearchCitiesQueryValidator()
    {
        RuleFor(query => query.Search)
            .MaximumLength(120).WithErrorCode("GEOGRAPHY_CITY_SEARCH_MAX_LENGTH");
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithErrorCode("GEOGRAPHY_CITY_PAGE_NUMBER_INVALID");
        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithErrorCode("GEOGRAPHY_CITY_PAGE_SIZE_INVALID");
    }
}
