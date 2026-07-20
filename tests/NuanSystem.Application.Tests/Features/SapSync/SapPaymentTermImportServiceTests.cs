using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Services;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapPaymentTermImportServiceTests
{
    [Fact]
    public async Task ImportFullAsync_MapsRepresentableTermAndPublishesSapB1Identity()
    {
        var reader = Substitute.For<ISapPaymentTermReader>();
        var repository = Substitute.For<ISapPaymentTermImportRepository>();
        var publisher = Substitute.For<ISyncEventPublisher>();
        reader.GetAllAsync(1, Arg.Any<CancellationToken>()).Returns([
            new SapPaymentTermRecord(7, "Credito 30", 30, 0, 1)
        ]);
        var globalId = Guid.NewGuid();
        repository.UpsertAsync(Arg.Any<SapPaymentTermUpsertData>(), Arg.Any<CancellationToken>()).Returns(
            new SapPaymentTermUpsertResult("Created", 10, globalId, "7", "Credito 30", 30, true, true,
                "SAP_B1", "7", DateTime.UtcNow, null, "Creada"));
        publisher.PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>()).Returns(
            Result<SyncPublishResult>.Success(new(true, 22, "Publicado")));
        var service = new SapPaymentTermImportService(reader, repository, publisher);

        var result = await service.ImportFullAsync(1, 9, "tester");

        result.Created.Should().Be(1);
        await repository.Received(1).UpsertAsync(Arg.Is<SapPaymentTermUpsertData>(data =>
            data.Code == "7" && data.Days == 30 && data.IsCredit && data.ExternalSystem == "SAP_B1" && data.ExternalCode == "7"),
            Arg.Any<CancellationToken>());
        await publisher.Received(1).PublishAsync(Arg.Is<SyncPublishRequest>(request =>
            request.EntityName == SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms
            && request.EntityGlobalId == globalId
            && request.SourceSystem == "SAP_B1"), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(30, 1, 1, "meses")]
    [InlineData(30, 0, 2, "cuotas")]
    [InlineData(-1, 0, 1, "negativo")]
    public async Task ImportFullAsync_ReportsConflictForUnrepresentableTerms(
        int days, int months, int installments, string expected)
    {
        var reader = Substitute.For<ISapPaymentTermReader>();
        var repository = Substitute.For<ISapPaymentTermImportRepository>();
        var publisher = Substitute.For<ISyncEventPublisher>();
        reader.GetAllAsync(1, Arg.Any<CancellationToken>()).Returns([
            new SapPaymentTermRecord(8, "No representable", days, months, installments)
        ]);
        var service = new SapPaymentTermImportService(reader, repository, publisher);

        var result = await service.ImportFullAsync(1, null, "worker");

        result.Conflicted.Should().Be(1);
        result.Items.Single().Message.Should().Contain(expected);
        await repository.DidNotReceive().UpsertAsync(Arg.Any<SapPaymentTermUpsertData>(), Arg.Any<CancellationToken>());
        await publisher.DidNotReceive().PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportFullAsync_RepublishesUnchangedTermForFullReconciliation()
    {
        var reader = Substitute.For<ISapPaymentTermReader>();
        var repository = Substitute.For<ISapPaymentTermImportRepository>();
        var publisher = Substitute.For<ISyncEventPublisher>();
        reader.GetAllAsync(1, Arg.Any<CancellationToken>()).Returns([new SapPaymentTermRecord(1, "Contado", 0, 0, 1)]);
        repository.UpsertAsync(Arg.Any<SapPaymentTermUpsertData>(), Arg.Any<CancellationToken>()).Returns(
            new SapPaymentTermUpsertResult("Unchanged", 1, Guid.NewGuid(), "1", "Contado", 0, false, true,
                "SAP_B1", "1", DateTime.UtcNow, null, "Sin cambios"));
        publisher.PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>()).Returns(
            Result<SyncPublishResult>.Success(new(true, 23, "Publicado para reconciliacion")));
        var service = new SapPaymentTermImportService(reader, repository, publisher);

        var result = await service.ImportFullAsync(1, null, "worker");

        result.Unchanged.Should().Be(1);
        await publisher.Received(1).PublishAsync(Arg.Is<SyncPublishRequest>(request =>
            request.Operation == SyncOperation.Updated
            && request.EntityName == SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportFullAsync_ReportsPublicationFailureWithoutDoubleCountingSavedRow()
    {
        var reader = Substitute.For<ISapPaymentTermReader>();
        var repository = Substitute.For<ISapPaymentTermImportRepository>();
        var publisher = Substitute.For<ISyncEventPublisher>();
        reader.GetAllAsync(1, Arg.Any<CancellationToken>()).Returns([new SapPaymentTermRecord(2, "Credito", 15, 0, 1)]);
        repository.UpsertAsync(Arg.Any<SapPaymentTermUpsertData>(), Arg.Any<CancellationToken>()).Returns(
            new SapPaymentTermUpsertResult("Updated", 2, Guid.NewGuid(), "2", "Credito", 15, true, true,
                "SAP_B1", "2", DateTime.UtcNow, DateTime.UtcNow, "Actualizada"));
        publisher.PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>()).Returns(
            Result<SyncPublishResult>.Failure("No se pudo publicar."));
        var service = new SapPaymentTermImportService(reader, repository, publisher);

        var result = await service.ImportFullAsync(1, null, "worker");

        result.Updated.Should().Be(0);
        result.Failed.Should().Be(1);
        result.Items.Single().Message.Should().Contain("No se pudo publicar");
    }
}
