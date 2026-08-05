using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Provinces.Handlers;
using NuanSystem.Application.Features.SapSync.Provinces.Services;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Tests.Features.SapSync.Provinces;

public sealed class SapProvinceSyncHandlerTests
{
    [Fact]
    public async Task NoRows_ReturnsSkippedAndUsesProvincesEntityCode()
    {
        var reader = Substitute.For<ISapProvinceReader>();
        reader.GetProvincesAsync(10, Arg.Any<CancellationToken>()).Returns([]);
        var handler = new SapProvinceSyncHandler(
            reader,
            new SapProvinceRecordProcessor(
                Substitute.For<IGeographyRepository>(), Substitute.For<ISender>()));

        var result = await handler.ImportFromSapAsync(Context());

        handler.EntityCode.Should().Be(SapSyncEntityCode.Provinces);
        result.Status.Should().Be(SapSyncStatus.Skipped);
        result.ProcessedCount.Should().Be(0);
    }

    [Fact]
    public async Task Export_RemainsOutsideApprovedSapToErpScope()
    {
        var handler = new SapProvinceSyncHandler(
            Substitute.For<ISapProvinceReader>(),
            new SapProvinceRecordProcessor(
                Substitute.For<IGeographyRepository>(), Substitute.For<ISender>()));

        var result = await handler.ExportToSapAsync(Context());

        result.Status.Should().Be(SapSyncStatus.NotImplemented);
    }

    private static NuanSystem.Application.Features.SapSync.Dtos.SapSyncExecutionContext Context() => new(
        CompanyId: 10,
        CompanyCode: "DEMO",
        EntityCode: SapSyncEntityCode.Provinces,
        Direction: SapSyncDirection.SapToErp,
        Operation: SapSyncOperation.Import,
        WorkerInstance: "worker-01",
        CorrelationId: Guid.NewGuid().ToString(),
        AttemptCount: 0,
        StartedAtUtc: DateTime.UtcNow);
}
