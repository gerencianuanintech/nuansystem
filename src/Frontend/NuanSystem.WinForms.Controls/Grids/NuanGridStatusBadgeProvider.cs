namespace NuanSystem.WinForms.Controls.Grids;

public delegate NuanGridBadgeStyle NuanGridStatusBadgeProvider(object? value);

public static class NuanGridStatusBadges
{
    public static NuanGridBadgeStyle DefaultProvider(object? value)
    {
        var text = Convert.ToString(value)?.Trim();
        return text switch
        {
            "InProcess" => NuanGridBadgeStyle.Info,
            "Applied" => NuanGridBadgeStyle.Success,
            "Error" => NuanGridBadgeStyle.Error,
            "DeadLetter" => NuanGridBadgeStyle.Critical,
            "Pending" => NuanGridBadgeStyle.Neutral,
            "Ignored" => NuanGridBadgeStyle.Neutral,
            _ => NuanGridBadgeStyle.Neutral
        };
    }
}
