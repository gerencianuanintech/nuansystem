using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapIdentityPolicyNavigationSqlContractTests
{
    [Fact]
    public void Navigation_script_registers_form_menu_key_and_operations()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln"))) directory = directory.Parent;
        var sql = File.ReadAllText(Path.Combine(directory!.FullName, "database", "sql", "235_master_sap_identity_policy_navigation.sql"));
        sql.Should().Contain("sap-identity-policy").And.Contain("FORM.ADMINISTRATION.SAP.IDENTITY_POLICY").And.Contain("ACTION.CONSULT").And.Contain("ACTION.UPDATE");
    }
}
