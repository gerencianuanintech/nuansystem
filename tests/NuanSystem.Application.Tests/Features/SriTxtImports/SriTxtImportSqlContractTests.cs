using System.Text.Json;
using FluentAssertions;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriDocuments.Dtos;
using NuanSystem.Application.Features.SriTxtImports.Dtos;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Application.Tests.Features.SriTxtImports;

public sealed class SriTxtImportSqlContractTests
{
    [Fact]
    public void TenantScript_DefinesStagedImportWithoutDuplicatingAccessKeyInDetail()
    {
        var script = ReadSourceFile("database", "sql", "138_tenant_sri_txt_import.sql");
        var detailTableStart = script.IndexOf("CREATE TABLE dbo.SriTxtImportRows", StringComparison.Ordinal);
        var detailTableEnd = script.IndexOf(
            "CREATE INDEX IX_SriTxtImportRows_Import_Status",
            detailTableStart,
            StringComparison.Ordinal);
        var detailTable = script[detailTableStart..detailTableEnd];

        script.Should().Contain("Status IN\n            (\n                N'Staged', N'Pending'");
        script.Should().Contain("CREATE UNIQUE INDEX UX_SriTxtImports_FileSha256");
        script.Should().Contain("CREATE TYPE dbo.SriTxtImportRowTableType");
        script.Should().Contain("WHERE source.ValidationStatus = 'Valid'");
        script.Should().Contain("N'Staged'");
        script.Should().Contain("SP_NA_POST_SRITXTIMPORT_ENCOLAR");
        script.Should().Contain("WHERE queue.Status = N'Staged'");
        script.Should().Contain("SET Status = N'Pending'");
        script.Should().Contain("N'EnqueueFromTxt', N'Staged', N'Pending'");
        script.Should().Contain("WITH (UPDLOCK, HOLDLOCK)");
        script.Should().Contain("20260727.138");
        script.Should().NotContain("INSERT dbo.SriDocumentAttempts");
        detailTable.Should().NotContain("AccessKey char(49)");
        detailTable.Should().Contain("AccessKeySha256 binary(32)");
        detailTable.Should().Contain("MaskedAccessKey varchar(20)");
    }

    [Fact]
    public void WorkerClaim_ContinuesToSelectOnlyPendingAndRetryScheduled()
    {
        var workerScript = ReadSourceFile(
            "database",
            "sql",
            "117_tenant_sri_worker_and_document_store.sql");
        var claimStart = workerScript.IndexOf(
            "CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRIDOCUMENTQUEUE_RECLAMAR",
            StringComparison.Ordinal);
        var claimEnd = workerScript.IndexOf("\nGO", claimStart, StringComparison.Ordinal);
        var claim = workerScript[claimStart..claimEnd];

        claim.Should().Contain("Status IN (N'Pending',N'RetryScheduled')");
        claim.Should().NotContain("N'Staged'");
    }

    [Fact]
    public void MasterScript_SeedsPermissionsWithoutRoleGrants()
    {
        var script = ReadSourceFile("database", "sql", "139_master_sri_txt_import_security.sql");

        foreach (var permission in ApprovedPermissions())
        {
            script.Should().Contain(permission);
            PermissionCodes.All.Should().Contain(permission);
        }

        script.Should().NotContain("RolePermissions");
        script.Should().NotContain("SecurityForms");
        script.Should().NotContain("SecurityMenus");
        script.Should().Contain("20260727.139");
    }

    [Fact]
    public void Api_UsesIndependentUploadAndEnqueuePermissions()
    {
        var endpoints = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Api",
            "Endpoints",
            "SriTxtImportEndpoints.cs");

        endpoints.Should().Contain("PermissionCodes.SriTxtImportsUpload");
        endpoints.Should().Contain("PermissionCodes.SriTxtImportsEnqueue");
        endpoints.Should().Contain("DisableAntiforgery");
        endpoints.Should().Contain("MaxProcessingSeconds");
    }

    [Fact]
    public void CrudScripts_ArePagedSanitizedAndDoNotGrantExistingRoles()
    {
        var tenant = ReadSourceFile("database", "sql", "142_tenant_sri_txt_import_crud.sql");
        var master = ReadSourceFile("database", "sql", "143_master_sri_txt_import_crud_security.sql");

        tenant.Should().Contain("SP_NA_GET_SRITXTIMPORT_LISTAR");
        tenant.Should().Contain("SP_NA_GET_SRITXTIMPORT_PORID");
        tenant.Should().Contain("SP_NA_GET_SRITXTIMPORT_FILAS");
        tenant.Should().Contain("OFFSET (@Page - 1) * @PageSize ROWS");
        tenant.Should().Contain("queue.Status = N'Staged'");
        tenant.Should().Contain("queue.Status = N'Pending'");
        tenant.Should().Contain("20260727.142");
        tenant.Should().Contain("THROW 51142");
        tenant.Should().NotContain("queue.AccessKey");
        tenant.Should().NotContain("import.HeaderLine");
        tenant.Should().NotContain("INSERT dbo.SriDocument");
        tenant.Should().NotContain("UPDATE dbo.SriDocument");

        master.Should().Contain("sri-txt-imports");
        master.Should().Contain("Code = N'ACTION.REFRESH'");
        master.Should().Contain("Code = N'ACTION.CONSULT'");
        master.Should().Contain("ACTION.SRI_TXT_IMPORTS.ENQUEUE");
        master.Should().Contain("ACTION.SRI_TXT_IMPORTS.OPEN_QUEUE");
        master.Should().Contain("Encolar TXT SRI");
        master.Should().Contain("Abrir cola SRI");
        master.Should().NotContain("ACTION.SRI_TXT_IMPORTS.REFRESH");
        master.Should().NotContain("ACTION.SRI_TXT_IMPORTS.CONSULT");
        master.Should().Contain("BEGIN TRANSACTION");
        master.Should().Contain("ROLLBACK TRANSACTION");
        master.Should().Contain("COMMIT TRANSACTION");
        master.Should().Contain("20260727.143");
        master.Should().Contain("THROW 51143");
        master.Should().NotContain("RolePermissions");
        master.Should().NotContain("SecurityRoleMenus");
        master.Should().NotContain("SecurityRoleFormOperations");
    }

    [Fact]
    public void RibbonMigration_RegistersUploadAndFilterWithoutGrantingApiPermissions()
    {
        var script = ReadSourceFile(
            "database",
            "sql",
            "147_master_sri_txt_import_ribbon_operations.sql");

        script.Should().Contain("ACTION.SRI_TXT_IMPORTS.UPLOAD");
        script.Should().Contain("ACTION.FILTER");
        script.Should().Contain("N'upload' AS ActionKey");
        script.Should().Contain("N'Inicio' AS RibbonPageName");
        script.Should().Contain("N'Acciones' AS RibbonGroupName");
        script.Should().Contain("Operaciones/importar_32.svg");
        script.Should().Contain("SRI.TXT_IMPORTS.VIEW");
        script.Should().Contain("SRI.TXT_IMPORTS.UPLOAD");
        script.Should().Contain("SecurityRoleFormOperations");
        script.Should().Contain("20260728.147");
        script.Should().Contain("THROW 51147");
        script.Should().NotContain("INSERT dbo.RolePermissions");
        script.Should().NotContain("INSERT INTO dbo.RolePermissions");
    }

    [Fact]
    public void MigrationSequence_AssignsEachNumberAndVersionToOneOwner()
    {
        var expected = new Dictionary<string, string>
        {
            ["138_tenant_sri_txt_import.sql"] = "20260727.138",
            ["139_master_sri_txt_import_security.sql"] = "20260727.139",
            ["140_tenant_price_list_transactional_outbox.sql"] = "20260727.140",
            ["141_master_price_list_transactional_registration.sql"] = "20260727.141",
            ["142_tenant_sri_txt_import_crud.sql"] = "20260727.142",
            ["143_master_sri_txt_import_crud_security.sql"] = "20260727.143"
        };
        var sqlDirectory = Path.Combine(RepositoryRoot(), "database", "sql");
        var numberedFiles = Directory.GetFiles(sqlDirectory, "*.sql")
            .Select(Path.GetFileName)
            .OfType<string>()
            .Where(name => int.TryParse(name[..3], out var number) && number is >= 138 and <= 143)
            .ToArray();

        numberedFiles.Should().BeEquivalentTo(expected.Keys);
        foreach (var item in expected)
            File.ReadAllText(Path.Combine(sqlDirectory, item.Key)).Should().Contain(item.Value);

        expected.Values.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Api_CrudRoutesRequireViewWithoutMergingMutationPermissions()
    {
        var endpoints = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Api",
            "Endpoints",
            "SriTxtImportEndpoints.cs");

        endpoints.Split("PermissionCodes.SriTxtImportsView").Should().HaveCount(4);
        endpoints.Split("PermissionCodes.SriTxtImportsUpload").Should().HaveCount(2);
        endpoints.Split("PermissionCodes.SriTxtImportsEnqueue").Should().HaveCount(2);
        endpoints.Should().Contain("\"/{id:long}/rows\"");
    }

    [Fact]
    public void CrudPersistence_UsesOnlyAuthenticatedTenantConnectionBoundary()
    {
        var repository = ReadSourceFile(
            "src",
            "Backend",
            "NuanSystem.Persistence",
            "Repositories",
            "SriTxtImportRepository.cs");

        repository.Should().Contain("ITenantConnectionFactory");
        repository.Should().Contain("connectionFactory.CreateConnection()");
        repository.Should().NotContain("IMasterConnectionFactory");
        repository.Should().NotContain("MasterConnection");
    }

    [Fact]
    public void QueueDto_MasksAccessKeyAndDisplaysStagedDistinctly()
    {
        var key = new string('1', 49);
        var dto = new SriDocumentQueueDetailDto
        {
            AccessKey = key,
            Status = SriDocumentQueueStatusCodes.Staged
        };

        var json = JsonSerializer.Serialize(dto);

        json.Should().NotContain(key);
        json.Should().Contain("********11111111");
        json.Should().Contain("Preparado");
        SriDocumentQueueStatusCodes.GetDisplayName(SriDocumentQueueStatusCodes.Pending)
            .Should().Be("Pendiente de consulta");
    }

    [Fact]
    public void ImportDetail_DoesNotSerializeStoredHeaderLine()
    {
        const string originalHeader = "CLAVE DE ACCESO\tRAZON SOCIAL";
        var dto = new SriTxtImportDetailDto
        {
            Id = 1,
            HeaderLine = originalHeader,
            OriginalFileName = "fixture.txt"
        };

        var json = JsonSerializer.Serialize(dto);

        json.Should().NotContain(originalHeader);
        json.Should().NotContain("HeaderLine");
    }

    private static IReadOnlyCollection<string> ApprovedPermissions() =>
    [
        "SRI.TXT_IMPORTS.VIEW",
        "SRI.TXT_IMPORTS.UPLOAD",
        "SRI.TXT_IMPORTS.VALIDATE",
        "SRI.TXT_IMPORTS.ENQUEUE",
        "SRI.TXT_IMPORTS.REPROCESS",
        "SRI.TXT_IMPORTS.DELETE",
        "SRI.TXT_IMPORTS.VIEW_ERRORS",
        "SRI.TXT_IMPORTS.VIEW_HISTORY",
        "SRI.TXT_IMPORTS.VIEW_SAP_STATUS"
    ];

    private static string ReadSourceFile(params string[] pathParts)
    {
        var path = Path.Combine(new[] { RepositoryRoot() }.Concat(pathParts).ToArray());
        return File.Exists(path)
            ? File.ReadAllText(path).ReplaceLineEndings("\n")
            : throw new FileNotFoundException($"No se encontro {Path.Combine(pathParts)}.");
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var gitPath = Path.Combine(directory.FullName, ".git");
            if (Directory.Exists(gitPath) || File.Exists(gitPath))
                return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("No se encontro la raiz del repositorio.");
    }
}
