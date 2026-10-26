using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal static class Phase7SqlEvidenceFiles
{
    public const int MaximumFileBytes=32*1024*1024;
    public const long MaximumPackageBytes=128L*1024*1024;
    public static JsonSerializerOptions Json {get;}=new()
    {
        UnmappedMemberHandling=JsonUnmappedMemberHandling.Disallow,AllowDuplicateProperties=false,
        RespectRequiredConstructorParameters=true,RespectNullableAnnotations=true,MaxDepth=64
    };
    public static string Hash(byte[] bytes)=>Convert.ToHexString(SHA256.HashData(bytes));
    public static bool IsHash(string? value)=>value is {Length:64} && value.All(Uri.IsHexDigit) && value.Any(c=>c!='0');
    public static string SafeRelative(string path)
    {
        Require(path is not null && Regex.IsMatch(path,"\\A[A-Za-z0-9][A-Za-z0-9_./-]{0,159}\\z")
            && path.Split('/').All(p=>p.Length>0 && p is not ("." or "..")),"PATH");
        return path;
    }
    public static string Resolve(string root,string relative)
    {
        SafeRelative(relative);var fullRoot=Path.GetFullPath(root);
        Require(Path.IsPathFullyQualified(root),"ROOT");
        var full=Path.GetFullPath(Path.Combine(fullRoot,relative.Replace('/',Path.DirectorySeparatorChar)));
        Require(full.StartsWith(Path.TrimEndingDirectorySeparator(fullRoot)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase),"PATH");
        CheckNoLinks(fullRoot);CheckNoLinks(full);
        return full;
    }
    public static void CheckNoLinks(string path)
    {
        for(string? current=Path.GetFullPath(path);current is not null;current=Path.GetDirectoryName(current))
            if(File.Exists(current) || Directory.Exists(current))Require((File.GetAttributes(current)&FileAttributes.ReparsePoint)==0,"LINK");
    }
    public static byte[] Read(string root,string path,int maximum,CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            using var stream=new FileStream(Resolve(root,path),FileMode.Open,FileAccess.Read,FileShare.Read);
            Require(stream.Length<=maximum && stream.Length<=MaximumFileBytes,"SIZE");
            var bytes=new byte[checked((int)stream.Length)];stream.ReadExactly(bytes);
            Require(stream.ReadByte()==-1,"FILE_CHANGED");token.ThrowIfCancellationRequested();return bytes;
        }
        catch(Phase7SqlGuardException){throw;}
        catch(OperationCanceledException) when(token.IsCancellationRequested){throw;}
        catch{throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_READ");}
    }
    public static void Write(string root,string path,byte[] bytes,CancellationToken token=default)
    {
        token.ThrowIfCancellationRequested();Require(bytes.Length<=MaximumFileBytes,"SIZE");
        var full=Resolve(root,path);Directory.CreateDirectory(Path.GetDirectoryName(full)!);CheckNoLinks(full);
        using var stream=new FileStream(full,FileMode.CreateNew,FileAccess.Write,FileShare.None);stream.Write(bytes);stream.Flush(true);
    }
    public static byte[] Encode<T>(T value)=>JsonSerializer.SerializeToUtf8Bytes(value,Json);
    public static T Decode<T>(byte[] bytes)
    {
        try{ValidateJson(bytes);return JsonSerializer.Deserialize<T>(bytes,Json)??throw new JsonException();}
        catch{throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_JSON");}
    }
    public static void ValidateJson(byte[] bytes)
    {
        using var document=JsonDocument.Parse(bytes,new JsonDocumentOptions{MaxDepth=64});
        void Check(JsonElement element)
        {
            if(element.ValueKind==JsonValueKind.Object)
            {
                var keys=new HashSet<string>(StringComparer.Ordinal);
                foreach(var property in element.EnumerateObject()){Require(keys.Add(property.Name),"DUPLICATE_JSON");Check(property.Value);}
            }
            else if(element.ValueKind==JsonValueKind.Array)foreach(var item in element.EnumerateArray())Check(item);
        }
        Check(document.RootElement);
    }
    public static void Require([DoesNotReturnIf(false)] bool valid,string code)
    {if(!valid)throw new Phase7SqlGuardException("PHASE7_SQL_EVIDENCE_"+code);}
}

internal sealed record Phase7SqlEvidenceFile(string RelativePath,long Length,string Sha256);
internal sealed record Phase7SqlCandidate(string RootPath,DateTimeOffset CapturedAtUtc,
    IReadOnlyList<Phase7SqlEvidenceFile> SourceFiles,IReadOnlyList<Phase7SqlEvidenceFile> BinaryFiles)
{
    [JsonIgnore] public byte[] SourceIndex=>Phase7SqlEvidenceFiles.Encode(SourceFiles.OrderBy(f=>f.RelativePath,StringComparer.Ordinal).ToArray());
    [JsonIgnore] public byte[] BinaryIndex=>Phase7SqlEvidenceFiles.Encode(BinaryFiles.OrderBy(f=>f.RelativePath,StringComparer.Ordinal).ToArray());
    [JsonIgnore] public string SourceSha256=>Phase7SqlEvidenceFiles.Hash(SourceIndex);
    [JsonIgnore] public string BinarySha256=>Phase7SqlEvidenceFiles.Hash(BinaryIndex);
    public static Phase7SqlCandidate Freeze(string root,IReadOnlyList<string> sources,IReadOnlyList<string> binaries,CancellationToken token=default)
    {
        long total=0;
        IReadOnlyList<Phase7SqlEvidenceFile> Index(IReadOnlyList<string> paths,bool binary)
        {
            Phase7SqlEvidenceFiles.Require(paths.Count is >0 and <=512 && paths.Distinct(StringComparer.OrdinalIgnoreCase).Count()==paths.Count,"CANDIDATE_FILES");
            return Array.AsReadOnly(paths.Order(StringComparer.Ordinal).Select(path=>
            {
                Phase7SqlEvidenceFiles.SafeRelative(path);
                var extension=Path.GetExtension(path);
                Phase7SqlEvidenceFiles.Require(binary?extension is ".dll" or ".exe" or ".pdb"
                    :extension is ".cs" or ".csproj" or ".props" or ".targets" or ".sql" or ".md","CANDIDATE_TYPE");
                var bytes=Phase7SqlEvidenceFiles.Read(root,path,Phase7SqlEvidenceFiles.MaximumFileBytes,token);
                total+=bytes.Length;Phase7SqlEvidenceFiles.Require(total<=Phase7SqlEvidenceFiles.MaximumPackageBytes,"TOTAL_SIZE");
                return new Phase7SqlEvidenceFile(path,bytes.LongLength,Phase7SqlEvidenceFiles.Hash(bytes));
            }).ToArray());
        }
        var result=new Phase7SqlCandidate(Path.GetFullPath(root),DateTimeOffset.UtcNow,Index(sources,false),Index(binaries,true));
        Phase7SqlEvidenceFiles.Require(result.SourceFiles.Concat(result.BinaryFiles).Sum(f=>f.Length)<=Phase7SqlEvidenceFiles.MaximumPackageBytes,"TOTAL_SIZE");
        return result;
    }
    public void Verify(CancellationToken token=default)
    {
        Phase7SqlEvidenceFiles.Require(CapturedAtUtc.Offset==TimeSpan.Zero && CapturedAtUtc!=default,"CANDIDATE_TIME");
        var actual=Freeze(RootPath,SourceFiles.Select(f=>f.RelativePath).ToArray(),BinaryFiles.Select(f=>f.RelativePath).ToArray(),token);
        Phase7SqlEvidenceFiles.Require(SourceSha256==actual.SourceSha256 && BinarySha256==actual.BinarySha256,"CANDIDATE_CHANGED");
    }
}
