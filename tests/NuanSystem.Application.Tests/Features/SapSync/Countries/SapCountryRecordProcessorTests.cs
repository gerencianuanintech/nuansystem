using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Countries.Commands;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;
using NuanSystem.Application.Features.SapSync.Countries.Services;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Tests.Features.SapSync.Countries;

public sealed class SapCountryRecordProcessorTests
{
    [Fact]
    public async Task NewCountry_IsCreatedWithSapIdentityAndWithoutInventingPhonePrefix()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([]);
        sender.Send(Arg.Any<CreateCountryCommand>(), Arg.Any<CancellationToken>())
            .Returns(call => Result<CountryDto>.Success(Local(
                101,
                Guid.NewGuid(),
                call.Arg<CreateCountryCommand>().Code,
                call.Arg<CreateCountryCommand>().Name,
                "SAP_B1",
                call.Arg<CreateCountryCommand>().Code)));
        var processor = new SapCountryRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            new SapCountrySnapshot(" ec ", " Ecuador ", " EC ", " ECU "), 7, "tester");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        result.ResultCode.Should().Be(SapCountryResultCodes.Created);
        await sender.Received(1).Send(
            Arg.Is<CreateCountryCommand>(command =>
                command.Code == "EC"
                && command.Name == "Ecuador"
                && command.Iso2 == "EC"
                && command.Iso3 == "ECU"
                && command.PhonePrefix == null
                && command.IsActive
                && command.ExternalSystem == "SAP_B1"
                && command.ExternalCode == "EC"
                && command.AuditUserId == 7),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LinkedCountry_UpdatePreservesLocalCodePhonePrefixActiveAndGlobalIdentity()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var globalId = Guid.NewGuid();
        var local = Local(10, globalId, "LOCAL-EC", "Nombre anterior", "SAP_B1", "EC");
        local.PhonePrefix = "+593";
        local.IsActive = false;
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([local]);
        sender.Send(Arg.Any<UpdateCountryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CountryDto>.Success(local));
        var processor = new SapCountryRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            new SapCountrySnapshot("EC", "Ecuador", "EC", "ECU"), 8, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Updated);
        result.LocalGlobalId.Should().Be(globalId);
        await sender.Received(1).Send(
            Arg.Is<UpdateCountryCommand>(command =>
                command.Id == 10
                && command.Code == "LOCAL-EC"
                && command.PhonePrefix == "+593"
                && !command.IsActive
                && command.ExternalSystem == "SAP_B1"
                && command.ExternalCode == "EC"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CodeOnlyCollision_RequiresApprovalWithoutAdoptionOrWrite()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var local = Local(10, Guid.NewGuid(), "EC", "Ecuador local", null, null);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([local]);
        var processor = new SapCountryRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            new SapCountrySnapshot("EC", "Ecuador SAP", "EC", "ECU"), null, "worker");

        result.Action.Should().Be(SapSyncExecutionDetailActions.Approval);
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.ApprovalRequired);
        result.ResultCode.Should().Be(SapCountryResultCodes.CodeCollisionApprovalRequired);
        result.LocalGlobalId.Should().Be(local.GlobalId);
        await sender.DidNotReceive().Send(Arg.Any<CreateCountryCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateCountryCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SecondFullCycle_IsUnchangedAndDoesNotDuplicate()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var created = Local(20, Guid.NewGuid(), "PE", "Peru", "SAP_B1", "PE");
        created.Iso2 = "PE";
        created.Iso3 = "PER";
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([]);
        sender.Send(Arg.Any<CreateCountryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CountryDto>.Success(created));
        var processor = new SapCountryRecordProcessor(repository, sender);
        var snapshot = new SapCountrySnapshot("PE", "Peru", "PE", "PER");

        var first = await processor.ProcessAsync(snapshot, null, "worker");
        var second = await processor.ProcessAsync(snapshot, null, "worker");

        first.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        second.Status.Should().Be(SapSyncExecutionDetailStatuses.Unchanged);
        second.LocalGlobalId.Should().Be(created.GlobalId);
        await sender.Received(1).Send(Arg.Any<CreateCountryCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateCountryCommand>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("", "Ecuador")]
    [InlineData("EC", " ")]
    public async Task InvalidIdentity_IsSkippedWithoutRepositoryOrWrite(string code, string name)
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var processor = new SapCountryRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            new SapCountrySnapshot(code, name, null, null), null, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Skipped);
        result.ResultCode.Should().Be(SapCountryResultCodes.Invalid);
        await repository.DidNotReceiveWithAnyArgs().GetCountriesAsync(default);
        await sender.DidNotReceiveWithAnyArgs().Send(default!, default);
    }

    private static CountryDto Local(
        int id,
        Guid globalId,
        string code,
        string name,
        string? externalSystem,
        string? externalCode) => new()
        {
            Id = id,
            GlobalId = globalId,
            Code = code,
            Name = name,
            ExternalSystem = externalSystem,
            ExternalCode = externalCode,
            IsActive = true
        };
}
