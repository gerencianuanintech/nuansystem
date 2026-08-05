using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Countries.Commands;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;
using NuanSystem.Application.Features.SapSync.Countries.Services;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Tests.Features.SapSync.Countries;

public sealed class SapCountryRetryProcessorTests
{
    [Fact]
    public async Task ValidCountryV1Snapshot_IsReprocessedFromApprovedPayload()
    {
        var repository = Substitute.For<IGeographyRepository>();
        var sender = Substitute.For<ISender>();
        repository.GetCountriesAsync(Arg.Any<CancellationToken>()).Returns([]);
        var globalId = Guid.NewGuid();
        sender.Send(Arg.Any<CreateCountryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CountryDto>.Success(new CountryDto
            {
                Id = 21,
                GlobalId = globalId,
                Code = "EC",
                Name = "Ecuador",
                ExternalSystem = "SAP_B1",
                ExternalCode = "EC",
                IsActive = true
            }));
        var processor = new SapCountryExecutionRetryProcessor(
            new SapCountryRecordProcessor(repository, sender));

        var result = await processor.ProcessAsync(Claim(new("EC", "Ecuador", "EC", "ECU")));

        processor.ApprovedSnapshotType.Should().Be(SapSyncApprovedSnapshotTypes.CountryV1);
        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Created);
        result.LocalEntityId.Should().Be(21);
        result.LocalGlobalId.Should().Be(globalId);
    }

    [Fact]
    public async Task SnapshotIdentityDifferentFromClaim_IsTerminalConflictWithoutWrite()
    {
        var sender = Substitute.For<ISender>();
        var processor = new SapCountryExecutionRetryProcessor(
            new SapCountryRecordProcessor(Substitute.For<IGeographyRepository>(), sender));
        var claim = Claim(new("PE", "Peru", "PE", "PER")) with { SourceRecordKey = "EC" };

        var result = await processor.ProcessAsync(claim);

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapCountryResultCodes.SnapshotInvalid);
        await sender.DidNotReceive().Send(Arg.Any<CreateCountryCommand>(), Arg.Any<CancellationToken>());
        await sender.DidNotReceive().Send(Arg.Any<UpdateCountryCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MalformedSnapshot_IsTerminalAndDoesNotLeakPayload()
    {
        const string json = "{invalid secret-password}";
        var processor = new SapCountryExecutionRetryProcessor(
            new SapCountryRecordProcessor(
                Substitute.For<IGeographyRepository>(), Substitute.For<ISender>()));
        var claim = new SapSyncExecutionDetailClaim(
            1, Guid.NewGuid(), "EC", SapSyncExecutionDetailStatuses.Processing, 1, 3,
            SapSyncApprovedSnapshotTypes.CountryV1, json,
            SHA256.HashData(Encoding.UTF8.GetBytes(json)), new string('A', 64),
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1));

        var result = await processor.ProcessAsync(claim);

        result.Status.Should().Be(SapSyncExecutionDetailStatuses.Conflict);
        result.ResultCode.Should().Be(SapCountryResultCodes.SnapshotInvalid);
        result.SafeMessage.Should().NotContain(json);
        result.SafeMessage.Should().NotContain("password");
    }

    private static SapSyncExecutionDetailClaim Claim(SapCountrySnapshot snapshot)
    {
        var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return new(
            1, Guid.NewGuid(), snapshot.CountryCode,
            SapSyncExecutionDetailStatuses.Processing, 1, 3,
            SapSyncApprovedSnapshotTypes.CountryV1, json,
            SHA256.HashData(Encoding.UTF8.GetBytes(json)), new string('B', 64),
            DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1));
    }
}
