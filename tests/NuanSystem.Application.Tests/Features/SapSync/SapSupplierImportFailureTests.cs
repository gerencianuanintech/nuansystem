using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.SapSync.Commands;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Handlers;
using NuanSystem.Application.Features.SapSync.Services;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSupplierImportFailureTests
{
    private const string SensitiveError = "Server=private-host;Password=do-not-disclose; SAP cookie=private-cookie";
    private const string ErrorCode = "SUPPLIER_IMPORT_FAILED";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Persistence_failure_is_safe_and_correlated_in_result_Inbox_and_public_log()
    {
        using var fixture = new Fixture();
        fixture.FailFirstRecord();

        var result = await fixture.Service.ImportAsync(10, fixture.Options);

        Assert.Equal(1, result.Failed);
        Assert.Equal(1, result.Created);
        var failed = Assert.Single(result.Items, item => item.Status == "Failed");
        var json = JsonSerializer.SerializeToElement(failed, JsonOptions);
        Assert.Equal(ErrorCode, json.GetProperty("errorCode").GetString());
        Assert.Equal(fixture.Options.CorrelationId, json.GetProperty("correlationId").GetString());
        Assert.Contains(fixture.Options.CorrelationId, failed.Message);
        Assert.DoesNotContain(SensitiveError, JsonSerializer.Serialize(result, JsonOptions));
        await fixture.Inbox.Received(1).MarkFailedAsync(1, ErrorCode,
            Arg.Is<string>(message => !message.Contains("private-host") && message.Contains(fixture.Options.CorrelationId)),
            null, Arg.Any<CancellationToken>());
        await fixture.PublicLog.Received(1).CreateAsync(Arg.Is<CreateSapSyncLogData>(data =>
            data.Status == "Failed" && data.ResponseJson != null && !data.ResponseJson.Contains("private-host")),
            Arg.Any<CancellationToken>());
        var diagnostic = Assert.Single(fixture.Logger.Entries);
        Assert.Equal(LogLevel.Error, diagnostic.Level);
        Assert.Equal(fixture.Options.CorrelationId, diagnostic.Properties["CorrelationId"]);
        Assert.Equal(ErrorCode, diagnostic.Properties["ErrorCode"]);
        Assert.Equal(10, diagnostic.Properties["CompanyId"]);
        Assert.Equal(typeof(InvalidOperationException).FullName, diagnostic.Properties["ExceptionType"]);
        Assert.Null(diagnostic.Exception);
        Assert.DoesNotContain("private-host", diagnostic.Message);
        Assert.DoesNotContain("private-host", JsonSerializer.Serialize(diagnostic.Properties));
        Assert.Empty(fixture.Watermark.ReceivedCalls());
    }

    [Fact]
    public async Task Manual_handler_retains_HTTP_200_and_explicit_partial_result_without_raw_error()
    {
        using var fixture = new Fixture();
        fixture.FailFirstRecord();
        var context = Substitute.For<ICompanyContext>();
        context.HasActiveCompany.Returns(true);
        context.CurrentCompany.Returns(new CompanyConnectionInfo(
            10, "TEST", "Test", DatabaseEngine.SqlServer, string.Empty, SapIntegrationMode.None));
        var handler = new ImportSuppliersFromSapCommandHandler(context, fixture.Service);

        var result = await handler.Handle(new ImportSuppliersFromSapCommand(1, "tester"), CancellationToken.None);
        var response = result.ToHttpResult();

        Assert.Equal(StatusCodes.Status200OK, Assert.IsAssignableFrom<IStatusCodeHttpResult>(response).StatusCode);
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Failed);
        Assert.Equal(1, result.Value.Created);
        Assert.Contains("errores", result.Message);
        var envelope = JsonSerializer.Serialize(Assert.IsAssignableFrom<IValueHttpResult>(response).Value, JsonOptions);
        Assert.DoesNotContain("private-host", envelope);
        Assert.Contains(ErrorCode, envelope);
    }

    [Fact]
    public async Task Worker_handler_marks_partial_failure_without_exposing_persistence_details()
    {
        using var fixture = new Fixture();
        fixture.FailFirstRecord();
        var handler = new SapSupplierSyncHandler(fixture.Service);

        var result = await handler.ImportFromSapAsync(WorkerContext());

        Assert.Equal(SapSyncStatus.Failed, result.Status);
        Assert.Equal(1, result.FailedCount);
        Assert.Equal(ErrorCode, result.ErrorCode);
        Assert.DoesNotContain("private-host", JsonSerializer.Serialize(result));
        await fixture.Watermark.DidNotReceive().UpsertSuccessAsync(Arg.Any<int>(), Arg.Any<string>(),
            Arg.Any<SapSyncDirection>(), Arg.Any<DateTime>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Repository_cancellation_propagates_without_failed_item_summary_or_next_record(bool cancelCaller)
    {
        using var fixture = new Fixture();
        using var cancellation = new CancellationTokenSource();
        fixture.ImportRepository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                if (cancelCaller) cancellation.Cancel();
                return Task.FromException<BusinessPartnerSapImportResultData>(new OperationCanceledException(cancellation.Token));
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            fixture.Service.ImportAsync(10, fixture.Options, cancellation.Token));

        await fixture.ImportRepository.Received(1).ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), cancellation.Token);
        Assert.Empty(fixture.PublicLog.ReceivedCalls());
        Assert.Empty(fixture.Watermark.ReceivedCalls());
        Assert.DoesNotContain(fixture.Inbox.ReceivedCalls(), call => call.GetMethodInfo().Name.StartsWith("Mark"));
        Assert.Empty(fixture.Logger.Entries);
    }

    [Fact]
    public async Task Already_cancelled_import_performs_no_reader_or_repository_work()
    {
        using var fixture = new Fixture();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            fixture.Service.ImportAsync(10, fixture.Options, cancellation.Token));

        Assert.Empty(fixture.Reader.ReceivedCalls());
        Assert.Empty(fixture.Repository.ReceivedCalls());
        Assert.Empty(fixture.ImportRepository.ReceivedCalls());
        Assert.Empty(fixture.PublicLog.ReceivedCalls());
        Assert.Empty(fixture.Inbox.ReceivedCalls());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task Cancellation_after_commit_is_observed_even_if_repository_ignores_token(int recordCount)
    {
        using var fixture = new Fixture(recordCount);
        using var cancellation = new CancellationTokenSource();
        fixture.ImportRepository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                cancellation.Cancel();
                return Task.FromResult(new BusinessPartnerSapImportResultData(1, "Created", "Creado."));
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            fixture.Service.ImportAsync(10, fixture.Options with { UseIncrementalWatermark = true }, cancellation.Token));

        await fixture.ImportRepository.Received(1).ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), cancellation.Token);
        Assert.DoesNotContain(fixture.Watermark.ReceivedCalls(), call => call.GetMethodInfo().Name == "UpsertSuccessAsync");
        Assert.Empty(fixture.PublicLog.ReceivedCalls());
    }

    [Fact]
    public async Task Cancellation_during_final_public_log_prevents_success_and_watermark_advance()
    {
        using var fixture = new Fixture(1);
        using var cancellation = new CancellationTokenSource();
        fixture.PublicLog.CreateAsync(Arg.Any<CreateSapSyncLogData>(), Arg.Any<CancellationToken>())
            .Returns(_ => { cancellation.Cancel(); return Task.FromResult(1L); });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            fixture.Service.ImportAsync(10, fixture.Options with { UseIncrementalWatermark = true }, cancellation.Token));

        Assert.DoesNotContain(fixture.Watermark.ReceivedCalls(), call => call.GetMethodInfo().Name == "UpsertSuccessAsync");
    }

    [Fact]
    public async Task Cancelled_empty_batch_does_not_return_a_success_summary()
    {
        using var fixture = new Fixture(0);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            fixture.Service.ImportBatchAsync(10, [], fixture.Options, cancellation.Token));

        Assert.Empty(fixture.Repository.ReceivedCalls());
        Assert.Empty(fixture.ImportRepository.ReceivedCalls());
        Assert.Empty(fixture.PublicLog.ReceivedCalls());
    }

    [Fact]
    public async Task Single_record_failure_has_safe_message_and_its_own_reference()
    {
        using var fixture = new Fixture(1);
        fixture.FailFirstRecord();

        var result = await fixture.Service.ImportOneAsync(Supplier("P001"), 1, "tester");

        var json = JsonSerializer.SerializeToElement(result, JsonOptions);
        Assert.Equal("Failed", result.Status);
        Assert.Equal(ErrorCode, json.GetProperty("errorCode").GetString());
        var correlation = json.GetProperty("correlationId").GetString();
        Assert.True(Guid.TryParse(correlation, out _));
        Assert.Contains(correlation!, result.Message);
        Assert.DoesNotContain("private-host", json.ToString());
        Assert.Equal(correlation, Assert.Single(fixture.Logger.Entries).Properties["CorrelationId"]);
    }

    [Fact]
    public async Task Cancellation_after_watermark_read_prevents_the_next_external_read()
    {
        using var fixture = new Fixture();
        using var cancellation = new CancellationTokenSource();
        fixture.Watermark.GetAsync(10, SapSyncEntityCode.Suppliers, SapSyncDirection.SapToErp, cancellation.Token)
            .Returns(_ => { cancellation.Cancel(); return Task.FromResult<SapSyncWatermarkDto?>(null); });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => fixture.Service.ImportAsync(
            10, fixture.Options with { UseIncrementalWatermark = true }, cancellation.Token));

        Assert.Empty(fixture.Reader.ReceivedCalls());
        Assert.Empty(fixture.Repository.ReceivedCalls());
        Assert.Empty(fixture.ImportRepository.ReceivedCalls());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Manual_and_worker_handlers_propagate_cancellation_without_returning_a_result(bool worker)
    {
        using var fixture = new Fixture();
        using var cancellation = new CancellationTokenSource();
        fixture.ImportRepository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), cancellation.Token)
            .Returns(_ => { cancellation.Cancel(); return Task.FromException<BusinessPartnerSapImportResultData>(new OperationCanceledException(cancellation.Token)); });

        if (worker)
        {
            var handler = new SapSupplierSyncHandler(fixture.Service);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.ImportFromSapAsync(WorkerContext(), cancellation.Token));
        }
        else
        {
            var context = Substitute.For<ICompanyContext>();
            context.HasActiveCompany.Returns(true);
            context.CurrentCompany.Returns(new CompanyConnectionInfo(
                10, "TEST", "Test", DatabaseEngine.SqlServer, string.Empty, SapIntegrationMode.None));
            var handler = new ImportSuppliersFromSapCommandHandler(context, fixture.Service);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.Handle(new ImportSuppliersFromSapCommand(), cancellation.Token));
        }

        Assert.Empty(fixture.PublicLog.ReceivedCalls());
        Assert.Empty(fixture.Logger.Entries);
    }

    [Fact]
    public async Task Empty_batch_reference_is_generated_once_and_shared_with_Inbox_and_diagnostics()
    {
        using var fixture = new Fixture(1);
        fixture.FailFirstRecord();

        var result = await fixture.Service.ImportAsync(10, fixture.Options with { CorrelationId = string.Empty });

        var json = JsonSerializer.SerializeToElement(Assert.Single(result.Items), JsonOptions);
        var correlation = json.GetProperty("correlationId").GetString();
        Assert.True(Guid.TryParse(correlation, out _));
        Assert.Equal(correlation, Assert.Single(fixture.Logger.Entries).Properties["CorrelationId"]);
        await fixture.Inbox.Received(1).UpsertSupplierAsync(10, "P001", Arg.Any<string>(), SapSyncStatus.Processing,
            fixture.Options.WorkerInstance, correlation!, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("Reader")]
    [InlineData("LocalRepository")]
    [InlineData("PublicLog")]
    public async Task Failures_outside_the_row_boundary_propagate_and_never_advance_watermark(string stage)
    {
        using var fixture = new Fixture(1);
        var failure = new InvalidOperationException("Unavailable test infrastructure");
        if (stage == "Reader")
            fixture.Reader.GetSuppliersAsync(10, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<IReadOnlyCollection<SapSupplierRecord>>(failure));
        else if (stage == "LocalRepository")
            fixture.Repository.GetAllAsync(null, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<IReadOnlyCollection<BusinessPartnerDto>>(failure));
        else
            fixture.PublicLog.CreateAsync(Arg.Any<CreateSapSyncLogData>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException<long>(failure));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Service.ImportAsync(
            10, fixture.Options with { UseIncrementalWatermark = true }));

        Assert.Same(failure, exception);
        Assert.DoesNotContain(fixture.Watermark.ReceivedCalls(), call => call.GetMethodInfo().Name == "UpsertSuccessAsync");
    }

    [Fact]
    public void Existing_success_result_keeps_its_JSON_fields_without_new_null_error_fields()
    {
        var item = new SapSupplierImportItemResultDto("P001", "Proveedor", "Created", "Creado.", 1);
        var json = JsonSerializer.SerializeToElement(item, JsonOptions);

        Assert.Equal(5, json.EnumerateObject().Count());
        Assert.Equal("Created", json.GetProperty("status").GetString());
        Assert.False(json.TryGetProperty("errorCode", out _));
        Assert.False(json.TryGetProperty("correlationId", out _));
    }

    private static SapSupplierRecord Supplier(string code) => new(code, "Proveedor", "099" + code,
        "S", null, null, null, "USD", true, null, null);

    private static SapSyncExecutionContext WorkerContext() => new(10, "TEST", SapSyncEntityCode.Suppliers,
        SapSyncDirection.SapToErp, SapSyncOperation.Import, "test-worker", "worker-reference", 0, DateTime.UtcNow);

    private sealed class Fixture : IDisposable
    {
        private readonly ServiceProvider provider;
        public ISapSupplierReader Reader { get; } = Substitute.For<ISapSupplierReader>();
        public IBusinessPartnerRepository Repository { get; } = Substitute.For<IBusinessPartnerRepository>();
        public ISapSupplierImportRepository ImportRepository { get; } = Substitute.For<ISapSupplierImportRepository>();
        public ISapSyncLogRepository PublicLog { get; } = Substitute.For<ISapSyncLogRepository>();
        public ISapSyncInboxRepository Inbox { get; } = Substitute.For<ISapSyncInboxRepository>();
        public ISapSyncWatermarkService Watermark { get; } = Substitute.For<ISapSyncWatermarkService>();
        public RecordingLogger Logger { get; } = new();
        public SapSupplierImportOptions Options { get; } = new(1, "tester", true, true, false, "test-worker", "batch-reference");
        public ISapSupplierImportService Service { get; }

        public Fixture(int recordCount = 2)
        {
            Reader.GetSuppliersAsync(10, Arg.Any<CancellationToken>())
                .Returns(Enumerable.Range(1, recordCount).Select(index => Supplier($"P{index:000}")).ToArray());
            Repository.GetAllAsync(null, Arg.Any<CancellationToken>()).Returns(Array.Empty<BusinessPartnerDto>());
            ImportRepository.ImportSupplierFromSapAsync(Arg.Any<BusinessPartnerSapImportData>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new BusinessPartnerSapImportResultData(2, "Created", "Creado.")));
            Inbox.UpsertSupplierAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<SapSyncStatus>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(1L);
            provider = new ServiceCollection().AddSingleton(Reader).AddSingleton(Repository).AddSingleton(ImportRepository).AddSingleton(PublicLog)
                .AddSingleton(Inbox).AddSingleton(Watermark).AddSingleton<ILogger<SapSupplierImportService>>(Logger)
                .AddTransient<ISapSupplierImportService, SapSupplierImportService>().BuildServiceProvider();
            Service = provider.GetRequiredService<ISapSupplierImportService>();
        }

        public void FailFirstRecord() => ImportRepository.ImportSupplierFromSapAsync(
            Arg.Is<BusinessPartnerSapImportData>(item => item.CardCode == "P001"), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<BusinessPartnerSapImportResultData>(new InvalidOperationException(SensitiveError)));

        public void Dispose() => provider.Dispose();
    }

    private sealed class RecordingLogger : ILogger<SapSupplierImportService>
    {
        public List<LogEntry> Entries { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Entries.Add(new(logLevel, exception, formatter(state, exception),
                ((IEnumerable<KeyValuePair<string, object?>>)state!).ToDictionary(item => item.Key, item => item.Value)));
    }

    private sealed record LogEntry(LogLevel Level, Exception? Exception, string Message, Dictionary<string, object?> Properties);
}
