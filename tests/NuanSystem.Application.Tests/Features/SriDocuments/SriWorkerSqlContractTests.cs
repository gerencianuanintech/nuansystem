using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SriDocuments;

public sealed class SriWorkerSqlContractTests
{
    [Fact]
    public void Script117_DefinesAtomicClaimLeaseAttemptAndImmutableAuthorizedXml()
    {
        var script = ReadSourceFile("database", "sql", "117_tenant_sri_worker_and_document_store.sql");

        script.Should().Contain("CREATE TABLE dbo.SriAuthorizedDocuments");
        script.Should().Contain("UQ_SriAuthorizedDocuments_Queue UNIQUE(QueueId)");
        script.Should().Contain("UQ_SriAuthorizedDocuments_Identity UNIQUE(Environment, AccessKey)");
        script.Should().Contain("SizeBytes > 0 AND SizeBytes <= 5242880");
        script.Should().Contain("DATALENGTH(XmlContent)=SizeBytes");
        script.Should().Contain("DATALENGTH(Sha256)=32");
        script.Should().Contain("WITH (UPDLOCK,READPAST,ROWLOCK)");
        script.Should().Contain("WHERE Environment=@Environment");
        script.Should().Contain("deleted.Status");
        script.Should().Contain("BEGIN TRANSACTION");
        script.Should().Contain("SP_NA_POST_SRIDOCUMENTQUEUE_LIBERARLEASESVENCIDOS");
        script.Should().Contain("SP_NA_POST_SRIDOCUMENTQUEUE_COMPLETARAUTORIZADO");
        script.Should().Contain("SP_NA_POST_SRIDOCUMENTQUEUE_COMPLETARINTENTO");
        script.Should().Contain("20260720.117");
        script.Should().NotContain("OPENQUERY");
        script.Should().NotContain("sp_addlinkedserver");
    }

    [Fact]
    public void WorkerConfiguration_IsDisabledAndUsesStrictOfficialEndpointsByDefault()
    {
        var settings = ReadSourceFile("src", "Backend", "NuanSystem.SriWorker", "appsettings.json");
        var program = ReadSourceFile("src", "Backend", "NuanSystem.SriWorker", "Program.cs");

        settings.Should().Contain("\"Enabled\": false");
        settings.Should().Contain("\"BatchSize\": 10");
        settings.Should().Contain("\"MaxConcurrency\": 2");
        settings.Should().Contain("\"LeaseSeconds\": 120");
        settings.Should().Contain("\"TimeoutSeconds\": 30");
        settings.Should().Contain("celcer.sri.gob.ec");
        settings.Should().Contain("cel.sri.gob.ec");
        program.Should().Contain("IsOfficialEndpoint");
        program.Should().Contain("AllowAutoRedirect = false");
        program.Should().NotContain("IgnoreSslErrors");
        program.Should().NotContain("ServerCertificateCustomValidationCallback");
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
