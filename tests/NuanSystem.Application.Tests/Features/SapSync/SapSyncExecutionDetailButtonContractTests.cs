using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncExecutionDetailButtonContractTests
{
    [Fact]
    public void DetailDialog_UsesCorporateButtonsAndSpecializedOperationalIcons()
    {
        var designer = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Sap",
            "SapSyncExecutionDetailForm.Designer.cs");

        designer.Should()
            .Contain("refreshButton.ButtonKind = NuanActionButtonKind.Save;")
            .And.Contain("retryButton.ButtonKind = NuanActionButtonKind.Save;")
            .And.Contain("releaseButton.ButtonKind = NuanActionButtonKind.Save;")
            .And.Contain("cancelButton.ButtonKind = NuanActionButtonKind.Cancel;")
            .And.Contain("retryButton.IconNameOverride = \"reintentar_ejecucion_16.svg\";")
            .And.Contain("releaseButton.IconNameOverride = \"liberar_lock_vencido_16.svg\";")
            .And.NotContain("retryButton.ButtonKind = NuanActionButtonKind.Retry;")
            .And.NotContain("releaseButton.ButtonKind = NuanActionButtonKind.Warning;");

        designer.Split("Size = new Size(100, 36);").Length.Should().BeGreaterThanOrEqualTo(5);

        foreach (var icon in new[] { "reintentar_ejecucion", "liberar_lock_vencido" })
        {
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Assets", "Icons", "Operaciones", $"{icon}_16.svg")
                .Should().Contain("width=\"16\"").And.Contain("stroke=\"#00B894\"");
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Assets", "Icons", "Operaciones", $"{icon}_32.svg")
                .Should().Contain("width=\"32\"").And.Contain("stroke=\"#00B894\"");
        }
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
            ?? throw new DirectoryNotFoundException("No se encontro la raiz del repositorio.");
    }
}
