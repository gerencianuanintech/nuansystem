using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Mapping;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SapIdentityPolicyEndpoints
{
    public static IEndpointRouteBuilder MapSapIdentityPolicyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sap/identity-policy", async (ICompanyContext companyContext, ISender sender, CancellationToken cancellationToken) =>
        {
            var companyId = companyContext.CurrentCompany?.CompanyId;
            if (companyId is null) return Results.BadRequest("No hay empresa activa.");
            return (await sender.Send(new GetSapIdentityPolicyQuery(companyId.Value), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.SapSyncProfilesView);

        app.MapPut("/api/sap/identity-policy", async (SapIdentityPolicyRequest request, ICompanyContext companyContext, ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
        {
            var companyId = companyContext.CurrentCompany?.CompanyId;
            if (companyId is null) return Results.BadRequest("No hay empresa activa.");
            var audit = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new SaveSapIdentityPolicyCommand(companyId.Value, request.CustomerNationalPrefix, request.CustomerForeignPrefix, request.SupplierNationalPrefix, request.SupplierForeignPrefix, request.PassportTaxpayerTypeCodes, request.PassportIdentificationTypeCode, audit.UserId, audit.UserName), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.SapSyncProfilesEdit);

        return app;
    }
}

public sealed record SapIdentityPolicyRequest(string CustomerNationalPrefix, string CustomerForeignPrefix, string SupplierNationalPrefix, string SupplierForeignPrefix, string PassportTaxpayerTypeCodes, string PassportIdentificationTypeCode);
