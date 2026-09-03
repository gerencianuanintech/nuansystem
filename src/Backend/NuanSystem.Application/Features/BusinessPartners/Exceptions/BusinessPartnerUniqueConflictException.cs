namespace NuanSystem.Application.Features.BusinessPartners.Exceptions;

public enum BusinessPartnerUniqueConflictKind
{
    Identification,
    Code,
    SapCardCode
}

public sealed class BusinessPartnerUniqueConflictException(
    BusinessPartnerUniqueConflictKind kind,
    Exception? innerException = null)
    : Exception("A BusinessPartner unique value conflicts with another record.", innerException)
{
    public BusinessPartnerUniqueConflictKind Kind { get; } = kind;
}
