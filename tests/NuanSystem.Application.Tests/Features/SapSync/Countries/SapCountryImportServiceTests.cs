using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Countries.Commands;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;
using NuanSystem.Application.Features.SapSync.Countries.Services;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Tests.Features.SapSync.Countries;

public sealed class SapCountryImportServiceTests
{
    [Fact]
    public async Task Preview_CodeWithoutConfirmedSapIdentity_RequiresApproval()
    {
        var reader = Substitute.For<ISapCountryReader>();
        var repository = Substitute.For<IGeographyRepository>();
        reader.GetCountriesAsync(1, Arg.Any<CancellationToken>())
            .Returns([new SapCountryRecord("EC", "Ecuador", "EC", "ECU")]);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>())
            .Returns([Local("EC", null, null)]);
        var service = Service(reader, repository, Substitute.For<ISapSyncLogRepository>(), Substitute.For<ISender>());

        var result = await service.PreviewAsync(1);

        result.Should().ContainSingle().Which.Should().Match<SapCountryPreviewItemDto>(item =>
            item.Status == SapSyncExecutionDetailStatuses.ApprovalRequired
            && item.ResultCode == SapCountryResultCodes.CodeCollisionApprovalRequired);
    }

    [Fact]
    public async Task Import_ProcessesFullReaderResultWithoutDeactivatingAbsentLocalCountries()
    {
        var reader = Substitute.For<ISapCountryReader>();
        var repository = Substitute.For<IGeographyRepository>();
        var logRepository = Substitute.For<ISapSyncLogRepository>();
        var sender = Substitute.For<ISender>();
        var ec = Local("EC", "SAP_B1", "EC");
        ec.Iso2 = "EC";
        ec.Iso3 = "ECU";
        var localAbsentFromSap = Local("CO", "SAP_B1", "CO");
        reader.GetCountriesAsync(1, Arg.Any<CancellationToken>())
            .Returns([new SapCountryRecord("EC", "Ecuador", "EC", "ECU")]);
        repository.GetCountriesAsync(Arg.Any<CancellationToken>())
            .Returns([ec, localAbsentFromSap]);
        var service = Service(reader, repository, logRepository, sender);

        var result = await service.ImportAsync(1, 7, "tester");

        result.TotalRead.Should().Be(1);
        result.Unchanged.Should().Be(1);
        localAbsentFromSap.IsActive.Should().BeTrue();
        await sender.DidNotReceive().Send(Arg.Any<UpdateCountryCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<CreateCountryCommand>(), Arg.Any<CancellationToken>());
        await logRepository.Received(1).CreateAsync(
            Arg.Is<CreateSapSyncLogData>(log => log.Status == "Succeeded" && log.ErrorMessage == null),
            Arg.Any<CancellationToken>());
    }

    private static SapCountryImportService Service(
        ISapCountryReader reader,
        IGeographyRepository repository,
        ISapSyncLogRepository logRepository,
        ISender sender) =>
        new(reader, new SapCountryRecordProcessor(repository, sender), logRepository);

    private static CountryDto Local(string code, string? externalSystem, string? externalCode) => new()
    {
        Id = code == "EC" ? 1 : 2,
        GlobalId = Guid.NewGuid(),
        Code = code,
        Name = code == "EC" ? "Ecuador" : "Colombia",
        ExternalSystem = externalSystem,
        ExternalCode = externalCode,
        IsActive = true
    };
}
