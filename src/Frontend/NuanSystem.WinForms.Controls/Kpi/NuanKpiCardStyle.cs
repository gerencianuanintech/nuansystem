namespace NuanSystem.WinForms.Controls.Kpi;

public sealed class NuanKpiCardStyle
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public Color HeaderColor { get; init; }

    public Color DescriptionColor { get; init; }

    public Color FooterBackColor { get; init; } = Color.White;

    public Color BorderColor { get; init; } = Color.FromArgb(221, 226, 240);

    public static NuanKpiCardStyle Pending()
    {
        return new NuanKpiCardStyle
        {
            Title = "Pending",
            Description = "Pendientes de procesamiento",
            HeaderColor = Color.FromArgb(71, 85, 105),
            DescriptionColor = Color.FromArgb(51, 65, 85)
        };
    }

    public static NuanKpiCardStyle InProcess()
    {
        return new NuanKpiCardStyle
        {
            Title = "InProcess",
            Description = "En proceso actualmente",
            HeaderColor = Color.FromArgb(37, 99, 235),
            DescriptionColor = Color.FromArgb(29, 78, 216)
        };
    }

    public static NuanKpiCardStyle Applied()
    {
        return new NuanKpiCardStyle
        {
            Title = "Applied",
            Description = "Aplicados correctamente",
            HeaderColor = Color.FromArgb(22, 163, 74),
            DescriptionColor = Color.FromArgb(21, 128, 61)
        };
    }

    public static NuanKpiCardStyle Error()
    {
        return new NuanKpiCardStyle
        {
            Title = "Error",
            Description = "Con errores (reintentos)",
            HeaderColor = Color.FromArgb(239, 68, 68),
            DescriptionColor = Color.FromArgb(185, 28, 28)
        };
    }

    public static NuanKpiCardStyle DeadLetter()
    {
        return new NuanKpiCardStyle
        {
            Title = "DeadLetter",
            Description = "Movidos a DeadLetter",
            HeaderColor = Color.FromArgb(190, 18, 60),
            DescriptionColor = Color.FromArgb(159, 18, 57)
        };
    }

    public static NuanKpiCardStyle Ignored()
    {
        return new NuanKpiCardStyle
        {
            Title = "Ignored",
            Description = "Ignorados por reglas",
            HeaderColor = Color.FromArgb(107, 114, 128),
            DescriptionColor = Color.FromArgb(55, 65, 81)
        };
    }
}
