using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Cities.Handlers;
using NuanSystem.Application.Features.SapSync.Cities.Services;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Tests.Features.SapSync.Cities;

public sealed class SapCitySyncHandlerTests
{
    [Fact]
    public async Task NoRows_ReturnsSkippedAndUsesCitiesEntityCode()
    {
        var reader = Substitute.For<ISapCityReader>();
        reader.GetCitiesAsync(10, Arg.Any<CancellationToken>()).Returns([]);
        var handler = new SapCitySyncHandler(
            reader,
            new SapCityRecordProcessor(
                Substitute.For<IGeographyRepository>(), Substitute.For<ISender>()));

        var result = await handler.ImportFromSapAsync(Context());

        handler.EntityCode.Should().Be(SapSyncEntityCode.Cities);
        result.Status.Should().Be(SapSyncStatus.Skipped);
        result.ProcessedCount.Should().Be(0);
    }

    [Fact]
    public async Task Export_RemainsOutsideApprovedSapToErpScope()
    {
        var handler = new SapCitySyncHandler(
            Substitute.For<ISapCityReader>(),
            new SapCityRecordProcessor(
                Substitute.For<IGeographyRepository>(), Substitute.For<ISender>()));

        var result = await handler.ExportToSapAsync(Context());

        result.Status.Should().Be(SapSyncStatus.NotImplemented);
    }

    private static SapSyncExecutionContext Context() => new(
        CompanyId: 10,
        CompanyCode: "DEMO",
        EntityCode: SapSyncEntityCode.Cities,
        Direction: SapSyncDirection.SapToErp,
        Operation: SapSyncOperation.Import,
        WorkerInstance: "worker-01",
        CorrelationId: Guid.NewGuid().ToString(),
        AttemptCount: 0,
        StartedAtUtc: DateTime.UtcNow);
}
