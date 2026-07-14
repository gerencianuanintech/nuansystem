namespace NuanSystem.WinForms.Controls.Buttons;

public sealed class NuanActionButtonStyle
{
    public string Text { get; init; } = string.Empty;

    public Color BackColor { get; init; }

    public Color HoverBackColor { get; init; }

    public Color PressedBackColor { get; init; }

    public Color ForeColor { get; init; } = Color.White;

    public string? IconName { get; init; }

    public static NuanActionButtonStyle FromKind(NuanActionButtonKind kind)
    {
        return kind switch
        {
            NuanActionButtonKind.Save => new NuanActionButtonStyle
            {
                Text = "Guardar",
                BackColor = Color.FromArgb(0, 184, 148),
                HoverBackColor = Color.FromArgb(0, 160, 128),
                PressedBackColor = Color.FromArgb(0, 137, 111),
                IconName = "diskette_32.svg"
            },
            NuanActionButtonKind.Cancel => new NuanActionButtonStyle
            {
                Text = "Cancelar",
                BackColor = Color.FromArgb(99, 110, 114),
                HoverBackColor = Color.FromArgb(78, 87, 90),
                PressedBackColor = Color.FromArgb(60, 67, 70),
                IconName = "cancelar_32.svg"
            },
            NuanActionButtonKind.Delete => new NuanActionButtonStyle
            {
                Text = "Eliminar",
                BackColor = Color.FromArgb(220, 38, 38),
                HoverBackColor = Color.FromArgb(185, 28, 28),
                PressedBackColor = Color.FromArgb(153, 27, 27),
                IconName = "eliminar_32.svg"
            },
            NuanActionButtonKind.Search => new NuanActionButtonStyle
            {
                Text = "Buscar",
                BackColor = Color.FromArgb(37, 99, 235),
                HoverBackColor = Color.FromArgb(29, 78, 216),
                PressedBackColor = Color.FromArgb(30, 64, 175),
                IconName = "buscar_32.svg"
            },
            NuanActionButtonKind.Refresh => new NuanActionButtonStyle
            {
                Text = "Actualizar",
                BackColor = Color.FromArgb(37, 99, 235),
                HoverBackColor = Color.FromArgb(29, 78, 216),
                PressedBackColor = Color.FromArgb(30, 64, 175),
                IconName = "actualizar_32.svg"
            },
            NuanActionButtonKind.Export => new NuanActionButtonStyle
            {
                Text = "Exportar",
                BackColor = Color.FromArgb(22, 163, 74),
                HoverBackColor = Color.FromArgb(21, 128, 61),
                PressedBackColor = Color.FromArgb(22, 101, 52),
                IconName = "exportar_32.svg"
            },
            NuanActionButtonKind.Retry => new NuanActionButtonStyle
            {
                Text = "Reintentar",
                BackColor = Color.FromArgb(37, 99, 235),
                HoverBackColor = Color.FromArgb(29, 78, 216),
                PressedBackColor = Color.FromArgb(30, 64, 175),
                IconName = "actualizar_32.svg"
            },
            NuanActionButtonKind.Warning => new NuanActionButtonStyle
            {
                Text = "Advertencia",
                BackColor = Color.FromArgb(245, 124, 0),
                HoverBackColor = Color.FromArgb(230, 105, 0),
                PressedBackColor = Color.FromArgb(194, 83, 0),
                IconName = "rechazar_32.svg"
            },
            NuanActionButtonKind.Neutral => new NuanActionButtonStyle
            {
                Text = "Aceptar",
                BackColor = Color.FromArgb(107, 114, 128),
                HoverBackColor = Color.FromArgb(75, 85, 99),
                PressedBackColor = Color.FromArgb(55, 65, 81),
                IconName = "ver_detalle_32.svg"
            },
            _ => new NuanActionButtonStyle
            {
                Text = "Aceptar",
                BackColor = Color.FromArgb(37, 99, 235),
                HoverBackColor = Color.FromArgb(29, 78, 216),
                PressedBackColor = Color.FromArgb(30, 64, 175),
                IconName = "aprobar_32.svg"
            }
        };
    }
}
