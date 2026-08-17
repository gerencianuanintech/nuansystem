using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;
namespace NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Commands;
public sealed class CreateStorageConditionCommandHandler(IStorageConditionRepository r,ITransactionRunner tx,IStorageConditionLocalOutboxWriter writer):ICommandHandler<CreateStorageConditionCommand,StorageConditionDto>
{
    public Task<Result<StorageConditionDto>> Handle(CreateStorageConditionCommand q,CancellationToken ct)
    {
        var code=NormalizeCode(q.Code);var data=new CreateStorageConditionData(Guid.NewGuid(),code,q.Name.Trim(),Optional(q.Description),q.SortOrder,q.IsActive,q.AuditUserId,Optional(q.AuditUserName));
        return tx.ExecuteInTenantTransactionAsync(async(c,t,x)=>{if(await r.ExistsByCodeAsync(code,null,c,t,x))return Fail("StorageConditionCodeAlreadyExists","El código de condición de almacenamiento ya existe.",nameof(q.Code));var id=await r.CreateAsync(data,c,t,x);if(id==-1)return Fail("StorageConditionCodeAlreadyExists","El código de condición de almacenamiento ya existe.",nameof(q.Code));if(id<=0)return Result<StorageConditionDto>.Failure("No se pudo crear la condición de almacenamiento.");var item=await r.GetByIdAsync(id,c,t,x)??throw new InvalidOperationException("La condición fue creada pero no pudo consultarse.");await writer.EnqueueAsync(item,SyncOperation.Created,c,t,x);return Result<StorageConditionDto>.Success(item,"Condición de almacenamiento creada correctamente.");},ct);
    }
    internal static string NormalizeCode(string v)=>v.Trim();
    internal static string? Optional(string? v)=>string.IsNullOrWhiteSpace(v)?null:v.Trim();
    internal static Result<StorageConditionDto> Fail(string code,string message,string field)=>Result<StorageConditionDto>.Failure(message,[new ApiError(code,message,field)]);
}
public sealed class UpdateStorageConditionCommandHandler(IStorageConditionRepository r,ITransactionRunner tx,IStorageConditionLocalOutboxWriter writer):ICommandHandler<UpdateStorageConditionCommand,StorageConditionDto>
{
    public Task<Result<StorageConditionDto>> Handle(UpdateStorageConditionCommand q,CancellationToken ct)
    {
        var code=CreateStorageConditionCommandHandler.NormalizeCode(q.Code);var data=new UpdateStorageConditionData(q.Id,code,q.Name.Trim(),CreateStorageConditionCommandHandler.Optional(q.Description),q.SortOrder,q.IsActive,q.AuditUserId,CreateStorageConditionCommandHandler.Optional(q.AuditUserName));
        return tx.ExecuteInTenantTransactionAsync(async(c,t,x)=>{var current=await r.GetByIdAsync(q.Id,c,t,x);if(current is null)return F("StorageConditionNotFound","No existe la condición de almacenamiento indicada.",nameof(q.Id));if(await r.ExistsByCodeAsync(code,q.Id,c,t,x))return F("StorageConditionCodeAlreadyExists","El código de condición de almacenamiento ya existe.",nameof(q.Code));var n=await r.UpdateAsync(data,c,t,x);if(n==-1)return F("StorageConditionCodeAlreadyExists","El código de condición de almacenamiento ya existe.",nameof(q.Code));if(n<=0)return Result<StorageConditionDto>.Failure("No se pudo actualizar la condición de almacenamiento.");var item=await r.GetByIdAsync(q.Id,c,t,x)??throw new InvalidOperationException("La condición fue actualizada pero no pudo consultarse.");await writer.EnqueueAsync(item,item.IsActive?SyncOperation.Updated:SyncOperation.Disabled,c,t,x);return Result<StorageConditionDto>.Success(item,"Condición de almacenamiento actualizada correctamente.");},ct);
    }
    private static Result<StorageConditionDto> F(string c,string m,string f)=>CreateStorageConditionCommandHandler.Fail(c,m,f);
}
public sealed class DeleteStorageConditionCommandHandler(IStorageConditionRepository r,ITransactionRunner tx,IStorageConditionLocalOutboxWriter writer):ICommandHandler<DeleteStorageConditionCommand,bool>
{
    public Task<Result<bool>> Handle(DeleteStorageConditionCommand q,CancellationToken ct)=>tx.ExecuteInTenantTransactionAsync(async(c,t,x)=>{var current=await r.GetByIdAsync(q.Id,c,t,x);if(current is null)return F("StorageConditionNotFound","No existe la condición de almacenamiento indicada.");var n=await r.DeleteAsync(q.Id,q.AuditUserId,CreateStorageConditionCommandHandler.Optional(q.AuditUserName),c,t,x);if(n==-3)return F("StorageConditionInUse","La condición de almacenamiento está asociada a otros registros.");if(n<=0)return Result<bool>.Failure("No se pudo eliminar la condición de almacenamiento.");await writer.EnqueueAsync(current,SyncOperation.Deleted,c,t,x);return Result<bool>.Success(true,"Condición de almacenamiento eliminada correctamente.");},ct);
    private static Result<bool> F(string c,string m)=>Result<bool>.Failure(m,[new ApiError(c,m,nameof(DeleteStorageConditionCommand.Id))]);
}
