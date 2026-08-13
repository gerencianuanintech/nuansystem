using System.ComponentModel;
using System.Drawing.Drawing2D;
using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Controls.Editors;

[DefaultEvent(nameof(IsOnChanged))]
[DefaultProperty(nameof(Checked))]
[ToolboxItem(true)]
public class NuanToggleSwitch : ToggleSwitch
{
    private Color activeColor = SystemColors.Highlight;
    private Color inactiveColor = SystemColors.ControlDark;
    private Color textColor = SystemColors.ControlText;
    private Color thumbColor = Color.White;

    public NuanToggleSwitch()
    {
        Properties.OnText = "Sí";
        Properties.OffText = "No";
        Properties.ShowText = true;
        Properties.AllowThumbAnimation = true;
        Properties.AutoHeight = false;
        Size = new Size(70, 20);
        MinimumSize = new Size(58, 20);
        AccessibleRole = AccessibleRole.CheckButton;
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint,
            true);
    }

    [Category("Nuan Apariencia")]
    public Color ActiveColor
    {
        get => activeColor;
        set
        {
            activeColor = value;
            Invalidate();
        }
    }

    [Category("Nuan Apariencia")]
    public Color InactiveColor
    {
        get => inactiveColor;
        set
        {
            inactiveColor = value;
            Invalidate();
        }
    }

    [Category("Nuan Apariencia")]
    public Color StateTextColor
    {
        get => textColor;
        set
        {
            textColor = value;
            Invalidate();
        }
    }

    [Category("Nuan Apariencia")]
    public Color ThumbColor
    {
        get => thumbColor;
        set
        {
            thumbColor = value;
            Invalidate();
        }
    }

    [Category("Nuan Comportamiento")]
    [Bindable(true)]
    [DefaultValue(false)]
    public bool Checked
    {
        get => IsOn;
        set => IsOn = value;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (ClientSize.Width <= 1 || ClientSize.Height <= 1)
        {
            return;
        }

        var background = ResolveBackgroundColor();
        e.Graphics.Clear(background);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var trackHeight = Math.Max(12, Math.Min(16, ClientSize.Height - 4));
        var trackWidth = Math.Min(42, Math.Max(32, ClientSize.Width - 24));
        var trackBounds = new Rectangle(0, (ClientSize.Height - trackHeight) / 2, trackWidth, trackHeight);
        var stateColor = IsOn ? activeColor : inactiveColor;
        if (!Enabled)
        {
            stateColor = Blend(stateColor, background, 0.58F);
        }

        using (var trackPath = CreateRoundedRectangle(trackBounds, trackHeight / 2))
        using (var trackBrush = new SolidBrush(stateColor))
        {
            e.Graphics.FillPath(trackBrush, trackPath);
        }

        var thumbDiameter = trackHeight - 4;
        var thumbX = IsOn
            ? trackBounds.Right - thumbDiameter - 2
            : trackBounds.Left + 2;
        var thumbBounds = new Rectangle(
            thumbX,
            trackBounds.Top + 2,
            thumbDiameter,
            thumbDiameter);
        var effectiveThumbColor = Enabled ? thumbColor : Blend(thumbColor, background, 0.28F);
        using (var thumbBrush = new SolidBrush(effectiveThumbColor))
        {
            e.Graphics.FillEllipse(thumbBrush, thumbBounds);
        }

        var stateText = IsOn ? Properties.OnText : Properties.OffText;
        var textBounds = new Rectangle(
            trackBounds.Right + 5,
            0,
            Math.Max(0, ClientSize.Width - trackBounds.Right - 5),
            ClientSize.Height);
        TextRenderer.DrawText(
            e.Graphics,
            stateText,
            Font,
            textBounds,
            Enabled ? textColor : SystemColors.GrayText,
            TextFormatFlags.Left |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis |
            TextFormatFlags.NoPadding |
            TextFormatFlags.SingleLine);

        if (Focused && ShowFocusCues)
        {
            ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle, textColor, background);
        }
    }

    private Color ResolveBackgroundColor()
    {
        if (BackColor != Color.Transparent)
        {
            return BackColor;
        }

        return Parent?.BackColor ?? SystemColors.Control;
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        var diameter = Math.Max(1, Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height)));
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static Color Blend(Color foreground, Color background, float backgroundWeight)
    {
        var weight = Math.Clamp(backgroundWeight, 0F, 1F);
        return Color.FromArgb(
            (int)Math.Round((foreground.R * (1F - weight)) + (background.R * weight)),
            (int)Math.Round((foreground.G * (1F - weight)) + (background.G * weight)),
            (int)Math.Round((foreground.B * (1F - weight)) + (background.B * weight)));
    }

    private bool ShouldSerializeActiveColor() => activeColor != SystemColors.Highlight;

    private bool ShouldSerializeInactiveColor() => inactiveColor != SystemColors.ControlDark;

    private bool ShouldSerializeStateTextColor() => textColor != SystemColors.ControlText;

    private bool ShouldSerializeThumbColor() => thumbColor != Color.White;
}
