using Dapper;
using FluentAssertions;
using NuanSystem.Application.Features.SriDocuments.Dtos;
using System.Data;

namespace NuanSystem.Application.Tests.Features.SriDocuments;

public sealed class SriDocumentMonitorPersistenceContractTests
{
    private const string MigrationFileName = "123_tenant_sri_document_monitor_summary_bigint_fix.sql";

    [Fact]
    public void Dapper_MaterializesMonitorSummaryWhenEveryAggregateIsBigInt()
    {
        using var table = CreateSummaryTable(typeof(long));
        table.Rows.Add(5L, 1L, 1L, 1L, 2L);
        using var reader = table.CreateDataReader();

        reader.Read().Should().BeTrue();
        var materialize = reader.GetRowParser<SriDocumentMonitorSummaryDto>();

        var summary = materialize(reader);

        summary.Should().Be(new SriDocumentMonitorSummaryDto(5, 1, 1, 1, 2));
    }

    [Fact]
    public void Dapper_MaterializesEmptyMonitorSummaryAsBigIntZeros()
    {
        using var table = CreateSummaryTable(typeof(long));
        table.Rows.Add(0L, 0L, 0L, 0L, 0L);
        using var reader = table.CreateDataReader();

        reader.Read().Should().BeTrue();
        var materialize = reader.GetRowParser<SriDocumentMonitorSummaryDto>();

        var summary = materialize(reader);

        summary.Should().Be(new SriDocumentMonitorSummaryDto(0, 0, 0, 0, 0));
    }

    [Fact]
    public void Dapper_RejectsLegacyIntAggregatesThatDoNotMatchThePositionalRecord()
    {
        using var table = CreateSummaryTable(typeof(int));
        table.Rows.Add(5L, 1, 1, 1, 2);
        using var reader = table.CreateDataReader();

        reader.Read().Should().BeTrue();

        FluentActions.Invoking(() => reader.GetRowParser<SriDocumentMonitorSummaryDto>())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*constructor*");
    }

    [Fact]
    public void ForwardMigration_ReturnsNonNullBigIntAggregatesAndRegistersOneVersion()
    {
        var sql = Read("database", "sql", MigrationFileName);

        sql.Should().Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SRIDOCUMENTMONITOR_RESUMEN")
            .And.Contain("COUNT_BIG(1) AS Total")
            .And.Contain("Version=N'20260725.123'")
            .And.Contain("VALUES(N'20260725.123'")
            .And.Contain("OBJECT_ID(N'dbo.SriDocumentQueue', N'U') IS NULL")
            .And.NotContain("UPDATE dbo.SchemaHistory")
            .And.NotContain("DELETE FROM dbo.SchemaHistory")
            .And.NotContain("DROP PROCEDURE")
            .And.NotContain("DROP TABLE");

        foreach (var alias in new[] { "Pending", "Querying", "Authorized", "Errors" })
        {
            sql.Should().Contain($"CONVERT(bigint,0)) AS {alias}");
        }
    }

    [Fact]
    public void TenantInitializer_AppliesForwardMigrationAfterItsPrerequisites()
    {
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs");

        var monitor = initializer.IndexOf("118_tenant_sri_document_monitor_and_download.sql", StringComparison.Ordinal);
        var operations = initializer.IndexOf("121_tenant_sri_worker_operational_summary.sql", StringComparison.Ordinal);
        var repair = initializer.IndexOf(MigrationFileName, StringComparison.Ordinal);

        monitor.Should().BeGreaterThanOrEqualTo(0);
        operations.Should().BeGreaterThan(monitor);
        repair.Should().BeGreaterThan(operations);
    }

    private static DataTable CreateSummaryTable(Type aggregateType)
    {
        var table = new DataTable();
        table.Columns.Add("Total", typeof(long));
        table.Columns.Add("Pending", aggregateType);
        table.Columns.Add("Querying", aggregateType);
        table.Columns.Add("Authorized", aggregateType);
        table.Columns.Add("Errors", aggregateType);
        return table;
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
