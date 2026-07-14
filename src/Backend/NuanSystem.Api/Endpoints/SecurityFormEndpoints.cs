using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Features.SecurityForms.Commands;
using NuanSystem.Application.Features.SecurityForms.Queries;

namespace NuanSystem.Api.Endpoints;

public static class SecurityFormEndpoints
{
    public static IEndpointRouteBuilder MapSecurityFormEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/security/forms", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityFormsQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityForms)
        .RequireFormOperation("security-forms", "refresh");

        app.MapGet("/api/security/forms/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityFormByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityForms)
        .RequireFormOperation("security-forms", "consult");

        app.MapPost("/api/security/forms", async (
            CreateSecurityFormCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityForms)
        .RequireFormOperation("security-forms", "create");

        app.MapPut("/api/security/forms/{id:int}", async (
            int id,
            UpdateSecurityFormCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                Id = id,
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityForms)
        .RequireFormOperation("security-forms", "update");

        app.MapDelete("/api/security/forms/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new DeleteSecurityFormCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityForms)
        .RequireFormOperation("security-forms", "delete");

        return app;
    }
}
