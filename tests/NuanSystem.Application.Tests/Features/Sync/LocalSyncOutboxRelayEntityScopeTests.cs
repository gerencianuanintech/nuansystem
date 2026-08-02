using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Persistence.Repositories.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class LocalSyncOutboxRelayEntityScopeTests
{
    [Fact]
    public async Task ProcessOnceAsync_WithNoEnabledEntities_FailsClosedBeforeTenantDiscovery()
    {
        var repository = Substitute.For<ILocalSyncOutboxRepository>();
        var relay = CreateRelay(
            new MasterBranchSyncWorkerOptions
            {
                Enabled = true,
                EnabledEntityAppliers = [],
                LocalOutboxRelay = new LocalOutboxRelayOptions { Enabled = true }
            },
            repository);

        var processed = await relay.ProcessOnceAsync();

        processed.Should().Be(0);
        await repository.DidNotReceive().GetRelayCompaniesAsync(Arg.Any<CancellationToken>());
        await repository.DidNotReceive().ReleaseExpiredLeasesAsync(
            Arg.Any<int>(), Arg.Any<string>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
        await repository.DidNotReceive().ClaimAsync(
            Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<TimeSpan>(),
            Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessOnceAsync_PassesTheSameNormalizedEntityScopeToReleaseAndClaim()
    {
        var repository = Substitute.For<ILocalSyncOutboxRepository>();
        repository.GetRelayCompaniesAsync(Arg.Any<CancellationToken>())
            .Returns([new LocalSyncOutboxCompanyDto(3002, "DEMO")]);
        repository.ClaimAsync(
                Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<TimeSpan>(),
                Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>())
            .Returns([]);
        var relay = CreateRelay(
            new MasterBranchSyncWorkerOptions
            {
                Enabled = true,
                WorkerInstance = " relay-89 ",
                EnabledEntityAppliers = [" Carrier ", "carrier", "Warehouse", " "],
                LocalOutboxRelay = new LocalOutboxRelayOptions { Enabled = true }
            },
            repository);

        var processed = await relay.ProcessOnceAsync();

        processed.Should().Be(0);
        await repository.Received(1).ReleaseExpiredLeasesAsync(
            3002,
            "relay-89",
            Arg.Is<IReadOnlyCollection<string>>(names =>
                names.Count == 2
                && names.Contains("Carrier")
                && names.Contains("Warehouse")),
            Arg.Any<CancellationToken>());
        await repository.Received(1).ClaimAsync(
            3002,
            "relay-89",
            25,
            TimeSpan.FromMinutes(5),
            Arg.Is<IReadOnlyCollection<string>>(names =>
                names.Count == 2
                && names.Contains("Carrier")
                && names.Contains("Warehouse")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Repository_NormalizesEntityNamesDeterministically()
    {
        var result = LocalSyncOutboxRepository.NormalizeEntityNames(
            [" Warehouse ", "carrier", "Carrier", "", "Item"]);

        result.Should().Equal("carrier", "Item", "Warehouse");
    }

    [Fact]
    public void Migration164_ScopesClaimAndLeaseReleaseAndFailsClosed()
    {
        var sql = Read("database", "sql", "164_tenant_local_outbox_entity_scope.sql");

        sql.Should().Contain("SP_NA_POST_LOCALOUTBOX_LIBERARLEASESVENCIDOS")
            .And.Contain("SP_NA_POST_LOCALOUTBOX_RECLAMAR")
            .And.Contain("@EnabledEntityNamesJson nvarchar(max) = N'[]'")
            .And.Contain("OPENJSON(@EnabledEntityNamesJson)")
            .And.Contain("enabled.EntityName = item.EntityName")
            .And.Contain("IF NOT EXISTS (SELECT 1 FROM @EnabledEntities)")
            .And.Contain("Version = N'20260801.164'")
            .And.NotContain("USE [NuanSystem_Master]")
            .And.NotContain("DELETE FROM dbo.SchemaHistory")
            .And.NotContain("DROP TABLE");
    }

    [Fact]
    public void TenantInitializer_RegistersMigration164AfterCarrierMigration()
    {
        var source = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerTenantDatabaseInitializer.cs");

        source.IndexOf("162_tenant_carrier_transactional_outbox.sql", StringComparison.Ordinal)
            .Should().BeLessThan(source.IndexOf("164_tenant_local_outbox_entity_scope.sql", StringComparison.Ordinal));
    }

    private static LocalSyncOutboxRelay CreateRelay(
        MasterBranchSyncWorkerOptions current,
        ILocalSyncOutboxRepository repository)
    {
        var options = Substitute.For<IOptionsMonitor<MasterBranchSyncWorkerOptions>>();
        options.CurrentValue.Returns(current);
        return new LocalSyncOutboxRelay(
            options,
            repository,
            Substitute.For<ILocalSyncOutboxPromotionService>(),
            Substitute.For<ILogger<LocalSyncOutboxRelay>>());
    }

    private static string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine([Root(), .. parts]));

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
