using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;
public sealed class StorageConditionSyncEventApplierTests
{
 [Fact] public async Task Collision_IsTerminal(){var repository=Substitute.For<IStorageConditionSyncApplyRepository>();var payload=Payload();var context=Context(payload);repository.ApplyAsync(2,context,Arg.Any<StorageConditionSyncPayload>(),SyncOperation.Created,Arg.Any<CancellationToken>()).Returns(new StorageConditionSyncApplyResult(false,false,true,null,"ocupado","SYNC_STORAGE_CONDITION_CODE_CONFLICT"));var result=await new StorageConditionSyncEventApplier(repository).ApplyAsync(context);result.Terminal.Should().BeTrue();result.ErrorCode.Should().Be("SYNC_STORAGE_CONDITION_CODE_CONFLICT");}
 [Fact] public async Task InvalidOrder_IsRejected(){var repository=Substitute.For<IStorageConditionSyncApplyRepository>();var payload=Payload() with{SortOrder=-1};var result=await new StorageConditionSyncEventApplier(repository).ApplyAsync(Context(payload));result.ErrorCode.Should().Be("SYNC_STORAGE_CONDITION_PAYLOAD_INVALID");await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default,default!,default!,default,default);}
 [Fact] public async Task GlobalIdMismatch_IsRejected(){var repository=Substitute.For<IStorageConditionSyncApplyRepository>();var payload=Payload();var result=await new StorageConditionSyncEventApplier(repository).ApplyAsync(Context(payload) with{EntityGlobalId=Guid.NewGuid()});result.ErrorCode.Should().Be("SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");}
 private static StorageConditionSyncPayload Payload()=>new(Guid.NewGuid(),"REFRIGERADO","Refrigerado",null,20,true,false,DateTime.UtcNow);
 private static SyncEventApplyContext Context(StorageConditionSyncPayload payload)=>new(Guid.NewGuid(),1,"StorageCondition",payload.GlobalId,SyncOperation.Created.ToString(),JsonSerializer.Serialize(new{payload},new JsonSerializerOptions(JsonSerializerDefaults.Web)),2,3);
}


