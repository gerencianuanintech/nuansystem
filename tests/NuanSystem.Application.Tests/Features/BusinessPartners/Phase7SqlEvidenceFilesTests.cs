using System.Text;
using NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

[Trait("Category","OfflineSafety")]
public sealed class Phase7SqlEvidenceFilesTests
{
    [Theory]
    [InlineData("../outside")][InlineData("/absolute")][InlineData("C:/secret")][InlineData("a\\b")]
    [InlineData("a//b")][InlineData("a/./b")][InlineData("a/../b")][InlineData("")]
    public void Traversal_and_ambiguous_paths_are_rejected(string path)
    {
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlEvidenceFiles.SafeRelative(path));
    }

    [Fact]
    public void Candidate_fingerprints_are_stable_across_fresh_run_ids_and_bind_actual_bytes()
    {
        var root=Root();Directory.CreateDirectory(Path.Combine(root,"src"));Directory.CreateDirectory(Path.Combine(root,"bin"));
        File.WriteAllText(Path.Combine(root,"src","test.cs"),"// synthetic source");
        File.WriteAllBytes(Path.Combine(root,"bin","test.dll"),[1,2,3]);
        var candidate=Phase7SqlCandidate.Freeze(root,["src/test.cs"],["bin/test.dll"]);
        var again=Phase7SqlCandidate.Freeze(root,["src/test.cs"],["bin/test.dll"]);
        Assert.Equal(candidate.SourceSha256,again.SourceSha256);Assert.Equal(candidate.BinarySha256,again.BinarySha256);
        candidate.Verify();
        File.WriteAllBytes(Path.Combine(root,"bin","test.dll"),[1,2,4]);
        Assert.Throws<Phase7SqlGuardException>(()=>candidate.Verify());
    }

    [Fact]
    public void Candidate_index_cannot_hide_case_collisions_or_configuration_secrets()
    {
        var root=Root();File.WriteAllText(Path.Combine(root,"A.cs"),"test");
        File.WriteAllBytes(Path.Combine(root,"test.dll"),[1]);File.WriteAllText(Path.Combine(root,"appsettings.json"),"{}");
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlCandidate.Freeze(root,["A.cs","a.cs"],["test.dll"]));
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlCandidate.Freeze(root,["appsettings.json"],["test.dll"]));
    }

    [Fact]
    public void Reads_are_bounded_and_cancelled_without_changing_source_files()
    {
        var root=Root();File.WriteAllBytes(Path.Combine(root,"large.txt"),new byte[129]);
        Assert.Throws<Phase7SqlGuardException>(()=>Phase7SqlEvidenceFiles.Read(root,"large.txt",128,default));
        using var stop=new CancellationTokenSource();stop.Cancel();
        Assert.ThrowsAny<OperationCanceledException>(()=>Phase7SqlEvidenceFiles.Read(root,"large.txt",1024,stop.Token));
        Assert.Equal(129,new FileInfo(Path.Combine(root,"large.txt")).Length);
    }

    [Fact]
    public void Boms_are_preserved_in_file_hashes()
    {
        var root=Root();var bytes=Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes("evidence")).ToArray();
        File.WriteAllBytes(Path.Combine(root,"run.trx"),bytes);
        Assert.Equal(bytes,Phase7SqlEvidenceFiles.Read(root,"run.trx",1024,default));
        Assert.NotEqual(Phase7SqlEvidenceFiles.Hash(bytes),Phase7SqlEvidenceFiles.Hash(Encoding.UTF8.GetBytes("evidence")));
    }

    internal static string Root()
    {
        var path=Path.GetFullPath(Path.Combine("artifacts","backend-solid-phase7-sql-evidence","offline",Guid.NewGuid().ToString("N")));
        Directory.CreateDirectory(path);return path; // keep test evidence; no recursive deletion
    }
}
