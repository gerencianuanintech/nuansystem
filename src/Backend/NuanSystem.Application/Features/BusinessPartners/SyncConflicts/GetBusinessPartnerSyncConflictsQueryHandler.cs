using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.SyncConflicts;

public sealed class GetBusinessPartnerSyncConflictsQueryHandler(
    ICompanyContext companyContext,
    IBusinessPartnerSyncConflictRepository repository)
    : IQueryHandler<GetBusinessPartnerSyncConflictsQuery, IReadOnlyCollection<BusinessPartnerSyncConflictDto>>
{
    public async Task<Result<IReadOnlyCollection<BusinessPartnerSyncConflictDto>>> Handle(
        GetBusinessPartnerSyncConflictsQuery request,
        CancellationToken cancellationToken)
    {
        var companyResult = BusinessPartnerSyncConflictCompanyGuard.RequireCentral(companyContext);
        if (companyResult is null)
        {
            return BusinessPartnerSyncConflictErrors.CompanyRequired<IReadOnlyCollection<BusinessPartnerSyncConflictDto>>();
        }

        if (!companyResult.IsMaster)
        {
            return BusinessPartnerSyncConflictErrors.MasterRequired<IReadOnlyCollection<BusinessPartnerSyncConflictDto>>();
        }

        var status = string.IsNullOrWhiteSpace(request.Status) ? "Open" : request.Status.Trim();
        if (status is not "Open" and not "Resolved")
        {
            return Result<IReadOnlyCollection<BusinessPartnerSyncConflictDto>>.Failure(
                "No fue posible consultar los conflictos de socios.",
                [new ApiError("BP_SYNC_CONFLICT_STATUS_INVALID", "El estado debe ser Open o Resolved.", "Status")]);
        }

        var conflicts = await repository.ListAsync(companyResult.CompanyId, status, cancellationToken);
        return Result<IReadOnlyCollection<BusinessPartnerSyncConflictDto>>.Success(
            conflicts.Select(BusinessPartnerSyncConflictMapper.ToDto).ToArray());
    }
}

internal static class BusinessPartnerSyncConflictCompanyGuard
{
    public static CompanyConnectionInfo? RequireCentral(ICompanyContext companyContext) =>
        companyContext.HasActiveCompany ? companyContext.CurrentCompany : null;
}

internal static class BusinessPartnerSyncConflictErrors
{
    public static Result<T> CompanyRequired<T>() => Result<T>.Failure(
        "No hay empresa activa para consultar conflictos de socios.",
        [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa central.", "X-Company-Code")]);

    public static Result<T> MasterRequired<T>() => Result<T>.Failure(
        "Los conflictos de socios solo pueden administrarse desde la empresa central.",
        [new ApiError(
            "BP_SYNC_CONFLICT_MASTER_REQUIRED",
            "Seleccione la empresa central para administrar conflictos de socios.",
            "X-Company-Code")]);
}
