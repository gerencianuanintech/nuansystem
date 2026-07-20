using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sri;
using NuanSystem.SriWorker.Options;
using NuanSystem.SriWorker.Services;

namespace NuanSystem.Application.Tests.Features.SriDocuments;

public sealed class SriWorkerProcessorTests
{
    private readonly ISriWorkerCompanyRepository _companies = Substitute.For<ISriWorkerCompanyRepository>();
    private readonly ISriWorkerQueueRepository _queue = Substitute.For<ISriWorkerQueueRepository>();
    private readonly ISriAuthorizationProvider _provider = Substitute.For<ISriAuthorizationProvider>();
    private readonly ILogger<SriWorkerProcessor> _logger = Substitute.For<ILogger<SriWorkerProcessor>>();

    [Fact]
    public async Task ProcessOnce_DoesNotReadCompanies_WhenWorkerIsDisabled()
    {
        var processor = CreateProcessor(new SriWorkerOptions { Enabled = false });

        var processed = await processor.ProcessOnceAsync();

        processed.Should().Be(0);
        await _companies.DidNotReceive().GetEnabledCompaniesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessOnce_PersistsAuthorizedXmlAndHash()
    {
        var job = Job(attempt: 1);
        ArrangeCompanyAndJob(job);
        _provider.QueryAsync(job.Environment, job.AccessKey, Arg.Any<CancellationToken>()).Returns(new SriAuthorizationResult(
            SriAuthorizationOutcome.Authorized, "AUTH-1", DateTimeOffset.UtcNow, "PRODUCCIÓN", "0999999999001", "01",
            [1, 2, 3], new byte[32]));
        _queue.CompleteAuthorizedAsync(1, Arg.Any<SriAuthorizedDocumentData>(), Arg.Any<CancellationToken>())
            .Returns(SriWorkerCompletionCode.Updated);

        var processed = await CreateProcessor(EnabledOptions()).ProcessOnceAsync();

        processed.Should().Be(1);
        await _queue.Received(1).CompleteAuthorizedAsync(1,
            Arg.Is<SriAuthorizedDocumentData>(value => value.QueueId == job.Id && value.XmlContent.SequenceEqual(new byte[] { 1, 2, 3 }) && value.Sha256.Length == 32),
            Arg.Any<CancellationToken>());
        await _queue.DidNotReceive().CompleteAttemptAsync(Arg.Any<int>(), Arg.Any<SriAttemptCompletionData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessOnce_SchedulesRetry_ForFirstNotFoundResult()
    {
        var job = Job(attempt: 1, createdAt: DateTime.UtcNow);
        ArrangeCompanyAndJob(job);
        _provider.QueryAsync(job.Environment, job.AccessKey, Arg.Any<CancellationToken>())
            .Returns(new SriAuthorizationResult(SriAuthorizationOutcome.NotFound));
        _queue.CompleteAttemptAsync(1, Arg.Any<SriAttemptCompletionData>(), Arg.Any<CancellationToken>())
            .Returns(SriWorkerCompletionCode.Updated);

        await CreateProcessor(EnabledOptions()).ProcessOnceAsync();

        await _queue.Received(1).CompleteAttemptAsync(1,
            Arg.Is<SriAttemptCompletionData>(value => value.Outcome == "Retry" && value.NextAttemptAt.HasValue),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessOnce_ClosesAsNotFound_AfterThreeResponses()
    {
        var job = Job(attempt: 3, createdAt: DateTime.UtcNow.AddMinutes(-5));
        ArrangeCompanyAndJob(job);
        _provider.QueryAsync(job.Environment, job.AccessKey, Arg.Any<CancellationToken>())
            .Returns(new SriAuthorizationResult(SriAuthorizationOutcome.NotFound));
        _queue.CompleteAttemptAsync(1, Arg.Any<SriAttemptCompletionData>(), Arg.Any<CancellationToken>())
            .Returns(SriWorkerCompletionCode.Updated);

        await CreateProcessor(EnabledOptions()).ProcessOnceAsync();

        await _queue.Received(1).CompleteAttemptAsync(1,
            Arg.Is<SriAttemptCompletionData>(value => value.Outcome == "NotFound" && value.NextAttemptAt == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void RetrySchedule_IsDeterministicBoundedAndJittered()
    {
        var options = EnabledOptions();

        var first = SriRetrySchedule.Calculate(21, 2, options);
        var repeated = SriRetrySchedule.Calculate(21, 2, options);
        var anotherQueue = SriRetrySchedule.Calculate(22, 2, options);

        first.Should().Be(repeated);
        first.Should().BeGreaterThan(TimeSpan.Zero).And.BeLessThanOrEqualTo(TimeSpan.FromSeconds(options.MaxRetrySeconds));
        anotherQueue.Should().NotBe(first);
    }

    private void ArrangeCompanyAndJob(SriClaimedDocumentDto job)
    {
        _companies.GetEnabledCompaniesAsync(Arg.Any<CancellationToken>())
            .Returns(new[] { new SriWorkerCompanyDto(1, "DEMO", "Production") });
        _queue.ClaimAsync(1, "Production", Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new[] { job });
    }

    private SriWorkerProcessor CreateProcessor(SriWorkerOptions current)
    {
        var monitor = Substitute.For<IOptionsMonitor<SriWorkerOptions>>();
        monitor.CurrentValue.Returns(current);
        return new SriWorkerProcessor(_companies, _queue, _provider, monitor, _logger);
    }

    private static SriWorkerOptions EnabledOptions() => new()
    {
        Enabled = true,
        WorkerInstance = "test-worker",
        BatchSize = 10,
        MaxConcurrency = 2,
        LeaseSeconds = 120,
        MaxAttempts = 5,
        NotFoundMaxAttempts = 3,
        NotFoundWindowMinutes = 30,
        BaseRetrySeconds = 30,
        MaxRetrySeconds = 900,
        RetryJitterRatio = 0.2
    };

    private static SriClaimedDocumentDto Job(int attempt, DateTime? createdAt = null) => new(21, "Production",
        SriAccessKeyTests.BuildKey("01", '2'), "01", "Manual", "test", null, attempt, 5, Guid.NewGuid(),
        createdAt ?? DateTime.UtcNow, "test-worker", DateTime.UtcNow.AddMinutes(2));
}
