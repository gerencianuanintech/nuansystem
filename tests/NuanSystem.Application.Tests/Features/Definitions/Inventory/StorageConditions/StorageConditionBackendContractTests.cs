using FluentAssertions;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Commands;
namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.StorageConditions;
public sealed class StorageConditionBackendContractTests
{
    [Fact] public void CodeNormalization_PreservesCasing(){var m=typeof(CreateStorageConditionCommandHandler).GetMethod("NormalizeCode",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic)!;m.Invoke(null,[" Ambiente "]).Should().Be("Ambiente");}
    [Fact] public async Task Validator_AllowsZeroOrderAndInactive(){var r=await new CreateStorageConditionCommandValidator().ValidateAsync(new CreateStorageConditionCommand("AMBIENTE","Ambiente",null,0,false));r.IsValid.Should().BeTrue();}
    [Fact] public async Task Validator_RejectsInvalidShape(){var r=await new CreateStorageConditionCommandValidator().ValidateAsync(new CreateStorageConditionCommand("","",new string('X',501),-1,true));r.IsValid.Should().BeFalse();r.Errors.Select(x=>x.PropertyName).Should().Contain(["Code","Name","Description","SortOrder"]);}
}
