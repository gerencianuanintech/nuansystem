using System.Text.Json;
using FluentAssertions;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriDocuments.Dtos;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Application.Tests.Features.SriTxtImports;

public sealed class SriTxtImportSqlContractTests
{
    [Fact]
    public void TenantScript_DefinesStagedImportWithoutDuplicatingAccessKeyInDetail()
    {
        var script = ReadSourceFile("database", "sql", "138_tenant_sri_txt_import.sql");
        var detailTableStart = script.IndexOf("CREATE TABLE dbo.SriTxtImportRows", StringComparison.Ordinal);
        var detailTableEnd = script.IndexOf("\n    );\nEND;", detailTableStart, StringComparison.Ordinal);
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
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontro {Path.Combine(pathParts)}.");
    }
}
