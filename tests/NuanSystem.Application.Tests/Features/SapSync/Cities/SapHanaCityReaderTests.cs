using System.Data;
using System.Data.Common;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.SapIntegration.Cities;
using NuanSystem.SapIntegration.Hana;

namespace NuanSystem.Application.Tests.Features.SapSync.Cities;

public sealed class SapHanaCityReaderTests
{
    private const string Query = """
        SELECT
            'EC' AS "CountryCode",
            LEFT(TRIM("Code"), 2) AS "ProvinceCode",
            TRIM("Code") AS "CityCode",
            TRIM("Name") AS "CityName"
        FROM "@MUNI_CANTO"
        """;

    [Fact]
    public async Task ConfiguredQuery_IsNormalizedExecutedForCompanyAndMappedByAliases()
    {
        var settingsRepository = Substitute.For<ISapCompanySettingsRepository>();
        settingsRepository.GetByCompanyIdAsync(7, Arg.Any<CancellationToken>())
            .Returns(Settings($"  {Query}\r\n  "));
        var queryClient = new CapturingHanaQueryClient(
            Row(" EC ", " 01 ", " 0101 ", " Quito "));
        var reader = new SapHanaCityReader(settingsRepository, queryClient);

        var result = await reader.GetCitiesAsync(7, CancellationToken.None);

        result.Should().ContainSingle().Which.Should().Be(
            new SapCityRecord("EC", "01", "0101", "Quito"));
        queryClient.CompanyId.Should().Be(7);
        queryClient.Sql.Should().Be(Query.Trim());
        queryClient.Parameters.Should().BeNull();
        await settingsRepository.Received(1)
            .GetByCompanyIdAsync(7, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task NullDatabaseValues_AreMappedAsEmptyStrings()
    {
        var settingsRepository = Substitute.For<ISapCompanySettingsRepository>();
        settingsRepository.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Settings(Query));
        var queryClient = new CapturingHanaQueryClient(Row(DBNull.Value, "01", "0101", DBNull.Value));
        var reader = new SapHanaCityReader(settingsRepository, queryClient);

        var result = await reader.GetCitiesAsync(1);

        result.Should().ContainSingle().Which.Should().Be(
            new SapCityRecord(string.Empty, "01", "0101", string.Empty));
    }

    [Fact]
    public async Task MissingConfiguration_FailsBeforeCallingHana()
    {
        var settingsRepository = Substitute.For<ISapCompanySettingsRepository>();
        settingsRepository.GetByCompanyIdAsync(3, Arg.Any<CancellationToken>())
            .Returns((SapCompanySettingsDto?)null);
        var queryClient = new CapturingHanaQueryClient(Row("EC", "01", "0101", "Quito"));
        var reader = new SapHanaCityReader(settingsRepository, queryClient);

        var action = () => reader.GetCitiesAsync(3);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SAP_CITY_QUERY_INVALID:*");
        queryClient.CallCount.Should().Be(0);
    }

    [Fact]
    public async Task CancellationToken_IsForwardedToHanaClient()
    {
        var settingsRepository = Substitute.For<ISapCompanySettingsRepository>();
        settingsRepository.GetByCompanyIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(Settings(Query));
        var queryClient = new CapturingHanaQueryClient(Row("EC", "01", "0101", "Quito"));
        var reader = new SapHanaCityReader(settingsRepository, queryClient);
        using var source = new CancellationTokenSource();

        await reader.GetCitiesAsync(2, source.Token);

        queryClient.CancellationToken.Should().Be(source.Token);
    }

    private static SapCompanySettingsDto Settings(string query) => new()
    {
        Id = 10,
        CompanyId = 1,
        CompanyCode = "DEMO",
        CitiesSelectQuery = query
    };

    private static DataTable Row(object countryCode, object provinceCode, object cityCode, object cityName)
    {
        var table = new DataTable();
        table.Columns.Add("CountryCode", typeof(string));
        table.Columns.Add("ProvinceCode", typeof(string));
        table.Columns.Add("CityCode", typeof(string));
        table.Columns.Add("CityName", typeof(string));
        table.Rows.Add(countryCode, provinceCode, cityCode, cityName);
        return table;
    }

    private sealed class CapturingHanaQueryClient(DataTable table) : ISapHanaQueryClient
    {
        public int CallCount { get; private set; }
        public int CompanyId { get; private set; }
        public string? Sql { get; private set; }
        public IReadOnlyDictionary<string, object?>? Parameters { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        public Task<IReadOnlyCollection<T>> QueryAsync<T>(
            int companyId,
            string sql,
            Func<DbDataReader, T> map,
            IReadOnlyDictionary<string, object?>? parameters = null,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            CompanyId = companyId;
            Sql = sql;
            Parameters = parameters;
            CancellationToken = cancellationToken;
            using var dataReader = table.CreateDataReader();
            var results = new List<T>();
            while (dataReader.Read())
            {
                results.Add(map(dataReader));
            }

            return Task.FromResult<IReadOnlyCollection<T>>(results);
        }
    }
}
