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

public sealed class CountrySyncPublishingTests
{
    private readonly IGeographyRepository _repository = Substitute.For<IGeographyRepository>();
    private readonly ISyncEventPublisher _publisher = Substitute.For<ISyncEventPublisher>();
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();

    [Fact]
    public async Task Create_PublishesCountryWithStableGlobalId()
    {
        SyncPublishRequest? captured = null;
        var country = CreateCountry();
        ConfigureCompany();
        _publisher.PublishAsync(Arg.Do<SyncPublishRequest>(request => captured = request), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(true, 1, "Publicado.")));
        _repository.CountryCodeExistsAsync("EC", null, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateCountryAsync(Arg.Any<SaveCountryData>(), Arg.Any<CancellationToken>()).Returns(country.Id);
        _repository.GetCountryByIdAsync(country.Id, Arg.Any<CancellationToken>()).Returns(country);
        var handler = new CreateCountryCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new CreateCountryCommand("EC", "Ecuador", "EC", "ECU", "+593", true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityName.Should().Be("Countries");
        captured.EntityGlobalId.Should().Be(country.GlobalId);
        captured.EntityCode.Should().Be("EC");
        captured.Operation.Should().Be(SyncOperation.Created);
        captured.Payload.Should().BeOfType<CountrySyncPayload>();
    }

    [Fact]
    public async Task Update_InactiveCountry_PublishesDisabledOperation()
    {
        SyncPublishRequest? captured = null;
        var current = CreateCountry();
        var inactive = CreateCountry(isActive: false, globalId: current.GlobalId);
        ConfigureCompany();
        _publisher.PublishAsync(Arg.Do<SyncPublishRequest>(request => captured = request), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(true, 2, "Publicado.")));
        _repository.GetCountryByIdAsync(current.Id, Arg.Any<CancellationToken>()).Returns(current, inactive);
        _repository.CountryCodeExistsAsync("EC", current.Id, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateCountryAsync(Arg.Any<SaveCountryData>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new UpdateCountryCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new UpdateCountryCommand(current.Id, "EC", "Ecuador", "EC", "ECU", "+593", false, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured!.EntityGlobalId.Should().Be(current.GlobalId);
        captured.Operation.Should().Be(SyncOperation.Disabled);
    }

    [Fact]
    public async Task Create_WithoutActiveCompany_KeepsStandaloneCrudWorking()
    {
        var country = CreateCountry();
        _companyContext.HasActiveCompany.Returns(false);
        _companyContext.CurrentCompany.Returns((CompanyConnectionInfo?)null);
        _repository.CountryCodeExistsAsync("EC", null, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateCountryAsync(Arg.Any<SaveCountryData>(), Arg.Any<CancellationToken>()).Returns(country.Id);
        _repository.GetCountryByIdAsync(country.Id, Arg.Any<CancellationToken>()).Returns(country);
        var handler = new CreateCountryCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new CreateCountryCommand("EC", "Ecuador", "EC", "ECU", "+593", true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _publisher.DidNotReceiveWithAnyArgs().PublishAsync(default!, default);
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

    private static CountryDto CreateCountry(bool isActive = true, Guid? globalId = null)
    {
        return new CountryDto
        {
            Id = 1,
            GlobalId = globalId ?? Guid.NewGuid(),
            Code = "EC",
            Name = "Ecuador",
            Iso2 = "EC",
            Iso3 = "ECU",
            PhonePrefix = "+593",
            IsActive = isActive,
            CreatedAt = new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = isActive ? null : new DateTime(2026, 7, 16, 11, 0, 0, DateTimeKind.Utc)
        };
    }
}
