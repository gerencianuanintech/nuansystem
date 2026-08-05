using System.Data;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.General.Provinces.Commands;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Definitions.General.Provinces;

public sealed class ProvinceSyncPublishingTests
{
    private readonly IGeographyRepository _repository = Substitute.For<IGeographyRepository>();
    private readonly IProvinceLocalOutboxWriter _writer = Substitute.For<IProvinceLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner _runner = new();

    [Fact]
    public async Task Create_WritesProvinceAndOutboxInSameTransaction()
    {
        var province = CreateProvince();
        _repository.ProvinceCodeExistsAsync(1,"AZU",null,_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateProvinceAsync(Arg.Any<SaveProvinceData>(),_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>()).Returns(2);
        _repository.GetProvinceByIdAsync(2,_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>()).Returns(province);
        var handler = new CreateProvinceCommandHandler(_repository,_runner,_writer);
        var result = await handler.Handle(new CreateProvinceCommand(1,"azu","Azuay",true,7,"admin","SAP_B1","EC|AZU"),CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(province,SyncOperation.Created,_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>());
        _runner.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Update_PreservesExternalReferenceAndWritesDisabled()
    {
        var current=CreateProvince(); var inactive=CreateProvince(false,current.GlobalId,current.CountryGlobalId);
        _repository.GetProvinceByIdAsync(2,_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>()).Returns(current,inactive);
        _repository.ProvinceCodeExistsAsync(1,"AZU",2,_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>()).Returns(false);
        SaveProvinceData? saved=null;
        _repository.UpdateProvinceAsync(Arg.Do<SaveProvinceData>(x=>saved=x),_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>()).Returns(true);
        var result=await new UpdateProvinceCommandHandler(_repository,_runner,_writer).Handle(new UpdateProvinceCommand(2,1,"AZU","Azuay",false,7,"admin"),CancellationToken.None);
        result.IsSuccess.Should().BeTrue(); saved!.ExternalSystem.Should().Be("SAP_B1"); saved.ExternalCode.Should().Be("EC|AZU");
        await _writer.Received(1).EnqueueAsync(inactive,SyncOperation.Disabled,_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_RollsBackWhenOutboxFails()
    {
        var province=CreateProvince();
        _repository.ProvinceCodeExistsAsync(1,"AZU",null,_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateProvinceAsync(Arg.Any<SaveProvinceData>(),_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>()).Returns(2);
        _repository.GetProvinceByIdAsync(2,_runner.Connection,_runner.Transaction,Arg.Any<CancellationToken>()).Returns(province);
        _writer.EnqueueAsync(Arg.Any<ProvinceDto>(),Arg.Any<SyncOperation>(),Arg.Any<IDbConnection>(),Arg.Any<IDbTransaction>(),Arg.Any<CancellationToken>()).Returns<Task<Guid?>>(_=>throw new InvalidOperationException("outbox"));
        var action=()=>new CreateProvinceCommandHandler(_repository,_runner,_writer).Handle(new CreateProvinceCommand(1,"AZU","Azuay",true,7,"admin"),CancellationToken.None);
        await action.Should().ThrowAsync<InvalidOperationException>(); _runner.RolledBack.Should().BeTrue();
    }

    [Fact]
    public void Migration_ReservesCompositeIdentityAndHasNoAdoption()
    {
        var sql=ReadSource("database","sql","172_tenant_province_transactional_outbox.sql");
        sql.Should().Contain("CREATE UNIQUE INDEX UX_Provinces_Country_Code")
            .And.Contain("CREATE UNIQUE INDEX UX_Provinces_Country_ExternalRef")
            .And.Contain("WHERE GlobalId=@CountryGlobalId")
            .And.Contain("SYNC_PROVINCE_PARENT_CONFLICT")
            .And.Contain("SYNC_PROVINCE_CODE_CONFLICT")
            .And.NotContain("SET GlobalId=@GlobalId");
    }

    private static ProvinceDto CreateProvince(bool active=true,Guid? id=null,Guid? countryId=null)=>new(){Id=2,GlobalId=id??Guid.NewGuid(),CountryId=1,CountryGlobalId=countryId??Guid.NewGuid(),CountryCode="EC",CountryName="Ecuador",Code="AZU",Name="Azuay",ExternalSystem="SAP_B1",ExternalCode="EC|AZU",IsActive=active,CreatedAt=DateTime.UtcNow};
    private static string ReadSource(params string[] parts){var d=new DirectoryInfo(AppContext.BaseDirectory);while(d is not null){var p=Path.Combine([d.FullName,..parts]);if(File.Exists(p))return File.ReadAllText(p);d=d.Parent;}throw new FileNotFoundException();}
    private sealed class ImmediateTransactionRunner:ITransactionRunner
    {
        public IDbConnection Connection{get;}=Substitute.For<IDbConnection>(); public IDbTransaction Transaction{get;}=Substitute.For<IDbTransaction>(); public bool Committed{get;private set;} public bool RolledBack{get;private set;}
        public async Task ExecuteInTenantTransactionAsync(Func<IDbConnection,IDbTransaction,CancellationToken,Task> op,CancellationToken ct=default)=>await ExecuteInTenantTransactionAsync<object?>(async(c,t,x)=>{await op(c,t,x);return null;},ct);
        public async Task<T> ExecuteInTenantTransactionAsync<T>(Func<IDbConnection,IDbTransaction,CancellationToken,Task<T>> op,CancellationToken ct=default){try{var r=await op(Connection,Transaction,ct);Committed=true;return r;}catch{RolledBack=true;throw;}}
    }
}
