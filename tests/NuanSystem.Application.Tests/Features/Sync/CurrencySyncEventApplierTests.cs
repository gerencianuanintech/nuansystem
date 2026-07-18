using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class CurrencySyncEventApplierTests
{
    [Fact]
    public async Task Created_UpsertsCurrencyByGlobalId()
    {
        var repository = Substitute.For<ICurrencySyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new CurrencySyncApplyResult(true, false, 1, "Creada."));
        var applier = new CurrencySyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<CurrencySyncPayload>(value => value.GlobalId == payload.GlobalId && value.Code == "USD"),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deleted_MarksCurrencyAsDeleted()
    {
        var repository = Substitute.For<ICurrencySyncApplyRepository>();
        var payload = CreatePayload(isActive: false);
        var context = CreateContext(payload, SyncOperation.Deleted);
        repository.DisableFromSyncAsync(2, context, payload, true, Arg.Any<CancellationToken>())
            .Returns(new CurrencySyncApplyResult(true, false, 1, "Eliminada."));
        var applier = new CurrencySyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).DisableFromSyncAsync(
            2,
            context,
            payload,
            markDeleted: true,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Persistence_ReconcilesLegacyRowsByCodeAndProtectsInboxIdempotency()
    {
        var repository = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "CurrencySyncApplyRepository.cs");

        repository.Should().Contain("WHERE GlobalId = @GlobalId");
        repository.Should().Contain("WHERE Code = @Code");
        repository.Should().Contain("WHERE EventId = @EventId");
        repository.Should().Contain("Status = N'Applied'");
        repository.Should().Contain("IsBaseCurrency = @IsBaseCurrency");
    }

    private static CurrencySyncPayload CreatePayload(bool isActive = true)
    {
        return new CurrencySyncPayload(
            Guid.NewGuid(),
            "USD",
            "Dolar",
            "$",
            "Moneda base",
            true,
            isActive,
            null,
            null,
            new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            null);
    }

    private static SyncEventApplyContext CreateContext(CurrencySyncPayload payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "Currencies",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = operation.ToString(),
            payload
        };

        return new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "Currencies",
            payload.GlobalId,
            operation.ToString(),
            JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            2,
            10);
    }

    private static string ReadSourceFile(params string[] pathParts)
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

        throw new FileNotFoundException($"No se encontro {Path.Combine(pathParts)}.");
    }
}
