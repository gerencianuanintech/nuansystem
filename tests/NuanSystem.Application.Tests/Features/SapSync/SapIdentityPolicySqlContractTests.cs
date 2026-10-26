using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapIdentityPolicySqlContractTests
{
    [Fact]
    public void Master_policy_script_contains_company_scoped_prefix_rules_and_crud_procedures()
    {
        var path = Find("database", "sql", "234_master_sap_identity_policies.sql");
        var sql = File.ReadAllText(path);
        sql.Should().Contain("CREATE TABLE dbo.SapBusinessPartnerIdentityPolicies")
            .And.Contain("CompanyId")
            .And.Contain("CustomerNationalPrefix")
            .And.Contain("SupplierForeignPrefix")
            .And.Contain("PassportTaxpayerTypeCodes")
            .And.Contain("SP_NA_GET_SAPIDENTITYPOLICY_BUSCAR")
            .And.Contain("SP_NA_POST_SAPIDENTITYPOLICY_GUARDAR");
    }

    private static string Find(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln"))) directory = directory.Parent;
        return Path.Combine([directory!.FullName, .. parts]);
    }
}
