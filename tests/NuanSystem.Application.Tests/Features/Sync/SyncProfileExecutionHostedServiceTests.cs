using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NuanSystem.Api.Options;
using NuanSystem.Api.Services;
using NuanSystem.Application.Abstractions.Common;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncProfileExecutionHostedServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenStoppingDuringDelay_CompletesNormally()
    {
        using var cancellation = new CancellationTokenSource();
        var executionService = Substitute.For<ISyncProfileExecutionService>();
        var executionRepository = Substitute.For<ISyncProfileExecutionRepository>();
        var scheduleCalculator = Substitute.For<ISyncScheduleCalculator>();
        var clock = Substitute.For<ISystemClock>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var scope = Substitute.For<IServiceScope>();
        var scopeFactory = Substitute.For<IServiceScopeFactory>();

        executionRepository
            .GetDueProfilesAsync(Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<DueSyncProfileDto>());
        executionService
            .ProcessPendingAsync(Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                cancellation.Cancel();
                return Task.CompletedTask;
            });

        serviceProvider.GetService(typeof(ISyncProfileExecutionService)).Returns(executionService);
        serviceProvider.GetService(typeof(ISyncProfileExecutionRepository)).Returns(executionRepository);
        serviceProvider.GetService(typeof(ISyncScheduleCalculator)).Returns(scheduleCalculator);
        serviceProvider.GetService(typeof(ISystemClock)).Returns(clock);
        scope.ServiceProvider.Returns(serviceProvider);
        scopeFactory.CreateScope().Returns(scope);

        var hostedService = new SyncProfileExecutionHostedService(
            scopeFactory,
            Options.Create(new SyncProfileExecutionWorkerOptions
            {
                Enabled = true,
                PollingSeconds = 5
            }),
            Substitute.For<ILogger<SyncProfileExecutionHostedService>>());

        var executeAsync = typeof(SyncProfileExecutionHostedService).GetMethod(
            "ExecuteAsync",
            BindingFlags.Instance | BindingFlags.NonPublic);

        executeAsync.Should().NotBeNull();
        var execution = (Task)executeAsync!.Invoke(hostedService, [cancellation.Token])!;

        var action = async () => await execution;

        await action.Should().NotThrowAsync<OperationCanceledException>();
        await executionService.Received(1).ProcessPendingAsync(Arg.Any<CancellationToken>());
    }
}
