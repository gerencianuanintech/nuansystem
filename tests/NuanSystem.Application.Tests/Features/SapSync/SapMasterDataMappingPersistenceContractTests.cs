using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapMasterDataMappingPersistenceContractTests
{
    [Fact]
    public void Repository_uses_parameterized_stored_procedures_and_cancellation_tokens()
    {
        var source = Read(
            "src", "Backend", "NuanSystem.Persistence", "Repositories", "SapSync", "SapMasterDataMappingRepository.cs");

        source.Should().Contain("CommandType.StoredProcedure")
            .And.Contain("cancellationToken: cancellationToken")
            .And.Contain("SP_NA_GET_SAPQUERYDEFINITIONS_LISTAR")
            .And.Contain("SP_NA_GET_SAPFIELDMAPPINGS_LISTAR")
            .And.Contain("SP_NA_POST_SAPQUERYDEFINITIONCREAR")
            .And.Contain("SP_NA_PUT_SAPFIELDMAPPINGSREEMPLAZAR")
            .And.Contain("SP_NA_PATCH_SAPQUERYDEFINITIONDESACTIVAR")
            .And.NotMatchRegex(@"(?im)^\s*(SELECT|FROM|JOIN|WHERE)\b");
    }

    private static string Read(params string[] path)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;
        return File.ReadAllText(Path.Combine([directory!.FullName, .. path]));
    }
}
