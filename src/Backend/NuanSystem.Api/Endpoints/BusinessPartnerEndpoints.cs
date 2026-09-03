using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class BusinessPartnerEndpoints
{
    public static IEndpointRouteBuilder MapBusinessPartnerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/commercial/business-partners", async (
            string? type,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBusinessPartnersQuery(type), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.BusinessPartnersRead);

        app.MapGet("/api/commercial/customers", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBusinessPartnersQuery("Customer"), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("customers", "refresh");

        app.MapGet("/api/commercial/suppliers", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBusinessPartnersQuery("Supplier"), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("suppliers", "refresh");

        app.MapGet("/api/commercial/customers/lookups", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBusinessPartnerLookupsQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("customers", "refresh");

        app.MapGet("/api/commercial/suppliers/lookups", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBusinessPartnerLookupsQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("suppliers", "refresh");

        app.MapGet("/api/commercial/business-partners/lookups", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBusinessPartnerLookupsQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.BusinessPartnersRead);

        app.MapGet("/api/commercial/business-partners/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBusinessPartnerByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.BusinessPartnersRead);

        app.MapGet("/api/commercial/customers/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBusinessPartnerByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("customers", "consult");

        app.MapGet("/api/commercial/suppliers/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetBusinessPartnerByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("suppliers", "consult");

        app.MapPost("/api/commercial/business-partners", async (
            CreateBusinessPartnerCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.BusinessPartnersManage);

        app.MapPost("/api/commercial/customers", async (
            CreateBusinessPartnerCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send((command with { PartnerType = command.PartnerType == "Both" ? "Both" : "Customer" }) with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("customers", "create");

        app.MapPost("/api/commercial/suppliers", async (
            CreateBusinessPartnerCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send((command with { PartnerType = command.PartnerType == "Both" ? "Both" : "Supplier" }) with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("suppliers", "create");

        app.MapPut("/api/commercial/business-partners/{id:int}", async (
            int id,
            UpdateBusinessPartnerCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.BusinessPartnersManage);

        app.MapPut("/api/commercial/customers/{id:int}", async (
            int id,
            UpdateBusinessPartnerCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("customers", "update");

        app.MapPut("/api/commercial/suppliers/{id:int}", async (
            int id,
            UpdateBusinessPartnerCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("suppliers", "update");

        app.MapDelete("/api/commercial/business-partners/{id:int}", async (
            int id,
            DeleteBusinessPartnerRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteBusinessPartnerCommand(id, request.ExpectedRowVersion, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.BusinessPartnersManage);

        app.MapDelete("/api/commercial/customers/{id:int}", async (
            int id,
            DeleteBusinessPartnerRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteBusinessPartnerCommand(id, request.ExpectedRowVersion, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("customers", "delete");

        app.MapDelete("/api/commercial/suppliers/{id:int}", async (
            int id,
            DeleteBusinessPartnerRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteBusinessPartnerCommand(id, request.ExpectedRowVersion, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("suppliers", "delete");

        return app;
    }
}

public sealed record DeleteBusinessPartnerRequest(string ExpectedRowVersion);
