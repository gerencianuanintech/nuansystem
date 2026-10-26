using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.Purchasing.PurchaseOrders;

public sealed class PurchaseOrderApprovalPersistenceContractTests
{
    [Fact]
    public void Migration_defines_one_atomic_locked_workflow_operation()
    {
        var script = ReadSource("database", "sql", "237_tenant_purchase_order_approval_workflow.sql");

        script.Should().Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_PATCH_PURCHASEORDERS_WORKFLOW");
        script.Should().Contain("SET ANSI_NULLS ON");
        script.Should().Contain("SET QUOTED_IDENTIFIER ON");
        script.Should().Contain("SET XACT_ABORT ON");
        script.Should().Contain("BEGIN TRANSACTION");
        script.Should().Contain("WITH (UPDLOCK, HOLDLOCK)");
        script.Should().Contain("UPDATE dbo.PurchaseOrderHeaders");
        script.Should().Contain("INSERT INTO dbo.PurchaseOrderApprovals");
        script.Should().Contain("UPDATE dbo.PurchaseOrderApprovals");
        script.Should().Contain("COMMIT TRANSACTION");
        script.Should().Contain("ROLLBACK TRANSACTION");
    }

    [Fact]
    public void Migration_prevents_two_open_approvals_for_the_same_order_and_level()
    {
        var script = ReadSource("database", "sql", "237_tenant_purchase_order_approval_workflow.sql");

        script.Should().Contain("CREATE UNIQUE INDEX UX_PurchaseOrderApprovals_OpenLevel");
        script.Should().Contain("ON dbo.PurchaseOrderApprovals (PurchaseOrderId, ApprovalLevel)");
        script.Should().Contain("WHERE Status = N'Pending'");
        script.Should().NotContain("DELETE FROM dbo.PurchaseOrderApprovals");
    }

    [Fact]
    public void Repository_maps_stable_outcomes_without_exposing_SQL_to_application()
    {
        var repository = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "Purchasing",
            "PurchaseOrderRepository.cs");
        var contract = ReadSource(
            "src", "Backend", "NuanSystem.Application", "Abstractions", "Data",
            "IPurchaseOrderRepository.cs");

        repository.Should().Contain("SP_NA_PATCH_PURCHASEORDERS_WORKFLOW");
        repository.Should().Contain("PurchaseOrderWorkflowTransitionResult.NotFound()");
        repository.Should().Contain("PurchaseOrderWorkflowTransitionResult.Conflict()");
        repository.Should().Contain("PurchaseOrderWorkflowTransitionResult.Updated(order)");
        contract.Should().NotContain("SqlException");
        contract.Should().NotContain("Microsoft.Data.SqlClient");
    }

    [Fact]
    public void Tenant_initializer_registers_workflow_after_purchase_order_schema()
    {
        var initializer = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerTenantDatabaseInitializer.cs");

        initializer.Should().Contain("237_tenant_purchase_order_approval_workflow.sql");
        initializer.IndexOf("237_tenant_purchase_order_approval_workflow.sql", StringComparison.Ordinal)
            .Should().BeGreaterThan(initializer.IndexOf("046_tenant_purchase_orders.sql", StringComparison.Ordinal));
    }

    private static string ReadSource(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. pathParts]);
            if (File.Exists(path))
            {
                return File.ReadAllText(path).ReplaceLineEndings("\n");
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(Path.Combine(pathParts));
    }
}
