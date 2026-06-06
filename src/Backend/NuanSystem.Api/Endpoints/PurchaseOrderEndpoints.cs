using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Commands;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Queries;

namespace NuanSystem.Api.Endpoints;

public static class PurchaseOrderEndpoints
{
    public static IEndpointRouteBuilder MapPurchaseOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/purchase-orders", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetPurchaseOrdersQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "refresh");

        app.MapGet("/api/purchase-orders/lookups", async (
            string? actionKey,
            ClaimsPrincipal user,
            ICompanyContext companyContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (companyContext.CurrentCompany is null)
            {
                return Results.BadRequest("Debe seleccionar una empresa.");
            }

            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new GetPurchaseOrderLookupsQuery(
                    auditUser.UserId ?? 0,
                    companyContext.CurrentCompany.CompanyCode,
                    PurchaseOrderSecurity.FormKeyEdit,
                    PurchaseOrderSecurity.DocumentType,
                    string.IsNullOrWhiteSpace(actionKey) ? PurchaseOrderSecurity.ActionCreate : actionKey),
                cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "refresh");

        app.MapGet("/api/purchase-orders/field-access", async (
            int seriesId,
            ClaimsPrincipal user,
            ICompanyContext companyContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (companyContext.CurrentCompany is null)
            {
                return Results.BadRequest("Debe seleccionar una empresa.");
            }

            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new GetPurchaseOrderFieldAccessQuery(
                    auditUser.UserId ?? 0,
                    companyContext.CurrentCompany.CompanyCode,
                    seriesId),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "consult");

        app.MapGet("/api/purchase-orders/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "consult");

        app.MapPost("/api/purchase-orders", async (
            CreatePurchaseOrderCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "create");

        app.MapPost("/api/purchase-orders/{id:int}/save", async (
            int id,
            UpdatePurchaseOrderCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "update");

        app.MapPut("/api/purchase-orders/{id:int}", async (
            int id,
            UpdatePurchaseOrderCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "update");

        app.MapDelete("/api/purchase-orders/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeletePurchaseOrderCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "delete");

        app.MapPost("/api/purchase-orders/{id:int}/send-to-approval", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new SendPurchaseOrderToApprovalCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "approve");

        app.MapPost("/api/purchase-orders/{id:int}/approve", async (
            int id,
            PurchaseOrderWorkflowRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new ApprovePurchaseOrderCommand(id, request.Observation, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "approve");

        app.MapPost("/api/purchase-orders/{id:int}/reject", async (
            int id,
            PurchaseOrderWorkflowRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new RejectPurchaseOrderCommand(id, request.Observation, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "approve");

        app.MapPost("/api/purchase-orders/{id:int}/sync-sap", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new SyncPurchaseOrderSapCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "syncsap");

        app.MapGet("/api/purchase-orders/{id:int}/sap-status", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "consult");

        app.MapGet("/api/purchase-orders/{id:int}/related-documents", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            var documents = result.IsSuccess && result.Value is not null
                ? Result<IReadOnlyCollection<PurchaseOrderRelatedDocumentDto>>.Success(result.Value.RelatedDocuments, result.Message)
                : Result<IReadOnlyCollection<PurchaseOrderRelatedDocumentDto>>.Failure(result.Message, result.Errors);
            return documents.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "consult");

        app.MapPost("/api/purchase-orders/{id:int}/related-documents", async (
            int id,
            PurchaseOrderRelatedDocumentSaveRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new AddPurchaseOrderRelatedDocumentCommand(id, request, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "update");

        app.MapDelete("/api/purchase-orders/{id:int}/related-documents/{relatedId:int}", async (
            int id,
            int relatedId,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeletePurchaseOrderRelatedDocumentCommand(id, relatedId, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "update");

        app.MapGet("/api/purchase-orders/{id:int}/attachments", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
            var attachments = result.IsSuccess && result.Value is not null
                ? Result<IReadOnlyCollection<PurchaseOrderAttachmentDto>>.Success(result.Value.Attachments, result.Message)
                : Result<IReadOnlyCollection<PurchaseOrderAttachmentDto>>.Failure(result.Message, result.Errors);
            return attachments.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "consult");

        app.MapPost("/api/purchase-orders/{id:int}/attachments", async (
            int id,
            PurchaseOrderAttachmentSaveRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new AddPurchaseOrderAttachmentCommand(id, request, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "update");

        app.MapDelete("/api/purchase-orders/{id:int}/attachments/{attachmentId:int}", async (
            int id,
            int attachmentId,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeletePurchaseOrderAttachmentCommand(id, attachmentId, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("purchase-orders", "update");

        return app;
    }
}

public sealed record PurchaseOrderWorkflowRequest(string? Observation);
