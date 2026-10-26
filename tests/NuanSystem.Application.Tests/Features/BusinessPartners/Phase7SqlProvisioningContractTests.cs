using System.Text.Json;
using System.Text.RegularExpressions;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category", "OfflineSafety")]
public sealed class Phase7SqlProvisioningContractTests
{
    [Theory]
    [InlineData("A", 901)]
    [InlineData("B", 902)]
    public void Provisioning_fault_definitions_match_the_suite_renderer(string role, int companyId)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;
        Assert.NotNull(directory);
        var fixture = Path.Combine(directory.FullName, "database", "lab", "phase7", "review-example", $"trigger-definitions-{role}.json");
        using var json = JsonDocument.Parse(File.ReadAllText(fixture));
        var expected = json.RootElement.EnumerateArray().Select(x => Normalize(x.GetProperty("Definition").GetString()!)).ToArray();
        var rendered = Phase7SqlFaultScript.Render(Guid.Parse("12345678-1234-1234-1234-123456789abc"), role, companyId);
        var actual = Regex.Matches(rendered.Sql, @"EXEC\(N'((?:[^']|'')*)'\);")
            .Select(x => Normalize(x.Groups[1].Value.Replace("''", "'"))).ToArray();
        Assert.Equal(3, actual.Length);
        Assert.Equal(expected, actual);
    }

    private static string Normalize(string text) => text.Replace("\r\n", "\n").TrimEnd();
}
