using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.BusinessPartners.SapCodes;

public sealed record GetBusinessPartnerSapCodePolicyQuery : IQuery<BusinessPartnerSapCodePolicyDto>;
