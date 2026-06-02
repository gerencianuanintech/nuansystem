using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class FinancialCatalogEndpoints
{
    public static IEndpointRouteBuilder MapFinancialCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        MapCatalog(app, "banks");
        MapCatalog(app, "bank-account-types");
        MapCatalog(app, "currencies");
        MapCatalog(app, "price-lists");
        MapCatalog(app, "purchasing-agents");
        MapCatalog(app, "accounting-payment-methods");
        MapCatalog(app, "payment-priorities");
        MapCatalog(app, "approval-flows");
        MapCatalog(app, "payment-document-types");
        MapCatalog(app, "branches");
        MapCatalog(app, "departments");
        MapCatalog(app, "business-lines");
        MapCatalog(app, "cost-centers");
        MapCatalog(app, "projects");

        return app;
    }

    private static void MapCatalog(IEndpointRouteBuilder app, string catalogKey)
    {
        var route = $"/api/financial-catalogs/{catalogKey}";
        var permissions = GetPermissions(catalogKey);

        app.MapGet(route, async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetFinancialCatalogsQuery(catalogKey), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Read);

        app.MapGet($"{route}/lookup", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetFinancialCatalogLookupQuery(catalogKey), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Read);

        app.MapGet($"{route}/{{id:int}}", async (int id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetFinancialCatalogByIdQuery(catalogKey, id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Read);

        app.MapPost(route, async (
            SaveFinancialCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new CreateFinancialCatalogCommand(
                    catalogKey,
                    request.Code,
                    request.Name,
                    request.Description,
                    request.IsActive,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Manage);

        app.MapPut($"{route}/{{id:int}}", async (
            int id,
            SaveFinancialCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new UpdateFinancialCatalogCommand(
                    catalogKey,
                    id,
                    request.Code,
                    request.Name,
                    request.Description,
                    request.IsActive,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Manage);

        app.MapDelete($"{route}/{{id:int}}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new DeleteFinancialCatalogCommand(catalogKey, id, auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Manage);
    }

    private static CatalogPermissions GetPermissions(string catalogKey)
    {
        return catalogKey switch
        {
            "banks" => new(PermissionCodes.FinancialCatalogsBanksRead, PermissionCodes.FinancialCatalogsBanksManage),
            "bank-account-types" => new(PermissionCodes.FinancialCatalogsBankAccountTypesRead, PermissionCodes.FinancialCatalogsBankAccountTypesManage),
            "currencies" => new(PermissionCodes.FinancialCatalogsCurrenciesRead, PermissionCodes.FinancialCatalogsCurrenciesManage),
            "price-lists" => new(PermissionCodes.FinancialCatalogsPriceListsRead, PermissionCodes.FinancialCatalogsPriceListsManage),
            "purchasing-agents" => new(PermissionCodes.FinancialCatalogsPurchasingAgentsRead, PermissionCodes.FinancialCatalogsPurchasingAgentsManage),
            "accounting-payment-methods" => new(PermissionCodes.FinancialCatalogsAccountingPaymentMethodsRead, PermissionCodes.FinancialCatalogsAccountingPaymentMethodsManage),
            "payment-priorities" => new(PermissionCodes.FinancialCatalogsPaymentPrioritiesRead, PermissionCodes.FinancialCatalogsPaymentPrioritiesManage),
            "approval-flows" => new(PermissionCodes.FinancialCatalogsApprovalFlowsRead, PermissionCodes.FinancialCatalogsApprovalFlowsManage),
            "payment-document-types" => new(PermissionCodes.FinancialCatalogsPaymentDocumentTypesRead, PermissionCodes.FinancialCatalogsPaymentDocumentTypesManage),
            "branches" => new(PermissionCodes.FinancialCatalogsBranchesRead, PermissionCodes.FinancialCatalogsBranchesManage),
            "departments" => new(PermissionCodes.FinancialCatalogsDepartmentsRead, PermissionCodes.FinancialCatalogsDepartmentsManage),
            "business-lines" => new(PermissionCodes.FinancialCatalogsBusinessLinesRead, PermissionCodes.FinancialCatalogsBusinessLinesManage),
            "cost-centers" => new(PermissionCodes.FinancialCatalogsCostCentersRead, PermissionCodes.FinancialCatalogsCostCentersManage),
            "projects" => new(PermissionCodes.FinancialCatalogsProjectsRead, PermissionCodes.FinancialCatalogsProjectsManage),
            _ => throw new InvalidOperationException($"Financial catalog '{catalogKey}' is not configured.")
        };
    }

    private sealed record SaveFinancialCatalogRequest(
        string Code,
        string Name,
        string? Description,
        bool IsActive);

    private sealed record CatalogPermissions(string Read, string Manage);
}
