using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class CountrySyncEventApplierTests
{
    [Fact]
    public async Task Created_UpsertsCountryByGlobalId()
    {
        var repository = Substitute.For<ICountrySyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new CountrySyncApplyResult(true, false, false, 1, "Creado."));
        var applier = new CountrySyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeTrue();
        await repository.Received(1).UpsertFromSyncAsync(
            2,
            context,
            Arg.Is<CountrySyncPayload>(value => value.GlobalId == payload.GlobalId && value.Code == "EC"),
            SyncOperation.Created,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deleted_MarksCountryAsDeleted()
    {
        var repository = Substitute.For<ICountrySyncApplyRepository>();
        var payload = CreatePayload(isActive: false);
        var context = CreateContext(payload, SyncOperation.Deleted);
        repository.DisableFromSyncAsync(2, context, payload, true, Arg.Any<CancellationToken>())
            .Returns(new CountrySyncApplyResult(true, false, false, 1, "Eliminado."));
        var applier = new CountrySyncEventApplier(repository);

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
    public async Task CodeCollision_IsReportedAsTerminalWithoutAdoption()
    {
        var repository = Substitute.For<ICountrySyncApplyRepository>();
        var payload = CreatePayload();
        var context = CreateContext(payload, SyncOperation.Created);
        repository.UpsertFromSyncAsync(2, context, payload, SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new CountrySyncApplyResult(false, false, true, null, "Conflicto.", "SYNC_COUNTRY_CODE_CONFLICT"));
        var applier = new CountrySyncEventApplier(repository);

        var result = await applier.ApplyAsync(context, CancellationToken.None);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_COUNTRY_CODE_CONFLICT");
    }

    [Fact]
    public void Persistence_UsesTerminalProcedureWithoutCodeAdoption()
    {
        var repository = ReadSourceFile(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "CountrySyncApplyRepository.cs");

        repository.Should().Contain("SP_NA_POST_COUNTRY_SYNC_APPLY_EVENT");
        repository.Should().Contain("SYNC_COUNTRY_CODE_CONFLICT");
        repository.Should().NotContain("WHERE Code = @Code");
    }

    private static CountrySyncPayload CreatePayload(bool isActive = true)
    {
        return new CountrySyncPayload(
            Guid.NewGuid(),
            "EC",
            "Ecuador",
            "EC",
            "ECU",
            "+593",
            isActive,
            false,
            "SAP_B1",
            "EC",
            new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            null);
    }

    private static SyncEventApplyContext CreateContext(CountrySyncPayload payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "Countries",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = operation.ToString(),
            payload
        };

        return new SyncEventApplyContext(
            Guid.NewGuid(),
            1,
            "Countries",
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
