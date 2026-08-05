using System.Data;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.General.Cities.Commands;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Definitions.General.Cities;

public sealed class CitySyncPublishingTests
{
    private readonly IGeographyRepository repository = Substitute.For<IGeographyRepository>();
    private readonly ICityLocalOutboxWriter writer = Substitute.For<ICityLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner runner = new();

    [Fact]
    public async Task Create_WritesCityAndOutboxInSameTransaction()
    {
        var city = CreateCity();
        repository.CityCodeExistsAsync(2, "CUE", null, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        repository.CreateCityAsync(Arg.Any<SaveCityData>(), runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(3);
        repository.GetCityByIdAsync(3, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(city);

        var result = await new CreateCityCommandHandler(repository, runner, writer).Handle(
            new CreateCityCommand(1, 2, "cue", "Cuenca", true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await writer.Received(1).EnqueueAsync(city, SyncOperation.Created, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>());
        runner.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Update_PreservesExternalReferenceAndWritesDisabled()
    {
        var current = CreateCity();
        var inactive = CreateCity(false, current.GlobalId, current.CountryGlobalId, current.ProvinceGlobalId);
        repository.GetCityByIdAsync(3, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(current, inactive);
        repository.CityCodeExistsAsync(2, "CUE", 3, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        SaveCityData? saved = null;
        repository.UpdateCityAsync(Arg.Do<SaveCityData>(value => saved = value), runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(true);

        var result = await new UpdateCityCommandHandler(repository, runner, writer).Handle(
            new UpdateCityCommand(3, 1, 2, "CUE", "Cuenca", false, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        saved!.ExternalSystem.Should().Be("EXTERNAL_CATALOG");
        saved.ExternalCode.Should().Be("EC|AZU|CUE");
        await writer.Received(1).EnqueueAsync(inactive, SyncOperation.Disabled, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_RollsBackWhenOutboxFails()
    {
        var city = CreateCity();
        repository.CityCodeExistsAsync(2, "CUE", null, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        repository.CreateCityAsync(Arg.Any<SaveCityData>(), runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(3);
        repository.GetCityByIdAsync(3, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(city);
        writer.EnqueueAsync(Arg.Any<CityDto>(), Arg.Any<SyncOperation>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>())
            .Returns<Task<Guid?>>(_ => throw new InvalidOperationException("outbox"));

        var action = () => new CreateCityCommandHandler(repository, runner, writer).Handle(
            new CreateCityCommand(1, 2, "CUE", "Cuenca", true, 7, "admin"),
            CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>();
        runner.RolledBack.Should().BeTrue();
    }

    [Fact]
    public async Task Update_RejectsParentChangeBeforePersistence()
    {
        var current = CreateCity();
        repository.GetCityByIdAsync(3, runner.Connection, runner.Transaction, Arg.Any<CancellationToken>()).Returns(current);

        var result = await new UpdateCityCommandHandler(repository, runner, writer).Handle(
            new UpdateCityCommand(3, 1, 99, "CUE", "Cuenca", true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "GEOGRAPHY_CITY_PARENT_CHANGE_NOT_ALLOWED");
        await repository.DidNotReceiveWithAnyArgs().UpdateCityAsync(default!, default!, default!, default);
        await writer.DidNotReceiveWithAnyArgs().EnqueueAsync(default!, default, default!, default!, default);
    }

    [Fact]
    public void Migration_ReservesCompositeIdentityAndHasNoAdoption()
    {
        var sql = ReadSource("database", "sql", "175_tenant_city_transactional_outbox.sql");
        sql.Should().Contain("CREATE UNIQUE INDEX UX_Cities_Province_Code")
            .And.Contain("CREATE UNIQUE INDEX UX_Cities_Province_ExternalRef")
            .And.Contain("WHERE GlobalId=@CountryGlobalId")
            .And.Contain("WHERE GlobalId=@ProvinceGlobalId")
            .And.Contain("SYNC_CITY_HIERARCHY_CONFLICT")
            .And.Contain("SYNC_CITY_PARENT_CONFLICT")
            .And.Contain("SYNC_CITY_CODE_CONFLICT")
            .And.Contain("No se puede reasignar la ciudad a otro pais o provincia.")
            .And.Contain("SELECT -6 ResultCode")
            .And.NotContain("SET GlobalId=@GlobalId");
    }

    private static CityDto CreateCity(bool active = true, Guid? id = null, Guid? countryId = null, Guid? provinceId = null) => new()
    {
        Id = 3,
        GlobalId = id ?? Guid.NewGuid(),
        CountryId = 1,
        CountryGlobalId = countryId ?? Guid.NewGuid(),
        CountryCode = "EC",
        CountryName = "Ecuador",
        ProvinceId = 2,
        ProvinceGlobalId = provinceId ?? Guid.NewGuid(),
        ProvinceCode = "AZU",
        ProvinceName = "Azuay",
        Code = "CUE",
        Name = "Cuenca",
        ExternalSystem = "EXTERNAL_CATALOG",
        ExternalCode = "EC|AZU|CUE",
        IsActive = active,
        CreatedAt = DateTime.UtcNow
    };

    private static string ReadSource(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. parts]);
            if (File.Exists(path)) return File.ReadAllText(path);
            directory = directory.Parent;
        }

        throw new FileNotFoundException();
    }

    private sealed class ImmediateTransactionRunner : ITransactionRunner
    {
        public IDbConnection Connection { get; } = Substitute.For<IDbConnection>();
        public IDbTransaction Transaction { get; } = Substitute.For<IDbTransaction>();
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public async Task ExecuteInTenantTransactionAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation, CancellationToken cancellationToken = default) =>
            await ExecuteInTenantTransactionAsync<object?>(async (connection, transaction, token) =>
            {
                await operation(connection, transaction, token);
                return null;
            }, cancellationToken);

        public async Task<T> ExecuteInTenantTransactionAsync<T>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await operation(Connection, Transaction, cancellationToken);
                Committed = true;
                return result;
            }
            catch
            {
                RolledBack = true;
                throw;
            }
        }
    }
}
