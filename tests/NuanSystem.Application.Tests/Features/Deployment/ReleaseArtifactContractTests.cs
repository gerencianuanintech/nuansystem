using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.Deployment;

public sealed class ReleaseArtifactContractTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void PublishScript_UsesExplicitSafeWinX64Contract()
    {
        var script = Read("tools/release/Publish-NuanSystemRelease.ps1");

        script.Should()
            .Contain("--configuration Release")
            .And.Contain("--runtime win-x64")
            .And.Contain("--self-contained false")
            .And.Contain("-p:IncludeSourceRevisionInInformationalVersion=false")
            .And.Contain("-p:PublishTrimmed=false")
            .And.Contain("-p:PublishSingleFile=false")
            .And.Contain("appsettings.Local.json")
            .And.Contain("WorkersEnabled = $false");
    }

    [Fact]
    public void PublishScript_PublishesFiveApprovedHostsSeparately()
    {
        var script = Read("tools/release/Publish-NuanSystemRelease.ps1");

        foreach (var project in new[]
                 {
                     "NuanSystem.Api",
                     "NuanSystem.SyncWorker",
                     "NuanSystem.MasterBranchSyncWorker",
                     "NuanSystem.SriWorker",
                     "NuanSystem.WinForms"
                 })
        {
            script.Should().Contain($"Name = \"{project}\"");
        }

        script.Should().Contain("release-manifest.json")
            .And.Contain("dependency-inventory.json")
            .And.Contain("file-hashes.json");
    }

    [Fact]
    public void PublishScript_RejectsSensitiveAndMutableReleaseContent()
    {
        var script = Read("tools/release/Publish-NuanSystemRelease.ps1");

        foreach (var forbidden in new[]
                 {
                     ".pfx", ".p12", ".pem", ".key", ".cer", ".crt", ".bak", ".log"
                 })
        {
            script.Should().Contain(forbidden);
        }

        script.Should().Contain("La release ya existe y es inmutable")
            .And.Contain("SensitiveValuesPresent = $false")
            .And.Contain("AccessKey")
            .And.Contain("claveAcceso");
    }

    [Fact]
    public void VerificationScript_RecomputesHashesAndVersions()
    {
        var script = Read("tools/release/Test-NuanSystemRelease.ps1");

        script.Should().Contain("Get-FileHash -Algorithm SHA256")
            .And.Contain("Compare-Object")
            .And.Contain("FileVersionInfo")
            .And.Contain("ProductVersion")
            .And.Contain("framework-dependent");
    }

    [Fact]
    public void PointerScript_SelectsOnlyManifestedRelease()
    {
        var script = Read("tools/release/Set-NuanSystemActiveRelease.ps1");

        script.Should().Contain("release-manifest.json")
            .And.Contain("file-hashes.json")
            .And.Contain("HashManifestSha256")
            .And.Contain("SourceCommit")
            .And.NotContain("Remove-Item");
    }

    private static string Read(string relativePath) =>
        File.ReadAllText(Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar)));

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null && !File.Exists(Path.Combine(current.FullName, "NuanSystem.sln")))
        {
            current = current.Parent;
        }

        return current?.FullName ?? throw new DirectoryNotFoundException("No se encontro la raiz del repositorio.");
    }
}
