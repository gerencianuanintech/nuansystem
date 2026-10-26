using FluentAssertions;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class UserSecurityStampContractTests
{
    [Fact]
    public void Token_validation_reads_active_state_and_persisted_stamp_in_one_query()
    {
        var service = ReadSource("src", "Backend", "NuanSystem.Persistence", "Security", "UserSecurityStateService.cs");

        service.Should().Contain("SecurityStamp");
        service.Should().Contain("IsActive = 1");
        service.Should().Contain("IsLocked = 0");
        service.Should().NotContain("GetRolesAsync");
        service.Should().NotContain("GetPermissionsAsync");
        service.Split("ExecuteScalarAsync").Should().HaveCount(2);
    }

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
}
