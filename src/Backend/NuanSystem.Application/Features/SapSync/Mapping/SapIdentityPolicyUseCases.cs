using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Mapping;

public sealed record GetSapIdentityPolicyQuery(int CompanyId) : IQuery<SapBusinessPartnerIdentityPolicy?>;

public sealed class GetSapIdentityPolicyQueryHandler(ISapBusinessPartnerIdentityPolicyRepository repository)
    : IQueryHandler<GetSapIdentityPolicyQuery, SapBusinessPartnerIdentityPolicy?>
{
    public async Task<Result<SapBusinessPartnerIdentityPolicy?>> Handle(GetSapIdentityPolicyQuery request, CancellationToken cancellationToken)
        => Result<SapBusinessPartnerIdentityPolicy?>.Success(await repository.GetAsync(request.CompanyId, cancellationToken));
}

public sealed record SaveSapIdentityPolicyCommand(
    int CompanyId,
    string CustomerNationalPrefix,
    string CustomerForeignPrefix,
    string SupplierNationalPrefix,
    string SupplierForeignPrefix,
    string PassportTaxpayerTypeCodes,
    string PassportIdentificationTypeCode,
    int? AuditUserId,
    string? AuditUserName) : ICommand<long>;

public sealed class SaveSapIdentityPolicyCommandHandler(ISapBusinessPartnerIdentityPolicyRepository repository)
    : ICommandHandler<SaveSapIdentityPolicyCommand, long>
{
    public async Task<Result<long>> Handle(SaveSapIdentityPolicyCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("La empresa es obligatoria.", [new ApiError("SAP_IDENTITY_COMPANY_REQUIRED", "La empresa es obligatoria.", nameof(request.CompanyId))]);

        try
        {
            var policy = new SapBusinessPartnerIdentityPolicy(request.CustomerNationalPrefix, request.CustomerForeignPrefix, request.SupplierNationalPrefix, request.SupplierForeignPrefix, request.PassportTaxpayerTypeCodes, request.PassportIdentificationTypeCode);
            var id = await repository.SaveAsync(request.CompanyId, policy, request.AuditUserId, request.AuditUserName, cancellationToken);
            return Result<long>.Success(id, "Política de identidad SAP guardada correctamente.");
        }
        catch (ArgumentException exception)
        {
            return Result<long>.Failure("La política de identidad SAP no es válida.", [new ApiError("SAP_IDENTITY_POLICY_INVALID", exception.Message)]);
        }
    }
}
