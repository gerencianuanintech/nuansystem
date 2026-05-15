using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Queries;

namespace NuanSystem.Api.Endpoints;

public static class AccountingEndpoints
{
    public static IEndpointRouteBuilder MapAccountingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/accounting/chart-of-accounts", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetChartOfAccountsQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("chart-of-accounts", "refresh");

        app.MapGet("/api/accounting/chart-of-accounts/lookups", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetChartOfAccountLookupQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("chart-of-accounts", "refresh");

        app.MapGet("/api/accounting/chart-of-accounts/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetChartOfAccountByIdQuery(id), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("chart-of-accounts", "consult");

        app.MapPost("/api/accounting/chart-of-accounts", async (
            CreateChartOfAccountCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("chart-of-accounts", "create");

        app.MapPut("/api/accounting/chart-of-accounts/{id:int}", async (
            int id,
            UpdateChartOfAccountCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("chart-of-accounts", "update");

        app.MapDelete("/api/accounting/chart-of-accounts/{id:int}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteChartOfAccountCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("chart-of-accounts", "delete");

        return app;
    }
}
