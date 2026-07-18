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

public sealed class ProvinceSyncPublishingTests
{
    private readonly IGeographyRepository _repository = Substitute.For<IGeographyRepository>();
    private readonly ISyncEventPublisher _publisher = Substitute.For<ISyncEventPublisher>();
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();

    [Fact]
    public async Task Create_PublishesProvinceWithCountryGlobalIdAndCompositeCode()
    {
        SyncPublishRequest? captured = null;
        var province = CreateProvince();
        ConfigureCompany();
        ConfigurePublisher(request => captured = request);
        _repository.ProvinceCodeExistsAsync(province.CountryId, "AZU", null, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateProvinceAsync(Arg.Any<SaveProvinceData>(), Arg.Any<CancellationToken>()).Returns(province.Id);
        _repository.GetProvinceByIdAsync(province.Id, Arg.Any<CancellationToken>()).Returns(province);
        var handler = new CreateProvinceCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new CreateProvinceCommand(province.CountryId, "AZU", "Azuay", true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityName.Should().Be("Provinces");
        captured.EntityGlobalId.Should().Be(province.GlobalId);
        captured.EntityCode.Should().Be("EC|AZU");
        var payload = captured.Payload.Should().BeOfType<ProvinceSyncPayload>().Subject;
        payload.CountryGlobalId.Should().Be(province.CountryGlobalId);
        payload.CountryCode.Should().Be("EC");
    }

    [Fact]
    public async Task Update_InactiveProvince_PublishesDisabledOperation()
    {
        SyncPublishRequest? captured = null;
        var current = CreateProvince();
        var inactive = CreateProvince(isActive: false, globalId: current.GlobalId, countryGlobalId: current.CountryGlobalId);
        ConfigureCompany();
        ConfigurePublisher(request => captured = request);
        _repository.GetProvinceByIdAsync(current.Id, Arg.Any<CancellationToken>()).Returns(current, inactive);
        _repository.ProvinceCodeExistsAsync(current.CountryId, "AZU", current.Id, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateProvinceAsync(Arg.Any<SaveProvinceData>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new UpdateProvinceCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new UpdateProvinceCommand(current.Id, current.CountryId, "AZU", "Azuay", false, 7, "admin"),
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

    private static ProvinceDto CreateProvince(
        bool isActive = true,
        Guid? globalId = null,
        Guid? countryGlobalId = null)
    {
        return new ProvinceDto
        {
            Id = 2,
            GlobalId = globalId ?? Guid.NewGuid(),
            CountryId = 1,
            CountryGlobalId = countryGlobalId ?? Guid.NewGuid(),
            CountryCode = "EC",
            CountryName = "Ecuador",
            Code = "AZU",
            Name = "Azuay",
            IsActive = isActive,
            CreatedAt = new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = isActive ? null : new DateTime(2026, 7, 16, 11, 0, 0, DateTimeKind.Utc)
        };
    }
}
