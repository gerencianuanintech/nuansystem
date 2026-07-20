using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SriDocuments;

public sealed class SriDocumentQueueContractTests
{
    [Fact]
    public void TenantScript_DefinesDurableIdempotentQueueAndAuditedTransitions()
    {
        var script = ReadSourceFile("database", "sql", "115_tenant_sri_document_queue.sql");

        script.Should().Contain("CREATE TABLE dbo.SriDocumentQueue");
        script.Should().Contain("CREATE UNIQUE INDEX UX_SriDocumentQueue_Environment_AccessKey");
        script.Should().Contain("CREATE TABLE dbo.SriDocumentAttempts");
        script.Should().Contain("CREATE TABLE dbo.AuditSriDocumentChanges");
        script.Should().Contain("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE");
        script.Should().Contain("WITH (UPDLOCK, HOLDLOCK)");
        script.Should().Contain("RowVersion rowversion NOT NULL");
        script.Should().Contain("Status NOT IN (N'Pending', N'RetryScheduled')");
        script.Should().Contain("Status NOT IN (N'Failed', N'DeadLetter')");
        script.Should().NotContain("XmlContent");
        script.Should().NotContain("PayloadXml");
    }

    [Fact]
    public void Api_UsesGranularPermissionsAndDoesNotExposeXmlYet()
    {
        var endpoints = ReadSourceFile("src", "Backend", "NuanSystem.Api", "Endpoints", "SriDocumentEndpoints.cs");

        endpoints.Should().Contain("PermissionCodes.SriDocumentsView");
        endpoints.Should().Contain("PermissionCodes.SriDocumentsEnqueue");
        endpoints.Should().Contain("PermissionCodes.SriDocumentsCancel");
        endpoints.Should().Contain("PermissionCodes.SriDocumentsReprocess");
        endpoints.Should().NotContain("DownloadXml");
        endpoints.Should().NotContain("ViewPayload");
    }

    [Fact]
    public void MasterScript_SeedsAllApprovedPermissionAliases()
    {
        var script = ReadSourceFile("database", "sql", "116_master_sri_document_queue_security.sql");

        foreach (var permission in new[]
                 {
                     "SRI.DOCUMENTS.VIEW", "SRI.DOCUMENTS.ENQUEUE", "SRI.DOCUMENTS.CANCEL",
                     "SRI.DOCUMENTS.REPROCESS", "SRI.DOCUMENTS.VIEW_PAYLOAD", "SRI.DOCUMENTS.DOWNLOAD_XML"
                 })
        {
            script.Should().Contain(permission);
        }
        script.Should().Contain("dbo.RolePermissions");
        script.Should().NotContain("SecurityMenus");
        script.Should().NotContain("SecurityForms");
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(path)) return File.ReadAllText(path);
            directory = directory.Parent;
        }
        throw new FileNotFoundException($"No se encontro {Path.Combine(pathParts)}.");
    }
}
