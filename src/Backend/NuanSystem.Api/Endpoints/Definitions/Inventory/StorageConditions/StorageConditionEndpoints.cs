using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Queries;
using NuanSystem.Shared.Constants;
namespace NuanSystem.Api.Endpoints.Definitions.Inventory.StorageConditions;
public static class StorageConditionEndpoints
{
    private const string FormKey="storage-conditions";
    public static IEndpointRouteBuilder MapStorageConditionEndpoints(this IEndpointRouteBuilder app)
    {
        var g=app.MapGroup("/api/definitions/inventory/storage-conditions");
        g.MapGet("",async(ISender s,CancellationToken ct)=>(await s.Send(new GetStorageConditionsQuery(),ct)).ToHttpResult()).RequirePermission(PermissionCodes.GeneralInventoryStorageConditionsRead).RequireFormOperation(FormKey,"refresh");
        g.MapGet("/lookup",async(string? includeCode,ISender s,CancellationToken ct)=>(await s.Send(new GetStorageConditionLookupQuery(includeCode),ct)).ToHttpResult()).RequireAuthorization(p=>{p.RequireAuthenticatedUser();p.RequireAssertion(c=>c.User.HasClaim(AuthClaimNames.Permission,PermissionCodes.ItemsRead)||c.User.HasClaim(AuthClaimNames.Permission,PermissionCodes.GeneralInventoryStorageConditionsRead)||c.User.HasClaim(AuthClaimNames.Permission,PermissionCodes.GeneralInventoryStorageConditionsManage));});
        g.MapGet("/{id:int}",async(int id,ISender s,CancellationToken ct)=>(await s.Send(new GetStorageConditionByIdQuery(id),ct)).ToHttpResult()).RequirePermission(PermissionCodes.GeneralInventoryStorageConditionsRead).RequireFormOperation(FormKey,"consult");
        g.MapGet("/{id:int}/history",async(int id,ISender s,CancellationToken ct)=>(await s.Send(new GetStorageConditionHistoryQuery(id),ct)).ToHttpResult()).RequirePermission(PermissionCodes.GeneralInventoryStorageConditionsRead).RequireFormOperation(FormKey,"history");
        g.MapPost("",async(SaveStorageConditionRequest r,ISender s,ClaimsPrincipal u,CancellationToken ct)=>{var a=u.GetAuditUser();return(await s.Send(new CreateStorageConditionCommand(r.Code,r.Name,r.Description,r.SortOrder,r.IsActive,a.UserId,a.UserName),ct)).ToHttpResult();}).RequirePermission(PermissionCodes.GeneralInventoryStorageConditionsManage).RequireFormOperation(FormKey,"create");
        g.MapPut("/{id:int}",async(int id,SaveStorageConditionRequest r,ISender s,ClaimsPrincipal u,CancellationToken ct)=>{var a=u.GetAuditUser();return(await s.Send(new UpdateStorageConditionCommand(id,r.Code,r.Name,r.Description,r.SortOrder,r.IsActive,a.UserId,a.UserName),ct)).ToHttpResult();}).RequirePermission(PermissionCodes.GeneralInventoryStorageConditionsManage).RequireFormOperation(FormKey,"update");
        g.MapDelete("/{id:int}",async(int id,ISender s,ClaimsPrincipal u,CancellationToken ct)=>{var a=u.GetAuditUser();return(await s.Send(new DeleteStorageConditionCommand(id,a.UserId,a.UserName),ct)).ToHttpResult();}).RequirePermission(PermissionCodes.GeneralInventoryStorageConditionsManage).RequireFormOperation(FormKey,"delete");
        return app;
    }
    private sealed record SaveStorageConditionRequest(string Code,string Name,string? Description,int SortOrder,bool IsActive);
}
