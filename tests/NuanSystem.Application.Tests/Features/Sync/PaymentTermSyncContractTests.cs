using FluentAssertions;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class PaymentTermSyncContractTests
{
    [Fact]
    public void Payload_PreservesApprovedPaymentTermFields()
    {
        var payload = new ReferenceCatalogSyncPayload(
            Guid.NewGuid(), "7", "Credito 30", null, null, null, null, false, true,
            "SAP_B1", "7", DateTime.UtcNow, null, 30, true);

        payload.Days.Should().Be(30);
        payload.IsCredit.Should().BeTrue();
        payload.ExternalSystem.Should().Be("SAP_B1");
    }

    [Fact]
    public void BranchApply_DoesNotRequireDescriptionColumnForPaymentTerms()
    {
        var repository = File.ReadAllText(Path.Combine(RepoRoot(),
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync",
            "ReferenceCatalogSyncApplyRepository.cs"));

        repository.Should().Contain("includeDescription: false");
        repository.Should().Contain("var descriptionColumn = includeDescription");
        repository.Should().Contain("var descriptionUpdate = includeDescription");
    }

    [Fact]
    public void ForwardScripts_KeepSapAndMasterBranchContractsExplicit()
    {
        var tenant = File.ReadAllText(Path.Combine(RepoRoot(), "database", "sql", "112_tenant_sap_payment_terms_sync.sql"));
        var master = File.ReadAllText(Path.Combine(RepoRoot(), "database", "sql", "113_master_payment_terms_sync_registration.sql"));
        var masterConfiguration = File.ReadAllText(Path.Combine(RepoRoot(), "database", "sql", "114_master_payment_terms_sync_configuration.sql"));

        tenant.Should().Contain("SP_NA_POST_BUSINESSPARTNERPAYMENTTERMS_IMPORTARSAP");
        tenant.Should().Contain("UX_BusinessPartnerPaymentTerms_ExternalRef");
        tenant.Should().Contain("no se adopta automaticamente");
        var applier = File.ReadAllText(Path.Combine(RepoRoot(), "src", "Backend", "NuanSystem.Persistence", "Repositories", "Sync", "ReferenceCatalogSyncApplyRepository.cs"));
        applier.Should().Contain("allowCodeReconciliation: false");
        applier.Should().Contain("no se adopta automaticamente durante la sincronizacion");
        master.Should().Contain("N'PaymentTerms'");
        master.Should().Contain("N'BusinessPartnerPaymentTerms'");
        master.Should().Contain("c.IsMaster=1");
        masterConfiguration.Should().Contain("dbo.SyncEntityConfigurations");
        masterConfiguration.Should().Contain("N'BusinessPartnerPaymentTerms'");
        masterConfiguration.Should().Contain("CONVERT(bit, 0)");
    }

    private static string RepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null && !File.Exists(Path.Combine(current.FullName, "NuanSystem.sln")))
            current = current.Parent;
        return current?.FullName ?? throw new DirectoryNotFoundException("No se encontro la raiz del repositorio.");
    }
}
