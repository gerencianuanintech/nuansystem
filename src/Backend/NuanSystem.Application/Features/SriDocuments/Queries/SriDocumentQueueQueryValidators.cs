using FluentValidation;

namespace NuanSystem.Application.Features.SriDocuments.Queries;

public sealed class GetSriDocumentQueueQueryValidator : AbstractValidator<GetSriDocumentQueueQuery>
{
    public GetSriDocumentQueueQueryValidator()
    {
        RuleFor(x => x.Filter.Environment).Must(value => string.IsNullOrWhiteSpace(value) || SriEnvironmentCodes.IsValid(value)).WithErrorCode("SRI_ENVIRONMENT_INVALID");
        RuleFor(x => x.Filter.Status).Must(SriDocumentQueueStatusCodes.IsValid).WithErrorCode("SRI_STATUS_INVALID");
        RuleFor(x => x.Filter.SourceType).Must(value => string.IsNullOrWhiteSpace(value) || SriSourceTypeCodes.IsValid(value)).WithErrorCode("SRI_SOURCE_TYPE_INVALID");
        RuleFor(x => x.Filter.AccessKey).Must(value => string.IsNullOrWhiteSpace(value) || SriAccessKey.HasValidFormat(value)).WithErrorCode("SRI_ACCESS_KEY_FORMAT");
        RuleFor(x => x.Filter.Page).GreaterThan(0).WithErrorCode("SRI_PAGE_INVALID");
        RuleFor(x => x.Filter.PageSize).InclusiveBetween(1, 500).WithErrorCode("SRI_PAGE_SIZE_INVALID");
    }
}

public sealed class GetSriDocumentQueueByIdQueryValidator : AbstractValidator<GetSriDocumentQueueByIdQuery>
{
    public GetSriDocumentQueueByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan(0).WithErrorCode("SRI_QUEUE_ID_INVALID");
}

public sealed class GetSriDocumentAttemptsQueryValidator : AbstractValidator<GetSriDocumentAttemptsQuery>
{
    public GetSriDocumentAttemptsQueryValidator() => RuleFor(x => x.QueueId).GreaterThan(0).WithErrorCode("SRI_QUEUE_ID_INVALID");
}
