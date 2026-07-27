using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.SriTxtImports;
using NuanSystem.Application.Features.SriTxtImports.Commands;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SriTxtImportEndpoints
{
    private const long MultipartRequestLimit = SriTxtImportLimits.MaxFileSizeBytes + (64 * 1024);

    public static IEndpointRouteBuilder MapSriTxtImportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sri/txt-imports");

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
