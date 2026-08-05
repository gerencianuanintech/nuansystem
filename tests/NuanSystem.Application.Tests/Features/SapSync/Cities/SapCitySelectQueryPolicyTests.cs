using FluentAssertions;
using NuanSystem.Application.Features.SapSync.Cities.Configuration;

namespace NuanSystem.Application.Tests.Features.SapSync.Cities;

public sealed class SapCitySelectQueryPolicyTests
{
    private const string HanaQuery = """
        SELECT
          'EC' AS "CountryCode",
          LEFT(TRIM("Code"), 2) AS "ProvinceCode",
          TRIM("Code") AS "CityCode",
          TRIM("Name") AS "CityName"
        FROM "@MUNI_CANTO"
        WHERE LENGTH(TRIM("Code")) >= 2
        ORDER BY "Code"
        """;

    [Fact]
    public void RecommendedHanaSelect_WithQuotedAliases_IsValid()
    {
        SapCitySelectQueryPolicy.TryValidate(HanaQuery, out var error).Should().BeTrue();
        error.Should().BeNull();
    }

    [Fact]
    public void Select_WithUnquotedAliases_IsValid()
    {
        const string query = "SELECT 'EC' AS CountryCode, '01' AS ProvinceCode, '0101' AS CityCode, 'Quito' AS CityName FROM DUMMY";
        SapCitySelectQueryPolicy.TryValidate(query, out _).Should().BeTrue();
    }

    [Theory]
    [InlineData("SELECT 'EC' AS CountryCode, '01' AS ProvinceCode, '0101' AS CityCode FROM DUMMY")]
    [InlineData("SELECT 'EC' AS CountryCode, '01' AS ProvinceCode, '0101' AS CityCode, 'Quito' AS CityName FROM DUMMY;")]
    [InlineData("SELECT 'EC' AS CountryCode, '01' AS ProvinceCode, '0101' AS CityCode, 'Quito' AS CityName FROM DUMMY -- comment")]
    [InlineData("UPDATE X SET Y = 1")]
    public void UnsafeOrIncompleteQuery_IsRejected(string query)
    {
        SapCitySelectQueryPolicy.TryValidate(query, out var error).Should().BeFalse();
        error.Should().NotBeNullOrWhiteSpace();
    }
}
