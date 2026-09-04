using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.BusinessPartners.SapCodes;

public sealed record UpdateBusinessPartnerSapCodePolicyCommand(
    bool IsEnabled,
    string PrefixMode,
    string PassportIdentificationTypeCode,
    string? ExpectedRowVersion,
    [property: JsonIgnore] int? AuditUserId,
    [property: JsonIgnore] string? AuditUserName) : ICommand<BusinessPartnerSapCodePolicyDto>;
