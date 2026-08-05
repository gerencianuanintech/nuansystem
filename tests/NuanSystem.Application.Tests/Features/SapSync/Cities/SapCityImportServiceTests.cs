using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.Definitions.General.Cities.Commands;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Cities.Services;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Tests.Features.SapSync.Cities;

public sealed class SapCityImportServiceTests
{
    [Fact]
    public async Task Preview_CodeCollisionIsScopedToResolvedProvince()
    {
        var reader = Substitute.For<ISapCityReader>();
        var repository = Substitute.For<IGeographyRepository>();
        var country = Country();
        var pichincha = Province(1, country, "01");
        var guayas = Province(2, country, "09");
        reader.GetCitiesAsync(1, Arg.Any<CancellationToken>())
            .Returns([new SapCityRecord("EC", "01", "0101", "Quito")]);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([pichincha, guayas]);
        repository.GetCitiesAsync(Arg.Any<CancellationToken>())
            .Returns([City(20, country, guayas, "0101", "Otra ciudad", null)]);
        var service = Service(reader, repository, Substitute.For<ISapSyncLogRepository>(),
            Substitute.For<ISender>());

        var result = await service.PreviewAsync(1);

        result.Should().ContainSingle().Which.Status.Should().Be("New");
    }

    [Fact]
    public async Task Import_UsesFullResultAndDoesNotDeactivateAbsentCity()
    {
        var reader = Substitute.For<ISapCityReader>();
        var repository = Substitute.For<IGeographyRepository>();
        var logRepository = Substitute.For<ISapSyncLogRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country();
        var province = Province(1, country, "01");
        var present = City(20, country, province, "LOCAL-QUITO", "Quito", "EC|01|0101");
        var absent = City(21, country, province, "LOCAL-CAYAMBE", "Cayambe", "EC|01|0102");
        reader.GetCitiesAsync(1, Arg.Any<CancellationToken>())
            .Returns([new SapCityRecord("EC", "01", "0101", "Quito")]);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([province]);
        repository.GetCitiesAsync(Arg.Any<CancellationToken>()).Returns([present, absent]);
        var service = Service(reader, repository, logRepository, sender);

        var result = await service.ImportAsync(1, 7, "tester");

        result.TotalRead.Should().Be(1);
        result.Unchanged.Should().Be(1);
        absent.IsActive.Should().BeTrue();
        await sender.DidNotReceive().Send(Arg.Any<UpdateCityCommand>(), Arg.Any<CancellationToken>());
        await logRepository.Received(1).CreateAsync(
            Arg.Is<CreateSapSyncLogData>(log => log.Status == "Succeeded" && log.ErrorMessage == null),
            Arg.Any<CancellationToken>());
    }

    private static SapCityImportService Service(
        ISapCityReader reader,
        IGeographyRepository repository,
        ISapSyncLogRepository logRepository,
        ISender sender) =>
        new(reader, new SapCityRecordProcessor(repository, sender), logRepository);

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

    private static ProvinceDto Province(int id, CountryDto country, string externalCode) => new()
    {
        Id = id,
        GlobalId = Guid.NewGuid(),
        CountryId = country.Id,
        CountryGlobalId = country.GlobalId,
        CountryCode = country.Code,
        CountryName = country.Name,
        Code = $"LOCAL-{externalCode}",
        Name = $"Provincia {externalCode}",
        ExternalSystem = "SAP_B1",
        ExternalCode = $"EC|{externalCode}",
        IsActive = true
    };

    private static CityDto City(
        int id,
        CountryDto country,
        ProvinceDto province,
        string code,
        string name,
        string? externalCode) => new()
    {
        Id = id,
        GlobalId = Guid.NewGuid(),
        CountryId = country.Id,
        CountryGlobalId = country.GlobalId,
        CountryCode = country.Code,
        CountryName = country.Name,
        ProvinceId = province.Id,
        ProvinceGlobalId = province.GlobalId,
        ProvinceCode = province.Code,
        ProvinceName = province.Name,
        Code = code,
        Name = name,
        ExternalSystem = externalCode is null ? null : "SAP_B1",
        ExternalCode = externalCode,
        IsActive = true
    };
}
