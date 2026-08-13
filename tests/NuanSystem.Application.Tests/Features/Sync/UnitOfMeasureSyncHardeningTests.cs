using System.Text.Json;
using System.Runtime.CompilerServices;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class UnitOfMeasureSyncHardeningTests
{
    [Fact]
    public async Task Applier_PropagatesTerminalCodeCollision()
    {
        var repository = Substitute.For<IUnitMeasureSyncApplyRepository>();
        var payload = new UnitMeasureSyncPayload(
            Guid.NewGuid(), "UND", "Unidad", null, "UND", "Quantity", 10, true, false, DateTime.UtcNow);
        var context = new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "UnitOfMeasure",
            payload.GlobalId,
            SyncOperation.Created.ToString(),
            JsonSerializer.Serialize(new { payload }, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            2,
            3);
        repository.ApplyAsync(
                2, context, Arg.Any<UnitMeasureSyncPayload>(),
                SyncOperation.Created,
                Arg.Any<CancellationToken>())
            .Returns(new UnitMeasureSyncApplyResult(
                false,
                false,
                true,
                null,
                "Codigo ocupado.",
                ErrorCode: "SYNC_UNIT_OF_MEASURE_CODE_CONFLICT"));

        var result = await new UnitMeasureSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.Retryable.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_UNIT_OF_MEASURE_CODE_CONFLICT");
    }

    [Fact]
    public void Repository_DoesNotAdoptUnitOfMeasureByCode_AndPreservesSapMappingSeparation()
    {
        var repository = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "UnitMeasureSyncApplyRepository.cs");
        var payload = ReadSource(
            "src", "Backend", "NuanSystem.Application", "Features", "Definitions", "Inventory", "UnitMeasures", "Dtos", "UnitMeasureDtos.cs");

        repository.Should().Contain("SP_NA_POST_UNIT_OF_MEASURE_SYNC_APPLY");
        repository.Should().Contain("SYNC_UNIT_OF_MEASURE_CODE_CONFLICT");
        repository.Should().Contain("Status=N'DeadLetter'");
        var payloadContract = payload[payload.IndexOf("public sealed record UnitMeasureSyncPayload", StringComparison.Ordinal)..];
        payloadContract.Should().NotContain("ExternalSystem").And.NotContain("ExternalCode").And.NotContain("SapCode");
    }

    [Fact]
    public void MasterMigration_DeclaresItemDependencyAndDedicatedIncrementalUom()
    {
        var dependency = ReadSource("database", "sql", "132_master_item_unit_of_measure_dependency.sql");
        var incremental = ReadSource("database", "sql", "197_master_unit_of_measures_sync_registration.sql");

        dependency.Should().Contain("N'Item'")
            .And.Contain("N'UnitOfMeasure'")
            .And.Contain("SyncEntityDefinitionDependencies")
            .And.Contain("20260726.132")
            .And.NotContain("SupportsIncremental")
            .And.NotContain("SyncEntityConfigurations");
        incremental.Should().Contain("UnitOfMeasure")
            .And.Contain("SupportsIncremental=1")
            .And.Contain("CONVERT(bit,0)")
            .And.Contain("Este script no activa perfiles ni workers");
    }

    [Fact]
    public void TenantMigration_AddsDescriptionRequiredByFullSourceAndApplier()
    {
        var script = ReadSource("database", "sql", "194_tenant_unit_of_measures_master.sql");
        var source = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "SyncFullEntitySources.cs");
        var repository = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "UnitMeasureSyncApplyRepository.cs");

        script.Should().Contain("IF COL_LENGTH(N'dbo.UnitOfMeasures', N'Description') IS NULL");
        script.Should().Contain("ALTER TABLE dbo.UnitOfMeasures ADD Description nvarchar(500) NULL");
        source.Should().Contain("public sealed class UnitMeasureFullEntitySource")
            .And.Contain("Description").And.Contain("MagnitudeCode");
        repository.Should().Contain("payload.Description").And.Contain("payload.MagnitudeCode");
    }

    private static string ReadSource(params string[] pathParts) =>
        File.ReadAllText(Path.Combine([FindRepositoryRoot().FullName, .. pathParts]));

    private static DirectoryInfo FindRepositoryRoot([CallerFilePath] string sourcePath = "")
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "nuansystem.sln"))) return directory;
            directory = directory.Parent;
        }

        directory = new FileInfo(sourcePath).Directory;
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "nuansystem.sln"))) return directory;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("No se encontro la raiz del repositorio NuanSystem.");
    }
}
