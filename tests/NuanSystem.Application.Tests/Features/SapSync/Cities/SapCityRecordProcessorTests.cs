using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Cities.Commands;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;
using NuanSystem.Application.Features.SapSync.Cities.Services;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Tests.Features.SapSync.Cities;

public sealed class SapCityRecordProcessorTests
{
    [Fact]
    public async Task NewCity_UsesConfirmedParentsAndCompositeIdentity()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country();
        var province = Province(country);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([province]);
        repository.GetCitiesAsync(Arg.Any<CancellationToken>()).Returns([]);
        sender.Send(Arg.Any<CreateCityCommand>(), Arg.Any<CancellationToken>())
            .Returns(call => Result<CityDto>.Success(City(country, province, 10,
                call.Arg<CreateCityCommand>().Code, call.Arg<CreateCityCommand>().Name,
                true, "SAP_B1", "EC|01|0101")));
        var processor = new SapCityRecordProcessor(repository, sender);

        var result = await processor.ProcessAsync(new(" ec ", " 01 ", " 0101 ", " Quito "), 7, "tester");

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        await sender.Received(1).Send(Arg.Is<CreateCityCommand>(x =>
            x.CountryId == country.Id && x.ProvinceId == province.Id && x.Code == "0101"
            && x.ExternalSystem == "SAP_B1" && x.ExternalCode == "EC|01|0101"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MissingConfirmedProvince_IsConflictWithoutWrite()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([Country()]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([]);
        var result = await new SapCityRecordProcessor(repository, sender)
            .ProcessAsync(new("EC", "01", "0101", "Quito"), null, "worker");
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapCityResultCodes.ProvinceNotFound);
        await sender.DidNotReceive().Send(Arg.Any<CreateCityCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LocalCodeCollision_RequiresApprovalAndIsNotAdopted()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country();
        var province = Province(country);
        var local = City(country, province, 10, "0101", "Local", true, null, null);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([province]);
        repository.GetCitiesAsync(Arg.Any<CancellationToken>()).Returns([local]);
        var result = await new SapCityRecordProcessor(repository, sender)
            .ProcessAsync(new("EC", "01", "0101", "SAP"), null, "worker");
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.ApprovalRequired);
        result.ResultCode.Should().Be(SapCityResultCodes.CodeCollisionApprovalRequired);
        await sender.DidNotReceiveWithAnyArgs().Send(default!, default);
    }

    [Fact]
    public async Task LinkedCity_UpdatePreservesGlobalIdCodeParentsAndInactiveState()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country();
        var province = Province(country);
        var globalId = Guid.NewGuid();
        var local = City(country, province, 10, "LOCAL", "Anterior", false, "SAP_B1", "EC|01|0101", globalId);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([province]);
        repository.GetCitiesAsync(Arg.Any<CancellationToken>()).Returns([local]);
        sender.Send(Arg.Any<UpdateCityCommand>(), Arg.Any<CancellationToken>()).Returns(Result<CityDto>.Success(local));
        var result = await new SapCityRecordProcessor(repository, sender)
            .ProcessAsync(new("EC", "01", "0101", "Nuevo"), 8, "worker");
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Updated);
        result.LocalGlobalId.Should().Be(globalId);
        await sender.Received(1).Send(Arg.Is<UpdateCityCommand>(x =>
            x.Id == local.Id && x.CountryId == country.Id && x.ProvinceId == province.Id
            && x.Code == "LOCAL" && !x.IsActive), Arg.Any<CancellationToken>());
    }

    private static CountryDto Country() => new() { Id = 1, GlobalId = Guid.NewGuid(), Code = "LOCAL-EC", Name = "Ecuador", ExternalSystem = "SAP_B1", ExternalCode = "EC", IsActive = true };
    private static ProvinceDto Province(CountryDto country) => new() { Id = 2, GlobalId = Guid.NewGuid(), CountryId = country.Id, CountryGlobalId = country.GlobalId, CountryCode = country.Code, CountryName = country.Name, Code = "LOCAL-01", Name = "Pichincha", ExternalSystem = "SAP_B1", ExternalCode = "EC|01", IsActive = true };
    private static CityDto City(CountryDto country, ProvinceDto province, int id, string code,
        string name, bool active, string? externalSystem, string? externalCode, Guid? globalId = null) => new()
        { Id = id, GlobalId = globalId ?? Guid.NewGuid(), CountryId = country.Id, CountryGlobalId = country.GlobalId,
          CountryCode = country.Code, CountryName = country.Name, ProvinceId = province.Id,
          ProvinceGlobalId = province.GlobalId, ProvinceCode = province.Code, ProvinceName = province.Name,
          Code = code, Name = name, IsActive = active, ExternalSystem = externalSystem, ExternalCode = externalCode };
}
