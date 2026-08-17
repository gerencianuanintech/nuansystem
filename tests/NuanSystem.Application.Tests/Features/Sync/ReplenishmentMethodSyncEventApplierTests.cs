using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;
public sealed class ReplenishmentMethodSyncEventApplierTests
{
 [Fact] public async Task Collision_IsTerminal(){var repository=Substitute.For<IReplenishmentMethodSyncApplyRepository>();var payload=Payload();var context=Context(payload);repository.ApplyAsync(2,context,Arg.Any<ReplenishmentMethodSyncPayload>(),SyncOperation.Created,Arg.Any<CancellationToken>()).Returns(new ReplenishmentMethodSyncApplyResult(false,false,true,null,"ocupado","SYNC_REPLENISHMENT_METHOD_CODE_CONFLICT"));var result=await new ReplenishmentMethodSyncEventApplier(repository).ApplyAsync(context);result.Terminal.Should().BeTrue();result.ErrorCode.Should().Be("SYNC_REPLENISHMENT_METHOD_CODE_CONFLICT");}
 [Fact] public async Task InvalidOrder_IsRejected(){var repository=Substitute.For<IReplenishmentMethodSyncApplyRepository>();var payload=Payload() with{SortOrder=-1};var result=await new ReplenishmentMethodSyncEventApplier(repository).ApplyAsync(Context(payload));result.ErrorCode.Should().Be("SYNC_REPLENISHMENT_METHOD_PAYLOAD_INVALID");await repository.DidNotReceiveWithAnyArgs().ApplyAsync(default,default!,default!,default,default);}
 [Fact] public async Task GlobalIdMismatch_IsRejected(){var repository=Substitute.For<IReplenishmentMethodSyncApplyRepository>();var payload=Payload();var result=await new ReplenishmentMethodSyncEventApplier(repository).ApplyAsync(Context(payload) with{EntityGlobalId=Guid.NewGuid()});result.ErrorCode.Should().Be("SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");}
 private static ReplenishmentMethodSyncPayload Payload()=>new(Guid.NewGuid(),"PUNTO_REORDEN","Punto de reorden",null,20,true,false,DateTime.UtcNow);
 private static SyncEventApplyContext Context(ReplenishmentMethodSyncPayload payload)=>new(Guid.NewGuid(),1,"ReplenishmentMethod",payload.GlobalId,SyncOperation.Created.ToString(),JsonSerializer.Serialize(new{payload},new JsonSerializerOptions(JsonSerializerDefaults.Web)),2,3);
}
