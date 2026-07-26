using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class UnitOfMeasureSyncHardeningTests
{
    [Fact]
    public async Task Applier_PropagatesTerminalCodeCollision()
    {
        var repository = Substitute.For<IReferenceCatalogSyncApplyRepository>();
        var payload = new ReferenceCatalogSyncPayload(
            Guid.NewGuid(), "UND", "Unidad", null, null, null, null, false, true,
            "SAP_B1", "1", DateTime.UtcNow, null);
        var context = new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "UnitOfMeasures",
            payload.GlobalId,
            SyncOperation.Created.ToString(),
            JsonSerializer.Serialize(new { payload }, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            2,
            3);
        repository.ApplyAsync(
                2,
                "UnitOfMeasures",
                context,
                Arg.Any<ReferenceCatalogSyncPayload>(),
                SyncOperation.Created,
                Arg.Any<CancellationToken>())
            .Returns(new ReferenceCatalogSyncApplyResult(
                false,
                false,
                null,
                "Codigo ocupado.",
                TerminalConflict: true,
                ErrorCode: "SYNC_UNIT_OF_MEASURE_CODE_CONFLICT"));

        var result = await new ReferenceCatalogSyncEventApplier(repository).ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.Retryable.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_UNIT_OF_MEASURE_CODE_CONFLICT");
    }

    [Fact]
    public void Repository_DoesNotAdoptUnitOfMeasureByCode_AndPreservesSapMappingSeparation()
    {
        var repository = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "ReferenceCatalogSyncApplyRepository.cs");
        var payload = ReadSource(
            "src", "Backend", "NuanSystem.Application", "Features", "Sync", "Dtos", "ReferenceCatalogSyncDtos.cs");

        repository.Should().Contain("\"UnitOfMeasures\" => BuildUpsertSql(");
        repository.Should().Contain("allowCodeReconciliation: false");
        repository.Should().Contain("SYNC_UNIT_OF_MEASURE_CODE_CONFLICT");
        repository.Should().Contain("Status=N'DeadLetter'");
        payload.Should().NotContain("SapCode");
    }

    [Fact]
    public void MasterMigration_DeclaresItemDependencyWithoutEnablingIncrementalUom()
    {
        var script = ReadSource("database", "sql", "132_master_item_unit_of_measure_dependency.sql");

        script.Should().Contain("Code = N'Item'");
        script.Should().Contain("Code = N'UnitOfMeasures'");
        script.Should().Contain("SyncEntityDefinitionDependencies");
        script.Should().Contain("20260726.132");
        script.Should().NotContain("SupportsIncremental");
        script.Should().NotContain("SyncEntityConfigurations");
    }

    private static string ReadSource(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            directory = directory.Parent;
        }
        throw new FileNotFoundException(Path.Combine(pathParts));
    }
}
