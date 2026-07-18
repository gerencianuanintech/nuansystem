using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class ProvinceSyncEventApplierTests
{
    [Fact]
    public async Task Created_UpsertsProvinceWithParentGlobalId()
    {
        var repository = Substitute.For<IProvinceSyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new ProvinceSyncApplyResult(true, false, 1, "Creada."));
        var applier = new ProvinceSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<ProvinceSyncPayload>(value =>
                value.GlobalId == payload.GlobalId &&
                value.CountryGlobalId == payload.CountryGlobalId &&
                value.Code == "AZU"),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MissingCountryGlobalId_IsRejectedBeforePersistence()
    {
        var repository = Substitute.For<IProvinceSyncApplyRepository>();
        var payload = CreatePayload() with { CountryGlobalId = Guid.Empty };
        var context = CreateContext(payload, SyncOperation.Created);
        var applier = new ProvinceSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.ErrorCode.Should().Be("SYNC_PARENT_GLOBAL_ID_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().UpsertFromSyncAsync(default, default!, default!, default, default);
    }

    [Fact]
    public void Persistence_ResolvesParentAndProtectsInboxIdempotency()
    {
        var repository = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "ProvinceSyncApplyRepository.cs");

        repository.Should().Contain("WHERE GlobalId = @CountryGlobalId");
        repository.Should().Contain("WHERE Code = @CountryCode");
        repository.Should().Contain("WHERE GlobalId = @GlobalId");
        repository.Should().Contain("WHERE EventId = @EventId");
        repository.Should().Contain("Status = N'Applied'");
        repository.Should().Contain("THROW 51085");
        repository.Should().NotContain("dbo.Cities");
    }

    private static ProvinceSyncPayload CreatePayload()
    {
        return new ProvinceSyncPayload(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "EC",
            "AZU",
            "Azuay",
            true,
            new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            null);
    }

    private static SyncEventApplyContext CreateContext(ProvinceSyncPayload payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "Provinces",
            globalId = payload.GlobalId,
            code = $"{payload.CountryCode}|{payload.Code}",
            operation = operation.ToString(),
            payload
        };

        return new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "Provinces",
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
