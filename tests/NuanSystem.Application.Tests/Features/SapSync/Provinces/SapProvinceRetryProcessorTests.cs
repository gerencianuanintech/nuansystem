using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Commands;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;
using NuanSystem.Application.Features.SapSync.Provinces.Services;

namespace NuanSystem.Application.Tests.Features.SapSync.Provinces;

public sealed class SapProvinceRetryProcessorTests
{
    [Fact]
    public async Task ValidProvinceV1Snapshot_IsReprocessedFromApprovedCompositeIdentity()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        var country = Country();
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([country]);
        repository.GetProvincesAsync(Arg.Any<CancellationToken>()).Returns([]);
        var globalId = Guid.NewGuid();
        sender.Send(Arg.Any<CreateProvinceCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<ProvinceDto>.Success(new ProvinceDto
            {
                Id = 21,
                GlobalId = globalId,
                CountryId = country.Id,
                Code = "AZU",
                Name = "Azuay",
                ExternalSystem = "SAP_B1",
                ExternalCode = "EC|AZU",
                IsActive = true
            }));
        var processor = new SapProvinceExecutionRetryProcessor(
            new SapProvinceRecordProcessor(repository, sender));

        var result = await processor.ProcessAsync(Claim(new("EC", "AZU", "Azuay")));

        processor.ApprovedSnapshotType.Should().Be(SapSyncApprovedSnapshotTypes.ProvinceV1);
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        result.LocalEntityId.Should().Be(21);
        result.LocalGlobalId.Should().Be(globalId);
    }

    [Fact]
    public async Task SnapshotCompositeIdentityDifferentFromClaim_IsTerminalWithoutWrite()
    {
        var sender = Substitute.For<ISender>();
        var processor = new SapProvinceExecutionRetryProcessor(
            new SapProvinceRecordProcessor(Substitute.For<IGeographyRepository>(), sender));
        var claim = Claim(new("EC", "AZU", "Azuay")) with { SourceRecordKey = "PE|AZU" };

        var result = await processor.ProcessAsync(claim);

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapProvinceResultCodes.SnapshotInvalid);
        await sender.DidNotReceive().Send(Arg.Any<CreateProvinceCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateProvinceCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MalformedSnapshot_IsTerminalAndDoesNotLeakPayload()
    {
        const string json = "{invalid secret-password}";
        var processor = new SapProvinceExecutionRetryProcessor(
            new SapProvinceRecordProcessor(
                Substitute.For<IGeographyRepository>(), Substitute.For<ISender>()));
        var claim = new SapSyncExecutionDetailClaim(
            1, Guid.NewGuid(), "EC|AZU", SapSyncExecutionDetailStatuses.Processing, 1, 3,
            SapSyncApprovedSnapshotTypes.ProvinceV1, json,
            SHA256.HashData(Encoding.UTF8.GetBytes(json)), new string('A', 64),
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1));

        var result = await processor.ProcessAsync(claim);

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapProvinceResultCodes.SnapshotInvalid);
        result.SafeMessage.Should().NotContain(json);
        result.SafeMessage.Should().NotContain("password");
    }

    private static SapSyncExecutionDetailClaim Claim(SapProvinceSnapshot snapshot)
    {
        var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return new(
            1, Guid.NewGuid(), snapshot.ExternalCode,
            SapSyncExecutionDetailStatuses.Processing, 1, 3,
            SapSyncApprovedSnapshotTypes.ProvinceV1, json,
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
}
