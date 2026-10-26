using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Services;
using NuanSystem.Application.Features.SapSync.Enums;
using NSubstitute;

namespace NuanSystem.Application.Tests.Features.SapSync;

internal static class SapSyncRegistryFixture
{
    internal static ISapSyncImporter Importer(string code = "Suppliers")
    {
        var importer = Substitute.For<ISapSyncImporter>();
        importer.EntityCode.Returns(code);
        importer.SupportedModes.Returns(new[] { "Full", "Incremental" });
        return importer;
    }

    internal static ISapSyncScheduledExecutionProcessor Processor(string code = "Suppliers")
    {
        var processor = Substitute.For<ISapSyncScheduledExecutionProcessor>();
        processor.EntityCode.Returns(code);
        processor.SupportedModes.Returns(new[] { "Full", "Incremental" });
        return processor;
    }

    internal static SapSyncProcessorRegistry Create() => new(
        [Importer("Suppliers"), Importer("Items"), Importer("PaymentTerms"), Importer("Warehouses"), Importer("Countries"), Importer("Provinces"), Importer("Cities")],
        [],
        [Processor("Suppliers"), Processor("Items"), Processor("PaymentTerms"), Processor("Warehouses"), Processor("Countries"), Processor("Provinces"), Processor("Cities")], []);
}

public sealed class SapSyncProcessorRegistryTests
{
    private static SapSyncHandlerCapabilityDto Capability(string code = "Suppliers") =>
        new(code, code, true, true, true, true, true, true);

    [Fact]
    public void Importer_does_not_advertise_export_or_scheduled_execution()
    {
        var registry = new SapSyncProcessorRegistry([SapSyncRegistryFixture.Importer()], [], [], []);
        Assert.NotNull(registry.FindImporter("suppliers"));
        Assert.Null(registry.FindExporter("Suppliers"));
        Assert.False(registry.Intersect(Capability()).IsImplemented);
        Assert.False(registry.SupportsScheduled("Suppliers", SapSyncDirection.SapToErp, "Full"));
    }

    [Fact]
    public void Effective_profile_capabilities_are_intersection_of_configuration_and_processors()
    {
        var processor = SapSyncRegistryFixture.Processor();
        processor.SupportedModes.Returns(new[] { "Full" });
        var registry = new SapSyncProcessorRegistry([SapSyncRegistryFixture.Importer()], [], [processor], []);
        var actual = registry.Intersect(Capability());
        Assert.True(actual.IsImplemented);
        Assert.True(actual.SupportsSapToErp);
        Assert.False(actual.SupportsErpToSap);
        Assert.False(actual.SupportsIncremental);
        Assert.False(registry.Intersect(Capability() with { SupportsFull = false }).SupportsFull);
        Assert.False(registry.Intersect(Capability() with { IsImplemented = false }).IsImplemented);
        Assert.False(registry.SupportsScheduled("Suppliers", SapSyncDirection.Both, "Full"));
        Assert.False(registry.SupportsScheduled("Suppliers", SapSyncDirection.ErpToSap, "Full"));
    }

    [Fact]
    public void Duplicate_exporters_fail_at_composition()
    {
        var exporter = Substitute.For<ISapSyncExporter>();
        exporter.EntityCode.Returns("Suppliers");
        exporter.SupportedModes.Returns(new[] { "Full" });
        Assert.Throws<InvalidOperationException>(() => new SapSyncProcessorRegistry([], [exporter, exporter], [], []));
    }

    [Fact]
    public void Duplicate_scheduled_processors_fail_at_composition()
    {
        var processor = SapSyncRegistryFixture.Processor();
        Assert.Throws<InvalidOperationException>(() => new SapSyncProcessorRegistry(
            [SapSyncRegistryFixture.Importer()], [], [processor, processor], []));
    }

    [Fact]
    public void Retry_processor_without_scheduled_processor_fails_at_composition()
    {
        var retry = Substitute.For<ISapSyncExecutionRetryProcessor>();
        retry.EntityCode.Returns("Suppliers");
        retry.ApprovedSnapshotType.Returns("SupplierV1");
        Assert.Throws<InvalidOperationException>(() => new SapSyncProcessorRegistry(
            [SapSyncRegistryFixture.Importer()], [], [], [retry]));
    }

    [Fact]
    public void Duplicate_retry_snapshot_types_fail_at_composition()
    {
        var retry = Substitute.For<ISapSyncExecutionRetryProcessor>();
        retry.EntityCode.Returns("Suppliers");
        retry.ApprovedSnapshotType.Returns("SupplierV1");
        Assert.Throws<InvalidOperationException>(() => new SapSyncProcessorRegistry(
            [SapSyncRegistryFixture.Importer()], [], [SapSyncRegistryFixture.Processor()], [retry, retry]));
    }

    [Theory]
    [InlineData("NotAMode")]
    [InlineData("")]
    public void Invalid_modes_fail_at_composition(string mode)
    {
        var importer = SapSyncRegistryFixture.Importer();
        importer.SupportedModes.Returns(new[] { mode });
        Assert.Throws<InvalidOperationException>(() => new SapSyncProcessorRegistry([importer], [], [], []));
    }

    [Fact]
    public void Duplicate_importers_fail_at_composition_even_with_different_casing()
    {
        Assert.Throws<InvalidOperationException>(() => new SapSyncProcessorRegistry(
            [SapSyncRegistryFixture.Importer("Suppliers"), SapSyncRegistryFixture.Importer("suppliers")], [], [], []));
    }

    [Fact]
    public void Scheduled_processor_without_importer_fails_at_composition()
    {
        Assert.Throws<InvalidOperationException>(() => new SapSyncProcessorRegistry(
            [], [], [SapSyncRegistryFixture.Processor()], []));
    }

    [Fact]
    public void Scheduled_modes_without_corresponding_importer_mode_fail_at_composition()
    {
        var importer = SapSyncRegistryFixture.Importer();
        importer.SupportedModes.Returns(new[] { "Full" });
        Assert.Throws<InvalidOperationException>(() => new SapSyncProcessorRegistry(
            [importer], [], [SapSyncRegistryFixture.Processor()], []));
    }
}
