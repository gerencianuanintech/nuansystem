using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class CitySyncEventApplierTests
{
    [Fact]
    public async Task Created_UpsertsCityWithBothParentGlobalIds()
    {
        var repository = Substitute.For<ICitySyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new CitySyncApplyResult(true, false, 1, "Creada."));
        var applier = new CitySyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<CitySyncPayload>(value =>
                value.GlobalId == payload.GlobalId &&
                value.CountryGlobalId == payload.CountryGlobalId &&
                value.ProvinceGlobalId == payload.ProvinceGlobalId),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(true, false, "SYNC_COUNTRY_GLOBAL_ID_REQUIRED")]
    [InlineData(false, true, "SYNC_PROVINCE_GLOBAL_ID_REQUIRED")]
    public async Task MissingParentGlobalId_IsRejectedBeforePersistence(
        bool clearCountry,
        bool clearProvince,
        string expectedCode)
    {
        var repository = Substitute.For<ICitySyncApplyRepository>();
        var payload = CreatePayload() with
        {
            CountryGlobalId = clearCountry ? Guid.Empty : Guid.NewGuid(),
            ProvinceGlobalId = clearProvince ? Guid.Empty : Guid.NewGuid()
        };
        var context = CreateContext(payload, SyncOperation.Created);
        var applier = new CitySyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.ErrorCode.Should().Be(expectedCode);
        await repository.DidNotReceiveWithAnyArgs().UpsertFromSyncAsync(default, default!, default!, default, default);
    }

    [Fact]
    public void Persistence_ValidatesHierarchyAndProtectsInboxIdempotency()
    {
        var repository = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "CitySyncApplyRepository.cs");

        repository.Should().Contain("WHERE GlobalId = @CountryGlobalId");
        repository.Should().Contain("WHERE GlobalId = @ProvinceGlobalId");
        repository.Should().Contain("@ProvinceCountryId <> @CountryId");
        repository.Should().Contain("WHERE GlobalId = @GlobalId");
        repository.Should().Contain("WHERE EventId = @EventId");
        repository.Should().Contain("Status = N'Applied'");
        repository.Should().Contain("THROW 51088");
    }

    private static CitySyncPayload CreatePayload()
    {
        return new CitySyncPayload(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "EC",
            Guid.NewGuid(),
            "AZU",
            "CUE",
            "Cuenca",
            true,
            new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            null);
    }

    private static SyncEventApplyContext CreateContext(CitySyncPayload payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "Cities",
            globalId = payload.GlobalId,
            code = $"{payload.CountryCode}|{payload.ProvinceCode}|{payload.Code}",
            operation = operation.ToString(),
            payload
        };

        return new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "Cities",
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
