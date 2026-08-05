using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Commands;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;
using NuanSystem.Application.Features.SapSync.Provinces.Services;

namespace NuanSystem.Application.Tests.Features.SapSync.Provinces;

public sealed class SapProvinceRecordProcessorTests
{
    [Fact]
    public async Task NewProvince_IsCreatedUnderConfirmedSapCountryWithCompositeIdentity()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country(1, "LOCAL-EC", "EC");
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([]);
        sender.Send(Arg.Any<CreateProvinceCommand>(), Arg.Any<CancellationToken>())
            .Returns(call => Result<ProvinceDto>.Success(Local(
                10, Guid.NewGuid(), country, call.Arg<CreateProvinceCommand>().Code,
                call.Arg<CreateProvinceCommand>().Name, "SAP_B1", "EC|AZU")));
        var processor = new SapProvinceRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            new SapProvinceSnapshot(" ec ", " azu ", " Azuay "), 7, "tester");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        result.ResultCode.Should().Be(SapProvinceResultCodes.Created);
        await sender.Received(1).Send(
            Arg.Is<CreateProvinceCommand>(command =>
                command.CountryId == country.Id
                && command.Code == "AZU"
                && command.Name == "Azuay"
                && command.IsActive
                && command.ExternalSystem == "SAP_B1"
                && command.ExternalCode == "EC|AZU"
                && command.AuditUserId == 7),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ParentCountryWithoutConfirmedSapIdentity_IsConflictWithoutWrite()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        repository.GetCountriesAsync(Arg.Any<CancellationToken>())
            .Returns([Country(1, "EC", externalCode: null)]);
        var processor = new SapProvinceRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            new SapProvinceSnapshot("EC", "AZU", "Azuay"), null, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapProvinceResultCodes.CountryNotFound);
        await repository.DidNotReceiveWithAnyArgs().GetProvincesAsync(default);
        await sender.DidNotReceive().Send(Arg.Any<CreateProvinceCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CodeCollisionWithinParent_RequiresApprovalWithoutAdoption()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country(1, "EC", "EC");
        var local = Local(10, Guid.NewGuid(), country, "AZU", "Azuay local", null, null);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([local]);
        var processor = new SapProvinceRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            new SapProvinceSnapshot("EC", "AZU", "Azuay SAP"), null, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.ApprovalRequired);
        result.ResultCode.Should().Be(SapProvinceResultCodes.CodeCollisionApprovalRequired);
        result.LocalGlobalId.Should().Be(local.GlobalId);
        await sender.DidNotReceive().Send(Arg.Any<CreateProvinceCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateProvinceCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SameCodeInDifferentCountry_DoesNotCollide()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var ecuador = Country(1, "EC", "EC");
        var peru = Country(2, "PE", "PE");
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([ecuador, peru]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>())
            .Returns([Local(20, Guid.NewGuid(), peru, "01", "Lima", null, null)]);
        sender.Send(Arg.Any<CreateProvinceCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<ProvinceDto>.Success(Local(21, Guid.NewGuid(), ecuador, "01", "Azuay", "SAP_B1", "EC|01")));
        var processor = new SapProvinceRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            new SapProvinceSnapshot("EC", "01", "Azuay"), null, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        await sender.Received(1).Send(
            Arg.Is<CreateProvinceCommand>(command => command.CountryId == ecuador.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LinkedProvince_UpdatePreservesGlobalIdCountryCodeAndActiveState()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country(1, "LOCAL-EC", "EC");
        var globalId = Guid.NewGuid();
        var local = Local(10, globalId, country, "LOCAL-AZU", "Anterior", "SAP_B1", "EC|AZU");
        local.IsActive = false;
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([local]);
        sender.Send(Arg.Any<UpdateProvinceCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<ProvinceDto>.Success(local));
        var processor = new SapProvinceRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(
            new SapProvinceSnapshot("EC", "AZU", "Azuay"), 8, "worker");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Updated);
        result.LocalGlobalId.Should().Be(globalId);
        await sender.Received(1).Send(
            Arg.Is<UpdateProvinceCommand>(command =>
                command.Id == local.Id
                && command.CountryId == country.Id
                && command.Code == "LOCAL-AZU"
                && !command.IsActive
                && command.ExternalCode == "EC|AZU"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SecondFullCycle_IsUnchangedWithoutDuplicate()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country(1, "EC", "EC");
        var created = Local(10, Guid.NewGuid(), country, "AZU", "Azuay", "SAP_B1", "EC|AZU");
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([]);
        sender.Send(Arg.Any<CreateProvinceCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<ProvinceDto>.Success(created));
        var processor = new SapProvinceRecordProcessor(repository, sender);
        var snapshot = new SapProvinceSnapshot("EC", "AZU", "Azuay");

        var first = await processor.ProcessAsync(snapshot, null, "worker");
        var second = await processor.ProcessAsync(snapshot, null, "worker");

        first.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        second.Status.Should().Be(SapSyncExecutionDetailStatuses.Unchanged);
        await sender.Received(1).Send(Arg.Any<CreateProvinceCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateProvinceCommand>(), Arg.Any<CancellationToken>());
    }

    private static CountryDto Country(int id, string code, string? externalCode) => new()
    {
        Id = id,
        GlobalId = Guid.NewGuid(),
        Code = code,
        Name = code,
        ExternalSystem = externalCode is null ? null : "SAP_B1",
        ExternalCode = externalCode,
        IsActive = true
    };

    private static ProvinceDto Local(
        int id,
        Guid globalId,
        CountryDto country,
        string code,
        string name,
        string? externalSystem,
        string? externalCode) => new()
        {
            Id = id,
            GlobalId = globalId,
            CountryId = country.Id,
            CountryGlobalId = country.GlobalId,
            CountryCode = country.Code,
            CountryName = country.Name,
            Code = code,
            Name = name,
            ExternalSystem = externalSystem,
            ExternalCode = externalCode,
            IsActive = true
        };
}
