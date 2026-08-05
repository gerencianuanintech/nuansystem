using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Countries.Handlers;
using NuanSystem.Application.Features.SapSync.Countries.Services;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Tests.Features.SapSync.Countries;

public sealed class SapCountrySyncHandlerTests
{
    [Fact]
    public async Task NoRows_ReturnsSkippedAndUsesCountriesEntityCode()
    {
        var reader = Substitute.For<ISapCountryReader>();
        reader.GetCountriesAsync(10, Arg.Any<CancellationToken>()).Returns([]);
        var handler = new SapCountrySyncHandler(
            reader,
            new SapCountryRecordProcessor(
                Substitute.For<IGeographyRepository>(), Substitute.For<ISender>()));

        var result = await handler.ImportFromSapAsync(Context());

        handler.EntityCode.Should().Be(SapSyncEntityCode.Countries);
        result.Status.Should().Be(SapSyncStatus.Skipped);
        result.ProcessedCount.Should().Be(0);
    }

    [Fact]
    public async Task Export_RemainsOutsideApprovedSapToErpScope()
    {
        var handler = new SapCountrySyncHandler(
            Substitute.For<ISapCountryReader>(),
            new SapCountryRecordProcessor(
                Substitute.For<IGeographyRepository>(), Substitute.For<ISender>()));

        var result = await handler.ExportToSapAsync(Context());

        result.Status.Should().Be(SapSyncStatus.NotImplemented);
    }

    private static NuanSystem.Application.Features.SapSync.Dtos.SapSyncExecutionContext Context() => new(
        CompanyId: 10,
        CompanyCode: "DEMO",
        EntityCode: SapSyncEntityCode.Countries,
        Direction: SapSyncDirection.SapToErp,
        Operation: SapSyncOperation.Import,
        WorkerInstance: "worker-01",
        CorrelationId: Guid.NewGuid().ToString(),
        AttemptCount: 0,
        StartedAtUtc: DateTime.UtcNow);
}
