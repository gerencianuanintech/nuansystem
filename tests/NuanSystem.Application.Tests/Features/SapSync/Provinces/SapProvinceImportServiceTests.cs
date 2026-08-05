using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Commands;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;
using NuanSystem.Application.Features.SapSync.Provinces.Services;

namespace NuanSystem.Application.Tests.Features.SapSync.Provinces;

public sealed class SapProvinceImportServiceTests
{
    [Fact]
    public async Task Preview_CodeCollisionIsScopedToResolvedCountry()
    {
        var reader = Substitute.For<ISapProvinceReader>();
        var repository = Substitute.For<IGeographyRepository>();
        var ec = Country(1, "EC");
        var pe = Country(2, "PE");
        reader.GetProvincesAsync(1, Arg.Any<CancellationToken>())
            .Returns([new SapProvinceRecord("EC", "01", "Azuay")]);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([ec, pe]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>())
            .Returns([Province(10, pe, "01", externalCode: null)]);
        var service = Service(reader, repository, Substitute.For<ISapSyncLogRepository>(), Substitute.For<ISender>());

        var result = await service.PreviewAsync(1);

        result.Should().ContainSingle().Which.Status.Should().Be("New");
    }

    [Fact]
    public async Task Import_UsesFullResultAndDoesNotDeactivateAbsentProvince()
    {
        var reader = Substitute.For<ISapProvinceReader>();
        var repository = Substitute.For<IGeographyRepository>();
        var logRepository = Substitute.For<ISapSyncLogRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country(1, "EC");
        var present = Province(10, country, "AZU", "EC|AZU");
        var absent = Province(11, country, "PIC", "EC|PIC");
        reader.GetProvincesAsync(1, Arg.Any<CancellationToken>())
            .Returns([new SapProvinceRecord("EC", "AZU", "Provincia AZU")]);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([present, absent]);
        var service = Service(reader, repository, logRepository, sender);

        var result = await service.ImportAsync(1, 7, "tester");

        result.TotalRead.Should().Be(1);
        result.Unchanged.Should().Be(1);
        absent.IsActive.Should().BeTrue();
        await sender.DidNotReceive().Send(Arg.Any<UpdateProvinceCommand>(), Arg.Any<CancellationToken>());
        await logRepository.Received(1).CreateAsync(
            Arg.Is<CreateSapSyncLogData>(log => log.Status == "Succeeded" && log.ErrorMessage == null),
            Arg.Any<CancellationToken>());
    }

    private static SapProvinceImportService Service(
        ISapProvinceReader reader,
        IGeographyRepository repository,
        ISapSyncLogRepository logRepository,
        ISender sender) =>
        new(reader, new SapProvinceRecordProcessor(repository, sender), logRepository);

    private static CountryDto Country(int id, string externalCode) => new()
    {
        Id = id,
        GlobalId = Guid.NewGuid(),
        Code = $"LOCAL-{externalCode}",
        Name = externalCode,
        ExternalSystem = "SAP_B1",
        ExternalCode = externalCode,
        IsActive = true
    };

    private static ProvinceDto Province(
        int id,
        CountryDto country,
        string code,
        string? externalCode) => new()
        {
            Id = id,
            GlobalId = Guid.NewGuid(),
            CountryId = country.Id,
            CountryGlobalId = country.GlobalId,
            CountryCode = country.Code,
            Code = code,
            Name = $"Provincia {code}",
            ExternalSystem = externalCode is null ? null : "SAP_B1",
            ExternalCode = externalCode,
            IsActive = true
        };
}
