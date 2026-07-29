using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Features.SriTxtImports;
using NuanSystem.Application.Features.SriTxtImports.Commands;
using NuanSystem.Application.Features.SriTxtImports.Dtos;
using NuanSystem.Application.Features.SriTxtImports.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SriTxtImportEndpoints
{
    private const long MultipartRequestLimit = SriTxtImportLimits.MaxFileSizeBytes + (64 * 1024);

    public static IEndpointRouteBuilder MapSriTxtImportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sri/txt-imports")
            .WithTags(SwaggerTags.Sri);

        group.MapGet(
                "",
                async (
                    DateTime? createdFrom,
                    DateTime? createdTo,
                    string? status,
                    string? fileName,
                    string? environment,
                    int? page,
                    int? pageSize,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                    (await sender.Send(
                        new GetSriTxtImportsQuery(
                            new SriTxtImportFilter(
                                createdFrom,
                                createdTo,
                                status,
                                fileName,
                                environment,
                                page ?? 1,
                                pageSize ?? 50)),
                        cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriTxtImportsView);

        group.MapGet(
                "/{id:long}",
                async (long id, ISender sender, CancellationToken cancellationToken) =>
                    (await sender.Send(new GetSriTxtImportByIdQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriTxtImportsView);

        group.MapGet(
                "/{id:long}/rows",
                async (
                    long id,
                    string? validity,
                    int? page,
                    int? pageSize,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                    (await sender.Send(
                        new GetSriTxtImportRowsQuery(
                            id,
                            new SriTxtImportRowFilter(
                                validity ?? SriTxtRowValidityCodes.All,
                                page ?? 1,
                                pageSize ?? 100)),
                        cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriTxtImportsView);

        group.MapPost(
                "/upload",
                async (
                    IFormFile file,
                    ISender sender,
                    ClaimsPrincipal user,
                    CancellationToken cancellationToken) =>
                {
                    var auditUser = user.GetAuditUser();
                    await using var stream = file.OpenReadStream();
                    using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    timeout.CancelAfter(TimeSpan.FromSeconds(SriTxtImportLimits.MaxProcessingSeconds));

                    return (await sender.Send(
                        new UploadSriTxtImportCommand(
                            file.FileName,
                            file.Length,
                            file.ContentType,
                            stream,
                            Guid.NewGuid(),
                            auditUser.UserId,
                            auditUser.UserName),
                        timeout.Token)).ToHttpResult();
                })
            .Accepts<IFormFile>("multipart/form-data")
            .WithMetadata(new RequestSizeLimitAttribute(MultipartRequestLimit))
            .DisableAntiforgery()
            .RequirePermission(PermissionCodes.SriTxtImportsUpload);

        group.MapPost(
                "/{id:long}/enqueue",
                async (
                    long id,
                    EnqueueSriTxtImportRequest request,
                    ISender sender,
                    ClaimsPrincipal user,
                    CancellationToken cancellationToken) =>
                {
                    var auditUser = user.GetAuditUser();
                    return (await sender.Send(
                        new EnqueueSriTxtImportCommand(
                            id,
                            request.RowVersion,
                            Guid.NewGuid(),
                            auditUser.UserId,
                            auditUser.UserName),
                        cancellationToken)).ToHttpResult();
                })
            .RequirePermission(PermissionCodes.SriTxtImportsEnqueue);

        return app;
    }

    private sealed record EnqueueSriTxtImportRequest(byte[] RowVersion);
}
