using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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

public sealed class SapCityRetryProcessorTests
{
    [Fact]
    public async Task ValidCityV1Snapshot_IsReprocessedFromApprovedCompositeIdentity()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country();
        var province = Province(country);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([province]);
        repository.GetCitiesAsync(Arg.Any<CancellationToken>()).Returns([]);
        var globalId = Guid.NewGuid();
        sender.Send(Arg.Any<CreateCityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CityDto>.Success(City(country, province, 21, globalId)));
        var processor = new SapCityExecutionRetryProcessor(
            new SapCityRecordProcessor(repository, sender));

        var result = await processor.ProcessAsync(Claim(new("EC", "01", "0101", "Quito")));

        processor.ApprovedSnapshotType.Should().Be(SapSyncApprovedSnapshotTypes.CityV1);
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        result.LocalEntityId.Should().Be(21);
        result.LocalGlobalId.Should().Be(globalId);
    }

    [Fact]
    public async Task SnapshotCompositeIdentityDifferentFromClaim_IsTerminalWithoutWrite()
    {
        var sender = Substitute.For<ISender>();
        var processor = new SapCityExecutionRetryProcessor(
            new SapCityRecordProcessor(Substitute.For<IGeographyRepository>(), sender));
        var claim = Claim(new("EC", "01", "0101", "Quito")) with
        {
            SourceRecordKey = "EC|09|0101"
        };

        var result = await processor.ProcessAsync(claim);

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapCityResultCodes.SnapshotInvalid);
        await sender.DidNotReceive().Send(Arg.Any<CreateCityCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateCityCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MalformedSnapshot_IsTerminalAndDoesNotLeakPayload()
    {
        const string json = "{invalid secret-password}";
        var processor = new SapCityExecutionRetryProcessor(
            new SapCityRecordProcessor(
                Substitute.For<IGeographyRepository>(), Substitute.For<ISender>()));
        var claim = new SapSyncExecutionDetailClaim(
            1, Guid.NewGuid(), "EC|01|0101", SapSyncExecutionDetailStatuses.Processing, 1, 3,
            SapSyncApprovedSnapshotTypes.CityV1, json,
            SHA256.HashData(Encoding.UTF8.GetBytes(json)), new string('A', 64),
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1));

        var result = await processor.ProcessAsync(claim);

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapCityResultCodes.SnapshotInvalid);
        result.SafeMessage.Should().NotContain(json);
        result.SafeMessage.Should().NotContain("password");
    }

    private static SapSyncExecutionDetailClaim Claim(SapCitySnapshot snapshot)
    {
        var json = JsonSerializer.Serialize(
            snapshot, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return new(
            1, Guid.NewGuid(), snapshot.ExternalCode,
            SapSyncExecutionDetailStatuses.Processing, 1, 3,
            SapSyncApprovedSnapshotTypes.CityV1, json,
            SHA256.HashData(Encoding.UTF8.GetBytes(json)), new string('B', 64),
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1));
    }

    private static CountryDto Country() => new()
    {
        Id = 1,
        GlobalId = Guid.NewGuid(),
        Code = "LOCAL-EC",
        Name = "Ecuador",
        ExternalSystem = "SAP_B1",
        ExternalCode = "EC",
        IsActive = true
    };

    private static ProvinceDto Province(CountryDto country) => new()
    {
        Id = 2,
        GlobalId = Guid.NewGuid(),
        CountryId = country.Id,
        CountryGlobalId = country.GlobalId,
        CountryCode = country.Code,
        CountryName = country.Name,
        Code = "LOCAL-01",
        Name = "Pichincha",
        ExternalSystem = "SAP_B1",
        ExternalCode = "EC|01",
        IsActive = true
    };

    private static CityDto City(
        CountryDto country,
        ProvinceDto province,
        int id,
        Guid globalId) => new()
    {
        Id = id,
        GlobalId = globalId,
        CountryId = country.Id,
        CountryGlobalId = country.GlobalId,
        CountryCode = country.Code,
        CountryName = country.Name,
        ProvinceId = province.Id,
        ProvinceGlobalId = province.GlobalId,
        ProvinceCode = province.Code,
        ProvinceName = province.Name,
        Code = "0101",
        Name = "Quito",
        ExternalSystem = "SAP_B1",
        ExternalCode = "EC|01|0101",
        IsActive = true
    };
}
