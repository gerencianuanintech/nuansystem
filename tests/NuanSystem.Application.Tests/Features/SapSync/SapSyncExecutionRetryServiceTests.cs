using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncExecutionRetryServiceTests
{
    [Fact]
    public async Task WithoutRegisteredProcessor_DoesNotClaim()
    {
        var repository = Substitute.For<ISapSyncExecutionRepository>();
        var service = new SapSyncExecutionRetryService(
            repository, [], Substitute.For<ISapSyncRetryPolicy>());

        var result = await service.ProcessNextAsync("worker", TimeSpan.FromMinutes(1), 5);

        result.Status.Should().Be(SapSyncRetryCycleResult.Idle);
        await repository.DidNotReceiveWithAnyArgs().TryClaimDueDetailAsync(default!, default!, default, default!, default);
    }

    [Fact]
    public async Task ApprovedSnapshot_IsVerifiedAndProcessed()
    {
        const string json = "{\"code\":\"WH-01\"}";
        var repository = Substitute.For<ISapSyncExecutionRepository>();
        var processor = Substitute.For<ISapSyncExecutionRetryProcessor>();
        processor.ApprovedSnapshotType.Returns("WarehouseV1");
        processor.ProcessAsync(Arg.Any<SapSyncExecutionDetailClaim>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionRetryProcessResult("Update", "Updated", 10, Guid.NewGuid(), "OK", "Actualizado."));
        repository.TryClaimDueDetailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionDetailClaim(1, Guid.NewGuid(), "WH-01", "Processing", 1, 3,
                "WarehouseV1", json, SHA256.HashData(Encoding.UTF8.GetBytes(json)), new string('A', 64), DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1)));
        repository.CompleteClaimedDetailAsync(Arg.Any<SapSyncExecutionDetailCompletion>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "Updated", []));

        var service = new SapSyncExecutionRetryService(repository, [processor], Substitute.For<ISapSyncRetryPolicy>());
        var result = await service.ProcessNextAsync("worker", TimeSpan.FromMinutes(1), 5);

        result.Status.Should().Be(SapSyncRetryCycleResult.Completed);
        await repository.Received(1).CompleteClaimedDetailAsync(
            Arg.Is<SapSyncExecutionDetailCompletion>(x => x.Status == "Updated" && x.OwnerToken == new string('A', 64)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InvalidHash_MovesDetailToDeadLetterWithoutCallingProcessor()
    {
        var repository = Substitute.For<ISapSyncExecutionRepository>();
        var processor = Substitute.For<ISapSyncExecutionRetryProcessor>();
        processor.ApprovedSnapshotType.Returns("WarehouseV1");
        repository.TryClaimDueDetailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionDetailClaim(1, Guid.NewGuid(), "WH-01", "Processing", 1, 3,
                "WarehouseV1", "{}", new byte[32], new string('B', 64), DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1)));
        repository.CompleteClaimedDetailAsync(Arg.Any<SapSyncExecutionDetailCompletion>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncExecutionWriteResult(1, "Updated", []));

        var service = new SapSyncExecutionRetryService(repository, [processor], Substitute.For<ISapSyncRetryPolicy>());
        var result = await service.ProcessNextAsync("worker", TimeSpan.FromMinutes(1), 5);

        result.Status.Should().Be(SapSyncRetryCycleResult.DeadLetter);
        await processor.DidNotReceiveWithAnyArgs().ProcessAsync(default!, default);
    }
}
