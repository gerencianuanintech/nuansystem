using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Handlers;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapPaymentTermSyncHandlerTests
{
    [Fact]
    public async Task ImportFromSapAsync_KeepsConflictDetailsVisibleInWorkerLogMessage()
    {
        var service = Substitute.For<ISapPaymentTermImportService>();
        service.ImportFullAsync(1, null, "SAP Sync Worker", Arg.Any<CancellationToken>()).Returns(
            new SapPaymentTermImportResultDto(
                1, 0, 0, 0, 1, 0,
                [new SapPaymentTermImportItemResultDto(9, "Cuotas", "Conflict", "Usa varias cuotas.")]));
        var handler = new SapPaymentTermSyncHandler(service);
        var context = new SapSyncExecutionContext(
            1, "MATRIZ", "PaymentTerms", SapSyncDirection.SapToErp, SapSyncOperation.Import,
            "test", "correlation", 0, DateTime.UtcNow);

        var result = await handler.ImportFromSapAsync(context);

        result.Status.Should().Be(SapSyncStatus.Synced);
        result.Message.Should().Contain("conflictos: 1");
        result.Message.Should().Contain("SAP 9: Usa varias cuotas.");
    }
}
