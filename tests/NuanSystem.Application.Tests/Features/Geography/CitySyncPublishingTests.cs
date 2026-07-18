using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Geography.Commands;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Geography;

public sealed class CitySyncPublishingTests
{
    private readonly IGeographyRepository _repository = Substitute.For<IGeographyRepository>();
    private readonly ISyncEventPublisher _publisher = Substitute.For<ISyncEventPublisher>();
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();

    [Fact]
    public async Task Create_PublishesCityWithBothParentsAndCompositeCode()
    {
        SyncPublishRequest? captured = null;
        var city = CreateCity();
        ConfigureCompany();
        ConfigurePublisher(request => captured = request);
        _repository.CityCodeExistsAsync(city.ProvinceId, "CUE", null, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateCityAsync(Arg.Any<SaveCityData>(), Arg.Any<CancellationToken>()).Returns(city.Id);
        _repository.GetCityByIdAsync(city.Id, Arg.Any<CancellationToken>()).Returns(city);
        var handler = new CreateCityCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new CreateCityCommand(city.CountryId, city.ProvinceId, "CUE", "Cuenca", true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityName.Should().Be("Cities");
        captured.EntityGlobalId.Should().Be(city.GlobalId);
        captured.EntityCode.Should().Be("EC|AZU|CUE");
        var payload = captured.Payload.Should().BeOfType<CitySyncPayload>().Subject;
        payload.CountryGlobalId.Should().Be(city.CountryGlobalId);
        payload.ProvinceGlobalId.Should().Be(city.ProvinceGlobalId);
    }

    [Fact]
    public async Task Update_InactiveCity_PublishesDisabledOperation()
    {
        SyncPublishRequest? captured = null;
        var current = CreateCity();
        var inactive = CreateCity(false, current.GlobalId, current.CountryGlobalId, current.ProvinceGlobalId);
        ConfigureCompany();
        ConfigurePublisher(request => captured = request);
        _repository.GetCityByIdAsync(current.Id, Arg.Any<CancellationToken>()).Returns(current, inactive);
        _repository.CityCodeExistsAsync(current.ProvinceId, "CUE", current.Id, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateCityAsync(Arg.Any<SaveCityData>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new UpdateCityCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new UpdateCityCommand(current.Id, current.CountryId, current.ProvinceId, "CUE", "Cuenca", false, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured!.Operation.Should().Be(SyncOperation.Disabled);
        captured.EntityGlobalId.Should().Be(current.GlobalId);
    }

    private void ConfigurePublisher(Action<SyncPublishRequest> capture)
    {
        _publisher.PublishAsync(Arg.Do(capture), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(true, 1, "Publicado.")));
    }

    private void ConfigureCompany()
    {
        _companyContext.HasActiveCompany.Returns(true);
        _companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            10,
            "MASTER",
            "Empresa Master",
            DatabaseEngine.SqlServer,
            "Server=(local);Database=NuanSystem_Tenant;",
            SapIntegrationMode.None,
            CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: true));
    }

    private static CityDto CreateCity(
        bool isActive = true,
        Guid? globalId = null,
        Guid? countryGlobalId = null,
        Guid? provinceGlobalId = null)
    {
        return new CityDto
        {
            Id = 3,
            GlobalId = globalId ?? Guid.NewGuid(),
            CountryId = 1,
            CountryGlobalId = countryGlobalId ?? Guid.NewGuid(),
            CountryCode = "EC",
            CountryName = "Ecuador",
            ProvinceId = 2,
            ProvinceGlobalId = provinceGlobalId ?? Guid.NewGuid(),
            ProvinceCode = "AZU",
            ProvinceName = "Azuay",
            Code = "CUE",
            Name = "Cuenca",
            IsActive = isActive,
            CreatedAt = new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = isActive ? null : new DateTime(2026, 7, 16, 11, 0, 0, DateTimeKind.Utc)
        };
    }
}
