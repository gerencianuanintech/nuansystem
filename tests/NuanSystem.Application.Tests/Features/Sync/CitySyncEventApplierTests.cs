using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
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
            .Returns(new CitySyncApplyResult(true, false, false, 1, "Creada."));

        var result = await new CitySyncEventApplier(repository).ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<CitySyncPayload>(value =>
                value.GlobalId == payload.GlobalId
                && value.CountryGlobalId == payload.CountryGlobalId
                && value.ProvinceGlobalId == payload.ProvinceGlobalId),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RepositoryTerminalConflict_IsPropagatedWithErrorCode()
    {
        var repository = Substitute.For<ICitySyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Updated);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Updated, Arg.Any<CancellationToken>())
            .Returns(new CitySyncApplyResult(false, false, true, null, "Conflicto de padres.", "SYNC_CITY_PARENT_CONFLICT"));

        var result = await new CitySyncEventApplier(repository).ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_CITY_PARENT_CONFLICT");
    }

    [Theory]
    [InlineData(true, false, "SYNC_COUNTRY_GLOBAL_ID_REQUIRED")]
    [InlineData(false, true, "SYNC_PROVINCE_GLOBAL_ID_REQUIRED")]
    public async Task MissingParentGlobalId_IsTerminalBeforePersistence(bool clearCountry, bool clearProvince, string expectedCode)
    {
        var repository = Substitute.For<ICitySyncApplyRepository>();
        var payload = CreatePayload() with
        {
            CountryGlobalId = clearCountry ? Guid.Empty : Guid.NewGuid(),
            ProvinceGlobalId = clearProvince ? Guid.Empty : Guid.NewGuid()
        };
        var context = CreateContext(payload, SyncOperation.Created);

        var result = await new CitySyncEventApplier(repository).ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be(expectedCode);
        await repository.DidNotReceiveWithAnyArgs().UpsertFromSyncAsync(default, default!, default!, default, default);
    }

    [Fact]
    public async Task OversizedCode_IsTerminalBeforePersistence()
    {
        var repository = Substitute.For<ICitySyncApplyRepository>();
        var payload = CreatePayload() with { Code = new string('X', 21) };
        var context = CreateContext(payload, SyncOperation.Created);

        var result = await new CitySyncEventApplier(repository).ApplyAsync(context, CancellationToken.None);

        result.ErrorCode.Should().Be("SYNC_CITY_PAYLOAD_INVALID");
        result.Terminal.Should().BeTrue();
        await repository.DidNotReceiveWithAnyArgs().UpsertFromSyncAsync(default, default!, default!, default, default);
    }

    [Fact]
    public void Persistence_UsesTerminalProcedureWithoutAdoptionOrTruncation()
    {
        var repository = ReadSourceFile("src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "CitySyncApplyRepository.cs");
        var sql = ReadSourceFile("database", "sql", "175_tenant_city_transactional_outbox.sql");

        repository.Should().Contain("SP_NA_POST_CITY_SYNC_APPLY_EVENT")
            .And.Contain("SYNC_CITY_CODE_CONFLICT")
            .And.Contain("SYNC_CITY_HIERARCHY_CONFLICT")
            .And.Contain("SYNC_CITY_PARENT_CONFLICT")
            .And.NotContain("Math.Min")
            .And.NotContain("WHERE Code = @CountryCode");
        sql.Should().Contain("WHERE GlobalId=@CountryGlobalId")
            .And.Contain("WHERE GlobalId=@ProvinceGlobalId")
            .And.Contain("WHERE GlobalId=@GlobalId")
            .And.NotContain("SET GlobalId=@GlobalId");
    }

    [Fact]
    public void FullSource_IncludesCityTombstonesAndExternalReferences()
    {
        var source = ReadSourceFile("src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "SyncFullEntitySources.cs");
        var cityBlock = source[source.IndexOf("public sealed class CityFullEntitySource", StringComparison.Ordinal)..source.IndexOf("public sealed class CurrencyFullEntitySource", StringComparison.Ordinal)];

        cityBlock.Should().Contain("city.IsDeleted")
            .And.Contain("city.ExternalSystem")
            .And.Contain("city.ExternalCode")
            .And.NotContain("WHERE city.IsDeleted = 0");
    }

    private static CitySyncPayload CreatePayload() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "EC",
        Guid.NewGuid(),
        "AZU",
        "CUE",
        "Cuenca",
        true,
        false,
        "EXTERNAL_CATALOG",
        "EC|AZU|CUE",
        new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
        null);

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
            var path = Path.Combine([directory.FullName, .. pathParts]);
            if (File.Exists(path)) return File.ReadAllText(path);
            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontro {Path.Combine(pathParts)}.");
    }
}
