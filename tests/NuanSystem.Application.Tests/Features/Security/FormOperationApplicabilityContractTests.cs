using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.Security;

public sealed class FormOperationApplicabilityContractTests
{
    [Fact]
    public void Migration179_ShouldCreateAuditedApplicabilityAndRegisterAfterNavigationRepair()
    {
        var sql = Read("database", "sql", "179_master_security_form_operation_applicability.sql");
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");

        sql.Should().Contain("CREATE TABLE dbo.SecurityFormOperations")
            .And.Contain("FK_SecurityFormOperations_Form")
            .And.Contain("FK_SecurityFormOperations_Operation")
            .And.Contain("UQ_SecurityFormOperations_Form_Operation")
            .And.Contain("CreatedByUserName nvarchar(120)")
            .And.Contain("IsDeleted bit NOT NULL")
            .And.Contain("Version = N'20260805.179'");

        initializer.IndexOf(
                "179_master_security_form_operation_applicability.sql",
                StringComparison.Ordinal)
            .Should().BeGreaterThan(initializer.IndexOf(
                "178_master_configuration_definitions_general_navigation_repair.sql",
                StringComparison.Ordinal));
    }

    [Fact]
    public void Migration179_ShouldCanonicalizeLegacyCrudWithoutOverwritingExplicitDecision()
    {
        var sql = Read("database", "sql", "179_master_security_form_operation_applicability.sql");

        sql.Should().Contain("WHERE Code = N'ACTION.CREATE'")
            .And.Contain("WHERE Code = N'ACTION.NEW'")
            .And.Contain("WHERE Code = N'ACTION.UPDATE'")
            .And.Contain("WHERE Code = N'ACTION.EDIT'")
            .And.Contain("canonical.OperationId = @CreateOperationId")
            .And.Contain("canonical.OperationId = @UpdateOperationId")
            .And.Contain("N'ACTION.CUSTOMIZE_COLUMNS', N'ACTION.EXPORT_EXCEL'")
            .And.NotContain("N'ACTION.RELOAD_ACCESS', N'ACTION.CREATE'");
    }

    [Fact]
    public void SecurityProcedures_ShouldFilterAndValidateApplicableOperations()
    {
        var sql = Read("database", "sql", "179_master_security_form_operation_applicability.sql");

        sql.Should().Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SEGURIDADOPERACIONESUSUARIO")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_ACCESOROLCARGAR")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_ACCESOROLGUARDAR")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMACCESS_OPERACIONES")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_PUT_SECURITYROLEFORMACCESS_GUARDAR")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMACCESS_VALIDAR")
            .And.Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_GET_SECURITYROLEFORMACCESS_USUARIO")
            .And.Contain("The payload contains an operation that is not applicable")
            .And.Contain("applicable.IsActive = 1")
            .And.Contain("applicable.IsDeleted = 0");
    }

    [Fact]
    public void SpecializedOperations_ShouldBeOwnedOnlyByTheirForms()
    {
        var sql = Read("database", "sql", "179_master_security_form_operation_applicability.sql");

        sql.Should().Contain("(N'sap-sync-profiles', N'ACTION.SAP_SYNC_PROFILES.VALIDATE')")
            .And.Contain("(N'sap-sync-executions', N'ACTION.SAP_SYNC_EXECUTIONS.RETRY')")
            .And.Contain("(N'sri-txt-imports', N'ACTION.SRI_TXT_IMPORTS.UPLOAD')")
            .And.Contain("(N'sri-document-monitor', N'ACTION.DOWNLOAD_XML')")
            .And.Contain("form.FormKey IN (N'countries', N'provinces', N'cities')")
            .And.Contain("operation.Code LIKE N'ACTION.SAP[_]SYNC[_]%'");
    }

    [Fact]
    public void EndpointAuthorization_ShouldUseOnlyTheRequestedCanonicalAction()
    {
        var source = Read(
            "src", "Backend", "NuanSystem.Api", "Extensions",
            "EndpointAuthorizationExtensions.cs");

        source.Should().Contain("return [actionKey];")
            .And.NotContain("\"create\" => [\"create\", \"new\"")
            .And.NotContain("\"refresh\" => [\"refresh\", \"actualizar\", \"read\", \"consult\"")
            .And.NotContain("HasSecurityAdminBypass")
            .And.NotContain("PermissionCodes.SecurityAccessBypass");
    }

    private static string Read(params string[] parts)
    {
        var root = FindRepositoryRoot();
        return File.ReadAllText(Path.Combine(new[] { root }.Concat(parts).ToArray()));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "database")))
            directory = directory.Parent;

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("No se encontró la raíz del repositorio.");
    }
}
