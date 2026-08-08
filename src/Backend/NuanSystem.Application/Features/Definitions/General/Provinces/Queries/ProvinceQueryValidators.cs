using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.General.Provinces.Queries;

public sealed class SearchProvincesQueryValidator : AbstractValidator<SearchProvincesQuery>
{
    public SearchProvincesQueryValidator()
    {
        RuleFor(query => query.Search)
            .MaximumLength(120).WithErrorCode("GEOGRAPHY_PROVINCE_SEARCH_MAX_LENGTH");
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithErrorCode("GEOGRAPHY_PROVINCE_PAGE_NUMBER_INVALID");
        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithErrorCode("GEOGRAPHY_PROVINCE_PAGE_SIZE_INVALID");
    }
}
