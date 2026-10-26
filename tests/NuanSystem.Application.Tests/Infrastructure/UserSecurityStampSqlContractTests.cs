using FluentAssertions;

namespace NuanSystem.Application.Tests.Infrastructure;

public sealed class UserSecurityStampSqlContractTests
{
    [Fact]
    public void Migration_rotates_stamps_for_credentials_users_roles_and_permissions()
    {
        var script = ReadSource("database", "sql", "238_master_user_security_stamp.sql");

        script.Should().Contain("SecurityStamp uniqueidentifier");
        script.Should().Contain("SET ANSI_NULLS ON");
        script.Should().Contain("SET QUOTED_IDENTIFIER ON");
        script.Should().Contain("TR_Users_RotateSecurityStamp");
        script.Should().Contain("TR_UserRoles_RotateSecurityStamp");
        script.Should().Contain("TR_RolePermissions_RotateSecurityStamp");
        script.Should().Contain("TR_Roles_RotateSecurityStamp");
        script.Should().Contain("PasswordHash");
        script.Should().Contain("IsActive");
        script.Should().Contain("IsLocked");
        script.Should().Contain("NEWID()");
    }

    [Fact]
    public void Master_initializer_registers_stamp_after_security_schema()
    {
        var initializer = ReadSource("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");

        initializer.IndexOf("238_master_user_security_stamp.sql", StringComparison.Ordinal)
            .Should().BeGreaterThan(initializer.IndexOf("010_master_security_roles.sql", StringComparison.Ordinal));
    }

    [Fact]
    public void Master_initializer_creates_worker_heartbeat_before_operational_repairs()
    {
        var initializer = ReadSource("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");

        var foundation = initializer.IndexOf("049_master_sap_sync_worker.sql", StringComparison.Ordinal);
        foundation.Should().BeGreaterThan(-1);
        foundation.Should().BeLessThan(initializer.IndexOf("120_master_worker_heartbeat_operations.sql", StringComparison.Ordinal));
        foundation.Should().BeLessThan(initializer.IndexOf("122_master_worker_heartbeat_operations_idempotency_fix.sql", StringComparison.Ordinal));
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
