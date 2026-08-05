using System.Data;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Definitions.General.Countries.Commands;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Definitions.General.Countries;

public sealed class CountrySyncPublishingTests
{
    private readonly IGeographyRepository _repository = Substitute.For<IGeographyRepository>();
    private readonly ICountryLocalOutboxWriter _writer = Substitute.For<ICountryLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner _transactionRunner = new();

    [Fact]
    public async Task Create_WritesCountryAndLocalOutboxInsideSameTransaction()
    {
        var country = CreateCountry();
        _repository.CountryCodeExistsAsync("EC", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateCountryAsync(Arg.Any<SaveCountryData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(country.Id);
        _repository.GetCountryByIdAsync(country.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(country);
        var handler = new CreateCountryCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(new CreateCountryCommand("ec", "Ecuador", "EC", "ECU", "+593", true, 7, "admin", "SAP_B1", "EC"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(country, SyncOperation.Created, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
        _transactionRunner.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WithNoExternalValues_PreservesExistingReferenceAndWritesDisabled()
    {
        var current = CreateCountry();
        var inactive = CreateCountry(false, current.GlobalId);
        _repository.GetCountryByIdAsync(current.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(current, inactive);
        _repository.CountryCodeExistsAsync("EC", current.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        SaveCountryData? saved = null;
        _repository.UpdateCountryAsync(Arg.Do<SaveCountryData>(value => saved = value), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);
        var handler = new UpdateCountryCommandHandler(_repository, _transactionRunner, _writer);

        var result = await handler.Handle(new UpdateCountryCommand(current.Id, "EC", "Ecuador", "EC", "ECU", "+593", false, 7, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        saved!.ExternalSystem.Should().Be("SAP_B1");
        saved.ExternalCode.Should().Be("EC");
        await _writer.Received(1).EnqueueAsync(inactive, SyncOperation.Disabled, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_RollsBackWhenLocalOutboxFails()
    {
        var country = CreateCountry();
        _repository.CountryCodeExistsAsync("EC", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateCountryAsync(Arg.Any<SaveCountryData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(country.Id);
        _repository.GetCountryByIdAsync(country.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(country);
        _writer.EnqueueAsync(Arg.Any<CountryDto>(), Arg.Any<SyncOperation>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>())
            .Returns<Task<Guid?>>(_ => throw new InvalidOperationException("Controlled outbox failure"));
        var handler = new CreateCountryCommandHandler(_repository, _transactionRunner, _writer);

        var action = () => handler.Handle(new CreateCountryCommand("EC", "Ecuador", "EC", "ECU", "+593", true, 7, "admin"), CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Controlled outbox failure");
        _transactionRunner.RolledBack.Should().BeTrue();
        _transactionRunner.Committed.Should().BeFalse();
    }

    [Fact]
    public void Migration_ReservesTombstonesAndRejectsCodeAdoption()
    {
        var migration = ReadSource("database", "sql", "168_tenant_country_transactional_outbox.sql");
        migration.Should().Contain("CREATE UNIQUE INDEX UX_Countries_Code ON dbo.Countries(Code)")
            .And.Contain("SP_NA_POST_COUNTRY_SYNC_APPLY_EVENT")
            .And.Contain("Code = @Code AND GlobalId <> @GlobalId")
            .And.Contain("SYNC_COUNTRY_CODE_CONFLICT")
            .And.Contain("ExternalSystem")
            .And.Contain("ExternalCode")
            .And.Contain("CREATE UNIQUE INDEX UX_Countries_ExternalRef")
            .And.Contain("Country external references must be unique")
            .And.Contain("DROP INDEX IX_Countries_ExternalRef")
            .And.Contain("SYNC_COUNTRY_EXTERNAL_CONFLICT")
            .And.NotContain("SET GlobalId = @GlobalId")
            .And.NotContain("adopcion automatica");
    }

    [Fact]
    public void FullSource_IncludesTombstonesAndExternalReferences()
    {
        var source = ReadSource("src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "SyncFullEntitySources.cs");
        var countryStart = source.IndexOf("public sealed class CountryFullEntitySource", StringComparison.Ordinal);
        var provinceStart = source.IndexOf("public sealed class ProvinceFullEntitySource", StringComparison.Ordinal);
        var countrySource = source[countryStart..provinceStart];

        countrySource.Should().Contain("IsDeleted")
            .And.Contain("ExternalSystem")
            .And.Contain("ExternalCode")
            .And.Contain("!row.IsDeleted && row.IsActive")
            .And.NotContain("WHERE IsDeleted = 0");
    }

    private static CountryDto CreateCountry(bool isActive = true, Guid? globalId = null) => new()
    {
        Id = 1,
        GlobalId = globalId ?? Guid.NewGuid(),
        Code = "EC",
        Name = "Ecuador",
        Iso2 = "EC",
        Iso3 = "ECU",
        PhonePrefix = "+593",
        ExternalSystem = "SAP_B1",
        ExternalCode = "EC",
        IsActive = isActive,
        CreatedAt = new DateTime(2026, 8, 4, 10, 0, 0, DateTimeKind.Utc)
    };

    private static string ReadSource(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. pathParts]);
            if (File.Exists(path)) return File.ReadAllText(path);
            directory = directory.Parent;
        }
        throw new FileNotFoundException(Path.Combine(pathParts));
    }

    private sealed class ImmediateTransactionRunner : ITransactionRunner
    {
        public IDbConnection Connection { get; } = Substitute.For<IDbConnection>();
        public IDbTransaction Transaction { get; } = Substitute.For<IDbTransaction>();
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public async Task ExecuteInTenantTransactionAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation, CancellationToken cancellationToken = default) =>
            await ExecuteInTenantTransactionAsync<object?>(async (connection, transaction, token) => { await operation(connection, transaction, token); return null; }, cancellationToken);

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
